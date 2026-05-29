using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AzureStorageExplorer.Models
{
    public class BlobItem
    {
        public int ItemId { get; set; }
        public int FolderId { get; set; }
        public string BlobName { get; set; }
        public string BlobPath { get; set; }
        public long BlobSize { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class BlobItemService
    {
        private readonly string _connectionString;

        public BlobItemService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<BlobItem>> GetBlobsByFolderAsync(int folderId)
        {
            var blobs = new List<BlobItem>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string query = @"
                        SELECT ItemId, FolderId, BlobName, BlobPath, BlobSize, CreatedDate, ModifiedDate
                        FROM BlobItems
                        WHERE FolderId = @folderId
                        ORDER BY BlobName ASC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(\"@folderId\", folderId);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                blobs.Add(new BlobItem
                                {
                                    ItemId = (int)reader[\"ItemId\"],
                                    FolderId = (int)reader[\"FolderId\"],
                                    BlobName = (string)reader[\"BlobName\"],
                                    BlobPath = (string)reader[\"BlobPath\"],
                                    BlobSize = (long)reader[\"BlobSize\"],
                                    CreatedDate = (DateTime)reader[\"CreatedDate\"],
                                    ModifiedDate = (DateTime)reader[\"ModifiedDate\"]
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($\"Error retrieving blob items: {ex.Message}\", ex);
            }

            return blobs;
        }

        public async Task<BlobItem> AddBlobToFolderAsync(int folderId, string blobName, string blobPath, long blobSize)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string query = @"
                        INSERT INTO BlobItems (FolderId, BlobName, BlobPath, BlobSize, CreatedDate, ModifiedDate)
                        VALUES (@folderId, @blobName, @blobPath, @blobSize, GETUTCDATE(), GETUTCDATE());
                        SELECT CAST(SCOPE_IDENTITY() as int);";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(\"@folderId\", folderId);
                        cmd.Parameters.AddWithValue(\"@blobName\", blobName);
                        cmd.Parameters.AddWithValue(\"@blobPath\", blobPath);
                        cmd.Parameters.AddWithValue(\"@blobSize\", blobSize);

                        int itemId = (int)await cmd.ExecuteScalarAsync();

                        return new BlobItem
                        {
                            ItemId = itemId,
                            FolderId = folderId,
                            BlobName = blobName,
                            BlobPath = blobPath,
                            BlobSize = blobSize,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedDate = DateTime.UtcNow
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($\"Error adding blob to folder: {ex.Message}\", ex);
            }
        }

        public async Task<bool> DeleteBlobItemAsync(int itemId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string query = \"DELETE FROM BlobItems WHERE ItemId = @itemId\";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(\"@itemId\", itemId);
                        int result = await cmd.ExecuteNonQueryAsync();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($\"Error deleting blob item: {ex.Message}\", ex);
            }
        }

        public async Task<bool> DeleteBlobsByFolderAsync(int folderId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string query = \"DELETE FROM BlobItems WHERE FolderId = @folderId\";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(\"@folderId\", folderId);
                        int result = await cmd.ExecuteNonQueryAsync();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($\"Error deleting blobs by folder: {ex.Message}\", ex);
            }
        }
    }
}
