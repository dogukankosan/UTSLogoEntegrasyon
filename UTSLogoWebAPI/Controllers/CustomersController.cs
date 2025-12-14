using Microsoft.AspNetCore.Mvc;
using System.Data;
using UTSLogoWebAPI.Classes;
using UTSLogoWebAPI.Models;

namespace UTSLogoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        #region ==================== MÜŞTERİ KAYIT ====================

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] CustomerRegisterModel model)
        {
            if (model == null)
                return BadRequest(new { success = false, message = "Geçersiz istek." });
            if (string.IsNullOrWhiteSpace(model.CustomerName))
                return BadRequest(new { success = false, message = "Müşteri adı boş olamaz." });
            if (model.Count < 0)
                return BadRequest(new { success = false, message = "Kontör 0'dan küçük olamaz." });
            string name = model.CustomerName.Trim();
            try
            {
                string checkQuery = "SELECT ID FROM dbo.Customers WHERE CustomerName = @name";
                Dictionary<string, object> checkParams = new Dictionary<string, object> { { "@name", name } };
                object exists = await SQLCrud.ExecuteScalarAsync(checkQuery, checkParams);
                if (exists != null && exists != DBNull.Value)
                    return Conflict(new { success = false, message = "Bu müşteri adı zaten kullanılıyor." });
                Guid guid = Guid.NewGuid();
                string insertQuery = @"
                    INSERT INTO dbo.Customers 
                    (CustomerGUID, CustomerName, UTSToken, Count, Status, CreatedDate)
                    VALUES (@guid, @name, @token, @count, 1, GETDATE())";
                Dictionary<string, object> insertParams = new Dictionary<string, object>
                {
                    { "@guid", guid },
                    { "@name", name },
                    { "@token", model.UTSToken ?? string.Empty },
                    { "@count", model.Count }
                };
                bool result = await SQLCrud.ExecuteCrudAsync(insertQuery, insertParams);
                if (!result)
                    return StatusCode(500, new { success = false, message = "Kayıt işlemi başarısız." });
                return Ok(new
                {
                    success = true,
                    customerGUID = guid.ToString(),
                    message = "Müşteri başarıyla kaydedildi."
                });
            }
            catch (Exception ex)
            {
                await SQLCrud.LogErrorAsync($"[Register] Hata: {ex.Message}", 0);
                return StatusCode(500, new { success = false, message = "Hata oluştu." });
            }
        }
        #endregion

        #region ==================== MÜŞTERİ BİLGİ ====================

        [HttpGet("Info")]
        public async Task<IActionResult> GetInfo([FromQuery] string guid)
        {
            if (!Guid.TryParse(guid, out Guid customerGUID))
                return BadRequest(new { success = false, message = "Geçerli GUID gereklidir." });
            try
            {
                string query = @"
                    SELECT CustomerName, Count, UTSToken, Status
                    FROM dbo.Customers
                    WHERE CustomerGUID = @guid AND Status = 1";
                Dictionary<string, object> param = new Dictionary<string, object> { { "@guid", customerGUID } };
                DataTable dt = await SQLCrud.GetDataTableAsync(query, param);
                if (dt == null || dt.Rows.Count == 0)
                    return NotFound(new { success = false, message = "Aktif müşteri bulunamadı." });
                DataRow row = dt.Rows[0];
                return Ok(new
                {
                    success = true,
                    customerName = row["CustomerName"].ToString(),
                    count = Convert.ToInt32(row["Count"]),
                    utsToken = row["UTSToken"].ToString(),
                    status = Convert.ToBoolean(row["Status"])
                });
            }
            catch (Exception ex)
            {
                await SQLCrud.LogErrorAsync($"[Info] Hata: {ex.Message}", 0);
                return StatusCode(500, new { success = false, message = "Hata oluştu." });
            }
        }
        #endregion

        #region ==================== KONTÖR İŞLEMLERİ ====================

        [HttpPut("DeductCount")]
        public async Task<IActionResult> DeductCount([FromBody] CountOperationModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.CustomerGUID))
                return BadRequest(new { success = false, message = "Geçersiz istek." });
            if (!Guid.TryParse(model.CustomerGUID, out Guid guid))
                return BadRequest(new { success = false, message = "Geçerli GUID gereklidir." });
            if (model.Amount <= 0)
                return BadRequest(new { success = false, message = "Miktar 0'dan büyük olmalıdır." });
            try
            {
                string q = "SELECT ID, Count FROM dbo.Customers WHERE CustomerGUID = @guid AND Status = 1";
                Dictionary<string, object> p = new Dictionary<string, object> { { "@guid", guid } };
                DataTable dt = await SQLCrud.GetDataTableAsync(q, p);
                if (dt == null || dt.Rows.Count == 0)
                    return NotFound(new { success = false, message = "Müşteri bulunamadı." });
                int current = Convert.ToInt32(dt.Rows[0]["Count"]);
                int id = Convert.ToInt32(dt.Rows[0]["ID"]);
                if (current < model.Amount)
                    return BadRequest(new { success = false, message = "Yetersiz kontör.", currentCount = current });
                int newCount = current - model.Amount;
                string update = "UPDATE dbo.Customers SET Count = @count, UpdatedDate = GETDATE() WHERE ID = @id";
                Dictionary<string, object> updateParams = new Dictionary<string, object>
                {
                    { "@count", newCount },
                    { "@id", id }
                };
                bool r = await SQLCrud.ExecuteCrudAsync(update, updateParams);
                if (!r)
                    return StatusCode(500, new { success = false, message = "Güncelleme başarısız." });
                return Ok(new
                {
                    success = true,
                    oldCount = current,
                    deducted = model.Amount,
                    newCount = newCount
                });
            }
            catch (Exception ex)
            {
                await SQLCrud.LogErrorAsync($"[DeductCount] Hata: {ex.Message}", 0);
                return StatusCode(500, new { success = false, message = "Hata oluştu." });
            }
        }

        [HttpPut("AddCount")]
        public async Task<IActionResult> AddCount([FromBody] CountOperationModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.CustomerGUID))
                return BadRequest(new { success = false, message = "Geçersiz istek." });
            if (!Guid.TryParse(model.CustomerGUID, out Guid guid))
                return BadRequest(new { success = false, message = "Geçerli GUID gereklidir." });
            if (model.Amount <= 0)
                return BadRequest(new { success = false, message = "Miktar 0'dan büyük olmalıdır." });
            try
            {
                string q = "SELECT ID, Count FROM dbo.Customers WHERE CustomerGUID = @guid AND Status = 1";
                Dictionary<string, object> p = new Dictionary<string, object> { { "@guid", guid } };
                DataTable dt = await SQLCrud.GetDataTableAsync(q, p);
                if (dt == null || dt.Rows.Count == 0)
                    return NotFound(new { success = false, message = "Müşteri bulunamadı." });
                int current = Convert.ToInt32(dt.Rows[0]["Count"]);
                int id = Convert.ToInt32(dt.Rows[0]["ID"]);
                long toplam = (long)current + model.Amount;
                if (toplam > int.MaxValue)
                    return BadRequest(new { success = false, message = "Kontör limiti aşıldı." });
                int newCount = (int)toplam;
                string update = "UPDATE dbo.Customers SET Count = @count, UpdatedDate = GETDATE() WHERE ID = @id";
                Dictionary<string, object> updateParams = new Dictionary<string, object>
                {
                    { "@count", newCount },
                    { "@id", id }
                };
                bool r = await SQLCrud.ExecuteCrudAsync(update, updateParams);
                if (!r)
                    return StatusCode(500, new { success = false, message = "Güncelleme başarısız." });
                return Ok(new
                {
                    success = true,
                    oldCount = current,
                    added = model.Amount,
                    newCount
                });
            }
            catch (Exception ex)
            {
                await SQLCrud.LogErrorAsync($"[AddCount] Hata: {ex.Message}", 0);
                return StatusCode(500, new { success = false, message = "Hata oluştu." });
            }
        }
        #endregion

        #region ==================== TOKEN ====================

        [HttpPut("UpdateUTSToken")]
        public async Task<IActionResult> UpdateUTSToken([FromBody] UTSTokenUpdateModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.CustomerGUID))
                return BadRequest(new { success = false, message = "Geçersiz istek." });
            if (!Guid.TryParse(model.CustomerGUID, out Guid guid))
                return BadRequest(new { success = false, message = "Geçerli GUID gereklidir." });
            if (string.IsNullOrWhiteSpace(model.UTSToken))
                return BadRequest(new { success = false, message = "UTS token boş olamaz." });
            try
            {
                string check = "SELECT ID FROM dbo.Customers WHERE CustomerGUID = @guid AND Status = 1";
                Dictionary<string, object> param = new Dictionary<string, object> { { "@guid", guid } };
                object existing = await SQLCrud.ExecuteScalarAsync(check, param);
                if (existing == null || existing == DBNull.Value)
                    return NotFound(new { success = false, message = "Müşteri bulunamadı." });
                int id = Convert.ToInt32(existing);
                string update = "UPDATE dbo.Customers SET UTSToken = @t, UpdatedDate = GETDATE() WHERE ID = @id";
                Dictionary<string, object> p = new Dictionary<string, object>
                {
                    { "@t", model.UTSToken },
                    { "@id", id }
                };
                bool r = await SQLCrud.ExecuteCrudAsync(update, p);
                if (!r)
                    return StatusCode(500, new { success = false, message = "Güncelleme başarısız." });
                return Ok(new { success = true, message = "Token güncellendi." });
            }
            catch (Exception ex)
            {
                await SQLCrud.LogErrorAsync($"[UpdateUTSToken] Hata: {ex.Message}", 0);
                return StatusCode(500, new { success = false, message = "Hata oluştu." });
            }
        }
        #endregion

        #region ==================== DURUM ====================

        [HttpPut("SetStatus")]
        public async Task<IActionResult> SetStatus([FromBody] StatusUpdateModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.CustomerGUID))
                return BadRequest(new { success = false, message = "Geçersiz istek." });
            if (!Guid.TryParse(model.CustomerGUID, out Guid guid))
                return BadRequest(new { success = false, message = "Geçerli GUID gereklidir." });
            try
            {
                string check = "SELECT ID FROM dbo.Customers WHERE CustomerGUID = @guid";
                Dictionary<string, object> param = new Dictionary<string, object> { { "@guid", guid } };
                object existing = await SQLCrud.ExecuteScalarAsync(check, param);
                if (existing == null || existing == DBNull.Value)
                    return NotFound(new { success = false, message = "Müşteri bulunamadı." });
                int id = Convert.ToInt32(existing);
                string update = "UPDATE dbo.Customers SET Status = @status, UpdatedDate = GETDATE() WHERE ID = @id";
                Dictionary<string, object> p = new Dictionary<string, object>
                {
                    { "@status", model.Status },
                    { "@id", id }
                };
                bool r = await SQLCrud.ExecuteCrudAsync(update, p);
                if (!r)
                    return StatusCode(500, new { success = false, message = "Güncelleme başarısız." });
                return Ok(new { success = true, message = "Durum güncellendi." });
            }
            catch (Exception ex)
            {
                await SQLCrud.LogErrorAsync($"[SetStatus] Hata: {ex.Message}", 0);
                return StatusCode(500, new { success = false, message = "Hata oluştu." });
            }
        }
        #endregion
    }
}