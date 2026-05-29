-- Create Database
CREATE DATABASE AzureStorageDB;
GO

USE AzureStorageDB;
GO

-- Create Folders Table
CREATE TABLE Folders (
    FolderId INT PRIMARY KEY IDENTITY(1,1),
    FolderName NVARCHAR(255) NOT NULL,
    ParentFolderId INT NULL,
    Description NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    CONSTRAINT FK_Folders_Parent FOREIGN KEY (ParentFolderId) REFERENCES Folders(FolderId)
);
GO

-- Create Blobs Table
CREATE TABLE Blobs (
    BlobId INT PRIMARY KEY IDENTITY(1,1),
    FolderId INT NOT NULL,
    BlobName NVARCHAR(255) NOT NULL,
    BlobPath NVARCHAR(MAX),
    Size BIGINT,
    ContentType NVARCHAR(100),
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE(),
    IsDeleted BIT DEFAULT 0,
    CONSTRAINT FK_Blobs_Folder FOREIGN KEY (FolderId) REFERENCES Folders(FolderId)
);
GO

-- Create ConnectionSettings Table
CREATE TABLE ConnectionSettings (
    SettingId INT PRIMARY KEY IDENTITY(1,1),
    SettingName NVARCHAR(255) NOT NULL UNIQUE,
    SettingValue NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE()
);
GO

-- Create Indexes
CREATE INDEX IX_Folders_ParentFolderId ON Folders(ParentFolderId);
CREATE INDEX IX_Folders_IsActive ON Folders(IsActive);
CREATE INDEX IX_Blobs_FolderId ON Blobs(FolderId);
CREATE INDEX IX_Blobs_BlobName ON Blobs(BlobName);
CREATE INDEX IX_Blobs_IsDeleted ON Blobs(IsDeleted);
GO

-- Insert Root Folder
INSERT INTO Folders (FolderName, ParentFolderId, Description, IsActive) 
VALUES ('Root', NULL, 'Root folder for all blobs', 1);
GO

-- Verify tables
SELECT 'Folders' as TableName, COUNT(*) as Count FROM Folders
UNION ALL
SELECT 'Blobs' as TableName, COUNT(*) as Count FROM Blobs
UNION ALL
SELECT 'ConnectionSettings' as TableName, COUNT(*) as Count FROM ConnectionSettings;
GO
