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
            tableLayoutPanel1 = new TableLayoutPanel();
            grpStep1 = new GroupBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            picImportedImage = new CustomPictureBox();
            lblImportedImagePath = new Label();
            txtImportedImagePath = new TextBox();
            btnCopyImportedImagePath = new Button();
            tableLayoutPanel1.SuspendLayout();
            grpStep1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picImportedImage).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 49.72119F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.27881F));
            tableLayoutPanel1.Controls.Add(grpStep1, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 40.7303352F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 59.2696648F));
            tableLayoutPanel1.Size = new Size(912, 712);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // grpStep1
            // 
            tableLayoutPanel1.SetColumnSpan(grpStep1, 2);
            grpStep1.Controls.Add(tableLayoutPanel2);
            grpStep1.Dock = DockStyle.Fill;
            grpStep1.Font = new Font("Segoe UI Variable Display Semib", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            grpStep1.Location = new Point(3, 3);
            grpStep1.Name = "grpStep1";
            grpStep1.Size = new Size(906, 283);
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
            tableLayoutPanel2.Size = new Size(900, 259);
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
            picImportedImage.Size = new Size(894, 210);
            picImportedImage.SizeMode = PictureBoxSizeMode.Zoom;
            picImportedImage.TabIndex = 0;
            picImportedImage.TabStop = false;
            picImportedImage.DragDrop += picImportedImage_DragDrop;
            // 
            // lblImportedImagePath
            // 
            lblImportedImagePath.Anchor = AnchorStyles.Right;
            lblImportedImagePath.AutoSize = true;
            lblImportedImagePath.Font = new Font("Segoe UI Variable Display Semib", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblImportedImagePath.Location = new Point(3, 229);
            lblImportedImagePath.Name = "lblImportedImagePath";
            lblImportedImagePath.Size = new Size(80, 17);
            lblImportedImagePath.TabIndex = 3;
            lblImportedImagePath.Text = "Image Path: ";
            // 
            // txtImportedImagePath
            // 
            txtImportedImagePath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtImportedImagePath.Enabled = false;
            txtImportedImagePath.Location = new Point(89, 225);
            txtImportedImagePath.Name = "txtImportedImagePath";
            txtImportedImagePath.ReadOnly = true;
            txtImportedImagePath.Size = new Size(768, 25);
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
            btnCopyImportedImagePath.Location = new Point(863, 221);
            btnCopyImportedImagePath.Name = "btnCopyImportedImagePath";
            btnCopyImportedImagePath.Size = new Size(32, 32);
            btnCopyImportedImagePath.TabIndex = 5;
            btnCopyImportedImagePath.UseVisualStyleBackColor = false;
            btnCopyImportedImagePath.Click += btnCopyImportedImagePath_Click;
            // 
            // CreateIconCtl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "CreateIconCtl";
            Size = new Size(912, 712);
            tableLayoutPanel1.ResumeLayout(false);
            grpStep1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picImportedImage).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private GroupBox grpStep1;
        private Button btnCopyImportedImagePath;
        private TextBox txtImportedImagePath;
        private Label lblImportedImagePath;
        private CustomPictureBox picImportedImage;
        private TableLayoutPanel tableLayoutPanel2;
    }
}
