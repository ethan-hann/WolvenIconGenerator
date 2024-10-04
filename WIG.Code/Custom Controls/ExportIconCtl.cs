using AetherUtils.Core.Logging;
using System.Drawing;
using WIG.Lib.Models;
using WIG.Lib.Utility;
using WolvenIconGenerator.Utility;

namespace WolvenIconGenerator.Custom_Controls
{
    public partial class ExportIconCtl : UserControl
    {
        private CancellationTokenSource _cancellationTokenSource;

        public ExportIconCtl()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                label1.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.CancellationPending || backgroundWorker1.IsBusy) return;

            backgroundWorker1.RunWorkerAsync();
        }

        private WolvenIcon? _icon;
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Invoke(() =>
            {
                SetProgressPercentage(0);
            });

            // Create a new CancellationTokenSource for each operation
            _cancellationTokenSource = new CancellationTokenSource();

            // Assuming the max value is initially unknown or very high
            int maxValue = 250;

            var progress = new Progress<int>(value =>
            {
                // Scale progress to fit between 0 and 100
                int scaledValue = (value * 100) / maxValue;
                pgIconProgress.Invoke((() => pgIconProgress.Value = Math.Min(100, scaledValue)));
            });

            _icon = IconManager.Instance.ExtractIconImageAsync(label1.Text, progress, true, _cancellationTokenSource.Token).Result;
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Icon creation finished. The icon's properties are shown until a new icon is created.", "Icon Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (_icon != null)
                propertyGrid1.SelectedObject = _icon;
        }

        private void AddStatusRow(string? status)
        {
            if (InvokeRequired)
            {
                Invoke(() => AddStatusRow(status));
            }
            else
            {
                if (status == null) return;
                dgvStatus.Rows.Add(DateTime.Now, status);
            }
        }

        private void SetProgressPercentage(int percentage)
        {
            if (InvokeRequired)
            {
                Invoke(() => SetProgressPercentage(percentage));
            }
            else
            {
                pgIconProgress.Value = percentage;
            }
        }

        private void ExportIconCtl_Load(object sender, EventArgs e)
        {
            IconManager.Instance.IconImportStarted += IconManagerStatusChanged;
            IconManager.Instance.IconImportFinished += IconManagerStatusChanged;
            IconManager.Instance.CliStatus += IconManagerStatusChanged;
        }

        ~ExportIconCtl()
        {
            IconManager.Instance.IconImportStarted -= IconManagerStatusChanged;
            IconManager.Instance.IconImportFinished -= IconManagerStatusChanged;
            IconManager.Instance.CliStatus -= IconManagerStatusChanged;
        }

        private void IconManagerStatusChanged(object? sender, StatusEventArgs e)
        {
            Invoke(() =>
            {
                AddStatusRow(e.Message);
                //SetProgressPercentage(e.ProgressPercentage);
            });
        }
    }
}
