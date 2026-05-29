# Azure Storage Explorer Simulator - Telerik Edition

A modern Windows Forms application built with **Telerik UI controls** and **.NET 9** that simulates Azure Storage Explorer functionality to connect to Azure Blob Storage, upload files, download blobs, and manage blob containers.

## ✨ Features

- 🔐 **Modern Telerik UI** - Professional-grade user interface with Telerik WinForms controls
- 🔗 **Azure Connection** - Connect to Azure Blob Storage using connection strings
- 📤 **Upload Files** - Upload files with progress tracking
- 📥 **Download Blobs** - Download blobs to your local machine
- 🗑️ **Delete Blobs** - Delete blobs with confirmation dialogs
- 🔄 **Refresh List** - Real-time blob list updates
- 📊 **Advanced Grid View** - Professional data grid with sorting, filtering, and selection
- ⚡ **Async Operations** - Non-blocking async/await for smooth UX
- 🎨 **Telerik Theme** - Modern TelerikMetro theme applied
- 📈 **Status Indicators** - Visual feedback with emoji status indicators

## 🔧 Requirements

- **.NET 9.0** (latest framework)
- **Windows 10/11** or **Windows Server 2019+**
- **Visual Studio 2022** (v17.9+) or **Visual Studio Code**
- **Azure Storage Account** (for testing)
- **Telerik License** (Community or Commercial)

## 📥 Installation & Setup

### 1. Clone the Repository
```bash
git clone https://github.com/hrdm1105-dotcom/azure-storage-explorer-sim.git
cd azure-storage-explorer-sim
```

### 2. Install Telerik NuGet Package

**Option A: Using Visual Studio Package Manager**
```
Install-Package Telerik.UI.for.WinForms -Version 2024.1.130
```

**Option B: Using .NET CLI**
```bash
dotnet add package Telerik.UI.for.WinForms
```

**Option C: Manual (if using Telerik package source)**
1. Configure your Telerik NuGet credentials
2. Run: `dotnet restore`

### 3. Restore and Build
```bash
dotnet restore
dotnet build
```

### 4. Run the Application
```bash
dotnet run
```

## 🚀 Usage Guide

### Getting Your Azure Storage Connection String

1. Go to **Azure Portal** - https://portal.azure.com
2. Navigate to **Storage Accounts**
3. Select your storage account
4. Click **Access Keys** (left sidebar)
5. Copy the **Connection String** (key1 or key2)

### Connection String Format

```
DefaultEndpointsProtocol=https;AccountName=yourstorageaccount;AccountKey=your-account-key==;EndpointSuffix=core.windows.net
```

### Using the Application

1. **Paste Connection String** into the connection settings field
2. **Enter Container Name** (e.g., "mycontainer")
3. **Click 🔐 Connect** - Establishes connection and loads blobs
4. **Manage Blobs:**

#### 📤 Upload File
- Click "📤 Upload File"
- Select file from your computer
- Progress bar shows upload status
- Blob list updates automatically

#### 📥 Download
- Select blob from the grid
- Click "📥 Download"
- Choose save location
- File downloads to your machine

#### 🗑️ Delete Selected
- Select blob from the grid
- Click "🗑️ Delete Selected"
- Confirm in dialog
- Blob is removed from container

#### 🔄 Refresh List
- Click "🔄 Refresh List"
- Updates blob list from Azure

## 🎨 UI Components (Telerik)

### Connection Settings Panel
- **RadTextBox** - Connection string and container name inputs
- **RadButton** - Connect button with icon support

### Actions Panel
- **RadButton** - Upload, Download, Delete, Refresh buttons with emojis
- Disabled until connection established
- Large 160x40 buttons for easy interaction

### Blob List View
- **RadGridView** - Professional data grid with:
  - **Column Headers**: Blob Name, Size, Created, Last Modified
  - **Auto-resizing columns**
  - **Full-row selection**
  - **Read-only cells**
  - **Sorting & Filtering capabilities**
  - **Single selection mode**

### Status Bar
- **RadLabel** - Displays operation status with emoji indicators
- Shows connection state and action feedback
- Real-time updates

### Progress Indicator
- **RadProgressBar** - Visible during upload/download operations
- Percentage display

## 📁 Project Structure

```
azure-storage-explorer-sim/
├── Program.cs                          # Entry point with Telerik theme
├── MainForm.cs                         # Main UI with Telerik controls
├── AzureStorageExplorer.csproj        # .NET 9 project file
├── README.md                           # This file
└── .gitignore                         # Git configuration
```

## 📦 NuGet Dependencies

```xml
<ItemGroup>
  <PackageReference Include="Azure.Storage.Blobs" Version="12.19.0" />
  <PackageReference Include="Azure.Identity" Version="1.10.0" />
  <PackageReference Include="Telerik.UI.for.WinForms" Version="2024.1.130" />
</ItemGroup>
```

## 🔒 Security Best Practices

⚠️ **Critical Security Guidelines:**

