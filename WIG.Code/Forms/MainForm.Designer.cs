namespace WolvenIconGenerator.Forms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            mainTabs = new TabControl();
            tabCreateIcon = new TabPage();
            tabExtractIcon = new TabPage();
            menuStrip1 = new MenuStrip();
            resetTabToolStripMenuItem = new ToolStripMenuItem();
            createIconToolStripMenuItem = new ToolStripMenuItem();
            exportIconToolStripMenuItem = new ToolStripMenuItem();
            cRAIntegrationToolStripMenuItem = new ToolStripMenuItem();
            checkCRAConnectionToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            checkForUpdatesToolStripMenuItem = new ToolStripMenuItem();
            mainTabs.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // mainTabs
            // 
            mainTabs.Controls.Add(tabCreateIcon);
            mainTabs.Controls.Add(tabExtractIcon);
            mainTabs.Dock = DockStyle.Fill;
            mainTabs.Location = new Point(0, 24);
            mainTabs.Name = "mainTabs";
            mainTabs.SelectedIndex = 0;
            mainTabs.Size = new Size(1090, 773);
            mainTabs.TabIndex = 0;
            // 
            // tabCreateIcon
            // 
            tabCreateIcon.Location = new Point(4, 24);
            tabCreateIcon.Name = "tabCreateIcon";
            tabCreateIcon.Padding = new Padding(3);
            tabCreateIcon.Size = new Size(1082, 745);
            tabCreateIcon.TabIndex = 0;
            tabCreateIcon.Text = "Create Icon";
            tabCreateIcon.UseVisualStyleBackColor = true;
            // 
            // tabExtractIcon
            // 
            tabExtractIcon.Location = new Point(4, 24);
            tabExtractIcon.Name = "tabExtractIcon";
            tabExtractIcon.Padding = new Padding(3);
            tabExtractIcon.Size = new Size(1082, 745);
            tabExtractIcon.TabIndex = 1;
            tabExtractIcon.Text = "Extract Icon";
            tabExtractIcon.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { resetTabToolStripMenuItem, cRAIntegrationToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1090, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // resetTabToolStripMenuItem
            // 
            resetTabToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { createIconToolStripMenuItem, exportIconToolStripMenuItem });
            resetTabToolStripMenuItem.Name = "resetTabToolStripMenuItem";
            resetTabToolStripMenuItem.Size = new Size(68, 20);
            resetTabToolStripMenuItem.Text = "Reset Tab";
            // 
            // createIconToolStripMenuItem
            // 
            createIconToolStripMenuItem.Name = "createIconToolStripMenuItem";
            createIconToolStripMenuItem.Size = new Size(134, 22);
            createIconToolStripMenuItem.Text = "Create Icon";
            // 
            // exportIconToolStripMenuItem
            // 
            exportIconToolStripMenuItem.Name = "exportIconToolStripMenuItem";
            exportIconToolStripMenuItem.Size = new Size(134, 22);
            exportIconToolStripMenuItem.Text = "Export Icon";
            // 
            // cRAIntegrationToolStripMenuItem
            // 
            cRAIntegrationToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { checkCRAConnectionToolStripMenuItem });
            cRAIntegrationToolStripMenuItem.Name = "cRAIntegrationToolStripMenuItem";
            cRAIntegrationToolStripMenuItem.Size = new Size(103, 20);
            cRAIntegrationToolStripMenuItem.Text = "CRA Integration";
            // 
            // checkCRAConnectionToolStripMenuItem
            // 
            checkCRAConnectionToolStripMenuItem.Name = "checkCRAConnectionToolStripMenuItem";
            checkCRAConnectionToolStripMenuItem.Size = new Size(196, 22);
            checkCRAConnectionToolStripMenuItem.Text = "Check CRA connection";
            checkCRAConnectionToolStripMenuItem.Click += checkCRAConnectionToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { checkForUpdatesToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            checkForUpdatesToolStripMenuItem.Size = new Size(171, 22);
            checkForUpdatesToolStripMenuItem.Text = "Check for Updates";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1090, 797);
            Controls.Add(mainTabs);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Wolven Engine Icon Generator";
            Load += MainForm_Load;
            mainTabs.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TabControl mainTabs;
        private TabPage tabCreateIcon;
        private TabPage tabExtractIcon;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private ToolStripMenuItem resetTabToolStripMenuItem;
        private ToolStripMenuItem createIconToolStripMenuItem;
        private ToolStripMenuItem exportIconToolStripMenuItem;
        private ToolStripMenuItem cRAIntegrationToolStripMenuItem;
        private ToolStripMenuItem checkCRAConnectionToolStripMenuItem;
    }
}
