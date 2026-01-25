// LoginForm.Designer.cs - Modern Tasarım
// Mevcut yapıyı koruyarak görsel iyileştirmeler

namespace UTSLogo.Forms
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.pnl_Header = new System.Windows.Forms.Panel();
            this.lbl_Title = new DevExpress.XtraEditors.LabelControl();
            this.btn_Close = new DevExpress.XtraEditors.SimpleButton();
            this.lbl_UserName = new DevExpress.XtraEditors.LabelControl();
            this.txt_UserName = new DevExpress.XtraEditors.TextEdit();
            this.lbl_Password = new DevExpress.XtraEditors.LabelControl();
            this.txt_Password = new DevExpress.XtraEditors.TextEdit();
            this.btn_Eyes = new DevExpress.XtraEditors.SimpleButton();
            this.btn_NotEye = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Login = new DevExpress.XtraEditors.SimpleButton();
            this.lbl_Version = new DevExpress.XtraEditors.LabelControl();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pnl_Header.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_UserName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Password.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pnl_Header
            // 
            this.pnl_Header.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(126)))), ((int)(((byte)(34)))));
            this.pnl_Header.Controls.Add(this.lbl_Title);
            this.pnl_Header.Controls.Add(this.btn_Close);
            this.pnl_Header.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.pnl_Header.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnl_Header.Location = new System.Drawing.Point(0, 0);
            this.pnl_Header.Name = "pnl_Header";
            this.pnl_Header.Size = new System.Drawing.Size(420, 45);
            this.pnl_Header.TabIndex = 0;
            this.pnl_Header.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnl_Header_MouseDown);
            // 
            // lbl_Title
            // 
            this.lbl_Title.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 11F);
            this.lbl_Title.Appearance.ForeColor = System.Drawing.Color.White;
            this.lbl_Title.Appearance.Options.UseFont = true;
            this.lbl_Title.Appearance.Options.UseForeColor = true;
            this.lbl_Title.Location = new System.Drawing.Point(15, 12);
            this.lbl_Title.Name = "lbl_Title";
            this.lbl_Title.Size = new System.Drawing.Size(215, 20);
            this.lbl_Title.TabIndex = 0;
            this.lbl_Title.Text = "🏢 Mutlu Yazılım | UTS Aktarım";
            this.lbl_Title.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbl_Title_MouseDown);
            // 
            // btn_Close
            // 
            this.btn_Close.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.btn_Close.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Close.Appearance.ForeColor = System.Drawing.Color.White;
            this.btn_Close.Appearance.Options.UseBackColor = true;
            this.btn_Close.Appearance.Options.UseFont = true;
            this.btn_Close.Appearance.Options.UseForeColor = true;
            this.btn_Close.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btn_Close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Close.Location = new System.Drawing.Point(380, 5);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(35, 35);
            this.btn_Close.TabIndex = 1;
            this.btn_Close.Text = "✕";
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // lbl_UserName
            // 
            this.lbl_UserName.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbl_UserName.Appearance.ForeColor = System.Drawing.Color.Black;
            this.lbl_UserName.Appearance.Options.UseFont = true;
            this.lbl_UserName.Appearance.Options.UseForeColor = true;
            this.lbl_UserName.Location = new System.Drawing.Point(57, 149);
            this.lbl_UserName.Name = "lbl_UserName";
            this.lbl_UserName.Size = new System.Drawing.Size(82, 17);
            this.lbl_UserName.TabIndex = 2;
            this.lbl_UserName.Text = "Kullanıcı Adı:";
            // 
            // txt_UserName
            // 
            this.txt_UserName.Location = new System.Drawing.Point(57, 174);
            this.txt_UserName.Name = "txt_UserName";
            this.txt_UserName.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.txt_UserName.Properties.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(126)))), ((int)(((byte)(34)))));
            this.txt_UserName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txt_UserName.Properties.Appearance.ForeColor = System.Drawing.Color.White;
            this.txt_UserName.Properties.Appearance.Options.UseBackColor = true;
            this.txt_UserName.Properties.Appearance.Options.UseBorderColor = true;
            this.txt_UserName.Properties.Appearance.Options.UseFont = true;
            this.txt_UserName.Properties.Appearance.Options.UseForeColor = true;
            this.txt_UserName.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txt_UserName.Properties.NullValuePrompt = "Kullanıcı adınızı giriniz...";
            this.txt_UserName.Size = new System.Drawing.Size(280, 26);
            this.txt_UserName.TabIndex = 3;
            // 
            // lbl_Password
            // 
            this.lbl_Password.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbl_Password.Appearance.ForeColor = System.Drawing.Color.Black;
            this.lbl_Password.Appearance.Options.UseFont = true;
            this.lbl_Password.Appearance.Options.UseForeColor = true;
            this.lbl_Password.Location = new System.Drawing.Point(57, 219);
            this.lbl_Password.Name = "lbl_Password";
            this.lbl_Password.Size = new System.Drawing.Size(32, 17);
            this.lbl_Password.TabIndex = 4;
            this.lbl_Password.Text = "Şifre:";
            // 
            // txt_Password
            // 
            this.txt_Password.Location = new System.Drawing.Point(57, 244);
            this.txt_Password.Name = "txt_Password";
            this.txt_Password.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.txt_Password.Properties.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(126)))), ((int)(((byte)(34)))));
            this.txt_Password.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txt_Password.Properties.Appearance.ForeColor = System.Drawing.Color.White;
            this.txt_Password.Properties.Appearance.Options.UseBackColor = true;
            this.txt_Password.Properties.Appearance.Options.UseBorderColor = true;
            this.txt_Password.Properties.Appearance.Options.UseFont = true;
            this.txt_Password.Properties.Appearance.Options.UseForeColor = true;
            this.txt_Password.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txt_Password.Properties.NullValuePrompt = "Şifrenizi giriniz...";
            this.txt_Password.Properties.PasswordChar = '*';
            this.txt_Password.Size = new System.Drawing.Size(280, 26);
            this.txt_Password.TabIndex = 5;
            // 
            // btn_Eyes
            // 
            this.btn_Eyes.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btn_Eyes.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btn_Eyes.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(126)))), ((int)(((byte)(34)))));
            this.btn_Eyes.Appearance.Options.UseBackColor = true;
            this.btn_Eyes.Appearance.Options.UseFont = true;
            this.btn_Eyes.Appearance.Options.UseForeColor = true;
            this.btn_Eyes.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btn_Eyes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Eyes.Location = new System.Drawing.Point(341, 244);
            this.btn_Eyes.Name = "btn_Eyes";
            this.btn_Eyes.Size = new System.Drawing.Size(36, 26);
            this.btn_Eyes.TabIndex = 6;
            this.btn_Eyes.Text = "👁";
            this.btn_Eyes.Click += new System.EventHandler(this.btn_Eyes_Click);
            // 
            // btn_NotEye
            // 
            this.btn_NotEye.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btn_NotEye.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btn_NotEye.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(126)))), ((int)(((byte)(34)))));
            this.btn_NotEye.Appearance.Options.UseBackColor = true;
            this.btn_NotEye.Appearance.Options.UseFont = true;
            this.btn_NotEye.Appearance.Options.UseForeColor = true;
            this.btn_NotEye.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btn_NotEye.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_NotEye.Location = new System.Drawing.Point(341, 244);
            this.btn_NotEye.Name = "btn_NotEye";
            this.btn_NotEye.Size = new System.Drawing.Size(36, 26);
            this.btn_NotEye.TabIndex = 7;
            this.btn_NotEye.Text = "🔒";
            this.btn_NotEye.Visible = false;
            this.btn_NotEye.Click += new System.EventHandler(this.btn_NotEye_Click);
            // 
            // btn_Login
            // 
            this.btn_Login.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(126)))), ((int)(((byte)(34)))));
            this.btn_Login.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.btn_Login.Appearance.ForeColor = System.Drawing.Color.White;
            this.btn_Login.Appearance.Options.UseBackColor = true;
            this.btn_Login.Appearance.Options.UseFont = true;
            this.btn_Login.Appearance.Options.UseForeColor = true;
            this.btn_Login.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btn_Login.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Login.Location = new System.Drawing.Point(48, 298);
            this.btn_Login.Name = "btn_Login";
            this.btn_Login.Size = new System.Drawing.Size(320, 45);
            this.btn_Login.TabIndex = 8;
            this.btn_Login.Text = "  😊  Gülümse";
            this.btn_Login.Click += new System.EventHandler(this.btn_Login_Click);
            // 
            // lbl_Version
            // 
            this.lbl_Version.Appearance.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lbl_Version.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(140)))), ((int)(((byte)(141)))));
            this.lbl_Version.Appearance.Options.UseFont = true;
            this.lbl_Version.Appearance.Options.UseForeColor = true;
            this.lbl_Version.Location = new System.Drawing.Point(130, 360);
            this.lbl_Version.Name = "lbl_Version";
            this.lbl_Version.Size = new System.Drawing.Size(146, 13);
            this.lbl_Version.TabIndex = 9;
            this.lbl_Version.Text = "v1.1.0 | © 2026 Mutlu Yazılım";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(108, 68);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(213, 61);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // LoginForm
            // 
            this.AcceptButton = this.btn_Login;
            this.CancelButton = this.btn_Close;
            this.ClientSize = new System.Drawing.Size(420, 390);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pnl_Header);
            this.Controls.Add(this.lbl_UserName);
            this.Controls.Add(this.txt_UserName);
            this.Controls.Add(this.lbl_Password);
            this.Controls.Add(this.txt_Password);
            this.Controls.Add(this.btn_Eyes);
            this.Controls.Add(this.btn_NotEye);
            this.Controls.Add(this.btn_Login);
            this.Controls.Add(this.lbl_Version);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.IconOptions.Image = ((System.Drawing.Image)(resources.GetObject("LoginForm.IconOptions.Image")));
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mutlu Yazılım | UTS Aktarım | V1.0.1";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.pnl_Header.ResumeLayout(false);
            this.pnl_Header.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_UserName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Password.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        // Kontrol tanımlamaları
        private System.Windows.Forms.Panel pnl_Header;
        private DevExpress.XtraEditors.LabelControl lbl_Title;
        private DevExpress.XtraEditors.SimpleButton btn_Close;
        private DevExpress.XtraEditors.LabelControl lbl_UserName;
        private DevExpress.XtraEditors.TextEdit txt_UserName;
        private DevExpress.XtraEditors.LabelControl lbl_Password;
        private DevExpress.XtraEditors.TextEdit txt_Password;
        private DevExpress.XtraEditors.SimpleButton btn_Eyes;
        private DevExpress.XtraEditors.SimpleButton btn_NotEye;
        private DevExpress.XtraEditors.SimpleButton btn_Login;
        private DevExpress.XtraEditors.LabelControl lbl_Version;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}