1. **Never hardcode connection strings** - Use Azure Key Vault in production
2. **Use Azure Identity** - Implement DefaultAzureCredential for better security
3. **Rotate keys regularly** - Update storage account keys periodically
4. **Use SAS tokens** - For time-limited access
5. **Network security** - Enable firewalls and service endpoints
6. **Encryption** - Enable Azure Storage encryption at rest and in transit

### Production Implementation Example

```csharp
// Use Azure Identity for authentication
var credential = new DefaultAzureCredential();
var containerClient = new BlobContainerClient(
    new Uri("https://youraccount.blob.core.windows.net/container"),
    credential);
```

## 🐛 Troubleshooting

### Installation Issues

**"Package Telerik.UI.for.WinForms could not be found"**
- Verify Telerik NuGet source is configured
- Check Telerik license is valid
- Try: `dotnet nuget update source Telerik --username [username] --password [password]`

**".NET 9 not installed"**
- Download from: https://dotnet.microsoft.com/download/dotnet/9.0
- Verify: `dotnet --version`

### Connection Issues

**"Invalid connection string"**
- Verify entire string is copied correctly
- Check for trailing spaces
- Ensure format matches Azure standard

**"Container not found"**
- Verify container name spelling
- Ensure container exists in storage account
- Check access permissions

**"Access Denied"**
- Verify storage account key is current
- Check account permissions
- Ensure connection string matches account

### UI Issues

**"Telerik controls not rendering"**
- Verify Telerik package is installed: `dotnet restore`
- Check Telerik license in your environment
- Restart Visual Studio if needed

**"Theme not applying"**
- Ensure theme is set in Program.cs
- Verify Telerik assemblies are loaded

## 🚀 Advanced Features

### Telerik Theme Customization

Available themes:
```csharp
ThemeResolutionService.ApplicationTheme = "TelerikMetro";      // Default
ThemeResolutionService.ApplicationTheme = "Windows11Light";
ThemeResolutionService.ApplicationTheme = "Windows11Dark";
ThemeResolutionService.ApplicationTheme = "Material";
ThemeResolutionService.ApplicationTheme = "Fluent";
```

### Grid View Customization

```csharp
blobGridView.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
blobGridView.AllowUserToResizeColumns = true;
blobGridView.EnableFiltering = true;
blobGridView.ShowFilteringRow = true;
```

## 📈 Performance Tips

- Use async operations for large file transfers
- Implement pagination for containers with many blobs
- Consider connection pooling for multiple operations
- Optimize grid view with virtual scrolling for large datasets

## 🎯 Future Enhancements

- [ ] Telerik RadDocking for custom layouts
- [ ] Advanced search with RadAutoComplete
- [ ] Blob metadata editor with property grid
- [ ] Batch operations with progress tracking
- [ ] Connection history with RadComboBox
- [ ] Settings dialog with theme selection
- [ ] Export to Excel/CSV functionality
- [ ] Real-time sync monitoring
- [ ] Container management (create/delete)
- [ ] Advanced filtering and sorting
- [ ] Blob snapshots and versioning
- [ ] SAS token generation

## 💻 Code Examples

### Upload with Telerik Progress

```csharp
progressBar.Visible = true;
using (var fileStream = File.OpenRead(filePath))
{
    var blobClient = _containerClient.GetBlobClient(fileName);
    await blobClient.UploadAsync(fileStream, overwrite: true);
}
progressBar.Visible = false;
```

### RadGridView Data Binding

```csharp
var blobList = new List<BlobItemData>();
await foreach (var blob in _containerClient.GetBlobsAsync())
{
    blobList.Add(new BlobItemData { ... });
}
blobGridView.DataSource = blobList;
```

### Telerik Message Boxes

```csharp
// Info
RadMessageBox.Show("Operation successful", "Success", 
    MessageBoxButtons.OK, RadMessageIcon.Info);

// Warning
RadMessageBox.Show("Please select an item", "Warning", 
    MessageBoxButtons.OK, RadMessageIcon.Warning);

// Error
RadMessageBox.Show("An error occurred", "Error", 
    MessageBoxButtons.OK, RadMessageIcon.Error);

// Question
var result = RadMessageBox.Show("Continue?", "Question", 
    MessageBoxButtons.YesNo, RadMessageIcon.Question);
```

## 📚 Resources

- [.NET 9 Documentation](https://docs.microsoft.com/dotnet/core/whats-new/dotnet-9)
- [Telerik UI for WinForms](https://www.telerik.com/products/winforms.aspx)
- [Azure Storage Documentation](https://docs.microsoft.com/azure/storage/)
- [Azure SDK for .NET](https://github.com/Azure/azure-sdk-for-net)
- [Telerik Community](https://www.telerik.com/forums)

## 🔗 Support

- **GitHub Issues**: Report bugs or request features
- **Telerik Support**: For Telerik-specific issues
- **Azure Support**: For Azure Storage questions

## 📄 License

This project is provided as-is for educational and development purposes.

---

**Created by:** hrdm1105-dotcom  
**Framework:** .NET 9.0  
**UI Framework:** Telerik UI for WinForms 2024.1+  
**Last Updated:** May 2026  
**Version:** 2.0.0 (Telerik Edition)
