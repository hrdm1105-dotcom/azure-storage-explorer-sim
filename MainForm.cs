using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureStorageExplorer
{
    public partial class MainForm : Form
    {
        private BlobContainerClient _containerClient;
        private string _connectionString;
        private string _containerName;

        public MainForm()
        {
            InitializeComponent();
            this.Text = "Azure Storage Explorer Simulator";
            this.Size = new System.Drawing.Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeComponent()
        {
            // Panel for connection settings
            var connectionPanel = new GroupBox();
            connectionPanel.Text = "Connection Settings";
            connectionPanel.Location = new System.Drawing.Point(10, 10);
            connectionPanel.Size = new System.Drawing.Size(960, 100);

            var label1 = new Label();
            label1.Text = "Connection String:";
            label1.Location = new System.Drawing.Point(10, 25);
            label1.Size = new System.Drawing.Size(100, 20);

            var connectionStringTextBox = new TextBox();
            connectionStringTextBox.Name = "connectionStringTextBox";
            connectionStringTextBox.Location = new System.Drawing.Point(120, 25);
            connectionStringTextBox.Size = new System.Drawing.Size(500, 20);
            connectionStringTextBox.UseSystemPasswordChar = false;

            var label2 = new Label();
            label2.Text = "Container Name:";
            label2.Location = new System.Drawing.Point(10, 55);
            label2.Size = new System.Drawing.Size(100, 20);

            var containerNameTextBox = new TextBox();
            containerNameTextBox.Name = "containerNameTextBox";
            containerNameTextBox.Location = new System.Drawing.Point(120, 55);
            containerNameTextBox.Size = new System.Drawing.Size(200, 20);

            var connectButton = new Button();
            connectButton.Text = "Connect";
            connectButton.Location = new System.Drawing.Point(630, 25);
            connectButton.Size = new System.Drawing.Size(100, 30);
            connectButton.Click += ConnectButton_Click;

            connectionPanel.Controls.Add(label1);
            connectionPanel.Controls.Add(connectionStringTextBox);
            connectionPanel.Controls.Add(label2);
            connectionPanel.Controls.Add(containerNameTextBox);
            connectionPanel.Controls.Add(connectButton);

            // Panel for actions
            var actionPanel = new GroupBox();
            actionPanel.Text = "Actions";
            actionPanel.Location = new System.Drawing.Point(10, 115);
            actionPanel.Size = new System.Drawing.Size(960, 60);

            var uploadButton = new Button();
            uploadButton.Name = "uploadButton";
            uploadButton.Text = "Upload File";
            uploadButton.Location = new System.Drawing.Point(10, 25);
            uploadButton.Size = new System.Drawing.Size(120, 30);
            uploadButton.Click += UploadButton_Click;
            uploadButton.Enabled = false;

            var refreshButton = new Button();
            refreshButton.Name = "refreshButton";
            refreshButton.Text = "Refresh List";
            refreshButton.Location = new System.Drawing.Point(140, 25);
            refreshButton.Size = new System.Drawing.Size(120, 30);
            refreshButton.Click += RefreshButton_Click;
            refreshButton.Enabled = false;

            var deleteButton = new Button();
            deleteButton.Name = "deleteButton";
            deleteButton.Text = "Delete Selected";
            deleteButton.Location = new System.Drawing.Point(270, 25);
            deleteButton.Size = new System.Drawing.Size(120, 30);
            deleteButton.Click += DeleteButton_Click;
            deleteButton.Enabled = false;

            var downloadButton = new Button();
            downloadButton.Name = "downloadButton";
            downloadButton.Text = "Download";
            downloadButton.Location = new System.Drawing.Point(400, 25);
            downloadButton.Size = new System.Drawing.Size(120, 30);
            downloadButton.Click += DownloadButton_Click;
            downloadButton.Enabled = false;

            actionPanel.Controls.Add(uploadButton);
            actionPanel.Controls.Add(refreshButton);
            actionPanel.Controls.Add(deleteButton);
            actionPanel.Controls.Add(downloadButton);

            // List view for blobs
            var blobListView = new ListView();
            blobListView.Name = "blobListView";
            blobListView.Location = new System.Drawing.Point(10, 185);
            blobListView.Size = new System.Drawing.Size(960, 450);
            blobListView.View = View.Details;
            blobListView.FullRowSelect = true;
            blobListView.GridLines = true;
            blobListView.Columns.Add("Blob Name", 300);
            blobListView.Columns.Add("Size (bytes)", 100);
            blobListView.Columns.Add("Created", 200);
            blobListView.Columns.Add("Last Modified", 200);

            // Status bar
            var statusLabel = new Label();
            statusLabel.Name = "statusLabel";
            statusLabel.Text = "Ready";
            statusLabel.Location = new System.Drawing.Point(10, 645);
            statusLabel.Size = new System.Drawing.Size(960, 20);
            statusLabel.BorderStyle = BorderStyle.Fixed3D;

            this.Controls.Add(connectionPanel);
            this.Controls.Add(actionPanel);
            this.Controls.Add(blobListView);
            this.Controls.Add(statusLabel);
        }

        private async void ConnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                var connectionStringTextBox = this.Controls.Cast<Control>()
                    .FirstOrDefault(c => c.Name == "connectionStringTextBox") as TextBox;
                var containerNameTextBox = this.Controls.Cast<Control>()
                    .FirstOrDefault(c => c.Name == "containerNameTextBox") as TextBox;
                var statusLabel = this.Controls.Cast<Control>()
                    .FirstOrDefault(c => c.Name == "statusLabel") as Label;

                _connectionString = connectionStringTextBox?.Text;
                _containerName = containerNameTextBox?.Text;

                if (string.IsNullOrWhiteSpace(_connectionString) || string.IsNullOrWhiteSpace(_containerName))
                {
                    MessageBox.Show("Please enter both connection string and container name.", "Validation Error");
                    return;
                }

                statusLabel.Text = "Connecting...";
                this.Refresh();

                // Create container client
                _containerClient = new BlobContainerClient(new Uri(_connectionString), new Azure.Storage.StorageSharedKeyCredential(
                    ExtractStorageAccountName(_connectionString), ExtractAccountKey(_connectionString)));

                // Test connection by getting properties
                await _containerClient.GetPropertiesAsync();

                statusLabel.Text = $"Connected to container: {_containerName}";
                
                // Enable action buttons
                var uploadButton = this.Controls.Cast<Control>().FirstOrDefault(c => c.Name == "uploadButton") as Button;
                var refreshButton = this.Controls.Cast<Control>().FirstOrDefault(c => c.Name == "refreshButton") as Button;
                var deleteButton = this.Controls.Cast<Control>().FirstOrDefault(c => c.Name == "deleteButton") as Button;
                var downloadButton = this.Controls.Cast<Control>().FirstOrDefault(c => c.Name == "downloadButton") as Button;

                if (uploadButton != null) uploadButton.Enabled = true;
                if (refreshButton != null) refreshButton.Enabled = true;
                if (deleteButton != null) deleteButton.Enabled = true;
                if (downloadButton != null) downloadButton.Enabled = true;

                await RefreshBlobList();
            }
            catch (Exception ex)
            {
                var statusLabel = this.Controls.Cast<Control>().FirstOrDefault(c => c.Name == "statusLabel") as Label;
                statusLabel.Text = $"Connection failed: {ex.Message}";
                MessageBox.Show($"Connection error: {ex.Message}", "Error");
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

                    var statusLabel = this.Controls.Cast<Control>().FirstOrDefault(c => c.Name == "statusLabel") as Label;
                    statusLabel.Text = $"Uploading {fileName}...";
                    this.Refresh();

                    using (var fileStream = File.OpenRead(filePath))
                    {
                        var blobClient = _containerClient.GetBlobClient(fileName);
                        await blobClient.UploadAsync(fileStream, overwrite: true);
                    }

                    statusLabel.Text = $"Successfully uploaded: {fileName}";
                    await RefreshBlobList();
                }
            }
            catch (Exception ex)
            {
                var statusLabel = this.Controls.Cast<Control>().FirstOrDefault(c => c.Name == "statusLabel") as Label;
                statusLabel.Text = $"Upload failed: {ex.Message}";
                MessageBox.Show($"Upload error: {ex.Message}", "Error");
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
                var blobListView = this.Controls.Cast<Control>().FirstOrDefault(c => c.Name == "blobListView") as ListView;
                var statusLabel = this.Controls.Cast<Control>().FirstOrDefault(c => c.Name == "statusLabel") as Label;

                blobListView.Items.Clear();
                statusLabel.Text = "Loading blobs...";
                this.Refresh();

                await foreach (var blobItem in _containerClient.GetBlobsAsync())
                {
                    var item = new ListViewItem(blobItem.Name);
                    item.SubItems.Add(blobItem.Properties.ContentLength?.ToString() ?? "Unknown");
                    item.SubItems.Add(blobItem.Properties.CreatedOn?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Unknown");
                    item.SubItems.Add(blobItem.Properties.LastModified?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Unknown");
                    
                    blobListView.Items.Add(item);
                }

                statusLabel.Text = $"Loaded {blobListView.Items.Count} blobs";
            }
            catch (Exception ex)
            {
                var statusLabel = this.Controls.Cast<Control>().FirstOrDefault(c => c.Name == "statusLabel") as Label;
                statusLabel.Text = $"Refresh failed: {ex.Message}";
                MessageBox.Show($"Error loading blobs: {ex.Message}", "Error");
            }
        }

        private async void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                var blobListView = this.Controls.Cast<Control>().FirstOrDefault(c => c.Name == "blobListView") as ListView;
                var statusLabel = this.Controls.Cast<Control>().FirstOrDefault(c => c.Name == "statusLabel") as Label;

                if (blobListView.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Please select a blob to delete.", "No Selection");
                    return;
                }

                var selectedItem = blobListView.SelectedItems[0];
                var blobName = selectedItem.Text;

                var result = MessageBox.Show($"Are you sure you want to delete '{blobName}'?", "Confirm Delete", MessageBoxButtons.YesNo);
                
                if (result == DialogResult.Yes)
                {
                    statusLabel.Text = $"Deleting {blobName}...";
                    this.Refresh();

                    var blobClient = _containerClient.GetBlobClient(blobName);
                    await blobClient.DeleteAsync();

                    statusLabel.Text = $"Successfully deleted: {blobName}";
                    await RefreshBlobList();
                }
            }
            catch (Exception ex)
            {
                var statusLabel = this.Controls.Cast<Control>().FirstOrDefault(c => c.Name == "statusLabel") as Label;
                statusLabel.Text = $"Delete failed: {ex.Message}";
                MessageBox.Show($"Error deleting blob: {ex.Message}", "Error");
            }
        }

        private async void DownloadButton_Click(object sender, EventArgs e)
        {
            try
            {
                var blobListView = this.Controls.Cast<Control>().FirstOrDefault(c => c.Name == "blobListView") as ListView;
                var statusLabel = this.Controls.Cast<Control>().FirstOrDefault(c => c.Name == "statusLabel") as Label;

                if (blobListView.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Please select a blob to download.", "No Selection");
                    return;
                }

                var selectedItem = blobListView.SelectedItems[0];
                var blobName = selectedItem.Text;

                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = blobName;
                saveFileDialog.Filter = "All files (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    statusLabel.Text = $"Downloading {blobName}...";
                    this.Refresh();

                    var blobClient = _containerClient.GetBlobClient(blobName);
                    var download = await blobClient.DownloadAsync();

                    using (var fileStream = File.Create(saveFileDialog.FileName))
                    {
                        await download.Value.Content.CopyToAsync(fileStream);
                    }

                    statusLabel.Text = $"Successfully downloaded: {blobName}";
                    MessageBox.Show("File downloaded successfully.", "Success");
                }
            }
            catch (Exception ex)
            {
                var statusLabel = this.Controls.Cast<Control>().FirstOrDefault(c => c.Name == "statusLabel") as Label;
                statusLabel.Text = $"Download failed: {ex.Message}";
                MessageBox.Show($"Error downloading blob: {ex.Message}", "Error");
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
    }
}
