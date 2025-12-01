namespace UTSLogo.Forms
{
    partial class UTSForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UTSForm));
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.nmr_Count = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_CustomerName = new DevExpress.XtraEditors.TextEdit();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_CustomerToken = new DevExpress.XtraEditors.TextEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_Token = new DevExpress.XtraEditors.TextEdit();
            this.btn_Save = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmr_Count)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CustomerName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CustomerToken.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Token.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // groupControl1
            // 
            this.groupControl1.CaptionImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("groupControl1.CaptionImageOptions.Image")));
            this.groupControl1.Controls.Add(this.panel1);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(608, 559);
            this.groupControl1.TabIndex = 3;
            this.groupControl1.Text = "UTS Bilgileri";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.nmr_Count);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txt_CustomerName);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txt_CustomerToken);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txt_Token);
            this.panel1.Controls.Add(this.btn_Save);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(2, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(276, 524);
            this.panel1.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label4.Location = new System.Drawing.Point(21, 186);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 17);
            this.label4.TabIndex = 21;
            this.label4.Text = "Müşteri Kontör:";
            // 
            // nmr_Count
            // 
            this.nmr_Count.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.nmr_Count.Location = new System.Drawing.Point(24, 208);
            this.nmr_Count.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nmr_Count.Name = "nmr_Count";
            this.nmr_Count.Size = new System.Drawing.Size(218, 24);
            this.nmr_Count.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label3.Location = new System.Drawing.Point(21, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 17);
            this.label3.TabIndex = 11;
            this.label3.Text = "Müşteri Adı:";
            // 
            // txt_CustomerName
            // 
            this.txt_CustomerName.Location = new System.Drawing.Point(21, 34);
            this.txt_CustomerName.Name = "txt_CustomerName";
            this.txt_CustomerName.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.txt_CustomerName.Properties.Appearance.Options.UseFont = true;
            this.txt_CustomerName.Properties.MaxLength = 250;
            this.txt_CustomerName.Size = new System.Drawing.Size(221, 24);
            this.txt_CustomerName.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label2.Location = new System.Drawing.Point(21, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "Müşteri Token:";
            // 
            // txt_CustomerToken
            // 
            this.txt_CustomerToken.Location = new System.Drawing.Point(21, 93);
            this.txt_CustomerToken.Name = "txt_CustomerToken";
            this.txt_CustomerToken.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.txt_CustomerToken.Properties.Appearance.Options.UseFont = true;
            this.txt_CustomerToken.Properties.MaxLength = 250;
            this.txt_CustomerToken.Size = new System.Drawing.Size(221, 24);
            this.txt_CustomerToken.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label1.Location = new System.Drawing.Point(21, 129);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "UTS Token:";
            // 
            // txt_Token
            // 
            this.txt_Token.Location = new System.Drawing.Point(21, 149);
            this.txt_Token.Name = "txt_Token";
            this.txt_Token.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.txt_Token.Properties.Appearance.Options.UseFont = true;
            this.txt_Token.Properties.MaxLength = 250;
            this.txt_Token.Size = new System.Drawing.Size(221, 24);
            this.txt_Token.TabIndex = 2;
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
            this.btn_Save.Location = new System.Drawing.Point(0, 475);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(274, 47);
            this.btn_Save.TabIndex = 4;
            this.btn_Save.Text = "Kaydet";
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // UTSForm
            // 
            this.AcceptButton = this.btn_Save;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(608, 559);
            this.Controls.Add(this.groupControl1);
            this.IconOptions.Image = ((System.Drawing.Image)(resources.GetObject("UTSForm.IconOptions.Image")));
            this.MaximizeBox = false;
            this.Name = "UTSForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UTS Bilgileri";
            this.Load += new System.EventHandler(this.UTSForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmr_Count)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CustomerName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CustomerToken.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Token.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupControl1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.TextEdit txt_Token;
        private DevExpress.XtraEditors.SimpleButton btn_Save;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nmr_Count;
        private System.Windows.Forms.Label label3;
        private DevExpress.XtraEditors.TextEdit txt_CustomerName;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraEditors.TextEdit txt_CustomerToken;
    }
}