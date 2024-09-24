namespace WolvenIconGenerator.Custom_Controls
{
    partial class ExportIconCtl
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
            button1 = new Button();
            label1 = new Label();
            button2 = new Button();
            openFileDialog1 = new OpenFileDialog();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            pgIconProgress = new ProgressBar();
            dgvStatus = new DataGridView();
            colTime = new DataGridViewTextBoxColumn();
            colOutput = new DataGridViewTextBoxColumn();
            propertyGrid1 = new PropertyGrid();
            ((System.ComponentModel.ISupportInitialize)dgvStatus).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(16, 21);
            button1.Name = "button1";
            button1.Size = new Size(155, 23);
            button1.TabIndex = 0;
            button1.Text = "Select archive";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(177, 24);
            label1.Name = "label1";
            label1.Size = new Size(37, 16);
            label1.TabIndex = 1;
            label1.Text = "label1";
            // 
            // button2
            // 
            button2.Location = new Point(16, 50);
            button2.Name = "button2";
            button2.Size = new Size(155, 23);
            button2.TabIndex = 2;
            button2.Text = "Extract";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            // 
            // pgIconProgress
            // 
            pgIconProgress.Location = new Point(0, 418);
            pgIconProgress.Name = "pgIconProgress";
            pgIconProgress.Size = new Size(823, 23);
            pgIconProgress.TabIndex = 4;
            // 
            // dgvStatus
            // 
            dgvStatus.AllowUserToAddRows = false;
            dgvStatus.AllowUserToDeleteRows = false;
            dgvStatus.BackgroundColor = Color.FromArgb(224, 224, 224);
            dgvStatus.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvStatus.Columns.AddRange(new DataGridViewColumn[] { colTime, colOutput });
            dgvStatus.Dock = DockStyle.Bottom;
            dgvStatus.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvStatus.Location = new Point(0, 447);
            dgvStatus.Name = "dgvStatus";
            dgvStatus.ReadOnly = true;
            dgvStatus.Size = new Size(1063, 272);
            dgvStatus.TabIndex = 5;
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
            // propertyGrid1
            // 
            propertyGrid1.Location = new Point(463, 3);
            propertyGrid1.Name = "propertyGrid1";
            propertyGrid1.Size = new Size(597, 409);
            propertyGrid1.TabIndex = 6;
            // 
            // ExportIconCtl
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(propertyGrid1);
            Controls.Add(dgvStatus);
            Controls.Add(pgIconProgress);
            Controls.Add(button2);
            Controls.Add(label1);
            Controls.Add(button1);
            Name = "ExportIconCtl";
            Size = new Size(1063, 719);
            Load += ExportIconCtl_Load;
            ((System.ComponentModel.ISupportInitialize)dgvStatus).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label label1;
        private Button button2;
        private OpenFileDialog openFileDialog1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private ProgressBar pgIconProgress;
        private DataGridView dgvStatus;
        private DataGridViewTextBoxColumn colTime;
        private DataGridViewTextBoxColumn colOutput;
        private PropertyGrid propertyGrid1;
    }
}
