using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using document_sharing_manager.Core.Domain;
using document_sharing_manager.Core.Interfaces;
using document_sharing_manager.Core.Data;
using System.Threading;
using System.Threading.Tasks;

namespace document_sharing_manager.Core.Infrastructure.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly string _connectionString;

        public DocumentRepository()
        {
            _connectionString = DatabaseHelper.ConnectionString;
        }

        private Document MapRowToEntity(DataRow row)
        {
            return new Document
            {
                Id = Convert.ToInt32(row["id"]),
                Ten = row["ten"].ToString(),
                DinhDang = row["dinh_dang"]?.ToString() ?? "",
                DuongDan = row["duong_dan"].ToString(),
                GhiChu = row["ghi_chu"].ToString(),
                NgayThem = Convert.ToDateTime(row["ngay_them"]),
                KichThuoc = row["kich_thuoc"] != DBNull.Value ? Convert.ToDecimal(row["kich_thuoc"]) : (decimal?)null,
                QuanTrong = Convert.ToInt32(row["quan_trong"]) == 1,
                Tags = row["tags"].ToString(),
                UserId = row["user_id"] != DBNull.Value ? Convert.ToInt32(row["user_id"]) : 0,
                Version = row["version"] != DBNull.Value ? Convert.ToInt32(row["version"]) : 1,
                SyncStatus = row["sync_status"] != DBNull.Value ? Convert.ToInt32(row["sync_status"]) : 0,
                LocalVersion = row["local_version"] != DBNull.Value ? Convert.ToInt32(row["local_version"]) : 1,
                RemoteId = row["remote_id"] != DBNull.Value ? Guid.Parse(row["remote_id"].ToString()) : throw new InvalidOperationException("Dữ liệu lỗi: Thiếu RemoteId cho tài liệu ID " + row["id"]),
                ServerId = row["server_id"] != DBNull.Value ? Convert.ToInt32(row["server_id"]) : (int?)null
            };
        }

        private List<Document> ExecuteAndMap(string query, System.Data.SQLite.SQLiteParameter[]? parameters = null)
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            List<Document> list = [];
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToEntity(row));
            }
            return list;
        }

        public List<Document> GetAll()
        {
            return ExecuteAndMap("SELECT * FROM tai_lieu WHERE (is_deleted IS NULL OR is_deleted = 0) ORDER BY ngay_them DESC");
        }

        public List<Document> GetByServer(int serverId)
        {
            string query = "SELECT * FROM tai_lieu WHERE server_id = @serverId AND (is_deleted IS NULL OR is_deleted = 0) ORDER BY ngay_them DESC";
            SQLiteParameter[] parameters = [new("@serverId", serverId)];
            return ExecuteAndMap(query, parameters);
        }

        public async Task<Document?> GetByPathAsync(string path, CancellationToken ct = default)
        {
            return await Task.Run(() => 
            {
                string query = "SELECT * FROM tai_lieu WHERE duong_dan = @path AND (is_deleted IS NULL OR is_deleted = 0)";
                SQLiteParameter[] parameters = [new("@path", path)];
                var list = ExecuteAndMap(query, parameters);
                return list.Count > 0 ? list[0] : null;
            }, ct);
        }

        public Document? GetById(int id)
        {
            string query = "SELECT * FROM tai_lieu WHERE id = @id";
            SQLiteParameter[] parameters = [new("@id", id)];
            var list = ExecuteAndMap(query, parameters);
            return list.Count > 0 ? list[0] : null;
        }

        public List<Document> Search(string keyword)
        {
             string query = @"SELECT * FROM tai_lieu
                           WHERE (is_deleted IS NULL OR is_deleted = 0)
                           AND (ten LIKE @keyword
                           OR ghi_chu LIKE @keyword)
                           ORDER BY ngay_them DESC";

             SQLiteParameter[] parameters = [new("@keyword", "%" + keyword + "%")];
             return ExecuteAndMap(query, parameters);
        }

        public List<Document> SearchAdvanced(string keyword, string format, DateTime? fromDate, DateTime? toDate, decimal? minSize, decimal? maxSize, bool? isImportant, int? serverId = null)
        {
            string baseQuery = @"SELECT * FROM tai_lieu WHERE (is_deleted IS NULL OR is_deleted = 0)";
            List<SQLiteParameter> parameterList = [];

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                baseQuery += " AND (ten LIKE @keyword OR ghi_chu LIKE @keyword OR tags LIKE @keyword)";
                parameterList.Add(new("@keyword", "%" + keyword + "%"));
            }

            if (!string.IsNullOrEmpty(format) && format != "Tất cả")
            {
                baseQuery += " AND dinh_dang = @dinh_dang";
                parameterList.Add(new("@dinh_dang", format));
            }

            if (fromDate.HasValue)
            {
                baseQuery += " AND date(ngay_them) >= date(@fromDate)";
                parameterList.Add(new("@fromDate", fromDate.Value.ToString("yyyy-MM-dd")));
            }

            if (toDate.HasValue)
            {
                baseQuery += " AND date(ngay_them) <= date(@toDate)";
                parameterList.Add(new("@toDate", toDate.Value.ToString("yyyy-MM-dd")));
            }

            if (minSize.HasValue)
            {
                baseQuery += " AND kich_thuoc >= @minSize";
                parameterList.Add(new("@minSize", minSize.Value));
            }

            if (maxSize.HasValue)
            {
                baseQuery += " AND kich_thuoc <= @maxSize";
                parameterList.Add(new("@maxSize", maxSize.Value));
            }

            if (isImportant.HasValue && isImportant.Value == true)
            {
                baseQuery += " AND quan_trong = 1";
            }

            if (serverId.HasValue)
            {
                baseQuery += " AND server_id = @serverId";
                parameterList.Add(new("@serverId", serverId.Value));
            }

            baseQuery += " ORDER BY ngay_them DESC";

            return ExecuteAndMap(baseQuery, [.. parameterList]);
        }

        public bool Add(Document doc)
        {
            return DatabaseHelper.InsertDocument(doc.Ten, doc.DinhDang, doc.DuongDan, doc.GhiChu, doc.KichThuoc, doc.QuanTrong, doc.UserId, doc.RemoteId, doc.Version, doc.Tags, doc.SyncStatus, doc.LocalVersion, doc.ServerId);
        }

        public bool Update(Document doc)
        {
             // We pass doc.Version as the expected version to ensure the record hasn't changed.
             return DatabaseHelper.UpdateDocument(doc.Id, doc.Ten, doc.DinhDang, doc.DuongDan, doc.GhiChu, doc.KichThuoc, doc.QuanTrong, doc.UserId, doc.RemoteId, doc.Version, doc.Version, doc.SyncStatus, doc.LocalVersion, doc.ServerId, doc.Tags);
        }

        public bool Delete(int id)
        {
            return DatabaseHelper.DeleteDocument(id);
        }

        public List<string> GetDistinctFormats()
        {
            var dt = DatabaseHelper.GetDistinctTypes();
             var list = new List<string>();
            foreach(DataRow row in dt.Rows) 
                list.Add(row["dinh_dang"]?.ToString() ?? "");
            return list;
        }

        public async Task<Document?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await Task.Run(() => GetById(id)); 
        }

        public async Task<IEnumerable<Document>> GetAllAsync(CancellationToken ct = default)
        {
            var dt = await Task.Run(() => DatabaseHelper.GetAllDocuments());
            var list = new List<Document>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToEntity(row));
            }
            return list;
        }

        public async Task AddAsync(Document entity, CancellationToken ct = default)
        {
            await Task.Run(() => Add(entity));
        }

        public async Task UpdateAsync(Document entity, CancellationToken ct = default)
        {
            if (!await Task.Run(() => Update(entity), ct))
            {
                throw new System.Data.DBConcurrencyException("The document has been modified by another user or does not exist.");
            }
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
             await Task.Run(() => Delete(id));
        }

        public async Task<IEnumerable<Document>> GetAllByUserIdAsync(int userId, CancellationToken ct = default)
        {
            string query = "SELECT * FROM tai_lieu WHERE (is_deleted IS NULL OR is_deleted = 0) AND user_id = @userId ORDER BY ngay_them DESC";
            SQLiteParameter[] parameters = [new("@userId", userId)];
            return await Task.Run(() => ExecuteAndMap(query, parameters));
        }

        public async Task<Document?> GetByIdAndUserIdAsync(int id, int userId, CancellationToken ct = default)
        {
            string query = "SELECT * FROM tai_lieu WHERE id = @id AND user_id = @userId";
            SQLiteParameter[] parameters = 
            [
                new("@id", id),
                new("@userId", userId)
            ];
            var list = await Task.Run(() => ExecuteAndMap(query, parameters));
            return list.Count > 0 ? list[0] : null;
        }

        public async Task<Document?> GetByVersionAsync(int docId, int version, int userId, CancellationToken ct = default)
        {
            string query = "SELECT * FROM tai_lieu WHERE id = @id AND version = @version AND user_id = @userId";
            SQLiteParameter[] parameters = 
            [
                new("@id", docId),
                new("@version", version),
                new("@userId", userId)
            ];
            var list = await Task.Run(() => ExecuteAndMap(query, parameters));
            return list.Count > 0 ? list[0] : null;
        }
        public async Task<Document?> GetByRemoteIdAsync(Guid remoteId, CancellationToken ct = default)
        {
            // We search including deleted records to avoid duplicates when syncing
            return await Task.Run(() => 
            {
                string query = "SELECT * FROM tai_lieu WHERE remote_id = @remoteId";
                SQLiteParameter[] parameters = [new("@remoteId", remoteId.ToString())];
                var list = ExecuteAndMap(query, parameters);
                return list.Count > 0 ? list[0] : null;
            }, ct);
        }
        public async Task DeleteByRemoteIdAsync(Guid remoteId, CancellationToken ct = default)
        {
            await Task.Run(() => 
            {
                string query = "UPDATE tai_lieu SET is_deleted = 1, deleted_at = datetime('now','localtime') WHERE remote_id = @remoteId";
                SQLiteParameter[] parameters = [new("@remoteId", remoteId.ToString())];
                DatabaseHelper.ExecuteNonQuery(query, parameters);
            }, ct);
        }

        public async Task<List<Document>> SearchAsync(string keyword, int userId, CancellationToken ct = default)
        {
            string query = @"SELECT * FROM tai_lieu
                           WHERE (is_deleted IS NULL OR is_deleted = 0)
                           AND user_id = @userId
                           AND (ten LIKE @keyword OR ghi_chu LIKE @keyword)
                           ORDER BY ngay_them DESC";

            SQLiteParameter[] parameters = 
            [
                new("@keyword", "%" + keyword + "%"),
                new("@userId", userId)
            ];
            return await Task.Run(() => ExecuteAndMap(query, parameters));
        }

        public async Task<List<Document>> SearchAdvancedAsync(string keyword, string format, DateTime? fromDate, DateTime? toDate, decimal? minSize, decimal? maxSize, bool? isImportant, int userId, int? serverId = null, CancellationToken ct = default)
        {
            string baseQuery = @"SELECT * FROM tai_lieu WHERE (is_deleted IS NULL OR is_deleted = 0) AND user_id = @userId";
            List<SQLiteParameter> parameterList = [];
            parameterList.Add(new("@userId", userId));

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                baseQuery += " AND (ten LIKE @keyword OR ghi_chu LIKE @keyword OR tags LIKE @keyword)";
                parameterList.Add(new("@keyword", "%" + keyword + "%"));
            }

            if (!string.IsNullOrEmpty(format) && format != "Tất cả")
            {
                baseQuery += " AND dinh_dang = @dinh_dang";
                parameterList.Add(new("@dinh_dang", format));
            }

            if (fromDate.HasValue)
            {
                baseQuery += " AND date(ngay_them) >= date(@fromDate)";
                parameterList.Add(new("@fromDate", fromDate.Value.ToString("yyyy-MM-dd")));
            }

            if (toDate.HasValue)
            {
                baseQuery += " AND date(ngay_them) <= date(@toDate)";
                parameterList.Add(new("@toDate", toDate.Value.ToString("yyyy-MM-dd")));
            }

            if (minSize.HasValue)
            {
                baseQuery += " AND kich_thuoc >= @minSize";
                parameterList.Add(new("@minSize", minSize.Value));
            }

            if (maxSize.HasValue)
            {
                baseQuery += " AND kich_thuoc <= @maxSize";
                parameterList.Add(new("@maxSize", maxSize.Value));
            }

            if (isImportant.HasValue && isImportant.Value == true)
            {
                baseQuery += " AND quan_trong = 1";
            }

            if (serverId.HasValue)
            {
                baseQuery += " AND server_id = @serverId";
                parameterList.Add(new("@serverId", serverId.Value));
            }

            baseQuery += " ORDER BY ngay_them DESC";

            return await Task.Run(() => ExecuteAndMap(baseQuery, [.. parameterList]));
        }
        public async Task<List<Document>> GetPendingSyncDocumentsAsync(int userId, CancellationToken ct = default)
        {
            // We include is_deleted = 1 here because we need to sync deletions too
            string query = "SELECT * FROM tai_lieu WHERE user_id = @userId AND sync_status != 0 ORDER BY ngay_them DESC";
            SQLiteParameter[] parameters = [new("@userId", userId)];
            return await Task.Run(() => ExecuteAndMap(query, parameters), ct);
        }

        public async Task UpdateSyncStatusAsync(int id, int syncStatus, int? newVersion = null, int? expectedVersion = null, int? newLocalVersion = null, CancellationToken ct = default)
        {
            await Task.Run(() => 
            {
                string query = @"UPDATE tai_lieu SET sync_status = @sync_status";
                List<SQLiteParameter> parameters = [new("@sync_status", syncStatus), new("@id", id)];
                
                if (newVersion.HasValue)
                {
                    query += ", version = @version";
                    parameters.Add(new("@version", newVersion.Value));
                }
                
                if (newLocalVersion.HasValue)
                {
                    query += ", local_version = @local_version";
                    parameters.Add(new("@local_version", newLocalVersion.Value));
                }
                
                query += " WHERE id = @id";
                
                if (expectedVersion.HasValue)
                {
                    query += " AND version = @expected_version";
                    parameters.Add(new("@expected_version", expectedVersion.Value));
                }
                
                DatabaseHelper.ExecuteNonQuery(query, [.. parameters]);
            }, ct);
        }
    }
}
