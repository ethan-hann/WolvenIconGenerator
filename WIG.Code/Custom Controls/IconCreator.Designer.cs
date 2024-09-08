namespace WolvenIconGenerator.Custom_Controls
{
    partial class IconCreator
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IconCreator));
            iconTabs = new TabControl();
            tabImportImage = new TabPage();
            tableLayoutPanel2 = new TableLayoutPanel();
            picImportedImage = new CustomPictureBox();
            lblImportedImagePath = new Label();
            txtImportedImagePath = new TextBox();
            btnCopyImportedImagePath = new Button();
            btnNextPage1 = new Button();
            tabIconName = new TabPage();
            tableLayoutPanel3 = new TableLayoutPanel();
            label2 = new Label();
            label1 = new Label();
            txtAtlasName = new TextBox();
            txtOutputPath = new TextBox();
            btnChooseOutput = new Button();
            btnPreviousPage1 = new Button();
            btnNextPage2 = new Button();
            tabCreateMod = new TabPage();
            tableLayoutPanel1 = new TableLayoutPanel();
            btnCancel = new Button();
            dgvStatus = new DataGridView();
            colTime = new DataGridViewTextBoxColumn();
            colOutput = new DataGridViewTextBoxColumn();
            btnCreateIcon = new Button();
            btnPreviousPage2 = new Button();
            btnNextPage3 = new Button();
            pgIconProgress = new ProgressBar();
            tabIconPropAndActions = new TabPage();
            tableLayoutPanel11 = new TableLayoutPanel();
            btnPreviousPage3 = new Button();
            tableLayoutPanel5 = new TableLayoutPanel();
            tableLayoutPanel10 = new TableLayoutPanel();
            btnCopyIconPart = new Button();
            txtIconPart = new TextBox();
            lblIconPart = new Label();
            tableLayoutPanel6 = new TableLayoutPanel();
            txtSha256Hash = new TextBox();
            btnCopySha256Hash = new Button();
            lblSha256Hash = new Label();
            tableLayoutPanel7 = new TableLayoutPanel();
            txtArchivePath = new TextBox();
            btnCopyArchivePath = new Button();
            lblArchivePath = new Label();
            lblImagePath = new Label();
            tableLayoutPanel8 = new TableLayoutPanel();
            txtImagePath = new TextBox();
            btnCopyImagePath = new Button();
            lblIconPath = new Label();
            tableLayoutPanel9 = new TableLayoutPanel();
            btnCopyIconPath = new Button();
            txtIconPath = new TextBox();
            btnOpenContainingFolder = new Button();
            panel1 = new Panel();
            richTextBox1 = new RichTextBox();
            btnSendToCRA = new Button();
            bgCreateIconWorker = new System.ComponentModel.BackgroundWorker();
            fldgBrowserOutput = new FolderBrowserDialog();
            iconTabs.SuspendLayout();
            tabImportImage.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picImportedImage).BeginInit();
            tabIconName.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tabCreateMod.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvStatus).BeginInit();
            tabIconPropAndActions.SuspendLayout();
            tableLayoutPanel11.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            tableLayoutPanel10.SuspendLayout();
            tableLayoutPanel6.SuspendLayout();
            tableLayoutPanel7.SuspendLayout();
            tableLayoutPanel8.SuspendLayout();
            tableLayoutPanel9.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // iconTabs
            // 
            iconTabs.Controls.Add(tabImportImage);
            iconTabs.Controls.Add(tabIconName);
            iconTabs.Controls.Add(tabCreateMod);
            iconTabs.Controls.Add(tabIconPropAndActions);
            resources.ApplyResources(iconTabs, "iconTabs");
            iconTabs.Name = "iconTabs";
            iconTabs.SelectedIndex = 0;
            // 
            // tabImportImage
            // 
            tabImportImage.Controls.Add(tableLayoutPanel2);
            resources.ApplyResources(tabImportImage, "tabImportImage");
            tabImportImage.Name = "tabImportImage";
            tabImportImage.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
            tableLayoutPanel2.Controls.Add(picImportedImage, 0, 0);
            tableLayoutPanel2.Controls.Add(lblImportedImagePath, 0, 2);
            tableLayoutPanel2.Controls.Add(txtImportedImagePath, 1, 2);
            tableLayoutPanel2.Controls.Add(btnCopyImportedImagePath, 2, 2);
            tableLayoutPanel2.Controls.Add(btnNextPage1, 1, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // picImportedImage
            // 
            picImportedImage.AllowDrop = true;
            tableLayoutPanel2.SetColumnSpan(picImportedImage, 3);
            resources.ApplyResources(picImportedImage, "picImportedImage");
            picImportedImage.Image = Properties.Resources.drag_and_drop;
            picImportedImage.Name = "picImportedImage";
            tableLayoutPanel2.SetRowSpan(picImportedImage, 2);
            picImportedImage.TabStop = false;
            picImportedImage.DragDrop += PicImportedImage_DragDrop;
            // 
            // lblImportedImagePath
            // 
            resources.ApplyResources(lblImportedImagePath, "lblImportedImagePath");
            lblImportedImagePath.Name = "lblImportedImagePath";
            // 
            // txtImportedImagePath
            // 
            resources.ApplyResources(txtImportedImagePath, "txtImportedImagePath");
            txtImportedImagePath.Name = "txtImportedImagePath";
            txtImportedImagePath.ReadOnly = true;
            // 
            // btnCopyImportedImagePath
            // 
            resources.ApplyResources(btnCopyImportedImagePath, "btnCopyImportedImagePath");
            btnCopyImportedImagePath.BackColor = Color.Yellow;
            btnCopyImportedImagePath.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnCopyImportedImagePath.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnCopyImportedImagePath.Image = Properties.Resources.copy_alt_16x16;
            btnCopyImportedImagePath.Name = "btnCopyImportedImagePath";
            btnCopyImportedImagePath.UseVisualStyleBackColor = false;
            btnCopyImportedImagePath.Click += BtnCopyImportedImagePath_Click;
            // 
            // btnNextPage1
            // 
            resources.ApplyResources(btnNextPage1, "btnNextPage1");
            btnNextPage1.BackColor = Color.Yellow;
            tableLayoutPanel2.SetColumnSpan(btnNextPage1, 2);
            btnNextPage1.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnNextPage1.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnNextPage1.Image = Properties.Resources.next_16x16;
            btnNextPage1.Name = "btnNextPage1";
            btnNextPage1.UseVisualStyleBackColor = false;
            btnNextPage1.Click += BtnNextPage1_Click;
            // 
            // tabIconName
            // 
            tabIconName.Controls.Add(tableLayoutPanel3);
            resources.ApplyResources(tabIconName, "tabIconName");
            tabIconName.Name = "tabIconName";
            tabIconName.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
            tableLayoutPanel3.Controls.Add(label2, 0, 1);
            tableLayoutPanel3.Controls.Add(label1, 0, 0);
            tableLayoutPanel3.Controls.Add(txtAtlasName, 1, 0);
            tableLayoutPanel3.Controls.Add(txtOutputPath, 1, 1);
            tableLayoutPanel3.Controls.Add(btnChooseOutput, 2, 1);
            tableLayoutPanel3.Controls.Add(btnPreviousPage1, 0, 2);
            tableLayoutPanel3.Controls.Add(btnNextPage2, 2, 2);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // txtAtlasName
            // 
            resources.ApplyResources(txtAtlasName, "txtAtlasName");
            tableLayoutPanel3.SetColumnSpan(txtAtlasName, 2);
            txtAtlasName.Name = "txtAtlasName";
            // 
            // txtOutputPath
            // 
            resources.ApplyResources(txtOutputPath, "txtOutputPath");
            txtOutputPath.Name = "txtOutputPath";
            txtOutputPath.ReadOnly = true;
            // 
            // btnChooseOutput
            // 
            btnChooseOutput.BackColor = Color.Yellow;
            resources.ApplyResources(btnChooseOutput, "btnChooseOutput");
            btnChooseOutput.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnChooseOutput.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnChooseOutput.Image = Properties.Resources.folder_16x16;
            btnChooseOutput.Name = "btnChooseOutput";
            btnChooseOutput.UseVisualStyleBackColor = false;
            btnChooseOutput.Click += BtnChooseOutput_Click;
            // 
            // btnPreviousPage1
            // 
            resources.ApplyResources(btnPreviousPage1, "btnPreviousPage1");
            btnPreviousPage1.BackColor = Color.Yellow;
            tableLayoutPanel3.SetColumnSpan(btnPreviousPage1, 2);
            btnPreviousPage1.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnPreviousPage1.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnPreviousPage1.Image = Properties.Resources.previous_16x16;
            btnPreviousPage1.Name = "btnPreviousPage1";
            btnPreviousPage1.UseVisualStyleBackColor = false;
            btnPreviousPage1.Click += BtnPreviousPage1_Click;
            // 
            // btnNextPage2
            // 
            resources.ApplyResources(btnNextPage2, "btnNextPage2");
            btnNextPage2.BackColor = Color.Yellow;
            btnNextPage2.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnNextPage2.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnNextPage2.Image = Properties.Resources.next_16x16;
            btnNextPage2.Name = "btnNextPage2";
            btnNextPage2.UseVisualStyleBackColor = false;
            btnNextPage2.Click += BtnNextPage2_Click;
            // 
            // tabCreateMod
            // 
            tabCreateMod.Controls.Add(tableLayoutPanel1);
            resources.ApplyResources(tabCreateMod, "tabCreateMod");
            tabCreateMod.Name = "tabCreateMod";
            tabCreateMod.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
            tableLayoutPanel1.Controls.Add(btnCancel, 1, 0);
            tableLayoutPanel1.Controls.Add(dgvStatus, 0, 1);
            tableLayoutPanel1.Controls.Add(btnCreateIcon, 0, 0);
            tableLayoutPanel1.Controls.Add(btnPreviousPage2, 0, 3);
            tableLayoutPanel1.Controls.Add(btnNextPage3, 1, 3);
            tableLayoutPanel1.Controls.Add(pgIconProgress, 0, 2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.Yellow;
            resources.ApplyResources(btnCancel, "btnCancel");
            btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnCancel.Image = Properties.Resources.cancel_16x16;
            btnCancel.Name = "btnCancel";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += BtnCancel_Click;
            // 
            // dgvStatus
            // 
            dgvStatus.AllowUserToAddRows = false;
            dgvStatus.AllowUserToDeleteRows = false;
            dgvStatus.BackgroundColor = Color.FromArgb(224, 224, 224);
            dgvStatus.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvStatus.Columns.AddRange(new DataGridViewColumn[] { colTime, colOutput });
            tableLayoutPanel1.SetColumnSpan(dgvStatus, 2);
            resources.ApplyResources(dgvStatus, "dgvStatus");
            dgvStatus.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvStatus.Name = "dgvStatus";
            dgvStatus.ReadOnly = true;
            // 
            // colTime
            // 
            colTime.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(colTime, "colTime");
            colTime.Name = "colTime";
            colTime.ReadOnly = true;
            // 
            // colOutput
            // 
            colOutput.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(colOutput, "colOutput");
            colOutput.Name = "colOutput";
            colOutput.ReadOnly = true;
            // 
            // btnCreateIcon
            // 
            btnCreateIcon.BackColor = Color.Yellow;
            resources.ApplyResources(btnCreateIcon, "btnCreateIcon");
            btnCreateIcon.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnCreateIcon.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnCreateIcon.Image = Properties.Resources.magic_wand_16x16;
            btnCreateIcon.Name = "btnCreateIcon";
            btnCreateIcon.UseVisualStyleBackColor = false;
            btnCreateIcon.Click += BtnCreateIcon_Click;
            // 
            // btnPreviousPage2
            // 
            resources.ApplyResources(btnPreviousPage2, "btnPreviousPage2");
            btnPreviousPage2.BackColor = Color.Yellow;
            btnPreviousPage2.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnPreviousPage2.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnPreviousPage2.Image = Properties.Resources.previous_16x16;
            btnPreviousPage2.Name = "btnPreviousPage2";
            btnPreviousPage2.UseVisualStyleBackColor = false;
            btnPreviousPage2.Click += BtnPreviousPage2_Click;
            // 
            // btnNextPage3
            // 
            resources.ApplyResources(btnNextPage3, "btnNextPage3");
            btnNextPage3.BackColor = Color.Yellow;
            btnNextPage3.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnNextPage3.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnNextPage3.Image = Properties.Resources.next_16x16;
            btnNextPage3.Name = "btnNextPage3";
            btnNextPage3.UseVisualStyleBackColor = false;
            btnNextPage3.Click += BtnNextPage3_Click;
            // 
            // pgIconProgress
            // 
            tableLayoutPanel1.SetColumnSpan(pgIconProgress, 2);
            resources.ApplyResources(pgIconProgress, "pgIconProgress");
            pgIconProgress.Name = "pgIconProgress";
            // 
            // tabIconPropAndActions
            // 
            tabIconPropAndActions.Controls.Add(tableLayoutPanel11);
            resources.ApplyResources(tabIconPropAndActions, "tabIconPropAndActions");
            tabIconPropAndActions.Name = "tabIconPropAndActions";
            tabIconPropAndActions.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel11
            // 
            resources.ApplyResources(tableLayoutPanel11, "tableLayoutPanel11");
            tableLayoutPanel11.Controls.Add(btnPreviousPage3, 0, 2);
            tableLayoutPanel11.Controls.Add(tableLayoutPanel5, 0, 0);
            tableLayoutPanel11.Controls.Add(btnOpenContainingFolder, 0, 1);
            tableLayoutPanel11.Controls.Add(panel1, 1, 1);
            tableLayoutPanel11.Name = "tableLayoutPanel11";
            // 
            // btnPreviousPage3
            // 
            resources.ApplyResources(btnPreviousPage3, "btnPreviousPage3");
            btnPreviousPage3.BackColor = Color.Yellow;
            btnPreviousPage3.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnPreviousPage3.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnPreviousPage3.Image = Properties.Resources.previous_16x16;
            btnPreviousPage3.Name = "btnPreviousPage3";
            btnPreviousPage3.UseVisualStyleBackColor = false;
            btnPreviousPage3.Click += BtnPreviousPage3_Click;
            // 
            // tableLayoutPanel5
            // 
            resources.ApplyResources(tableLayoutPanel5, "tableLayoutPanel5");
            tableLayoutPanel11.SetColumnSpan(tableLayoutPanel5, 2);
            tableLayoutPanel5.Controls.Add(tableLayoutPanel10, 1, 4);
            tableLayoutPanel5.Controls.Add(lblIconPart, 0, 4);
            tableLayoutPanel5.Controls.Add(tableLayoutPanel6, 1, 2);
            tableLayoutPanel5.Controls.Add(lblSha256Hash, 0, 2);
            tableLayoutPanel5.Controls.Add(tableLayoutPanel7, 1, 1);
            tableLayoutPanel5.Controls.Add(lblArchivePath, 0, 1);
            tableLayoutPanel5.Controls.Add(lblImagePath, 0, 0);
            tableLayoutPanel5.Controls.Add(tableLayoutPanel8, 1, 0);
            tableLayoutPanel5.Controls.Add(lblIconPath, 0, 3);
            tableLayoutPanel5.Controls.Add(tableLayoutPanel9, 1, 3);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            // 
            // tableLayoutPanel10
            // 
            resources.ApplyResources(tableLayoutPanel10, "tableLayoutPanel10");
            tableLayoutPanel10.Controls.Add(btnCopyIconPart, 0, 0);
            tableLayoutPanel10.Controls.Add(txtIconPart, 0, 0);
            tableLayoutPanel10.Name = "tableLayoutPanel10";
            // 
            // btnCopyIconPart
            // 
            resources.ApplyResources(btnCopyIconPart, "btnCopyIconPart");
            btnCopyIconPart.BackColor = Color.Yellow;
            btnCopyIconPart.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnCopyIconPart.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnCopyIconPart.Image = Properties.Resources.copy_alt_16x16;
            btnCopyIconPart.Name = "btnCopyIconPart";
            btnCopyIconPart.UseVisualStyleBackColor = false;
            btnCopyIconPart.Click += BtnCopyIconPart_Click;
            // 
            // txtIconPart
            // 
            resources.ApplyResources(txtIconPart, "txtIconPart");
            txtIconPart.Name = "txtIconPart";
            txtIconPart.ReadOnly = true;
            // 
            // lblIconPart
            // 
            resources.ApplyResources(lblIconPart, "lblIconPart");
            lblIconPart.Name = "lblIconPart";
            // 
            // tableLayoutPanel6
            // 
            resources.ApplyResources(tableLayoutPanel6, "tableLayoutPanel6");
            tableLayoutPanel6.Controls.Add(txtSha256Hash, 0, 0);
            tableLayoutPanel6.Controls.Add(btnCopySha256Hash, 1, 0);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            // 
            // txtSha256Hash
            // 
            resources.ApplyResources(txtSha256Hash, "txtSha256Hash");
            txtSha256Hash.Name = "txtSha256Hash";
            txtSha256Hash.ReadOnly = true;
            // 
            // btnCopySha256Hash
            // 
            resources.ApplyResources(btnCopySha256Hash, "btnCopySha256Hash");
            btnCopySha256Hash.BackColor = Color.Yellow;
            btnCopySha256Hash.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnCopySha256Hash.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnCopySha256Hash.Image = Properties.Resources.copy_alt_16x16;
            btnCopySha256Hash.Name = "btnCopySha256Hash";
            btnCopySha256Hash.UseVisualStyleBackColor = false;
            btnCopySha256Hash.Click += BtnCopySha256Hash_Click;
            // 
            // lblSha256Hash
            // 
            resources.ApplyResources(lblSha256Hash, "lblSha256Hash");
            lblSha256Hash.Name = "lblSha256Hash";
            // 
            // tableLayoutPanel7
            // 
            resources.ApplyResources(tableLayoutPanel7, "tableLayoutPanel7");
            tableLayoutPanel7.Controls.Add(txtArchivePath, 0, 0);
            tableLayoutPanel7.Controls.Add(btnCopyArchivePath, 1, 0);
            tableLayoutPanel7.Name = "tableLayoutPanel7";
            // 
            // txtArchivePath
            // 
            resources.ApplyResources(txtArchivePath, "txtArchivePath");
            txtArchivePath.Name = "txtArchivePath";
            txtArchivePath.ReadOnly = true;
            // 
            // btnCopyArchivePath
            // 
            resources.ApplyResources(btnCopyArchivePath, "btnCopyArchivePath");
            btnCopyArchivePath.BackColor = Color.Yellow;
            btnCopyArchivePath.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnCopyArchivePath.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnCopyArchivePath.Image = Properties.Resources.copy_alt_16x16;
            btnCopyArchivePath.Name = "btnCopyArchivePath";
            btnCopyArchivePath.UseVisualStyleBackColor = false;
            btnCopyArchivePath.Click += BtnCopyArchivePath_Click;
            // 
            // lblArchivePath
            // 
            resources.ApplyResources(lblArchivePath, "lblArchivePath");
            lblArchivePath.Name = "lblArchivePath";
            // 
            // lblImagePath
            // 
            resources.ApplyResources(lblImagePath, "lblImagePath");
            lblImagePath.Name = "lblImagePath";
            // 
            // tableLayoutPanel8
            // 
            resources.ApplyResources(tableLayoutPanel8, "tableLayoutPanel8");
            tableLayoutPanel8.Controls.Add(txtImagePath, 0, 0);
            tableLayoutPanel8.Controls.Add(btnCopyImagePath, 1, 0);
            tableLayoutPanel8.Name = "tableLayoutPanel8";
            // 
            // txtImagePath
            // 
            resources.ApplyResources(txtImagePath, "txtImagePath");
            txtImagePath.Name = "txtImagePath";
            txtImagePath.ReadOnly = true;
            // 
            // btnCopyImagePath
            // 
            resources.ApplyResources(btnCopyImagePath, "btnCopyImagePath");
            btnCopyImagePath.BackColor = Color.Yellow;
            btnCopyImagePath.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnCopyImagePath.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnCopyImagePath.Image = Properties.Resources.copy_alt_16x16;
            btnCopyImagePath.Name = "btnCopyImagePath";
            btnCopyImagePath.UseVisualStyleBackColor = false;
            btnCopyImagePath.Click += BtnCopyImagePath_Click;
            // 
            // lblIconPath
            // 
            resources.ApplyResources(lblIconPath, "lblIconPath");
            lblIconPath.Name = "lblIconPath";
            // 
            // tableLayoutPanel9
            // 
            resources.ApplyResources(tableLayoutPanel9, "tableLayoutPanel9");
            tableLayoutPanel9.Controls.Add(btnCopyIconPath, 0, 0);
            tableLayoutPanel9.Controls.Add(txtIconPath, 0, 0);
            tableLayoutPanel9.Name = "tableLayoutPanel9";
            // 
            // btnCopyIconPath
            // 
            resources.ApplyResources(btnCopyIconPath, "btnCopyIconPath");
            btnCopyIconPath.BackColor = Color.Yellow;
            btnCopyIconPath.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnCopyIconPath.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnCopyIconPath.Image = Properties.Resources.copy_alt_16x16;
            btnCopyIconPath.Name = "btnCopyIconPath";
            btnCopyIconPath.UseVisualStyleBackColor = false;
            btnCopyIconPath.Click += BtnCopyIconPath_Click;
            // 
            // txtIconPath
            // 
            resources.ApplyResources(txtIconPath, "txtIconPath");
            txtIconPath.Name = "txtIconPath";
            txtIconPath.ReadOnly = true;
            // 
            // btnOpenContainingFolder
            // 
            btnOpenContainingFolder.BackColor = Color.Yellow;
            btnOpenContainingFolder.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnOpenContainingFolder.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            resources.ApplyResources(btnOpenContainingFolder, "btnOpenContainingFolder");
            btnOpenContainingFolder.Image = Properties.Resources.folder_16x16;
            btnOpenContainingFolder.Name = "btnOpenContainingFolder";
            btnOpenContainingFolder.UseVisualStyleBackColor = false;
            btnOpenContainingFolder.Click += BtnOpenContainingFolder_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(richTextBox1);
            panel1.Controls.Add(btnSendToCRA);
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = Color.White;
            richTextBox1.BorderStyle = BorderStyle.None;
            resources.ApplyResources(richTextBox1, "richTextBox1");
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            // 
            // btnSendToCRA
            // 
            btnSendToCRA.BackColor = Color.Yellow;
            resources.ApplyResources(btnSendToCRA, "btnSendToCRA");
            btnSendToCRA.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnSendToCRA.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnSendToCRA.Image = Properties.Resources.send_16x16;
            btnSendToCRA.Name = "btnSendToCRA";
            btnSendToCRA.UseVisualStyleBackColor = false;
            btnSendToCRA.Click += BtnSendToCRA_Click;
            // 
            // bgCreateIconWorker
            // 
            bgCreateIconWorker.WorkerReportsProgress = true;
            bgCreateIconWorker.WorkerSupportsCancellation = true;
            bgCreateIconWorker.DoWork += BgCreateIconWorker_DoWork;
            bgCreateIconWorker.RunWorkerCompleted += BgCreateIconWorker_RunWorkerCompleted;
            // 
            // fldgBrowserOutput
            // 
            resources.ApplyResources(fldgBrowserOutput, "fldgBrowserOutput");
            // 
            // IconCreator
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(iconTabs);
            Name = "IconCreator";
            Load += IconCreator_Load;
            iconTabs.ResumeLayout(false);
            tabImportImage.ResumeLayout(false);
            tabImportImage.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picImportedImage).EndInit();
            tabIconName.ResumeLayout(false);
            tabIconName.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            tabCreateMod.ResumeLayout(false);
            tabCreateMod.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvStatus).EndInit();
            tabIconPropAndActions.ResumeLayout(false);
            tabIconPropAndActions.PerformLayout();
            tableLayoutPanel11.ResumeLayout(false);
            tableLayoutPanel11.PerformLayout();
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel5.PerformLayout();
            tableLayoutPanel10.ResumeLayout(false);
            tableLayoutPanel10.PerformLayout();
            tableLayoutPanel6.ResumeLayout(false);
            tableLayoutPanel6.PerformLayout();
            tableLayoutPanel7.ResumeLayout(false);
            tableLayoutPanel7.PerformLayout();
            tableLayoutPanel8.ResumeLayout(false);
            tableLayoutPanel8.PerformLayout();
            tableLayoutPanel9.ResumeLayout(false);
            tableLayoutPanel9.PerformLayout();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl iconTabs;
        private TabPage tabImportImage;
        private TabPage tabIconName;
        private TabPage tabCreateMod;
        private TabPage tabIconPropAndActions;
        private TableLayoutPanel tableLayoutPanel2;
        private Button btnNextPage1;
        private CustomPictureBox picImportedImage;
        private Label lblImportedImagePath;
        private TextBox txtImportedImagePath;
        private Button btnCopyImportedImagePath;
        private TableLayoutPanel tableLayoutPanel3;
        private Label label2;
        private Label label1;
        private TextBox txtAtlasName;
        private TextBox txtOutputPath;
        private Button btnChooseOutput;
        private Button btnPreviousPage1;
        private Button btnNextPage2;
        private DataGridView dgvStatus;
        private DataGridViewTextBoxColumn colTime;
        private DataGridViewTextBoxColumn colOutput;
        private Button btnCancel;
        private Button btnCreateIcon;
        private TableLayoutPanel tableLayoutPanel1;
        private Button btnNextPage3;
        private Button btnPreviousPage2;
        private ProgressBar pgIconProgress;
        private TableLayoutPanel tableLayoutPanel5;
        private TableLayoutPanel tableLayoutPanel10;
        private Button btnCopyIconPart;
        private TextBox txtIconPart;
        private Label lblIconPart;
        private TableLayoutPanel tableLayoutPanel6;
        private TextBox txtSha256Hash;
        private Button btnCopySha256Hash;
        private Label lblSha256Hash;
        private TableLayoutPanel tableLayoutPanel7;
        private TextBox txtArchivePath;
        private Button btnCopyArchivePath;
        private Label lblArchivePath;
        private Label lblImagePath;
        private TableLayoutPanel tableLayoutPanel8;
        private TextBox txtImagePath;
        private Button btnCopyImagePath;
        private Label lblIconPath;
        private TableLayoutPanel tableLayoutPanel9;
        private Button btnCopyIconPath;
        private TextBox txtIconPath;
        private TableLayoutPanel tableLayoutPanel11;
        private Button btnSendToCRA;
        private Button btnOpenContainingFolder;
        private Button btnPreviousPage3;
        private Panel panel1;
        private RichTextBox richTextBox1;
        private System.ComponentModel.BackgroundWorker bgCreateIconWorker;
        private FolderBrowserDialog fldgBrowserOutput;
    }
}
