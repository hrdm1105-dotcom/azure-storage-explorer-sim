using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace AzureStorageExplorer
{
    public class MainForm : RadForm
    {
        private BlobContainerClient _containerClient;
        private string _connectionString;
        private string _containerName;

        // UI Controls
        private RadTextBox connectionStringTextBox;
        private RadTextBox containerNameTextBox;
        private RadButton connectButton;
        private RadButton uploadButton;
        private RadButton downloadButton;
        private RadButton deleteButton;
        private RadButton refreshButton;
        private RadGridView blobGridView;
        private RadLabel statusLabel;
        private RadProgressBar progressBar;

        public MainForm()
        {
            InitializeComponent();
            this.Text = "Azure Storage Explorer Simulator - Telerik Edition";
            this.Size = new System.Drawing.Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            ThemeResolutionService.ApplicationTheme = "TelerikMetro";
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Connection Settings Group
            var connectionGroupBox = new GroupBox();
            connectionGroupBox.Text = "Connection Settings";
            connectionGroupBox.Location = new System.Drawing.Point(10, 10);
            connectionGroupBox.Size = new System.Drawing.Size(1370, 110);

            var label1 = new RadLabel();
            label1.Text = "Connection String:";
            label1.Location = new System.Drawing.Point(10, 25);
            label1.Size = new System.Drawing.Size(120, 20);

            connectionStringTextBox = new RadTextBox();
            connectionStringTextBox.Location = new System.Drawing.Point(140, 25);
            connectionStringTextBox.Size = new System.Drawing.Size(700, 30);
            connectionStringTextBox.Text = "";

            var label2 = new RadLabel();
            label2.Text = "Container Name:";
            label2.Location = new System.Drawing.Point(10, 65);
            label2.Size = new System.Drawing.Size(120, 20);

            containerNameTextBox = new RadTextBox();
            containerNameTextBox.Location = new System.Drawing.Point(140, 65);
            containerNameTextBox.Size = new System.Drawing.Size(250, 30);
            containerNameTextBox.Text = "";

            connectButton = new RadButton();
            connectButton.Text = "🔐 Connect";
            connectButton.Location = new System.Drawing.Point(900, 35);
            connectButton.Size = new System.Drawing.Size(150, 45);
            connectButton.Click += ConnectButton_Click;
            connectButton.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold);

            connectionGroupBox.Controls.Add(label1);
            connectionGroupBox.Controls.Add(connectionStringTextBox);
            connectionGroupBox.Controls.Add(label2);
            connectionGroupBox.Controls.Add(containerNameTextBox);
            connectionGroupBox.Controls.Add(connectButton);

            // Actions Group
            var actionsGroupBox = new GroupBox();
            actionsGroupBox.Text = "Actions";
            actionsGroupBox.Location = new System.Drawing.Point(10, 130);
            actionsGroupBox.Size = new System.Drawing.Size(1370, 75);

            uploadButton = new RadButton();
            uploadButton.Text = "📤 Upload File";
            uploadButton.Location = new System.Drawing.Point(10, 25);
            uploadButton.Size = new System.Drawing.Size(160, 40);
            uploadButton.Click += UploadButton_Click;
            uploadButton.Enabled = false;
            uploadButton.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);

            downloadButton = new RadButton();
            downloadButton.Text = "📥 Download";
            downloadButton.Location = new System.Drawing.Point(180, 25);
            downloadButton.Size = new System.Drawing.Size(160, 40);
            downloadButton.Click += DownloadButton_Click;
            downloadButton.Enabled = false;
            downloadButton.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);

            deleteButton = new RadButton();
            deleteButton.Text = "🗑️ Delete Selected";
            deleteButton.Location = new System.Drawing.Point(350, 25);
            deleteButton.Size = new System.Drawing.Size(160, 40);
            deleteButton.Click += DeleteButton_Click;
            deleteButton.Enabled = false;
            deleteButton.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);

            refreshButton = new RadButton();
            refreshButton.Text = "🔄 Refresh List";
            refreshButton.Location = new System.Drawing.Point(520, 25);
            refreshButton.Size = new System.Drawing.Size(160, 40);
            refreshButton.Click += RefreshButton_Click;
            refreshButton.Enabled = false;
            refreshButton.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);

            actionsGroupBox.Controls.Add(uploadButton);
            actionsGroupBox.Controls.Add(downloadButton);
            actionsGroupBox.Controls.Add(deleteButton);
            actionsGroupBox.Controls.Add(refreshButton);

            // Blob Grid View
            blobGridView = new RadGridView();
            blobGridView.Location = new System.Drawing.Point(10, 215);
            blobGridView.Size = new System.Drawing.Size(1370, 600);
            blobGridView.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            blobGridView.ReadOnly = false;
            blobGridView.AllowUserToResizeColumns = true;
            blobGridView.AllowUserToResizeRows = false;
            blobGridView.AllowUserToDeleteRows = false;
            blobGridView.SelectionMode = GridViewSelectionMode.FullRowSelect;
            blobGridView.MultiSelect = false;
            blobGridView.ShowGroupPanel = false;
            blobGridView.EnableFiltering = true;
            blobGridView.ShowFilteringRow = true;

            // Setup columns
            var nameColumn = new GridViewTextBoxColumn();
            nameColumn.HeaderText = "Blob Name";
            nameColumn.Name = "BlobName";
            nameColumn.FieldName = "BlobName";
            nameColumn.Width = 500;
            nameColumn.ReadOnly = true;

            var sizeColumn = new GridViewTextBoxColumn();
            sizeColumn.HeaderText = "Size (bytes)";
            sizeColumn.Name = "Size";
            sizeColumn.FieldName = "Size";
            sizeColumn.Width = 150;
            sizeColumn.ReadOnly = true;

            var createdColumn = new GridViewTextBoxColumn();
            createdColumn.HeaderText = "Created";
            createdColumn.Name = "Created";
            createdColumn.FieldName = "Created";
            createdColumn.Width = 200;
            createdColumn.ReadOnly = true;

            var modifiedColumn = new GridViewTextBoxColumn();
            modifiedColumn.HeaderText = "Last Modified";
            modifiedColumn.Name = "LastModified";
            modifiedColumn.FieldName = "LastModified";
            modifiedColumn.Width = 200;
            modifiedColumn.ReadOnly = true;

            blobGridView.Columns.Add(nameColumn);
            blobGridView.Columns.Add(sizeColumn);
            blobGridView.Columns.Add(createdColumn);
            blobGridView.Columns.Add(modifiedColumn);

            // Progress Bar
            progressBar = new RadProgressBar();
            progressBar.Location = new System.Drawing.Point(10, 825);
            progressBar.Size = new System.Drawing.Size(1370, 25);
            progressBar.Visible = false;
            progressBar.ShowPercentage = true;

            // Status Label
            statusLabel = new RadLabel();
            statusLabel.Text = "✅ Ready";
            statusLabel.Location = new System.Drawing.Point(10, 860);
            statusLabel.Size = new System.Drawing.Size(1370, 25);
            statusLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            statusLabel.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);

            // Add controls to form
            this.Controls.Add(connectionGroupBox);
            this.Controls.Add(actionsGroupBox);
            this.Controls.Add(blobGridView);
            this.Controls.Add(progressBar);
            this.Controls.Add(statusLabel);

            this.ResumeLayout();
        }

        private async void ConnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                _connectionString = connectionStringTextBox.Text;
                _containerName = containerNameTextBox.Text;

                if (string.IsNullOrWhiteSpace(_connectionString) || string.IsNullOrWhiteSpace(_containerName))
                {
                    RadMessageBox.Show("Please enter both connection string and container name.", "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Warning);
                    return;
                }

                statusLabel.Text = "🔄 Connecting...";
                this.Refresh();

                // Create container client from connection string
                _containerClient = new BlobContainerClient(
                    new Uri($"https://{ExtractStorageAccountName(_connectionString)}.blob.core.windows.net/{_containerName}"),
                    new Azure.Storage.StorageSharedKeyCredential(ExtractStorageAccountName(_connectionString), ExtractAccountKey(_connectionString)));

                // Test connection
                await _containerClient.GetPropertiesAsync();

                statusLabel.Text = $"✅ Connected to container: {_containerName}";

                // Enable action buttons
                uploadButton.Enabled = true;
                downloadButton.Enabled = true;
                deleteButton.Enabled = true;
                refreshButton.Enabled = true;

                await RefreshBlobList();
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"❌ Connection failed: {ex.Message}";
                RadMessageBox.Show($"Connection error: {ex.Message}", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private async void UploadButton_Click(object sender, EventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Select a file to upload";
                openFileDialog.Filter = "All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var filePath = openFileDialog.FileName;
                    var fileName = Path.GetFileName(filePath);

                    statusLabel.Text = $"⬆️ Uploading {fileName}...";
                    progressBar.Visible = true;
                    progressBar.Value = 0;
                    this.Refresh();

                    using (var fileStream = File.OpenRead(filePath))
                    {
                        var blobClient = _containerClient.GetBlobClient(fileName);
                        await blobClient.UploadAsync(fileStream, overwrite: true);
                    }

                    statusLabel.Text = $"✅ Successfully uploaded: {fileName}";
                    progressBar.Visible = false;
                    RadMessageBox.Show($"File '{fileName}' uploaded successfully!", "Success", MessageBoxButtons.OK, RadMessageIcon.Info);
                    await RefreshBlobList();
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"❌ Upload failed: {ex.Message}";
                progressBar.Visible = false;
                RadMessageBox.Show($"Upload error: {ex.Message}", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private async void RefreshButton_Click(object sender, EventArgs e)
        {
            await RefreshBlobList();
        }

        private async Task RefreshBlobList()
        {
            try
            {
                blobGridView.DataSource = null;
                statusLabel.Text = "🔄 Loading blobs...";
                progressBar.Visible = true;
                this.Refresh();

                var blobList = new List<BlobItemData>();

                await foreach (var blobItem in _containerClient.GetBlobsAsync())
                {
                    blobList.Add(new BlobItemData
                    {
                        BlobName = blobItem.Name,
                        Size = blobItem.Properties.ContentLength?.ToString() ?? "Unknown",
                        Created = blobItem.Properties.CreatedOn?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Unknown",
                        LastModified = blobItem.Properties.LastModified?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Unknown"
                    });
                }

                blobGridView.DataSource = blobList;
                statusLabel.Text = $"✅ Loaded {blobList.Count} blobs";
                progressBar.Visible = false;
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"❌ Refresh failed: {ex.Message}";
                progressBar.Visible = false;
                RadMessageBox.Show($"Error loading blobs: {ex.Message}", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private async void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (blobGridView.SelectedRows.Count == 0)
                {
                    RadMessageBox.Show("Please select a blob to delete.", "No Selection", MessageBoxButtons.OK, RadMessageIcon.Warning);
                    return;
                }

                var selectedRow = blobGridView.SelectedRows[0];
                var blobName = selectedRow.Cells["BlobName"].Value.ToString();

                var result = RadMessageBox.Show($"Are you sure you want to delete '{blobName}'?", "Confirm Delete", MessageBoxButtons.YesNo, RadMessageIcon.Question);

                if (result == DialogResult.Yes)
                {
                    statusLabel.Text = $"🗑️ Deleting {blobName}...";
                    progressBar.Visible = true;
                    this.Refresh();

                    var blobClient = _containerClient.GetBlobClient(blobName);
                    await blobClient.DeleteAsync();

                    statusLabel.Text = $"✅ Successfully deleted: {blobName}";
                    progressBar.Visible = false;
                    await RefreshBlobList();
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"❌ Delete failed: {ex.Message}";
                progressBar.Visible = false;
                RadMessageBox.Show($"Error deleting blob: {ex.Message}", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private async void DownloadButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (blobGridView.SelectedRows.Count == 0)
                {
                    RadMessageBox.Show("Please select a blob to download.", "No Selection", MessageBoxButtons.OK, RadMessageIcon.Warning);
                    return;
                }

                var selectedRow = blobGridView.SelectedRows[0];
                var blobName = selectedRow.Cells["BlobName"].Value.ToString();

                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = blobName;
                saveFileDialog.Filter = "All files (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    statusLabel.Text = $"⬇️ Downloading {blobName}...";
                    progressBar.Visible = true;
                    this.Refresh();

                    var blobClient = _containerClient.GetBlobClient(blobName);
                    var download = await blobClient.DownloadAsync();

                    using (var fileStream = File.Create(saveFileDialog.FileName))
                    {
                        await download.Value.Content.CopyToAsync(fileStream);
                    }

                    statusLabel.Text = $"✅ Successfully downloaded: {blobName}";
                    progressBar.Visible = false;
                    RadMessageBox.Show("File downloaded successfully.", "Success", MessageBoxButtons.OK, RadMessageIcon.Info);
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"❌ Download failed: {ex.Message}";
                progressBar.Visible = false;
                RadMessageBox.Show($"Error downloading blob: {ex.Message}", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private string ExtractStorageAccountName(string connectionString)
        {
            try
            {
                var parts = connectionString.Split(';');
                var accountNamePart = parts.FirstOrDefault(p => p.StartsWith("AccountName="));
                if (accountNamePart != null)
                {
                    return accountNamePart.Replace("AccountName=", "");
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private string ExtractAccountKey(string connectionString)
        {
            try
            {
                var parts = connectionString.Split(';');
                var keyPart = parts.FirstOrDefault(p => p.StartsWith("AccountKey="));
                return keyPart?.Replace("AccountKey=", "");
            }
            catch
            {
                return null;
            }
        }

        // Model for grid binding
        private class BlobItemData
        {
            public string BlobName { get; set; }
            public string Size { get; set; }
            public string Created { get; set; }
            public string LastModified { get; set; }
        }
    }
}
