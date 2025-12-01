using Microsoft.AspNetCore.Mvc;
using UTSLogoWebAPI.Classes;
using UTSLogoWebAPI.Models;
using System.Data;

namespace UTSLogoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UTSController : ControllerBase
    {
        private const int DebitAmount = 1;

        [HttpPost("Query")]
        public async Task<IActionResult> Query([FromBody] UTSQueryRequestModel model)
        {
            // Log giriş
            string logRequestDetails = model != null ?
                $"GUID: {model.CustomerGUID}, UNO: {model.UNO ?? "NULL"}, LNO: {model.LNO ?? "NULL"}, SN: {model.SN ?? "NULL"}, Adet: {model.Adet}" :
                "NULL Model";
            await SQLCrud.LogErrorAsync($"[UTSQuery GİRİŞ] İstek Alındı. Detay: {logRequestDetails}");

            #region === 1. VALİDASYON ===

            if (model == null)
                return BadRequest(new { success = false, message = "Geçersiz istek." });

            if (string.IsNullOrWhiteSpace(model.CustomerGUID))
                return BadRequest(new { success = false, message = "CustomerGUID zorunludur." });

            if (!Guid.TryParse(model.CustomerGUID, out Guid customerGuid))
                return BadRequest(new { success = false, message = "Geçerli bir CustomerGUID giriniz." });

            // CustomerToken validasyonu YOK - ApiKeyAuthFilter zaten header'dan kontrol ediyor!

            if (string.IsNullOrWhiteSpace(model.UNO))
                return BadRequest(new { success = false, message = "UNO zorunludur." });

            if (string.IsNullOrWhiteSpace(model.LNO))
                return BadRequest(new { success = false, message = "LNO zorunludur." });

            if (string.IsNullOrWhiteSpace(model.SN))
                return BadRequest(new { success = false, message = "SN zorunludur." });

            if (model.Adet <= 0)
                return BadRequest(new { success = false, message = "Adet 0'dan büyük olmalıdır." });

            #endregion

            short currentCustomerId = 0;
            string customerName = "BilinmeyenMüşteri";
            int currentCount = 0;

            try
            {
                #region === 2. MÜŞTERİ KONTROLÜ ===

                string checkQuery = @"
                    SELECT ID, CustomerName, UTSToken, Count, Status 
                    FROM dbo.Customers 
                    WHERE CustomerGUID = @guid";

                var checkParams = new Dictionary<string, object> { { "@guid", customerGuid } };
                DataTable dt = await SQLCrud.GetDataTableAsync(checkQuery, checkParams);

                if (dt == null || dt.Rows.Count == 0)
                {
                    await SQLCrud.LogErrorAsync($"[UTSQuery] Müşteri bulunamadı. GUID: {model.CustomerGUID}");
                    return NotFound(new { success = false, message = "Müşteri bulunamadı." });
                }

                DataRow row = dt.Rows[0];
                currentCustomerId = Convert.ToInt16(row["ID"]);
                customerName = row["CustomerName"].ToString();
                string utsToken = row["UTSToken"]?.ToString() ?? string.Empty;
                currentCount = Convert.ToInt32(row["Count"]);
                bool isActive = Convert.ToBoolean(row["Status"]);

                await SQLCrud.LogErrorAsync($"[UTSQuery] Müşteri bulundu. ID: {currentCustomerId}, İsim: {customerName}", currentCustomerId);

                // Aktif mi?
                if (!isActive)
                {
                    await SQLCrud.LogErrorAsync($"[UTSQuery] Pasif müşteri. Müşteri: {customerName}", currentCustomerId);
                    return StatusCode(403, new
                    {
                        success = false,
                        message = "Müşteri hesabı pasif durumda."
                    });
                }

                // UTS Token var mı?
                if (string.IsNullOrWhiteSpace(utsToken))
                {
                    await SQLCrud.LogErrorAsync($"[UTSQuery] UTS Token yok. Müşteri: {customerName}", currentCustomerId);
                    return StatusCode(403, new
                    {
                        success = false,
                        message = "UTS Token tanımlanmamış. Lütfen yönetici ile iletişime geçin."
                    });
                }

                // Kontör yeterli mi?
                if (currentCount < DebitAmount)
                {
                    await SQLCrud.LogErrorAsync($"[UTSQuery] Kontör yetersiz. Müşteri: {customerName}, Mevcut: {currentCount}", currentCustomerId);
                    return StatusCode(402, new
                    {
                        success = false,
                        message = "Kontör yetersiz.",
                        mevcutKontor = currentCount,
                        gerekliKontor = DebitAmount
                    });
                }

                #endregion

                #region === 3. UTS SORGULAMA ===

                await SQLCrud.LogErrorAsync($"[UTSQuery GİDİŞ] Sorgu UTS'ye gönderiliyor. UNO: {model.UNO}, LNO: {model.LNO}, SN: {model.SN}", currentCustomerId);

                var (utsResponse, utsSuccess) = await UTSApiClient.SorgulaTekilUrunAsync(
                    model.UNO,
                    model.LNO,
                    model.SN,
                    utsToken,
                    customerName
                );

                if (!utsSuccess)
                {
                    return StatusCode(503, new
                    {
                        success = false,
                        message = utsResponse?.Mesaj ?? "UTS servisine erişilemiyor veya beklenmeyen hata oluştu.",
                        mevcutKontor = currentCount
                    });
                }

                if (utsResponse?.UrunListesi == null || utsResponse.UrunListesi.Count == 0)
                {
                    await SQLCrud.LogErrorAsync(
                        $"[UTSQuery] UTS Ürün Bulunamadı. UNO: {model.UNO}, LNO: {model.LNO}, SN: {model.SN}. UTS Mesajı: {utsResponse?.Mesaj}",
                        currentCustomerId);

                    return NotFound(new
                    {
                        success = false,
                        message = "UTS'de bu ürün bulunamadı.",
                        uno = model.UNO,
                        lno = model.LNO,
                        sn = model.SN,
                        mevcutKontor = currentCount
                    });
                }

                var urun = utsResponse.UrunListesi[0];
                await SQLCrud.LogErrorAsync(
                    $"[UTSQuery] Ürün bulundu. UNO: {model.UNO}, LNO: {model.LNO}. UTS Adet: {urun.Adet}",
                    currentCustomerId);

                #endregion



                #region === 5. KONTÖR DÜŞME ===

                int newCount = currentCount - DebitAmount;

                string updateQuery = @"
                    UPDATE dbo.Customers 
                    SET Count = @newCount, UpdatedDate = GETDATE() 
                    WHERE ID = @id";

                var updateParams = new Dictionary<string, object>
                {
                    { "@newCount", newCount },
                    { "@id", currentCustomerId }
                };

                bool debitSuccess = await SQLCrud.ExecuteCrudAsync(updateQuery, updateParams);

                if (!debitSuccess)
                {
                    await SQLCrud.LogErrorAsync(
                        $"[UTSQuery] KRİTİK: Kontör düşülemedi! Müşteri: {customerName}",
                        currentCustomerId);

                    return StatusCode(500, new
                    {
                        success = false,
                        message = "Sorgu başarılı ancak kontör düşülürken hata oluştu.",
                        mevcutKontor = currentCount
                    });
                }

                await SQLCrud.LogErrorAsync(
                    $"[UTSQuery] Kontör düşüldü. Müşteri: {customerName}. Eski: {currentCount}, Yeni: {newCount}",
                    currentCustomerId);

                #endregion

                #region === 6. BAŞARILI YANIT ===

                return Ok(new
                {
                    success = true,
                    message = "Sorgu başarılı.",
                    urun = new
                    {
                        uno = urun.Uno,
                        lno = urun.Lno,
                        utsStokMiktari = urun.Adet,
                        toplamKullanilabilirAdet = urun.ToplamKullanilabilirAdet,
                        kalanKullanilabilirAdet = urun.KullanilabilirKalanAdet,
                        uretimTarihi = urun.UretimTarihiString,
                        sonKullanmaTarihi = urun.SonKullanmaTarihiString,
                        urunTipi = urun.UrunTipi,
                        markaModel = urun.MarkaModelEtiket
                    },
                    istenenAdet = model.Adet,
                    oncekiKontor = currentCount,
                    dusulenKontor = DebitAmount,
                    kalanKontor = newCount
                });

                #endregion
            }
            catch (Exception ex)
            {
                await SQLCrud.LogErrorAsync($"[UTSQuery] Beklenmeyen hata: {ex.Message} | Detay: {ex.StackTrace}", currentCustomerId);
                return StatusCode(500, new { success = false, message = "Beklenmeyen bir hata oluştu. Lütfen logları kontrol edin." });
            }
        }
    }
}