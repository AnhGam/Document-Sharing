using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
// using System.Windows.Forms; // Removed for Backend-Frontend Decoupling
using System.Security.Cryptography;
using System.Text;
using document_sharing_manager.Core.Domain;

namespace document_sharing_manager.Core.Data
{
    /// <summary>
    /// Class quản lý kết nối và thao tác với SQLite Database (Local)
    /// </summary>
    public partial class DatabaseHelper
    {
        private static string _databasePath = string.Empty;
        private static string _connectionString = string.Empty;
        public static bool SuppressPopups { get; set; } = false;


        /// <summary>
        /// Đường dẫn đến file database SQLite
        /// </summary>
        public static string DatabasePath
        {
            get
            {
                if (string.IsNullOrEmpty(_databasePath))
                {
                    string appFolder = AppDomain.CurrentDomain.BaseDirectory;
                    _databasePath = Path.Combine(appFolder, "data", "document_sharing.db");
                }
                return _databasePath;
            }
        }

        /// <summary>
        /// Connection string cho SQLite
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    _connectionString = $"Data Source={DatabasePath};Version=3;Pooling=True;";
                }
                return _connectionString;
            }
        }

        /// <summary>
        /// Khởi tạo database - tạo file và các bảng nếu chưa tồn tại
        /// </summary>
        public static void InitializeDatabase()
        {
            try
            {
                // Tạo thư mục data nếu chưa có
                string dataFolder = Path.GetDirectoryName(DatabasePath) ?? string.Empty;
                if (!Directory.Exists(dataFolder))
                {
                    Directory.CreateDirectory(dataFolder);
                }

                // Tạo file database nếu chưa có
                if (!File.Exists(DatabasePath))
                {
                    string oldDbPath = Path.Combine(dataFolder, "study_documents.db");
                    if (File.Exists(oldDbPath))
                    {
                        // Di chuyển dữ liệu từ bản cũ sang bản mới
                        File.Move(oldDbPath, DatabasePath);
                    }
                    else
                    {
                        SQLiteConnection.CreateFile(DatabasePath);
                    }
                }

                // Tạo các bảng
                CreateTables();
            }
            catch (Exception ex)
            {
                // Logic for server: Log the error or throw
                // MessageBox removed for Backend decoupling
                throw new InvalidOperationException("Lỗi khởi tạo database: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Tạo các bảng trong database
        /// </summary>
        private static void CreateTables()
        {
            string createTablesQuery = @"
                -- Bảng tài liệu chính
                CREATE TABLE IF NOT EXISTS tai_lieu (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ten TEXT NOT NULL,
                    dinh_dang TEXT,
                    duong_dan TEXT,
                    ghi_chu TEXT,
                    ngay_them DATETIME DEFAULT (datetime('now', 'localtime')),
                    kich_thuoc NUMERIC,
                    quan_trong INTEGER DEFAULT 0,
                    tags TEXT,
                    is_deleted INTEGER DEFAULT 0,
                    deleted_at DATETIME,
                    user_id INTEGER NOT NULL,
                    version INTEGER DEFAULT 1,
                    sync_status INTEGER DEFAULT 0, -- 0: Synced, 1: PendingUpload, 2: PendingDownload, 3: Conflict
                    local_version INTEGER DEFAULT 1,
                    remote_id TEXT NOT NULL,
                    server_id INTEGER
                );

                -- Bảng quản lý các server đã kết nối
                CREATE TABLE IF NOT EXISTS managed_servers (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    base_url TEXT NOT NULL UNIQUE,
                    access_token TEXT,
                    refresh_token TEXT,
                    server_password TEXT,
                    is_active INTEGER DEFAULT 1,
                    last_sync_date DATETIME,
                    connection_status INTEGER DEFAULT 0, -- 0: Connected, 1: Unauthorized, 2: Offline
                    created_at DATETIME DEFAULT (datetime('now', 'localtime')),
                    updated_at DATETIME
                );

                -- Bảng collections (bộ sưu tập)
                CREATE TABLE IF NOT EXISTS collections (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    description TEXT,
                    created_at DATETIME DEFAULT (datetime('now', 'localtime'))
                );

                -- Bảng liên kết tài liệu với collections
                CREATE TABLE IF NOT EXISTS collection_items (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    collection_id INTEGER NOT NULL,
                    document_id INTEGER NOT NULL,
                    added_at DATETIME DEFAULT (datetime('now', 'localtime')),
                    FOREIGN KEY (collection_id) REFERENCES collections(id) ON DELETE CASCADE,
                    FOREIGN KEY (document_id) REFERENCES tai_lieu(id) ON DELETE CASCADE,
                    UNIQUE(collection_id, document_id)
                );

                -- Bảng ghi chú cá nhân
                CREATE TABLE IF NOT EXISTS personal_notes (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    document_id INTEGER NOT NULL,
                    note_content TEXT,
                    status TEXT DEFAULT 'Chưa đọc',
                    created_at DATETIME DEFAULT (datetime('now', 'localtime')),
                    updated_at DATETIME DEFAULT (datetime('now', 'localtime')),
                    FOREIGN KEY (document_id) REFERENCES tai_lieu(id) ON DELETE CASCADE
                );

                -- Index để tối ưu tìm kiếm
                -- Indexes removed for danh_muc
                CREATE INDEX IF NOT EXISTS idx_tai_lieu_dinh_dang ON tai_lieu(dinh_dang);
                CREATE INDEX IF NOT EXISTS idx_tai_lieu_ngay_them ON tai_lieu(ngay_them);
                CREATE INDEX IF NOT EXISTS idx_collection_items_collection ON collection_items(collection_id);
                CREATE INDEX IF NOT EXISTS idx_collection_items_document ON collection_items(document_id);
                CREATE INDEX IF NOT EXISTS idx_tai_lieu_user_id ON tai_lieu(user_id);
            ";

            using var conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(createTablesQuery, conn);
            cmd.ExecuteNonQuery();

            // Migration: Add user_id and version if missing in existing installations
            MigrateAddColumn(conn, "tai_lieu", "user_id", "INTEGER NOT NULL DEFAULT 1");
            MigrateAddColumn(conn, "tai_lieu", "version", "INTEGER DEFAULT 1");
            MigrateAddColumn(conn, "tai_lieu", "sync_status", "INTEGER DEFAULT 0");
            MigrateAddColumn(conn, "tai_lieu", "local_version", "INTEGER DEFAULT 1");
            MigrateAddColumn(conn, "tai_lieu", "remote_id", "TEXT");
            MigrateAddColumn(conn, "tai_lieu", "server_id", "INTEGER");
            MigrateRenameColumn(conn, "personal_notes", "content", "note_content");
            
            // Ensure all documents have a remote_id if they don't
            using (var cmdFill = new SQLiteCommand("UPDATE tai_lieu SET remote_id = lower(hex(randomblob(4)) || '-' || hex(randomblob(2)) || '-' || '4' || substr(hex(randomblob(2)), 2) || '-' || substr('89ab', abs(random()) % 4 + 1, 1) || substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6))) WHERE remote_id IS NULL OR remote_id = ''", conn))
            {
                cmdFill.ExecuteNonQuery();
            }

            // Enable WAL mode for better concurrency
            using (var walCmd = new SQLiteCommand("PRAGMA journal_mode=WAL;", conn))
            {
                walCmd.ExecuteNonQuery();
            }

            // Migration: bang recent_files
            using var cmd2 = new SQLiteCommand(@"
                CREATE TABLE IF NOT EXISTS recent_files (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    document_id INTEGER NOT NULL UNIQUE,
                    opened_at DATETIME DEFAULT (datetime('now','localtime')),
                    FOREIGN KEY (document_id) REFERENCES tai_lieu(id) ON DELETE CASCADE
                );", conn);
            cmd2.ExecuteNonQuery();

            // Migration: bang document_relations
            using var cmd3 = new SQLiteCommand(@"
                CREATE TABLE IF NOT EXISTS document_relations (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    doc_id_1 INTEGER NOT NULL,
                    doc_id_2 INTEGER NOT NULL,
                    relation_type TEXT DEFAULT 'related',
                    created_at DATETIME DEFAULT (datetime('now','localtime')),
                    FOREIGN KEY (doc_id_1) REFERENCES tai_lieu(id) ON DELETE CASCADE,
                    FOREIGN KEY (doc_id_2) REFERENCES tai_lieu(id) ON DELETE CASCADE,
                    UNIQUE(doc_id_1, doc_id_2)
                );", conn);
            cmd3.ExecuteNonQuery();
        }

        private static void MigrateAddColumn(SQLiteConnection conn, string table, string column, string type)
        {
            try
            {
                using var cmd = new SQLiteCommand($"ALTER TABLE {table} ADD COLUMN {column} {type}", conn);
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException)
            {
                // Column already exists — ignore
            }
        }

        private static void MigrateRenameColumn(SQLiteConnection conn, string table, string oldColumn, string newColumn)
        {
            try
            {
                // SQLite 3.25.0+ supports RENAME COLUMN
                using var cmd = new SQLiteCommand($"ALTER TABLE {table} RENAME COLUMN {oldColumn} TO {newColumn}", conn);
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException)
            {
                // Column might already be renamed or old version of SQLite
            }
        }

        /// <summary>
        /// Kiểm tra kết nối đến database
        /// </summary>
        public static bool TestConnection()
        {
            try
            {
                using var conn = new SQLiteConnection(ConnectionString);
                conn.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Thực hiện câu lệnh SELECT và trả về DataTable
        /// </summary>
        public static DataTable ExecuteQuery(string query, System.Data.SQLite.SQLiteParameter[]? parameters = null)
        {
            DataTable dt = new();
            try
            {
                using var conn = new SQLiteConnection(ConnectionString);
                conn.Open();
                using var cmd = new SQLiteCommand(query, conn);
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                using SQLiteDataAdapter adapter = new(cmd);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Lỗi truy vấn: " + ex.Message, ex);
            }
            return dt;
        }

        /// <summary>
        /// Thực hiện câu lệnh INSERT, UPDATE, DELETE
        /// </summary>
        public static int ExecuteNonQuery(string query, System.Data.SQLite.SQLiteParameter[]? parameters = null)
        {
            int affectedRows = 0;
            try
            {
                using var conn = new SQLiteConnection(ConnectionString);
                conn.Open();
                using var cmd = new SQLiteCommand(query, conn);
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                affectedRows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Lỗi thực thi: " + ex.Message, ex);
            }
            return affectedRows;
        }

        /// <summary>
        /// Thực hiện câu lệnh trả về giá trị đơn (COUNT, SUM, v.v.)
        /// </summary>
        public static object? ExecuteScalar(string query, System.Data.SQLite.SQLiteParameter[]? parameters = null)
        {
            object? result = null;
            try
            {
                using var conn = new SQLiteConnection(ConnectionString);
                conn.Open();
                using var cmd = new SQLiteCommand(query, conn);
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                result = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Lỗi thực thi scalar: " + ex.Message, ex);
            }
            return result;
        }

        /// <summary>
        /// Lấy tất cả tài liệu
        /// </summary>
        public static DataTable GetAllDocuments()
        {
            string query = "SELECT * FROM tai_lieu WHERE (is_deleted IS NULL OR is_deleted = 0) ORDER BY ngay_them DESC";
            return ExecuteQuery(query);
        }

        /// <summary>
        /// Lấy tài liệu (tương thích với code cũ)
        /// </summary>
        public static DataTable GetDocumentsForCurrentUser()
        {
            return GetAllDocuments();
        }

        /// <summary>
        /// Tìm kiếm tài liệu theo từ khóa
        /// </summary>
        public static DataTable SearchDocuments(string keyword)
        {
            string query = @"SELECT * FROM tai_lieu
                           WHERE (is_deleted IS NULL OR is_deleted = 0)
                           AND (ten LIKE @keyword
                           OR ghi_chu LIKE @keyword)
                           ORDER BY ngay_them DESC";

            System.Data.SQLite.SQLiteParameter[] parameters = 
            [
                new("@keyword", "%" + keyword + "%")
            ];

            return ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// Lọc tài liệu theo danh mục và định dạng
        /// </summary>
        public static DataTable FilterDocuments(string dinhDang)
        {
            string query = "SELECT * FROM tai_lieu WHERE 1=1";

            if (!string.IsNullOrEmpty(dinhDang) && !string.Equals(dinhDang, "Tất cả", StringComparison.OrdinalIgnoreCase))
            {
                query += " AND dinh_dang = @dinh_dang";
            }

            query += " ORDER BY ngay_them DESC";

            System.Data.SQLite.SQLiteParameter[] parameters = 
            [
                new("@dinh_dang", string.IsNullOrEmpty(dinhDang) ? DBNull.Value : (object)dinhDang)
            ];

            return ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// Tìm kiếm và lọc tài liệu nâng cao
        /// </summary>
        public static DataTable SearchDocumentsAdvanced(
            string? keyword = null,
            string? dinhDang = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            decimal? minSize = null,
            decimal? maxSize = null,
            bool? isImportant = null)
        {
            string baseQuery = @"SELECT * FROM tai_lieu WHERE (is_deleted IS NULL OR is_deleted = 0)";

            List<System.Data.SQLite.SQLiteParameter> parameterList = [];

            // Keyword search
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                baseQuery += " AND (ten LIKE @keyword OR ghi_chu LIKE @keyword OR tags LIKE @keyword)";
                parameterList.Add(new("@keyword", "%" + keyword + "%"));
            }

            // Định dạng
            if (!string.IsNullOrEmpty(dinhDang) && !string.Equals(dinhDang, "Tất cả", StringComparison.OrdinalIgnoreCase))
            {
                baseQuery += " AND dinh_dang = @dinh_dang";
                parameterList.Add(new("@dinh_dang", dinhDang));
            }

            // Ngày từ
            if (fromDate.HasValue)
            {
                baseQuery += " AND date(ngay_them) >= date(@fromDate)";
                parameterList.Add(new("@fromDate", fromDate.Value.ToString("yyyy-MM-dd")));
            }

            // Ngày đến
            if (toDate.HasValue)
            {
                baseQuery += " AND date(ngay_them) <= date(@toDate)";
                parameterList.Add(new("@toDate", toDate.Value.ToString("yyyy-MM-dd")));
            }

            // Kích thước min
            if (minSize.HasValue)
            {
                baseQuery += " AND kich_thuoc >= @minSize";
                parameterList.Add(new("@minSize", minSize.Value));
            }

            // Kích thước max
            if (maxSize.HasValue)
            {
                baseQuery += " AND kich_thuoc <= @maxSize";
                parameterList.Add(new("@maxSize", maxSize.Value));
            }

            // Quan trọng
            if (isImportant == true)
            {
                baseQuery += " AND quan_trong = 1";
            }

            baseQuery += " ORDER BY ngay_them DESC";

            return ExecuteQuery(baseQuery, [.. parameterList]);
        }

        /// <summary>
        /// Kiểm tra xem tài liệu đã tồn tại trong database dựa trên đường dẫn chưa
        /// </summary>
        public static bool CheckDocumentExists(string duongDan)
        {
            if (string.IsNullOrEmpty(duongDan)) return false;
            
            try
            {
                using var conn = new SQLiteConnection(ConnectionString);
                conn.Open();
                string query = "SELECT COUNT(*) FROM tai_lieu WHERE duong_dan = @path AND (is_deleted IS NULL OR is_deleted = 0)";
                using var cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@path", duongDan);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
            catch { return false; }
        }

        public static bool InsertDocument(string ten, string dinhDang,
            string duongDan, string ghiChu, decimal? kichThuoc, bool quanTrong,
            int userId, Guid remoteId, int version = 1, string? tags = null, int syncStatus = 1, int localVersion = 1, int? serverId = null)
        {
            string query = @"INSERT INTO tai_lieu
                (ten, dinh_dang, duong_dan, ghi_chu, kich_thuoc, quan_trong, tags, user_id, version, sync_status, local_version, remote_id, server_id)
                VALUES
                (@ten, @dinh_dang, @duong_dan, @ghi_chu, @kich_thuoc, @quan_trong, @tags, @user_id, @version, @sync_status, @local_version, @remote_id, @server_id)";

            System.Data.SQLite.SQLiteParameter[] parameters = 
            [
                new("@ten", ten),
                new("@dinh_dang", string.IsNullOrEmpty(dinhDang) ? DBNull.Value : (object)dinhDang),
                new("@duong_dan", duongDan ?? (object)DBNull.Value),
                new("@ghi_chu", string.IsNullOrEmpty(ghiChu) ? DBNull.Value : (object)ghiChu),
                new("@kich_thuoc", kichThuoc.HasValue ? (object)kichThuoc.Value : DBNull.Value),
                new("@quan_trong", quanTrong ? 1 : 0),
                new("@tags", string.IsNullOrEmpty(tags) ? DBNull.Value : (object)tags!),
                new("@user_id", userId),
                new("@version", version),
                new("@sync_status", syncStatus),
                new("@local_version", localVersion),
                new("@remote_id", remoteId.ToString()),
                new("@server_id", serverId.HasValue ? (object)serverId.Value : DBNull.Value)
            ];

            return ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Insert multiple documents in a single transaction for high performance.
        /// </summary>
        public static int InsertDocumentsBatch(List<Document> documents)
        {
            if (documents == null || documents.Count == 0) return 0;

            int successCount = 0;
            using var conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            using var transaction = conn.BeginTransaction();
            try
            {
                string query = @"INSERT INTO tai_lieu
                    (ten, dinh_dang, duong_dan, ghi_chu, kich_thuoc, quan_trong, tags, user_id, version, sync_status, local_version, remote_id, server_id)
                    VALUES
                    (@ten, @dinh_dang, @duong_dan, @ghi_chu, @kich_thuoc, @quan_trong, @tags, @user_id, @version, @sync_status, @local_version, @remote_id, @server_id)";

                using (var cmd = new SQLiteCommand(query, conn, transaction))
                {
                    cmd.Parameters.Add("@ten", System.Data.DbType.String);
                    cmd.Parameters.Add("@dinh_dang", System.Data.DbType.String);
                    cmd.Parameters.Add("@duong_dan", System.Data.DbType.String);
                    cmd.Parameters.Add("@ghi_chu", System.Data.DbType.String);
                    cmd.Parameters.Add("@kich_thuoc", System.Data.DbType.Decimal);
                    cmd.Parameters.Add("@quan_trong", System.Data.DbType.Int32);
                    cmd.Parameters.Add("@tags", System.Data.DbType.String);
                    cmd.Parameters.Add("@user_id", System.Data.DbType.Int32);
                    cmd.Parameters.Add("@version", System.Data.DbType.Int32);
                    cmd.Parameters.Add("@sync_status", System.Data.DbType.Int32);
                    cmd.Parameters.Add("@local_version", System.Data.DbType.Int32);
                    cmd.Parameters.Add("@remote_id", System.Data.DbType.String);
                    cmd.Parameters.Add("@server_id", System.Data.DbType.Int32);

                    foreach (var doc in documents)
                    {
                        cmd.Parameters["@ten"].Value = doc.Ten;
                        cmd.Parameters["@dinh_dang"].Value = string.IsNullOrEmpty(doc.DinhDang) ? DBNull.Value : (object)doc.DinhDang;
                        cmd.Parameters["@duong_dan"].Value = doc.DuongDan ?? (object)DBNull.Value;
                        cmd.Parameters["@ghi_chu"].Value = string.IsNullOrEmpty(doc.GhiChu) ? DBNull.Value : (object)doc.GhiChu;
                        cmd.Parameters["@kich_thuoc"].Value = doc.KichThuoc.HasValue ? (object)doc.KichThuoc.Value : DBNull.Value;
                        cmd.Parameters["@quan_trong"].Value = doc.QuanTrong ? 1 : 0;
                        cmd.Parameters["@tags"].Value = string.IsNullOrEmpty(doc.Tags) ? DBNull.Value : (object)doc.Tags!;
                        cmd.Parameters["@user_id"].Value = doc.UserId;
                        cmd.Parameters["@version"].Value = doc.Version;
                        cmd.Parameters["@sync_status"].Value = doc.SyncStatus;
                        cmd.Parameters["@local_version"].Value = doc.LocalVersion;
                        cmd.Parameters["@remote_id"].Value = doc.RemoteId.ToString();
                        cmd.Parameters["@server_id"].Value = doc.ServerId.HasValue ? (object)doc.ServerId.Value : DBNull.Value;

                        if (cmd.ExecuteNonQuery() > 0) successCount++;
                    }
                }
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            return successCount;
        }

        /// <summary>
        /// Cập nhật tài liệu
        /// </summary>
        public static bool UpdateDocument(int id, string ten, string dinhDang,
            string duongDan, string ghiChu, decimal? kichThuoc, bool quanTrong,
            int userId, Guid remoteId, int newVersion, int? expectedVersion, int syncStatus, int localVersion, int? serverId = null, string? tags = null)
        {
            string query = @"UPDATE tai_lieu SET
                ten = @ten,
                dinh_dang = @dinh_dang,
                duong_dan = @duong_dan,
                ghi_chu = @ghi_chu,
                kich_thuoc = @kich_thuoc,
                quan_trong = @quan_trong,
                tags = @tags,
                user_id = @user_id,
                version = @new_version,
                sync_status = @sync_status,
                local_version = @local_version,
                remote_id = @remote_id,
                server_id = @server_id
                WHERE id = @id AND (@expected_version IS NULL OR version = @expected_version)";

            System.Data.SQLite.SQLiteParameter[] parameters = 
            [
                new("@id", id),
                new("@ten", ten),
                new("@dinh_dang", string.IsNullOrEmpty(dinhDang) ? DBNull.Value : (object)dinhDang),
                new("@duong_dan", duongDan ?? (object)DBNull.Value),
                new("@ghi_chu", string.IsNullOrEmpty(ghiChu) ? DBNull.Value : (object)ghiChu),
                new("@kich_thuoc", kichThuoc.HasValue ? (object)kichThuoc.Value : DBNull.Value),
                new("@quan_trong", quanTrong ? 1 : 0),
                new("@tags", string.IsNullOrEmpty(tags) ? DBNull.Value : (object)tags!),
                new("@user_id", userId),
                new("@new_version", newVersion),
                new("@expected_version", (object?)expectedVersion ?? DBNull.Value),
                new("@sync_status", syncStatus),
                new("@local_version", localVersion),
                new("@remote_id", remoteId.ToString()),
                new("@server_id", (object?)serverId ?? DBNull.Value)
            ];

            return ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Xóa tài liệu
        /// </summary>
        public static bool DeleteDocument(int id)
        {
            // Mark as deleted AND set sync_status to 1 (PendingSync/Upload) so the sync engine picks it up
            string query = "UPDATE tai_lieu SET is_deleted = 1, sync_status = 1, deleted_at = datetime('now','localtime') WHERE id = @id";

            System.Data.SQLite.SQLiteParameter[] parameters = 
            [
                new("@id", id)
            ];

            return ExecuteNonQuery(query, parameters) > 0;
        }

        #region Phase 2 Methods

        #region Tags Methods

        /// <summary>
        /// Lấy danh sách tags đã sử dụng (cho autocomplete)
        /// </summary>
        public static List<string> GetDistinctTags()
        {
            string query = @"SELECT DISTINCT tags FROM tai_lieu
                            WHERE tags IS NOT NULL AND tags != ''";

            DataTable dt = ExecuteQuery(query);
            List<string> allTags = [];

            foreach (DataRow row in dt.Rows)
            {
                string tagsString = row["tags"].ToString() ?? string.Empty;
                string[] tags = tagsString.Split([';'], StringSplitOptions.RemoveEmptyEntries);
                foreach (string tag in tags)
                {
                    string trimmedTag = tag.Trim().ToLower();
                    if (!string.IsNullOrEmpty(trimmedTag) && !allTags.Contains(trimmedTag))
                    {
                        allTags.Add(trimmedTag);
                    }
                }
            }

            allTags.Sort();
            return allTags;
        }

        #endregion

        #endregion

        #region Collections Methods

        /// <summary>
        /// Lấy danh sách collections
        /// </summary>
        public static DataTable GetCollections()
        {
            string query = @"SELECT c.id, c.name, c.description, c.created_at,
                            (SELECT COUNT(*) FROM collection_items ci WHERE ci.collection_id = c.id) as item_count
                            FROM collections c
                            ORDER BY c.name";

            return ExecuteQuery(query);
        }

        /// <summary>
        /// Tạo collection mới
        /// </summary>
        public static int CreateCollection(string name, string? description = null)
        {
            string query = @"INSERT INTO collections (name, description)
                            VALUES (@name, @description);
                            SELECT last_insert_rowid();";

            System.Data.SQLite.SQLiteParameter[] parameters = 
            [
                new("@name", name),
                new("@description", string.IsNullOrEmpty(description) ? DBNull.Value : (object)description!)
            ];

            object? result = ExecuteScalar(query, parameters);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        /// <summary>
        /// Cập nhật collection
        /// </summary>
        public static bool UpdateCollection(int collectionId, string name, string? description = null)
        {
            string query = @"UPDATE collections SET name = @name, description = @description
                            WHERE id = @id";

            System.Data.SQLite.SQLiteParameter[] parameters = 
            [
                new("@id", collectionId),
                new("@name", name),
                new("@description", string.IsNullOrEmpty(description) ? DBNull.Value : (object)description!)
            ];

            return ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Xóa collection
        /// </summary>
        public static bool DeleteCollection(int collectionId)
        {
            // Xóa items trước
            string deleteItems = "DELETE FROM collection_items WHERE collection_id = @id";
            ExecuteNonQuery(deleteItems, [new("@id", collectionId)]);

            // Xóa collection
            string query = "DELETE FROM collections WHERE id = @id";
            System.Data.SQLite.SQLiteParameter[] parameters = 
            [
                new("@id", collectionId)
            ];

            return ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Lấy tài liệu trong collection
        /// </summary>
        public static DataTable GetDocumentsInCollection(int collectionId)
        {
            string query = @"SELECT t.*, ci.added_at
                            FROM tai_lieu t
                            INNER JOIN collection_items ci ON t.id = ci.document_id
                            WHERE ci.collection_id = @collectionId
                            ORDER BY ci.added_at DESC";

            System.Data.SQLite.SQLiteParameter[] parameters = 
            [
                new("@collectionId", collectionId)
            ];

            return ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// Thêm tài liệu vào collection
        /// </summary>
        public static bool AddDocumentToCollection(int collectionId, int documentId)
        {
            // Kiểm tra đã tồn tại chưa
            string checkQuery = "SELECT COUNT(*) FROM collection_items WHERE collection_id = @colId AND document_id = @docId";
            System.Data.SQLite.SQLiteParameter[] checkParams = 
            [
                new("@colId", collectionId),
                new("@docId", documentId)
            ];

            object? exists = ExecuteScalar(checkQuery, checkParams);
            if (exists != null && Convert.ToInt32(exists) > 0)
            {
                return false; // Đã tồn tại
            }

            string query = "INSERT INTO collection_items (collection_id, document_id) VALUES (@colId, @docId)";
            System.Data.SQLite.SQLiteParameter[] parameters = 
            [
                new("@colId", collectionId),
                new("@docId", documentId)
            ];

            return ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Xóa tài liệu khỏi collection
        /// </summary>
        public static bool RemoveDocumentFromCollection(int collectionId, int documentId)
        {
            string query = "DELETE FROM collection_items WHERE collection_id = @colId AND document_id = @docId";
            System.Data.SQLite.SQLiteParameter[] parameters = 
            [
                new("@colId", collectionId),
                new("@docId", documentId)
            ];

            return ExecuteNonQuery(query, parameters) > 0;
        }

        #endregion


        /// <summary>
        /// Lấy thống kê số lượng tài liệu theo loại
        /// </summary>
        public static DataTable GetStatisticsByType()
        {
            string query = @"SELECT COALESCE(NULLIF(dinh_dang, ''), 'Chưa phân loại') as dinh_dang, COUNT(*) as so_luong
                           FROM tai_lieu
                           WHERE (is_deleted IS NULL OR is_deleted = 0)
                           GROUP BY COALESCE(NULLIF(dinh_dang, ''), 'Chưa phân loại')
                           ORDER BY so_luong DESC";

            return ExecuteQuery(query);
        }

        /// <summary>
        /// Đếm tổng số tài liệu
        /// </summary>
        public static int GetTotalDocumentCount()
        {
            string query = "SELECT COUNT(*) FROM tai_lieu WHERE (is_deleted IS NULL OR is_deleted = 0)";
            object? result = ExecuteScalar(query);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        #region Dashboard Statistics

        /// <summary>
        /// Lấy thống kê tổng quan cho Dashboard
        /// </summary>
        public static DashboardStats GetDashboardStatistics()
        {
            var stats = new DashboardStats();

            // Tổng tài liệu
            string totalQuery = "SELECT COUNT(*) FROM tai_lieu WHERE (is_deleted IS NULL OR is_deleted = 0)";
            object? totalResult = ExecuteScalar(totalQuery);
            stats.TotalDocuments = totalResult != null ? Convert.ToInt32(totalResult) : 0;

            // Tài liệu quan trọng
            string importantQuery = "SELECT COUNT(*) FROM tai_lieu WHERE quan_trong = 1 AND (is_deleted IS NULL OR is_deleted = 0)";
            object? importantResult = ExecuteScalar(importantQuery);
            stats.ImportantDocuments = importantResult != null ? Convert.ToInt32(importantResult) : 0;

            // Tài liệu chưa có file
            string noFileQuery = "SELECT COUNT(*) FROM tai_lieu WHERE (is_deleted IS NULL OR is_deleted = 0) AND (duong_dan IS NULL OR duong_dan = '')";
            object? noFileResult = ExecuteScalar(noFileQuery);
            stats.NoFileDocuments = noFileResult != null ? Convert.ToInt32(noFileResult) : 0;


            // Số collections
            string collectionQuery = "SELECT COUNT(*) FROM collections";
            object? collectionResult = ExecuteScalar(collectionQuery);
            stats.TotalCollections = collectionResult != null ? Convert.ToInt32(collectionResult) : 0;

            return stats;
        }

        /// <summary>
        /// Lấy thống kê tài liệu theo ngày (7 ngày gần nhất)
        /// </summary>
        public static DataTable GetDocumentsByDay(int days = 7)
        {
            string query = @"
                WITH RECURSIVE DateSeries(ngay) AS (
                    SELECT date('now', 'localtime')
                    UNION ALL
                    SELECT date(ngay, '-1 day')
                    FROM DateSeries
                    WHERE date(ngay, '-1 day') >= date('now', 'localtime', '-' || (@days - 1) || ' days')
                )
                SELECT
                    ds.ngay,
                    strftime('%d/%m', ds.ngay) as ngay_format,
                    COALESCE(COUNT(t.id), 0) as so_luong
                FROM DateSeries ds
                LEFT JOIN tai_lieu t ON date(t.ngay_them) = ds.ngay
                    AND (t.is_deleted IS NULL OR t.is_deleted = 0)
                GROUP BY ds.ngay
                ORDER BY ds.ngay ASC";

            System.Data.SQLite.SQLiteParameter[] parameters = 
            [
                new("@days", days)
            ];

            return ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// Lấy thống kê tài liệu theo tháng (12 tháng gần nhất)
        /// </summary>
        public static DataTable GetDocumentsByMonth(int months = 12)
        {
            string query = @"
                WITH RECURSIVE MonthSeries(thang) AS (
                    SELECT date('now', 'localtime', 'start of month')
                    UNION ALL
                    SELECT date(thang, '-1 month')
                    FROM MonthSeries
                    WHERE date(thang, '-1 month') >= date('now', 'localtime', 'start of month', '-' || (@months - 1) || ' months')
                )
                SELECT
                    ms.thang,
                    strftime('%m/%Y', ms.thang) as thang_format,
                    COALESCE(COUNT(t.id), 0) as so_luong
                FROM MonthSeries ms
                LEFT JOIN tai_lieu t ON strftime('%Y-%m', t.ngay_them) = strftime('%Y-%m', ms.thang)
                    AND (t.is_deleted IS NULL OR t.is_deleted = 0)
                GROUP BY ms.thang
                ORDER BY ms.thang ASC";

            System.Data.SQLite.SQLiteParameter[] parameters = 
            [
                new("@months", months)
            ];

            return ExecuteQuery(query, parameters);
        }

        #endregion

        #region Quản lý Danh mục và Định dạng tài liệu


        /// <summary>
        /// Lấy danh sách định dạng DISTINCT kèm số lượng
        /// </summary>
        public static DataTable GetDistinctTypes()
        {
            string query = @"SELECT dinh_dang, COUNT(*) as so_luong
                           FROM tai_lieu
                           WHERE dinh_dang IS NOT NULL AND dinh_dang != ''
                           GROUP BY dinh_dang
                           ORDER BY dinh_dang";

            return ExecuteQuery(query);
        }


        /// <summary>
        /// Cập nhật tên định dạng
        /// </summary>
        public static bool UpdateTypeName(string oldName, string newName)
        {
            string query = "UPDATE tai_lieu SET dinh_dang = @newName WHERE dinh_dang = @oldName";

            System.Data.SQLite.SQLiteParameter[] parameters = 
            [
                new("@oldName", oldName),
                new("@newName", newName)
            ];

            return ExecuteNonQuery(query, parameters) > 0;
        }


        /// <summary>
        /// Xóa tài liệu có loại này
        /// </summary>
        public static bool DeleteDocumentsByType(string typeName)
        {
            string query = "DELETE FROM tai_lieu WHERE dinh_dang = @typeName";

            System.Data.SQLite.SQLiteParameter[] parameters = 
            [
                new("@typeName", typeName)
            ];

            return ExecuteNonQuery(query, parameters) > 0;
        }

        #endregion

        #region Personal Notes

        /// <summary>
        /// Lấy ghi chú của tài liệu
        /// </summary>
        public static DataTable GetPersonalNote(int documentId)
        {
            string query = "SELECT * FROM personal_notes WHERE document_id = @documentId";

            System.Data.SQLite.SQLiteParameter[] parameters = 
            [
                new("@documentId", documentId)
            ];

            return ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// Lưu hoặc cập nhật ghi chú
        /// </summary>
        public static bool SavePersonalNote(int documentId, string content)
        {
            // Kiểm tra đã có note chưa
            string checkQuery = "SELECT COUNT(*) FROM personal_notes WHERE document_id = @documentId";
            System.Data.SQLite.SQLiteParameter[] checkParams = 
            [
                new("@documentId", documentId)
            ];

            object? exists = ExecuteScalar(checkQuery, checkParams);
            bool hasNote = exists != null && Convert.ToInt32(exists) > 0;

            if (hasNote)
            {
                // Update
                string updateQuery = @"UPDATE personal_notes
                                      SET note_content = @content, updated_at = datetime('now', 'localtime')
                                      WHERE document_id = @documentId";
                System.Data.SQLite.SQLiteParameter[] updateParams = 
                [
                    new("@documentId", documentId),
                    new("@content", string.IsNullOrEmpty(content) ? DBNull.Value : (object)content)
                ];
                return ExecuteNonQuery(updateQuery, updateParams) > 0;
            }
            else
            {
                // Insert
                string insertQuery = @"INSERT INTO personal_notes (document_id, note_content)
                                      VALUES (@documentId, @content)";
                System.Data.SQLite.SQLiteParameter[] insertParams = 
                [
                    new("@documentId", documentId),
                    new("@content", string.IsNullOrEmpty(content) ? DBNull.Value : (object)content)
                ];
                return ExecuteNonQuery(insertQuery, insertParams) > 0;
            }
        }

        /// <summary>
        /// Xóa ghi chú
        /// </summary>
        public static bool DeletePersonalNote(int documentId)
        {
            string query = "DELETE FROM personal_notes WHERE document_id = @documentId";

            System.Data.SQLite.SQLiteParameter[] parameters = 
            [
                new("@documentId", documentId)
            ];

            return ExecuteNonQuery(query, parameters) > 0;
        }

        #endregion

        #region Recycle Bin Methods

        public static DataTable GetDeletedDocuments()
        {
            string query = "SELECT * FROM tai_lieu WHERE is_deleted = 1 ORDER BY deleted_at DESC";
            return ExecuteQuery(query);
        }

        public static bool RestoreDocument(int id)
        {
            string query = "UPDATE tai_lieu SET is_deleted = 0, deleted_at = NULL WHERE id = @id";
            return ExecuteNonQuery(query, [new("@id", id)]) > 0;
        }

        public static bool PermanentDeleteDocument(int id)
        {
            string query = "DELETE FROM tai_lieu WHERE id = @id";
            return ExecuteNonQuery(query, [new("@id", id)]) > 0;
        }

        public static int EmptyRecycleBin()
        {
            string query = "DELETE FROM tai_lieu WHERE is_deleted = 1";
            return ExecuteNonQuery(query);
        }

        public static int GetDeletedDocumentCount()
        {
            string query = "SELECT COUNT(*) FROM tai_lieu WHERE is_deleted = 1";
            object? result = ExecuteScalar(query);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        #endregion

        #region Bulk Actions

        public static int BulkSoftDelete(List<int> ids)
        {
            if (ids == null || ids.Count == 0) return 0;
            string idList = string.Join(",", ids);
            string query = $"UPDATE tai_lieu SET is_deleted = 1, deleted_at = datetime('now','localtime') WHERE id IN ({idList})";
            return ExecuteNonQuery(query);
        }


        public static int BulkToggleImportant(List<int> ids, bool important)
        {
            if (ids == null || ids.Count == 0) return 0;
            string idList = string.Join(",", ids);
            string query = $"UPDATE tai_lieu SET quan_trong = @val WHERE id IN ({idList})";
            return ExecuteNonQuery(query, [new("@val", important ? 1 : 0)]);
        }

        #endregion

        #region Recent Files Methods

        public static void AddRecentFile(int documentId)
        {
            string upsert = @"INSERT OR REPLACE INTO recent_files (document_id, opened_at)
                              VALUES (@docId, datetime('now','localtime'))";
            ExecuteNonQuery(upsert, [new("@docId", documentId)]);

            // Keep only 20 most recent
            string trim = @"DELETE FROM recent_files WHERE id NOT IN
                            (SELECT id FROM recent_files ORDER BY opened_at DESC LIMIT 20)";
            ExecuteNonQuery(trim);
        }

        public static DataTable GetRecentFiles()
        {
            string query = @"SELECT t.id, t.ten, t.dinh_dang, t.duong_dan, r.opened_at
                             FROM recent_files r
                             INNER JOIN tai_lieu t ON r.document_id = t.id
                             WHERE (t.is_deleted IS NULL OR t.is_deleted = 0)
                             ORDER BY r.opened_at DESC
                             LIMIT 20";
            return ExecuteQuery(query);
        }

        public static void RemoveRecentFile(int documentId)
        {
            ExecuteNonQuery("DELETE FROM recent_files WHERE document_id = @docId",
                [new("@docId", documentId)]);
        }

        public static void ClearRecentFiles()
        {
            ExecuteNonQuery("DELETE FROM recent_files");
        }

        #endregion

        #region Backup & Restore Methods

        public static void BackupDatabase(string destPath)
        {
            File.Copy(DatabasePath, destPath, true);
        }

        public static void RestoreDatabase(string srcPath)
        {
            File.Copy(srcPath, DatabasePath, true);
        }

        #endregion

        #region Related Documents Methods

        public static void AddDocumentRelation(int docId1, int docId2, string relationType = "related")
        {
            int lo = Math.Min(docId1, docId2);
            int hi = Math.Max(docId1, docId2);
            string query = @"INSERT OR IGNORE INTO document_relations (doc_id_1, doc_id_2, relation_type)
                             VALUES (@d1, @d2, @type)";
            ExecuteNonQuery(query, 
            [
                new("@d1", lo),
                new("@d2", hi),
                new("@type", relationType)
            ]);
        }

        public static DataTable GetRelatedDocuments(int docId)
        {
            string query = @"SELECT t.id, t.ten, t.dinh_dang, t.duong_dan, r.relation_type, r.id as relation_id
                             FROM document_relations r
                             INNER JOIN tai_lieu t ON (t.id = CASE WHEN r.doc_id_1 = @docId THEN r.doc_id_2 ELSE r.doc_id_1 END)
                             WHERE (r.doc_id_1 = @docId OR r.doc_id_2 = @docId)
                             AND t.is_deleted = 0
                             ORDER BY t.ten";
            return ExecuteQuery(query, [new("@docId", docId)]);
        }

        public static void RemoveDocumentRelation(int relationId)
        {
            ExecuteNonQuery("DELETE FROM document_relations WHERE id = @id",
                [new("@id", relationId)]);
        }

        /// <summary>
        /// Gợi ý các tài liệu liên quan dựa trên Danh mục hoặc Thẻ gắn
        /// </summary>
        public static DataTable GetSuggestedRelatedDocuments(int docId)
        {
            string query = @"
                SELECT id, ten, dinh_dang, tags
                FROM tai_lieu t
                WHERE id != @docId 
                AND is_deleted = 0
                AND (
                    -- Có chung ít nhất 1 tag (giả sử tag cách nhau bởi dấu ;)
                    EXISTS (
                        SELECT 1 FROM tai_lieu t2 
                        WHERE t2.id = @docId 
                        AND t2.tags IS NOT NULL 
                        AND t.tags LIKE '%' || t2.tags || '%' -- Cách làm đơn giản cho demo
                    )
                )
                AND id NOT IN (
                    -- Loại bỏ những cái đã liên kết rồi
                    SELECT doc_id_2 FROM document_relations WHERE doc_id_1 = @docId
                    UNION
                    SELECT doc_id_1 FROM document_relations WHERE doc_id_2 = @docId
                )
                LIMIT 5";
            
            return ExecuteQuery(query, [new("@docId", docId)]);
        }

        #endregion

        #region Managed Servers Methods

        /// <summary>
        /// Lấy danh sách tất cả các server đã join
        /// </summary>
        public static List<ManagedServer> GetManagedServers()
        {
            string query = "SELECT * FROM managed_servers ORDER BY created_at DESC";
            DataTable dt = ExecuteQuery(query);
            List<ManagedServer> servers = [];

            foreach (DataRow row in dt.Rows)
            {
                servers.Add(new ManagedServer
                {
                    Id = Convert.ToInt32(row["id"]),
                    Name = row["name"].ToString() ?? string.Empty,
                    BaseUrl = row["base_url"].ToString() ?? string.Empty,
                    AccessToken = Decrypt(row["access_token"]?.ToString()) ?? string.Empty,
                    RefreshToken = Decrypt(row["refresh_token"]?.ToString()) ?? string.Empty,
                    ServerPassword = Decrypt(row["server_password"]?.ToString()) ?? string.Empty,
                    IsActive = Convert.ToInt32(row["is_active"]) == 1,
                    ConnectionStatus = Convert.ToInt32(row["connection_status"]),
                    LastSyncDate = row["last_sync_date"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["last_sync_date"]) : null
                });
            }

            return servers;
        }

        /// <summary>
        /// Thêm một server mới
        /// </summary>
        public static bool InsertServer(string name, string url, string? accessToken = null, string? refreshToken = null, string? password = null)
        {
            string query = @"INSERT INTO managed_servers (name, base_url, access_token, refresh_token, server_password)
                            VALUES (@name, @url, @token, @refresh, @pass)";

            System.Data.SQLite.SQLiteParameter[] parameters = 
            [
                new("@name", name),
                new("@url", url.TrimEnd('/')),
                new("@token", Encrypt(accessToken) ?? (object)DBNull.Value),
                new("@refresh", Encrypt(refreshToken) ?? (object)DBNull.Value),
                new("@pass", Encrypt(password) ?? (object)DBNull.Value)
            ];

            return ExecuteNonQuery(query, parameters) > 0;
        }

        private static readonly byte[] AesKey = Encoding.UTF8.GetBytes("DocSharing_2024_SecurityKey_32ch"); // 32 bytes for AES-256
        private static readonly byte[] AesIV = Encoding.UTF8.GetBytes("DocSharing_2024_"); // 16 bytes IV

        private static string Encrypt(string? text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            try
            {
                using Aes aes = Aes.Create();
                aes.Key = AesKey;
                aes.IV = AesIV;
                
                using var encryptor = aes.CreateEncryptor();
                byte[] input = Encoding.UTF8.GetBytes(text);
                byte[] output = encryptor.TransformFinalBlock(input, 0, input.Length);
                return "AES:" + Convert.ToBase64String(output);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Mã hóa AES thất bại.", ex);
            }
        }

        private static string Decrypt(string? text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            if (!text!.StartsWith("AES:")) 
            {
                // Fallback for old XOR encrypted data (prefixed with ENC:)
                if (text.StartsWith("ENC:")) return DecryptOld(text);
                return text;
            }
            
            try
            {
                using Aes aes = Aes.Create();
                aes.Key = AesKey;
                aes.IV = AesIV;

                using var decryptor = aes.CreateDecryptor();
                byte[] input = Convert.FromBase64String(text.Substring(4));
                byte[] output = decryptor.TransformFinalBlock(input, 0, input.Length);
                return Encoding.UTF8.GetString(output);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Giải mã AES thất bại.", ex);
            }
        }

        private static string DecryptOld(string text)
        {
            try
            {
                var bytes = Convert.FromBase64String(text.Substring(4));
                byte[] key = Encoding.UTF8.GetBytes("DocSharing_2024_Key");
                for (int i = 0; i < bytes.Length; i++) bytes[i] ^= key[i % key.Length];
                return Encoding.UTF8.GetString(bytes);
            }
            catch { return text; }
        }

        /// <summary>
        /// Cập nhật thông tin Server (ví dụ sau khi sync xong hoặc đổi token)
        /// </summary>
        public static bool UpdateServer(ManagedServer server)
        {
            string query = @"UPDATE managed_servers SET 
                            name = @name, 
                            access_token = @token, 
                            refresh_token = @refresh, 
                            is_active = @active, 
                            last_sync_date = @sync,
                            connection_status = @status,
                            updated_at = datetime('now', 'localtime')
                            WHERE id = @id";

            System.Data.SQLite.SQLiteParameter[] parameters = 
            [
                new("@id", server.Id),
                new("@name", server.Name),
                new("@token", Encrypt(server.AccessToken)),
                new("@refresh", Encrypt(server.RefreshToken)),
                new("@active", server.IsActive ? 1 : 0),
                new("@sync", server.LastSyncDate.HasValue ? (object)server.LastSyncDate.Value : DBNull.Value),
                new("@status", server.ConnectionStatus)
            ];

            return ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Xóa server khỏi danh sách (ngắt kết nối)
        /// </summary>
        public static bool DeleteServer(int serverId)
        {
            using var conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            using var transaction = conn.BeginTransaction();
            try
            {
                // Soft-delete documents belonging to this server
                string deleteDocs = "UPDATE tai_lieu SET is_deleted = 1, deleted_at = datetime('now', 'localtime') WHERE server_id = @id";
                using var cmdDocs = new SQLiteCommand(deleteDocs, conn, transaction);
                cmdDocs.Parameters.AddWithValue("@id", serverId);
                cmdDocs.ExecuteNonQuery();

                // Delete the server record
                string query = "DELETE FROM managed_servers WHERE id = @id";
                using var cmdServer = new SQLiteCommand(query, conn, transaction);
                cmdServer.Parameters.AddWithValue("@id", serverId);
                var result = cmdServer.ExecuteNonQuery() > 0;
                
                transaction.Commit();
                return result;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new InvalidOperationException("Lỗi khi xóa server: " + ex.Message, ex);
            }
        }

        #endregion
    }
}


