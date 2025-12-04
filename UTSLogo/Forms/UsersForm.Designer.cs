namespace UTSLogo.Forms
{
    partial class UsersForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UsersForm));
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.btn_Clear = new DevExpress.XtraEditors.SimpleButton();
            this.chk_Status = new DevExpress.XtraEditors.CheckEdit();
            this.btn_Eyes = new DevExpress.XtraEditors.SimpleButton();
            this.btn_NotEye = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Save = new DevExpress.XtraEditors.SimpleButton();
            this.chc_IsAdmin = new DevExpress.XtraEditors.CheckEdit();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_Password = new DevExpress.XtraEditors.TextEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_userName = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chk_Status.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chc_IsAdmin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Password.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_userName.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // groupControl1
            // 
            this.groupControl1.CaptionImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("groupControl1.CaptionImageOptions.Image")));
            this.groupControl1.Controls.Add(this.gridControl1);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(405, 483);
            this.groupControl1.TabIndex = 9;
            this.groupControl1.Text = "Kullanıcı Listesi";
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(2, 33);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(401, 448);
            this.gridControl1.TabIndex = 7;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            // 
            // groupControl2
            // 
            this.groupControl2.CaptionImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("groupControl2.CaptionImageOptions.Image")));
            this.groupControl2.Controls.Add(this.btn_Clear);
            this.groupControl2.Controls.Add(this.chk_Status);
            this.groupControl2.Controls.Add(this.btn_Eyes);
            this.groupControl2.Controls.Add(this.btn_NotEye);
            this.groupControl2.Controls.Add(this.btn_Save);
            this.groupControl2.Controls.Add(this.chc_IsAdmin);
            this.groupControl2.Controls.Add(this.label2);
            this.groupControl2.Controls.Add(this.txt_Password);
            this.groupControl2.Controls.Add(this.label1);
            this.groupControl2.Controls.Add(this.txt_userName);
            this.groupControl2.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupControl2.Location = new System.Drawing.Point(405, 0);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(305, 483);
            this.groupControl2.TabIndex = 10;
            this.groupControl2.Text = "Kullanıcı İşlemi";
            // 
            // btn_Clear
            // 
            this.btn_Clear.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.btn_Clear.Appearance.Options.UseBackColor = true;
            this.btn_Clear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Clear.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_Clear.ImageOptions.Image")));
            this.btn_Clear.Location = new System.Drawing.Point(269, 0);
            this.btn_Clear.Name = "btn_Clear";
            this.btn_Clear.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btn_Clear.Size = new System.Drawing.Size(36, 29);
            this.btn_Clear.TabIndex = 11;
            this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
            // 
            // chk_Status
            // 
            this.chk_Status.EditValue = true;
            this.chk_Status.Location = new System.Drawing.Point(15, 191);
            this.chk_Status.Name = "chk_Status";
            this.chk_Status.Properties.Caption = "Aktif Mi ?";
            this.chk_Status.Size = new System.Drawing.Size(221, 20);
            this.chk_Status.TabIndex = 5;
            // 
            // btn_Eyes
            // 
            this.btn_Eyes.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.btn_Eyes.Appearance.Options.UseBackColor = true;
            this.btn_Eyes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Eyes.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_Eyes.ImageOptions.Image")));
            this.btn_Eyes.Location = new System.Drawing.Point(243, 124);
            this.btn_Eyes.Name = "btn_Eyes";
            this.btn_Eyes.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btn_Eyes.Size = new System.Drawing.Size(36, 29);
            this.btn_Eyes.TabIndex = 2;
            this.btn_Eyes.Click += new System.EventHandler(this.btn_Eyes_Click);
            // 
            // btn_NotEye
            // 
            this.btn_NotEye.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.btn_NotEye.Appearance.Options.UseBackColor = true;
            this.btn_NotEye.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_NotEye.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_NotEye.ImageOptions.Image")));
            this.btn_NotEye.Location = new System.Drawing.Point(242, 124);
            this.btn_NotEye.Name = "btn_NotEye";
            this.btn_NotEye.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btn_NotEye.Size = new System.Drawing.Size(36, 30);
            this.btn_NotEye.TabIndex = 3;
            this.btn_NotEye.Click += new System.EventHandler(this.btn_NotEye_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Success;
            this.btn_Save.Appearance.Font = new System.Drawing.Font("Tahoma", 15.25F);
            this.btn_Save.Appearance.Options.UseBackColor = true;
            this.btn_Save.Appearance.Options.UseFont = true;
            this.btn_Save.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Save.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btn_Save.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_Save.ImageOptions.Image")));
            this.btn_Save.Location = new System.Drawing.Point(2, 434);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(301, 47);
            this.btn_Save.TabIndex = 6;
            this.btn_Save.Text = "Kaydet";
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // chc_IsAdmin
            // 
            this.chc_IsAdmin.Location = new System.Drawing.Point(15, 165);
            this.chc_IsAdmin.Name = "chc_IsAdmin";
            this.chc_IsAdmin.Properties.Caption = "Admin Mi ?";
            this.chc_IsAdmin.Size = new System.Drawing.Size(221, 20);
            this.chc_IsAdmin.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label2.Location = new System.Drawing.Point(16, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "Kullanıcı Şifre:";
            // 
            // txt_Password
            // 
            this.txt_Password.Location = new System.Drawing.Point(16, 129);
            this.txt_Password.Name = "txt_Password";
            this.txt_Password.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.txt_Password.Properties.Appearance.Options.UseFont = true;
            this.txt_Password.Properties.MaxLength = 100;
            this.txt_Password.Properties.PasswordChar = '*';
            this.txt_Password.Size = new System.Drawing.Size(221, 24);
            this.txt_Password.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label1.Location = new System.Drawing.Point(12, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Kullanıcı Adı:";
            // 
            // txt_userName
            // 
            this.txt_userName.Location = new System.Drawing.Point(15, 72);
            this.txt_userName.Name = "txt_userName";
            this.txt_userName.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.txt_userName.Properties.Appearance.Options.UseFont = true;
            this.txt_userName.Properties.MaxLength = 100;
            this.txt_userName.Size = new System.Drawing.Size(221, 24);
            this.txt_userName.TabIndex = 0;
            // 
            // UsersForm
            // 
            this.AcceptButton = this.btn_Save;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(714, 483);
            this.Controls.Add(this.groupControl2);
            this.Controls.Add(this.groupControl1);
            this.IconOptions.Image = ((System.Drawing.Image)(resources.GetObject("UsersForm.IconOptions.Image")));
            this.MaximizeBox = false;
            this.Name = "UsersForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Kullanıcı Listesi";
            this.Load += new System.EventHandler(this.UsersForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            this.groupControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chk_Status.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chc_IsAdmin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Password.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_userName.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.TextEdit txt_userName;
        private DevExpress.XtraEditors.CheckEdit chc_IsAdmin;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraEditors.TextEdit txt_Password;
        private DevExpress.XtraEditors.SimpleButton btn_Save;
        private DevExpress.XtraEditors.SimpleButton btn_Eyes;
        private DevExpress.XtraEditors.SimpleButton btn_NotEye;
        private DevExpress.XtraEditors.CheckEdit chk_Status;
        private DevExpress.XtraEditors.SimpleButton btn_Clear;
    }
}