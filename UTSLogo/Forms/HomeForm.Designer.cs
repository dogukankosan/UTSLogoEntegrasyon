namespace UTSLogo.Forms
{
    partial class HomeForm
    {
 
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HomeForm));
            this.accordionControl1 = new DevExpress.XtraBars.Navigation.AccordionControl();
            this.companyChooseControlElement2 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.accordionControlElement1 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.sqlSettingsControlElement1 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.usersControlElement2 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.allListErrorControlElement3 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.UTSControlElement3 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.accordionControlSeparator1 = new DevExpress.XtraBars.Navigation.AccordionControlSeparator();
            this.accordionControlElement2 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.themaControlElement3 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.UsererrorListControlElement3 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.accordionControlSeparator2 = new DevExpress.XtraBars.Navigation.AccordionControlSeparator();
            this.invoicesControlElement2 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.salesInvoicesControlElement2 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.salesTruckControlElement3 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.popupMenu2 = new DevExpress.XtraBars.PopupMenu(this.components);
            this.skinBarSubItem1 = new DevExpress.XtraBars.SkinBarSubItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.accordionControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupMenu2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // accordionControl1
            // 
            this.accordionControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.accordionControl1.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] {
            this.companyChooseControlElement2,
            this.accordionControlElement1,
            this.accordionControlSeparator1,
            this.accordionControlElement2,
            this.accordionControlSeparator2,
            this.invoicesControlElement2});
            this.accordionControl1.Location = new System.Drawing.Point(0, 0);
            this.accordionControl1.Name = "accordionControl1";
            this.accordionControl1.ScrollBarMode = DevExpress.XtraBars.Navigation.ScrollBarMode.Fluent;
            this.accordionControl1.Size = new System.Drawing.Size(335, 567);
            this.accordionControl1.TabIndex = 0;
            this.accordionControl1.Text = "accordionControl1";
            this.accordionControl1.ViewType = DevExpress.XtraBars.Navigation.AccordionControlViewType.HamburgerMenu;
            // 
            // companyChooseControlElement2
            // 
            this.companyChooseControlElement2.HeaderTemplate.AddRange(new DevExpress.XtraBars.Navigation.HeaderElementInfo[] {
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.Text),
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.Image),
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.HeaderControl),
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.ContextButtons)});
            this.companyChooseControlElement2.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("companyChooseControlElement2.ImageOptions.Image")));
            this.companyChooseControlElement2.Name = "companyChooseControlElement2";
            this.companyChooseControlElement2.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.companyChooseControlElement2.Text = "Şirket Seçiniz";
            this.companyChooseControlElement2.Click += new System.EventHandler(this.companyChooseControlElement2_Click);
            // 
            // accordionControlElement1
            // 
            this.accordionControlElement1.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] {
            this.sqlSettingsControlElement1,
            this.usersControlElement2,
            this.allListErrorControlElement3,
            this.UTSControlElement3});
            this.accordionControlElement1.Expanded = true;
            this.accordionControlElement1.Name = "accordionControlElement1";
            this.accordionControlElement1.Text = "Sistem Ayarları";
            // 
            // sqlSettingsControlElement1
            // 
            this.sqlSettingsControlElement1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("sqlSettingsControlElement1.ImageOptions.Image")));
            this.sqlSettingsControlElement1.Name = "sqlSettingsControlElement1";
            this.sqlSettingsControlElement1.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.sqlSettingsControlElement1.Text = "SQL Ayarları";
            this.sqlSettingsControlElement1.Click += new System.EventHandler(this.sqlSettingsControlElement1_Click);
            // 
            // usersControlElement2
            // 
            this.usersControlElement2.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("usersControlElement2.ImageOptions.Image")));
            this.usersControlElement2.Name = "usersControlElement2";
            this.usersControlElement2.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.usersControlElement2.Text = "Kullanıcılar";
            this.usersControlElement2.Click += new System.EventHandler(this.usersControlElement2_Click_1);
            // 
            // allListErrorControlElement3
            // 
            this.allListErrorControlElement3.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("allListErrorControlElement3.ImageOptions.Image")));
            this.allListErrorControlElement3.Name = "allListErrorControlElement3";
            this.allListErrorControlElement3.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.allListErrorControlElement3.Text = "Tüm Hata Kayıtları";
            this.allListErrorControlElement3.Click += new System.EventHandler(this.allListErrorControlElement3_Click);
            // 
            // UTSControlElement3
            // 
            this.UTSControlElement3.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("UTSControlElement3.ImageOptions.Image")));
            this.UTSControlElement3.Name = "UTSControlElement3";
            this.UTSControlElement3.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.UTSControlElement3.Text = "UTS";
            this.UTSControlElement3.Click += new System.EventHandler(this.UTSControlElement3_Click);
            // 
            // accordionControlSeparator1
            // 
            this.accordionControlSeparator1.Name = "accordionControlSeparator1";
            // 
            // accordionControlElement2
            // 
            this.accordionControlElement2.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] {
            this.themaControlElement3,
            this.UsererrorListControlElement3});
            this.accordionControlElement2.Expanded = true;
            this.accordionControlElement2.Name = "accordionControlElement2";
            this.accordionControlElement2.Text = "Kullanıcı Ayarları";
            // 
            // themaControlElement3
            // 
            this.themaControlElement3.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("themaControlElement3.ImageOptions.Image")));
            this.themaControlElement3.Name = "themaControlElement3";
            this.themaControlElement3.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.themaControlElement3.Text = "Temalar";
            this.themaControlElement3.Click += new System.EventHandler(this.themaControlElement3_Click);
            // 
            // UsererrorListControlElement3
            // 
            this.UsererrorListControlElement3.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("UsererrorListControlElement3.ImageOptions.Image")));
            this.UsererrorListControlElement3.Name = "UsererrorListControlElement3";
            this.UsererrorListControlElement3.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.UsererrorListControlElement3.Text = "Hata Kayıtları";
            this.UsererrorListControlElement3.Click += new System.EventHandler(this.UsererrorListControlElement3_Click);
            // 
            // accordionControlSeparator2
            // 
            this.accordionControlSeparator2.Name = "accordionControlSeparator2";
            // 
            // invoicesControlElement2
            // 
            this.invoicesControlElement2.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] {
            this.salesInvoicesControlElement2,
            this.salesTruckControlElement3});
            this.invoicesControlElement2.Expanded = true;
            this.invoicesControlElement2.Name = "invoicesControlElement2";
            this.invoicesControlElement2.Text = "İşlemler";
            // 
            // salesInvoicesControlElement2
            // 
            this.salesInvoicesControlElement2.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("salesInvoicesControlElement2.ImageOptions.Image")));
            this.salesInvoicesControlElement2.Name = "salesInvoicesControlElement2";
            this.salesInvoicesControlElement2.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.salesInvoicesControlElement2.Text = "Satış Faturaları";
            this.salesInvoicesControlElement2.Click += new System.EventHandler(this.salesInvoicesControlElement2_Click);
            // 
            // salesTruckControlElement3
            // 
            this.salesTruckControlElement3.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("salesTruckControlElement3.ImageOptions.Image")));
            this.salesTruckControlElement3.Name = "salesTruckControlElement3";
            this.salesTruckControlElement3.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.salesTruckControlElement3.Text = "Satış İrsaliyeleri";
            this.salesTruckControlElement3.Click += new System.EventHandler(this.salesTruckControlElement3_Click);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.pictureBox1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(335, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1242, 567);
            this.panelControl1.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(2, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1238, 563);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // popupMenu2
            // 
            this.popupMenu2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.skinBarSubItem1)});
            this.popupMenu2.Manager = this.barManager1;
            this.popupMenu2.Name = "popupMenu2";
            // 
            // skinBarSubItem1
            // 
            this.skinBarSubItem1.Caption = "Tema";
            this.skinBarSubItem1.Id = 0;
            this.skinBarSubItem1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("skinBarSubItem1.ImageOptions.Image")));
            this.skinBarSubItem1.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("skinBarSubItem1.ImageOptions.LargeImage")));
            this.skinBarSubItem1.Name = "skinBarSubItem1";
            // 
            // barManager1
            // 
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.skinBarSubItem1});
            this.barManager1.MaxItemId = 1;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Size = new System.Drawing.Size(1577, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 567);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Size = new System.Drawing.Size(1577, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 567);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1577, 0);
            this.barDockControlRight.Manager = this.barManager1;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 567);
            // 
            // HomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1577, 567);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.accordionControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.IconOptions.Image = ((System.Drawing.Image)(resources.GetObject("HomeForm.IconOptions.Image")));
            this.Name = "HomeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mutlu Yazılım | UTS Aktarım | V1.1.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HomeForm_FormClosing);
            this.Load += new System.EventHandler(this.HomeForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.accordionControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupMenu2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Navigation.AccordionControl accordionControl1;
        private DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement1;
        private DevExpress.XtraBars.Navigation.AccordionControlElement sqlSettingsControlElement1;
        private DevExpress.XtraBars.Navigation.AccordionControlElement invoicesControlElement2;
        private DevExpress.XtraBars.Navigation.AccordionControlElement usersControlElement2;
        private DevExpress.XtraBars.Navigation.AccordionControlElement salesInvoicesControlElement2;
        private DevExpress.XtraBars.Navigation.AccordionControlElement companyChooseControlElement2;
        private DevExpress.XtraBars.Navigation.AccordionControlSeparator accordionControlSeparator1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement2;
        private DevExpress.XtraBars.Navigation.AccordionControlElement themaControlElement3;
        private DevExpress.XtraBars.Navigation.AccordionControlSeparator accordionControlSeparator2;
        private DevExpress.XtraBars.Navigation.AccordionControlElement allListErrorControlElement3;
        private DevExpress.XtraBars.Navigation.AccordionControlElement UsererrorListControlElement3;
        private DevExpress.XtraBars.PopupMenu popupMenu2;
        private DevExpress.XtraBars.SkinBarSubItem skinBarSubItem1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.Navigation.AccordionControlElement UTSControlElement3;
        private DevExpress.XtraBars.Navigation.AccordionControlElement salesTruckControlElement3;
    }
}