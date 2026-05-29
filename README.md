# Azure Storage Explorer Simulator

A Windows Forms application that simulates Azure Storage Explorer functionality to connect to Azure Blob Storage, upload files, download blobs, and manage blob containers.

## Features

✨ **Core Features:**
- 🔐 **Connect to Azure Storage** - Connect using connection strings
- 📤 **Upload Files** - Upload files to blob containers
- 📥 **Download Blobs** - Download blobs to your local machine
- 🗑️ **Delete Blobs** - Delete blobs with confirmation
- 🔄 **Refresh** - Reload blob list to see latest changes
- 📊 **Blob Details** - View blob metadata (size, created date, last modified)

## Requirements

- **.NET 6.0 or higher** (.NET 7.0+ recommended)
- **Windows 10/11** (or Windows Server 2019+)
- **Visual Studio 2022** or **Visual Studio Code** with C# extension
- **Azure Storage Account** (for testing)

## Installation & Setup

### 1. Clone the Repository
```bash
git clone https://github.com/hrdm1105-dotcom/azure-storage-explorer-sim.git
cd azure-storage-explorer-sim
```

### 2. Install Dependencies
```bash
dotnet restore
```

### 3. Build the Project
```bash
dotnet build
```

### 4. Run the Application
```bash
dotnet run
```

## Usage Guide

### Getting Your Azure Storage Connection String

1. **Sign in to Azure Portal** - https://portal.azure.com
2. **Navigate to Storage Accounts**
3. **Select your storage account**
4. **Go to Access Keys** (left menu)
5. **Copy the Connection String** from key1 or key2

### Connection String Format

Your connection string should look like:
```
DefaultEndpointsProtocol=https;AccountName=yourstorageaccount;AccountKey=your-account-key==;EndpointSuffix=core.windows.net
```

### Using the Application

1. **Paste Connection String** into the "Connection String" field
2. **Enter Container Name** (e.g., "mycontainer")
3. **Click Connect** to establish connection
4. **Once Connected**, use these actions:

#### 📤 Upload File
- Click "Upload File"
- Select a file from your computer
- File will be uploaded to the blob container

#### 📥 Download
- Select a blob from the list
- Click "Download"
- Choose save location
- File downloads to your machine

#### 🗑️ Delete Selected
- Select a blob from the list
- Click "Delete Selected"
- Confirm deletion when prompted

#### 🔄 Refresh List
- Click "Refresh List" to reload blobs
- Shows all blobs currently in container

## Blob List View

The blob list displays:
- **Blob Name** - Name of the blob file
- **Size (bytes)** - File size in bytes
- **Created** - Creation timestamp
- **Last Modified** - Last modification timestamp

## Application Architecture

```
AzureStorageExplorer/
├── Program.cs                 # Entry point
├── MainForm.cs               # UI & Logic
├── AzureStorageExplorer.csproj # Project config
├── README.md                 # This file
└── .gitignore               # Git configuration
```

### Key Components

- **MainForm.cs** - Main Windows Form containing:
  - Connection Settings Panel
  - Action Buttons Panel
  - Blob List View
  - Status Bar

- **Azure SDK Integration**
  - Uses `Azure.Storage.Blobs` v12.19.0
  - Async/await patterns for non-blocking operations
  - Connection string authentication

## NuGet Dependencies

```xml
<PackageReference Include="Azure.Storage.Blobs" Version="12.19.0" />
<PackageReference Include="Azure.Identity" Version="1.10.0" />
```

## Error Handling

The application handles:
- ❌ Invalid connection strings
- ❌ Network connectivity issues
- ❌ File upload/download failures
- ❌ Blob deletion errors
- ❌ Container access issues

All errors are displayed in:
1. Status bar at bottom of form
2. Message box dialogs

## Security Best Practices

⚠️ **Important Security Considerations:**

1. **Never hardcode connection strings** in production code
2. **Use Azure Key Vault** to store credentials
3. **Implement Managed Identities** when possible
4. **Rotate storage account keys** regularly
5. **Use SAS tokens** for time-limited access
6. **Apply network security** (firewalls, service endpoints)
7. **Enable Azure Storage encryption** at rest

### For Production Use:

```csharp
// Use Azure Identity instead of connection strings
var credential = new DefaultAzureCredential();
var containerClient = new BlobContainerClient(
    new Uri("https://youraccount.blob.core.windows.net/container"),
    credential);
```

## Troubleshooting

### Connection Issues

**"Invalid connection string"**
- Verify the entire connection string is copied correctly
- Check for spaces or special characters
- Ensure you copied from Azure Portal

**"Container not found"**
- Verify container name spelling
- Ensure container exists in storage account
- Check you have access permissions

**"Access denied"**
- Verify storage account key is correct
- Check account permissions
- Ensure connection string hasn't expired

### File Upload Issues

**"Upload error"**
- Check file size limitations
- Verify file is not locked by another process
- Ensure container write permissions

**"File too large"**
- Azure blob has limits for single-block uploads
- Consider using block blob uploads for large files

### Performance Tips

- For large files, implement progress indicators
- Use batch operations for multiple files
- Consider connection pooling

## Future Enhancements

📋 **Planned Features:**
- [ ] Create/Delete containers
- [ ] Folder-like organization with blob prefixes
- [ ] Progress bars for large transfers
- [ ] Batch file upload
- [ ] Blob metadata viewing/editing
- [ ] Connection history
- [ ] Search and filter functionality
- [ ] Blob properties dialog
- [ ] Blob snapshots support
- [ ] Leased blobs handling

## Code Examples

### Upload Example
```csharp
using (var fileStream = File.OpenRead(filePath))
{
    var blobClient = _containerClient.GetBlobClient(fileName);
    await blobClient.UploadAsync(fileStream, overwrite: true);
}
```

### Download Example
```csharp
var blobClient = _containerClient.GetBlobClient(blobName);
var download = await blobClient.DownloadAsync();
using (var fileStream = File.Create(savePath))
{
    await download.Value.Content.CopyToAsync(fileStream);
}
```

### Delete Example
```csharp
var blobClient = _containerClient.GetBlobClient(blobName);
await blobClient.DeleteAsync();
```

## Support & Contribution

- Report issues via GitHub Issues
- Submit pull requests for improvements
- Follow Microsoft's contribution guidelines

## License

This project is provided as-is for educational and development purposes.

## Resources

- [Azure Storage Documentation](https://docs.microsoft.com/en-us/azure/storage/)
- [Azure SDK for .NET](https://github.com/Azure/azure-sdk-for-net)
- [Windows Forms Documentation](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/)
- [Azure Storage Explorer (Official)](https://azure.microsoft.com/en-us/features/storage-explorer/)

---

**Created by:** hrdm1105-dotcom  
**Last Updated:** 2026  
**Version:** 1.0.0
