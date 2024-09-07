namespace WolvenIconGenerator.Custom_Controls
{
    partial class CreateIconCtl
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
            tlpMainTable = new TableLayoutPanel();
            grpStep1 = new GroupBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            picImportedImage = new CustomPictureBox();
            lblImportedImagePath = new Label();
            txtImportedImagePath = new TextBox();
            btnCopyImportedImagePath = new Button();
            grpAtlasName = new GroupBox();
            tableLayoutPanel3 = new TableLayoutPanel();
            label2 = new Label();
            label1 = new Label();
            txtAtlasName = new TextBox();
            txtOutputPath = new TextBox();
            btnChooseOutput = new Button();
            grpCreateIcon = new GroupBox();
            dgvStatus = new DataGridView();
            colTime = new DataGridViewTextBoxColumn();
            colOutput = new DataGridViewTextBoxColumn();
            tableLayoutPanel4 = new TableLayoutPanel();
            btnCancel = new Button();
            btnCreateIcon = new Button();
            grpAfterCreation = new GroupBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            btnSendToCRA = new Button();
            btnOpenContainingFolder = new Button();
            grpIconProperties = new GroupBox();
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
            fldgBrowserOutput = new FolderBrowserDialog();
            bgCreateIconWorker = new System.ComponentModel.BackgroundWorker();
            tlpMainTable.SuspendLayout();
            grpStep1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picImportedImage).BeginInit();
            grpAtlasName.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            grpCreateIcon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvStatus).BeginInit();
            tableLayoutPanel4.SuspendLayout();
            grpAfterCreation.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            grpIconProperties.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            tableLayoutPanel10.SuspendLayout();
            tableLayoutPanel6.SuspendLayout();
            tableLayoutPanel7.SuspendLayout();
            tableLayoutPanel8.SuspendLayout();
            tableLayoutPanel9.SuspendLayout();
            SuspendLayout();
            // 
            // tlpMainTable
            // 
            tlpMainTable.ColumnCount = 2;
            tlpMainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 49.72119F));
            tlpMainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.27881F));
            tlpMainTable.Controls.Add(grpStep1, 0, 0);
            tlpMainTable.Controls.Add(grpAtlasName, 0, 1);
            tlpMainTable.Controls.Add(grpCreateIcon, 0, 2);
            tlpMainTable.Controls.Add(grpAfterCreation, 0, 4);
            tlpMainTable.Controls.Add(grpIconProperties, 0, 3);
            tlpMainTable.Dock = DockStyle.Fill;
            tlpMainTable.Location = new Point(0, 0);
            tlpMainTable.Name = "tlpMainTable";
            tlpMainTable.RowCount = 5;
            tlpMainTable.RowStyles.Add(new RowStyle());
            tlpMainTable.RowStyles.Add(new RowStyle());
            tlpMainTable.RowStyles.Add(new RowStyle());
            tlpMainTable.RowStyles.Add(new RowStyle());
            tlpMainTable.RowStyles.Add(new RowStyle());
            tlpMainTable.Size = new Size(1051, 891);
            tlpMainTable.TabIndex = 1;
            // 
            // grpStep1
            // 
            tlpMainTable.SetColumnSpan(grpStep1, 2);
            grpStep1.Controls.Add(tableLayoutPanel2);
            grpStep1.Dock = DockStyle.Fill;
            grpStep1.Font = new Font("Segoe UI Variable Display Semib", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            grpStep1.Location = new Point(3, 3);
            grpStep1.Name = "grpStep1";
            grpStep1.Size = new Size(1045, 204);
            grpStep1.TabIndex = 0;
            grpStep1.TabStop = false;
            grpStep1.Text = "1 - Import .PNG";
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10.0820637F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 89.91794F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 39F));
            tableLayoutPanel2.Controls.Add(picImportedImage, 0, 0);
            tableLayoutPanel2.Controls.Add(lblImportedImagePath, 0, 2);
            tableLayoutPanel2.Controls.Add(txtImportedImagePath, 1, 2);
            tableLayoutPanel2.Controls.Add(btnCopyImportedImagePath, 2, 2);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 21);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 3;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 43F));
            tableLayoutPanel2.Size = new Size(1039, 180);
            tableLayoutPanel2.TabIndex = 6;
            // 
            // picImportedImage
            // 
            picImportedImage.AllowDrop = true;
            tableLayoutPanel2.SetColumnSpan(picImportedImage, 3);
            picImportedImage.Dock = DockStyle.Fill;
            picImportedImage.Image = Properties.Resources.drag_and_drop;
            picImportedImage.Location = new Point(3, 3);
            picImportedImage.Name = "picImportedImage";
            tableLayoutPanel2.SetRowSpan(picImportedImage, 2);
            picImportedImage.Size = new Size(1033, 130);
            picImportedImage.SizeMode = PictureBoxSizeMode.Zoom;
            picImportedImage.TabIndex = 0;
            picImportedImage.TabStop = false;
            picImportedImage.DragDrop += PicImportedImage_DragDrop;
            // 
            // lblImportedImagePath
            // 
            lblImportedImagePath.Anchor = AnchorStyles.Right;
            lblImportedImagePath.AutoSize = true;
            lblImportedImagePath.Font = new Font("Segoe UI Variable Display Semib", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblImportedImagePath.Location = new Point(17, 149);
            lblImportedImagePath.Name = "lblImportedImagePath";
            lblImportedImagePath.Size = new Size(80, 17);
            lblImportedImagePath.TabIndex = 3;
            lblImportedImagePath.Text = "Image Path: ";
            // 
            // txtImportedImagePath
            // 
            txtImportedImagePath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtImportedImagePath.Enabled = false;
            txtImportedImagePath.Location = new Point(103, 145);
            txtImportedImagePath.Name = "txtImportedImagePath";
            txtImportedImagePath.ReadOnly = true;
            txtImportedImagePath.Size = new Size(893, 25);
            txtImportedImagePath.TabIndex = 4;
            // 
            // btnCopyImportedImagePath
            // 
            btnCopyImportedImagePath.Anchor = AnchorStyles.Left;
            btnCopyImportedImagePath.BackColor = Color.Yellow;
            btnCopyImportedImagePath.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnCopyImportedImagePath.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnCopyImportedImagePath.FlatStyle = FlatStyle.Flat;
            btnCopyImportedImagePath.Font = new Font("Segoe UI Variable Display Semib", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCopyImportedImagePath.Image = Properties.Resources.copy_alt_16x16;
            btnCopyImportedImagePath.Location = new Point(1002, 142);
            btnCopyImportedImagePath.Name = "btnCopyImportedImagePath";
            btnCopyImportedImagePath.Size = new Size(32, 32);
            btnCopyImportedImagePath.TabIndex = 5;
            btnCopyImportedImagePath.UseVisualStyleBackColor = false;
            btnCopyImportedImagePath.Click += BtnCopyImportedImagePath_Click;
            // 
            // grpAtlasName
            // 
            tlpMainTable.SetColumnSpan(grpAtlasName, 2);
            grpAtlasName.Controls.Add(tableLayoutPanel3);
            grpAtlasName.Dock = DockStyle.Fill;
            grpAtlasName.Font = new Font("Segoe UI Variable Display Semib", 9.75F, FontStyle.Bold);
            grpAtlasName.Location = new Point(3, 213);
            grpAtlasName.Name = "grpAtlasName";
            grpAtlasName.Size = new Size(1045, 122);
            grpAtlasName.TabIndex = 1;
            grpAtlasName.TabStop = false;
            grpAtlasName.Text = "2 - Icon Name and Output";
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 3;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.1508379F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 87.84916F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 183F));
            tableLayoutPanel3.Controls.Add(label2, 0, 1);
            tableLayoutPanel3.Controls.Add(label1, 0, 0);
            tableLayoutPanel3.Controls.Add(txtAtlasName, 1, 0);
            tableLayoutPanel3.Controls.Add(txtOutputPath, 1, 1);
            tableLayoutPanel3.Controls.Add(btnChooseOutput, 2, 1);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 21);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            tableLayoutPanel3.Size = new Size(1039, 98);
            tableLayoutPanel3.TabIndex = 0;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Variable Display Semib", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(45, 71);
            label2.Name = "label2";
            label2.Size = new Size(56, 17);
            label2.TabIndex = 6;
            label2.Text = "Output: ";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Variable Display Semib", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(19, 22);
            label1.Name = "label1";
            label1.Size = new Size(82, 17);
            label1.TabIndex = 4;
            label1.Text = "Atlas Name: ";
            // 
            // txtAtlasName
            // 
            txtAtlasName.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel3.SetColumnSpan(txtAtlasName, 2);
            txtAtlasName.Location = new Point(107, 18);
            txtAtlasName.Name = "txtAtlasName";
            txtAtlasName.Size = new Size(929, 25);
            txtAtlasName.TabIndex = 5;
            // 
            // txtOutputPath
            // 
            txtOutputPath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtOutputPath.Enabled = false;
            txtOutputPath.Location = new Point(107, 67);
            txtOutputPath.Name = "txtOutputPath";
            txtOutputPath.ReadOnly = true;
            txtOutputPath.Size = new Size(745, 25);
            txtOutputPath.TabIndex = 7;
            // 
            // btnChooseOutput
            // 
            btnChooseOutput.BackColor = Color.Yellow;
            btnChooseOutput.Dock = DockStyle.Fill;
            btnChooseOutput.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnChooseOutput.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnChooseOutput.FlatStyle = FlatStyle.Flat;
            btnChooseOutput.Font = new Font("Segoe UI Variable Display Semib", 9.75F, FontStyle.Bold);
            btnChooseOutput.Image = Properties.Resources.folder_16x16;
            btnChooseOutput.Location = new Point(858, 65);
            btnChooseOutput.Name = "btnChooseOutput";
            btnChooseOutput.Size = new Size(178, 30);
            btnChooseOutput.TabIndex = 8;
            btnChooseOutput.Text = "Choose Folder...";
            btnChooseOutput.TextAlign = ContentAlignment.MiddleRight;
            btnChooseOutput.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnChooseOutput.UseVisualStyleBackColor = false;
            btnChooseOutput.Click += BtnChooseOutput_Click;
            // 
            // grpCreateIcon
            // 
            tlpMainTable.SetColumnSpan(grpCreateIcon, 2);
            grpCreateIcon.Controls.Add(dgvStatus);
            grpCreateIcon.Controls.Add(tableLayoutPanel4);
            grpCreateIcon.Dock = DockStyle.Fill;
            grpCreateIcon.Font = new Font("Segoe UI Variable Display Semib", 9.75F, FontStyle.Bold);
            grpCreateIcon.Location = new Point(3, 341);
            grpCreateIcon.Name = "grpCreateIcon";
            grpCreateIcon.Size = new Size(1045, 244);
            grpCreateIcon.TabIndex = 2;
            grpCreateIcon.TabStop = false;
            grpCreateIcon.Text = "3 - Create \"Mod\"";
            // 
            // dgvStatus
            // 
            dgvStatus.AllowUserToAddRows = false;
            dgvStatus.AllowUserToDeleteRows = false;
            dgvStatus.BackgroundColor = Color.FromArgb(224, 224, 224);
            dgvStatus.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvStatus.Columns.AddRange(new DataGridViewColumn[] { colTime, colOutput });
            dgvStatus.Dock = DockStyle.Fill;
            dgvStatus.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvStatus.Location = new Point(3, 67);
            dgvStatus.Name = "dgvStatus";
            dgvStatus.ReadOnly = true;
            dgvStatus.Size = new Size(1039, 174);
            dgvStatus.TabIndex = 1;
            // 
            // colTime
            // 
            colTime.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colTime.HeaderText = "Timestamp";
            colTime.Name = "colTime";
            colTime.ReadOnly = true;
            // 
            // colOutput
            // 
            colOutput.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colOutput.HeaderText = "Output";
            colOutput.Name = "colOutput";
            colOutput.ReadOnly = true;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Controls.Add(btnCancel, 1, 0);
            tableLayoutPanel4.Controls.Add(btnCreateIcon, 0, 0);
            tableLayoutPanel4.Dock = DockStyle.Top;
            tableLayoutPanel4.Location = new Point(3, 21);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Size = new Size(1039, 46);
            tableLayoutPanel4.TabIndex = 0;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.Yellow;
            btnCancel.Dock = DockStyle.Fill;
            btnCancel.Enabled = false;
            btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Image = Properties.Resources.cancel_16x16;
            btnCancel.Location = new Point(522, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(514, 40);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel Operation";
            btnCancel.TextAlign = ContentAlignment.MiddleRight;
            btnCancel.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnCreateIcon
            // 
            btnCreateIcon.BackColor = Color.Yellow;
            btnCreateIcon.Dock = DockStyle.Fill;
            btnCreateIcon.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnCreateIcon.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnCreateIcon.FlatStyle = FlatStyle.Flat;
            btnCreateIcon.Image = Properties.Resources.magic_wand_16x16;
            btnCreateIcon.Location = new Point(3, 3);
            btnCreateIcon.Name = "btnCreateIcon";
            btnCreateIcon.Size = new Size(513, 40);
            btnCreateIcon.TabIndex = 0;
            btnCreateIcon.Text = "Create Icon";
            btnCreateIcon.TextAlign = ContentAlignment.MiddleRight;
            btnCreateIcon.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnCreateIcon.UseVisualStyleBackColor = false;
            btnCreateIcon.Click += btnCreateIcon_Click;
            // 
            // grpAfterCreation
            // 
            tlpMainTable.SetColumnSpan(grpAfterCreation, 2);
            grpAfterCreation.Controls.Add(tableLayoutPanel1);
            grpAfterCreation.Dock = DockStyle.Fill;
            grpAfterCreation.Font = new Font("Segoe UI Variable Display Semib", 9.75F, FontStyle.Bold);
            grpAfterCreation.Location = new Point(3, 822);
            grpAfterCreation.Name = "grpAfterCreation";
            grpAfterCreation.Size = new Size(1045, 66);
            grpAfterCreation.TabIndex = 5;
            grpAfterCreation.TabStop = false;
            grpAfterCreation.Text = "5 - Post-Creation Actions";
            grpAfterCreation.Visible = false;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(btnSendToCRA, 0, 0);
            tableLayoutPanel1.Controls.Add(btnOpenContainingFolder, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(3, 21);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(1039, 42);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // btnSendToCRA
            // 
            btnSendToCRA.BackColor = Color.Yellow;
            btnSendToCRA.Dock = DockStyle.Fill;
            btnSendToCRA.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnSendToCRA.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnSendToCRA.FlatStyle = FlatStyle.Flat;
            btnSendToCRA.Image = Properties.Resources.send_16x16;
            btnSendToCRA.Location = new Point(522, 3);
            btnSendToCRA.Name = "btnSendToCRA";
            btnSendToCRA.Size = new Size(514, 36);
            btnSendToCRA.TabIndex = 5;
            btnSendToCRA.Text = "Send Last Icon to Cyber Radio Assistant";
            btnSendToCRA.TextAlign = ContentAlignment.MiddleRight;
            btnSendToCRA.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnSendToCRA.UseVisualStyleBackColor = false;
            btnSendToCRA.Click += btnSendToCRA_Click;
            // 
            // btnOpenContainingFolder
            // 
            btnOpenContainingFolder.BackColor = Color.Yellow;
            btnOpenContainingFolder.Dock = DockStyle.Fill;
            btnOpenContainingFolder.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnOpenContainingFolder.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnOpenContainingFolder.FlatStyle = FlatStyle.Flat;
            btnOpenContainingFolder.Image = Properties.Resources.folder_16x16;
            btnOpenContainingFolder.Location = new Point(3, 3);
            btnOpenContainingFolder.Name = "btnOpenContainingFolder";
            btnOpenContainingFolder.Size = new Size(513, 36);
            btnOpenContainingFolder.TabIndex = 4;
            btnOpenContainingFolder.Text = "Open Containing Folder";
            btnOpenContainingFolder.TextAlign = ContentAlignment.MiddleRight;
            btnOpenContainingFolder.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnOpenContainingFolder.UseVisualStyleBackColor = false;
            btnOpenContainingFolder.Click += btnOpenContainingFolder_Click;
            // 
            // grpIconProperties
            // 
            tlpMainTable.SetColumnSpan(grpIconProperties, 2);
            grpIconProperties.Controls.Add(tableLayoutPanel5);
            grpIconProperties.Dock = DockStyle.Fill;
            grpIconProperties.Font = new Font("Segoe UI Variable Display Semib", 9.75F, FontStyle.Bold);
            grpIconProperties.Location = new Point(3, 591);
            grpIconProperties.Name = "grpIconProperties";
            grpIconProperties.Size = new Size(1045, 225);
            grpIconProperties.TabIndex = 6;
            grpIconProperties.TabStop = false;
            grpIconProperties.Text = "4 - Icon Properties";
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 2;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 85F));
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
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(3, 21);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 5;
            tableLayoutPanel5.RowStyles.Add(new RowStyle());
            tableLayoutPanel5.RowStyles.Add(new RowStyle());
            tableLayoutPanel5.RowStyles.Add(new RowStyle());
            tableLayoutPanel5.RowStyles.Add(new RowStyle());
            tableLayoutPanel5.RowStyles.Add(new RowStyle());
            tableLayoutPanel5.Size = new Size(1039, 201);
            tableLayoutPanel5.TabIndex = 2;
            // 
            // tableLayoutPanel10
            // 
            tableLayoutPanel10.ColumnCount = 2;
            tableLayoutPanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 90F));
            tableLayoutPanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tableLayoutPanel10.Controls.Add(btnCopyIconPart, 0, 0);
            tableLayoutPanel10.Controls.Add(txtIconPart, 0, 0);
            tableLayoutPanel10.Dock = DockStyle.Fill;
            tableLayoutPanel10.Location = new Point(158, 162);
            tableLayoutPanel10.Name = "tableLayoutPanel10";
            tableLayoutPanel10.RowCount = 1;
            tableLayoutPanel10.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel10.Size = new Size(878, 36);
            tableLayoutPanel10.TabIndex = 10;
            // 
            // btnCopyIconPart
            // 
            btnCopyIconPart.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            btnCopyIconPart.BackColor = Color.Yellow;
            btnCopyIconPart.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnCopyIconPart.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnCopyIconPart.FlatStyle = FlatStyle.Flat;
            btnCopyIconPart.Image = Properties.Resources.copy_alt_16x16;
            btnCopyIconPart.Location = new Point(793, 7);
            btnCopyIconPart.Name = "btnCopyIconPart";
            btnCopyIconPart.Size = new Size(82, 22);
            btnCopyIconPart.TabIndex = 3;
            btnCopyIconPart.UseVisualStyleBackColor = false;
            // 
            // txtIconPart
            // 
            txtIconPart.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtIconPart.Location = new Point(3, 5);
            txtIconPart.Name = "txtIconPart";
            txtIconPart.ReadOnly = true;
            txtIconPart.Size = new Size(784, 25);
            txtIconPart.TabIndex = 2;
            // 
            // lblIconPart
            // 
            lblIconPart.Anchor = AnchorStyles.Right;
            lblIconPart.AutoSize = true;
            lblIconPart.Location = new Point(88, 171);
            lblIconPart.Name = "lblIconPart";
            lblIconPart.Size = new Size(64, 17);
            lblIconPart.TabIndex = 8;
            lblIconPart.Text = "Icon Part:";
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 2;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 90F));
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tableLayoutPanel6.Controls.Add(txtSha256Hash, 0, 0);
            tableLayoutPanel6.Controls.Add(btnCopySha256Hash, 1, 0);
            tableLayoutPanel6.Dock = DockStyle.Fill;
            tableLayoutPanel6.Location = new Point(158, 86);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 1;
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel6.Size = new Size(878, 35);
            tableLayoutPanel6.TabIndex = 6;
            // 
            // txtSha256Hash
            // 
            txtSha256Hash.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtSha256Hash.Location = new Point(3, 5);
            txtSha256Hash.Name = "txtSha256Hash";
            txtSha256Hash.ReadOnly = true;
            txtSha256Hash.Size = new Size(784, 25);
            txtSha256Hash.TabIndex = 1;
            // 
            // btnCopySha256Hash
            // 
            btnCopySha256Hash.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            btnCopySha256Hash.BackColor = Color.Yellow;
            btnCopySha256Hash.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnCopySha256Hash.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnCopySha256Hash.FlatStyle = FlatStyle.Flat;
            btnCopySha256Hash.Image = Properties.Resources.copy_alt_16x16;
            btnCopySha256Hash.Location = new Point(793, 6);
            btnCopySha256Hash.Name = "btnCopySha256Hash";
            btnCopySha256Hash.Size = new Size(82, 22);
            btnCopySha256Hash.TabIndex = 2;
            btnCopySha256Hash.UseVisualStyleBackColor = false;
            // 
            // lblSha256Hash
            // 
            lblSha256Hash.Anchor = AnchorStyles.Right;
            lblSha256Hash.AutoSize = true;
            lblSha256Hash.Location = new Point(63, 95);
            lblSha256Hash.Name = "lblSha256Hash";
            lblSha256Hash.Size = new Size(89, 17);
            lblSha256Hash.TabIndex = 5;
            lblSha256Hash.Text = "SHA256 Hash:";
            // 
            // tableLayoutPanel7
            // 
            tableLayoutPanel7.ColumnCount = 2;
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 90F));
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tableLayoutPanel7.Controls.Add(txtArchivePath, 0, 0);
            tableLayoutPanel7.Controls.Add(btnCopyArchivePath, 1, 0);
            tableLayoutPanel7.Dock = DockStyle.Fill;
            tableLayoutPanel7.Location = new Point(158, 49);
            tableLayoutPanel7.Name = "tableLayoutPanel7";
            tableLayoutPanel7.RowCount = 1;
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel7.Size = new Size(878, 31);
            tableLayoutPanel7.TabIndex = 4;
            // 
            // txtArchivePath
            // 
            txtArchivePath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtArchivePath.Location = new Point(3, 3);
            txtArchivePath.Name = "txtArchivePath";
            txtArchivePath.ReadOnly = true;
            txtArchivePath.Size = new Size(784, 25);
            txtArchivePath.TabIndex = 1;
            // 
            // btnCopyArchivePath
            // 
            btnCopyArchivePath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            btnCopyArchivePath.BackColor = Color.Yellow;
            btnCopyArchivePath.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnCopyArchivePath.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnCopyArchivePath.FlatStyle = FlatStyle.Flat;
            btnCopyArchivePath.Image = Properties.Resources.copy_alt_16x16;
            btnCopyArchivePath.Location = new Point(793, 3);
            btnCopyArchivePath.Name = "btnCopyArchivePath";
            btnCopyArchivePath.Size = new Size(82, 25);
            btnCopyArchivePath.TabIndex = 2;
            btnCopyArchivePath.UseVisualStyleBackColor = false;
            // 
            // lblArchivePath
            // 
            lblArchivePath.Anchor = AnchorStyles.Right;
            lblArchivePath.AutoSize = true;
            lblArchivePath.Location = new Point(67, 56);
            lblArchivePath.Name = "lblArchivePath";
            lblArchivePath.Size = new Size(85, 17);
            lblArchivePath.TabIndex = 3;
            lblArchivePath.Text = "Archive Path:";
            // 
            // lblImagePath
            // 
            lblImagePath.Anchor = AnchorStyles.Right;
            lblImagePath.AutoSize = true;
            lblImagePath.Location = new Point(75, 14);
            lblImagePath.Name = "lblImagePath";
            lblImagePath.Size = new Size(77, 17);
            lblImagePath.TabIndex = 0;
            lblImagePath.Text = "Image Path:";
            // 
            // tableLayoutPanel8
            // 
            tableLayoutPanel8.ColumnCount = 2;
            tableLayoutPanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 90F));
            tableLayoutPanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tableLayoutPanel8.Controls.Add(txtImagePath, 0, 0);
            tableLayoutPanel8.Controls.Add(btnCopyImagePath, 1, 0);
            tableLayoutPanel8.Dock = DockStyle.Fill;
            tableLayoutPanel8.Location = new Point(158, 3);
            tableLayoutPanel8.Name = "tableLayoutPanel8";
            tableLayoutPanel8.RowCount = 1;
            tableLayoutPanel8.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel8.Size = new Size(878, 40);
            tableLayoutPanel8.TabIndex = 2;
            // 
            // txtImagePath
            // 
            txtImagePath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtImagePath.Location = new Point(3, 7);
            txtImagePath.Name = "txtImagePath";
            txtImagePath.ReadOnly = true;
            txtImagePath.Size = new Size(784, 25);
            txtImagePath.TabIndex = 1;
            // 
            // btnCopyImagePath
            // 
            btnCopyImagePath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            btnCopyImagePath.BackColor = Color.Yellow;
            btnCopyImagePath.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnCopyImagePath.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnCopyImagePath.FlatStyle = FlatStyle.Flat;
            btnCopyImagePath.Image = Properties.Resources.copy_alt_16x16;
            btnCopyImagePath.Location = new Point(793, 7);
            btnCopyImagePath.Name = "btnCopyImagePath";
            btnCopyImagePath.Size = new Size(82, 25);
            btnCopyImagePath.TabIndex = 2;
            btnCopyImagePath.UseVisualStyleBackColor = false;
            // 
            // lblIconPath
            // 
            lblIconPath.Anchor = AnchorStyles.Right;
            lblIconPath.AutoSize = true;
            lblIconPath.Location = new Point(86, 133);
            lblIconPath.Name = "lblIconPath";
            lblIconPath.Size = new Size(66, 17);
            lblIconPath.TabIndex = 7;
            lblIconPath.Text = "Icon Path:";
            // 
            // tableLayoutPanel9
            // 
            tableLayoutPanel9.ColumnCount = 2;
            tableLayoutPanel9.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 90F));
            tableLayoutPanel9.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tableLayoutPanel9.Controls.Add(btnCopyIconPath, 0, 0);
            tableLayoutPanel9.Controls.Add(txtIconPath, 0, 0);
            tableLayoutPanel9.Dock = DockStyle.Fill;
            tableLayoutPanel9.Location = new Point(158, 127);
            tableLayoutPanel9.Name = "tableLayoutPanel9";
            tableLayoutPanel9.RowCount = 1;
            tableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel9.Size = new Size(878, 29);
            tableLayoutPanel9.TabIndex = 9;
            // 
            // btnCopyIconPath
            // 
            btnCopyIconPath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            btnCopyIconPath.BackColor = Color.Yellow;
            btnCopyIconPath.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 255);
            btnCopyIconPath.FlatAppearance.MouseOverBackColor = Color.FromArgb(2, 215, 242);
            btnCopyIconPath.FlatStyle = FlatStyle.Flat;
            btnCopyIconPath.Image = Properties.Resources.copy_alt_16x16;
            btnCopyIconPath.Location = new Point(793, 3);
            btnCopyIconPath.Name = "btnCopyIconPath";
            btnCopyIconPath.Size = new Size(82, 22);
            btnCopyIconPath.TabIndex = 3;
            btnCopyIconPath.UseVisualStyleBackColor = false;
            // 
            // txtIconPath
            // 
            txtIconPath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtIconPath.Location = new Point(3, 3);
            txtIconPath.Name = "txtIconPath";
            txtIconPath.ReadOnly = true;
            txtIconPath.Size = new Size(784, 25);
            txtIconPath.TabIndex = 2;
            // 
            // fldgBrowserOutput
            // 
            fldgBrowserOutput.Description = "Choose an output folder for the final .archive file";
            fldgBrowserOutput.UseDescriptionForTitle = true;
            // 
            // bgCreateIconWorker
            // 
            bgCreateIconWorker.WorkerReportsProgress = true;
            bgCreateIconWorker.WorkerSupportsCancellation = true;
            bgCreateIconWorker.DoWork += bgCreateIconWorker_DoWork;
            bgCreateIconWorker.ProgressChanged += bgCreateIconWorker_ProgressChanged;
            bgCreateIconWorker.RunWorkerCompleted += bgCreateIconWorker_RunWorkerCompleted;
            // 
            // CreateIconCtl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tlpMainTable);
            Name = "CreateIconCtl";
            Size = new Size(1051, 891);
            Load += CreateIconCtl_Load;
            tlpMainTable.ResumeLayout(false);
            grpStep1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picImportedImage).EndInit();
            grpAtlasName.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            grpCreateIcon.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvStatus).EndInit();
            tableLayoutPanel4.ResumeLayout(false);
            grpAfterCreation.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            grpIconProperties.ResumeLayout(false);
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
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tlpMainTable;
        private GroupBox grpStep1;
        private Button btnCopyImportedImagePath;
        private TextBox txtImportedImagePath;
        private Label lblImportedImagePath;
        private CustomPictureBox picImportedImage;
        private TableLayoutPanel tableLayoutPanel2;
        private GroupBox grpAtlasName;
        private TableLayoutPanel tableLayoutPanel3;
        private Label label1;
        private TextBox txtAtlasName;
        private GroupBox grpCreateIcon;
        private TableLayoutPanel tableLayoutPanel4;
        private Button btnCancel;
        private Button btnCreateIcon;
        private DataGridView dgvStatus;
        private DataGridViewTextBoxColumn colTime;
        private DataGridViewTextBoxColumn colOutput;
        private Label label2;
        private TextBox txtOutputPath;
        private Button btnChooseOutput;
        private FolderBrowserDialog fldgBrowserOutput;
        private GroupBox grpAfterCreation;
        private TableLayoutPanel tableLayoutPanel1;
        private Button btnSendToCRA;
        private Button btnOpenContainingFolder;
        private GroupBox grpIconProperties;
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
        private System.ComponentModel.BackgroundWorker bgCreateIconWorker;
    }
}
