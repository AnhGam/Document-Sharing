using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace study_document_manager
{
    /// <summary>
    /// Class quản lý kết nối và thao tác với SQLite Database (Local)
    /// </summary>
    public partial class DatabaseHelper
    {
        private static string _databasePath;
        private static string _connectionString;

        /// <summary>
        /// Đường dẫn đến file database SQLite
        /// </summary>
        public static string DatabasePath
        {
            get
            {
                if (string.IsNullOrEmpty(_databasePath))
                {
                    string appFolder = Path.GetDirectoryName(Application.ExecutablePath);
                    _databasePath = Path.Combine(appFolder, "data", "study_documents.db");
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
                    _connectionString = $"Data Source={DatabasePath};Version=3;";
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
                string dataFolder = Path.GetDirectoryName(DatabasePath);
                if (!Directory.Exists(dataFolder))
                {
                    Directory.CreateDirectory(dataFolder);
                }

                // Tạo file database nếu chưa có
                if (!File.Exists(DatabasePath))
                {
                    SQLiteConnection.CreateFile(DatabasePath);
                }

                // Tạo các bảng
                CreateTables();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi tạo database: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    danh_muc TEXT,
                    dinh_dang TEXT,
                    duong_dan TEXT,
                    ghi_chu TEXT,
                    ngay_them DATETIME DEFAULT (datetime('now', 'localtime')),
                    kich_thuoc REAL,
                    quan_trong INTEGER DEFAULT 0,
                    tags TEXT,
                    is_deleted INTEGER DEFAULT 0,
                    deleted_at DATETIME
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
                CREATE INDEX IF NOT EXISTS idx_tai_lieu_danh_muc ON tai_lieu(danh_muc);
                CREATE INDEX IF NOT EXISTS idx_tai_lieu_dinh_dang ON tai_lieu(dinh_dang);
                CREATE INDEX IF NOT EXISTS idx_tai_lieu_ngay_them ON tai_lieu(ngay_them);
                CREATE INDEX IF NOT EXISTS idx_collection_items_collection ON collection_items(collection_id);
                CREATE INDEX IF NOT EXISTS idx_collection_items_document ON collection_items(document_id);
            ";

            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(createTablesQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Các cột migration đã được tích hợp vào schema ban đầu
                // Nếu cần thêm cột sau này, hãy sử dụng MigrateAddColumn tại đây

                // Migration: fix personal_notes column names and add status if missing
                // Các cột này đã được tích hợp vào schema gốc ở trên

                // Migration: bang recent_files
                using (var cmd2 = new SQLiteCommand(@"
                    CREATE TABLE IF NOT EXISTS recent_files (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        document_id INTEGER NOT NULL UNIQUE,
                        opened_at DATETIME DEFAULT (datetime('now','localtime')),
                        FOREIGN KEY (document_id) REFERENCES tai_lieu(id) ON DELETE CASCADE
                    );", conn))
                {
                    cmd2.ExecuteNonQuery();
                }

                // Migration: bang document_relations
                using (var cmd3 = new SQLiteCommand(@"
                    CREATE TABLE IF NOT EXISTS document_relations (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        doc_id_1 INTEGER NOT NULL,
                        doc_id_2 INTEGER NOT NULL,
                        relation_type TEXT DEFAULT 'related',
                        created_at DATETIME DEFAULT (datetime('now','localtime')),
                        FOREIGN KEY (doc_id_1) REFERENCES tai_lieu(id) ON DELETE CASCADE,
                        FOREIGN KEY (doc_id_2) REFERENCES tai_lieu(id) ON DELETE CASCADE,
                        UNIQUE(doc_id_1, doc_id_2)
                    );", conn))
                {
                    cmd3.ExecuteNonQuery();
                }
            }
        }

        private static void MigrateAddColumn(SQLiteConnection conn, string table, string column, string type)
        {
            try
            {
                using (var cmd = new SQLiteCommand($"ALTER TABLE {table} ADD COLUMN {column} {type}", conn))
                {
                    cmd.ExecuteNonQuery();
                }
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
                using (var cmd = new SQLiteCommand($"ALTER TABLE {table} RENAME COLUMN {oldColumn} TO {newColumn}", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SQLiteException)
            {
                // Column might already be renamed or old version of SQLite
                // If old version, we'd need a complex migration (create new table, copy data, drop old)
                // Now, let's hope it's 3.25.0+ or column doesn't exist anymore
            }
        }

        /// <summary>
        /// Kiểm tra kết nối đến database
        /// </summary>
        public static bool TestConnection()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối database: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Thực hiện câu lệnh SELECT và trả về DataTable
        /// </summary>
        public static DataTable ExecuteQuery(string query, SQLiteParameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi truy vấn: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        /// <summary>
        /// Thực hiện câu lệnh INSERT, UPDATE, DELETE
        /// </summary>
        public static int ExecuteNonQuery(string query, SQLiteParameter[] parameters = null)
        {
            int affectedRows = 0;
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        affectedRows = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thực thi: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return affectedRows;
        }

        /// <summary>
        /// Thực hiện câu lệnh trả về giá trị đơn (COUNT, SUM, v.v.)
        /// </summary>
        public static object ExecuteScalar(string query, SQLiteParameter[] parameters = null)
        {
            object result = null;
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        result = cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thực thi: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                           OR danh_muc LIKE @keyword
                           OR ghi_chu LIKE @keyword)
                           ORDER BY ngay_them DESC";

            SQLiteParameter[] parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@keyword", "%" + keyword + "%")
            };

            return ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// Lọc tài liệu theo danh mục và định dạng
        /// </summary>
        public static DataTable FilterDocuments(string danhMuc, string dinhDang)
        {
            string query = "SELECT * FROM tai_lieu WHERE 1=1";

            if (!string.IsNullOrEmpty(danhMuc) && danhMuc != "Tất cả")
            {
                query += " AND danh_muc = @danh_muc";
            }

            if (!string.IsNullOrEmpty(dinhDang) && dinhDang != "Tất cả")
            {
                query += " AND dinh_dang = @dinh_dang";
            }

            query += " ORDER BY ngay_them DESC";

            SQLiteParameter[] parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@danh_muc", string.IsNullOrEmpty(danhMuc) ? DBNull.Value : (object)danhMuc),
                new SQLiteParameter("@dinh_dang", string.IsNullOrEmpty(dinhDang) ? DBNull.Value : (object)dinhDang)
            };

            return ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// Tìm kiếm và lọc tài liệu nâng cao
        /// </summary>
        public static DataTable SearchDocumentsAdvanced(
            string keyword = null,
            string danhMuc = null,
            string dinhDang = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            double? minSize = null,
            double? maxSize = null,
            bool? isImportant = null)
        {
            string baseQuery = @"SELECT * FROM tai_lieu WHERE (is_deleted IS NULL OR is_deleted = 0)";

            List<SQLiteParameter> parameterList = new List<SQLiteParameter>();

            // Keyword search
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                baseQuery += " AND (ten LIKE @keyword OR danh_muc LIKE @keyword OR ghi_chu LIKE @keyword OR tags LIKE @keyword)";
                parameterList.Add(new SQLiteParameter("@keyword", "%" + keyword + "%"));
            }

            // Danh mục
            if (!string.IsNullOrEmpty(danhMuc) && danhMuc != "Tất cả")
            {
                baseQuery += " AND danh_muc = @danh_muc";
                parameterList.Add(new SQLiteParameter("@danh_muc", danhMuc));
            }

            // Định dạng
            if (!string.IsNullOrEmpty(dinhDang) && dinhDang != "Tất cả")
            {
                baseQuery += " AND dinh_dang = @dinh_dang";
                parameterList.Add(new SQLiteParameter("@dinh_dang", dinhDang));
            }

            // Ngày từ
            if (fromDate.HasValue)
            {
                baseQuery += " AND date(ngay_them) >= date(@fromDate)";
                parameterList.Add(new SQLiteParameter("@fromDate", fromDate.Value.ToString("yyyy-MM-dd")));
            }

            // Ngày đến
            if (toDate.HasValue)
            {
                baseQuery += " AND date(ngay_them) <= date(@toDate)";
                parameterList.Add(new SQLiteParameter("@toDate", toDate.Value.ToString("yyyy-MM-dd")));
            }

            // Kích thước min
            if (minSize.HasValue)
            {
                baseQuery += " AND kich_thuoc >= @minSize";
                parameterList.Add(new SQLiteParameter("@minSize", minSize.Value));
            }

            // Kích thước max
            if (maxSize.HasValue)
            {
                baseQuery += " AND kich_thuoc <= @maxSize";
                parameterList.Add(new SQLiteParameter("@maxSize", maxSize.Value));
            }

            // Quan trọng
            if (isImportant.HasValue && isImportant.Value == true)
            {
                baseQuery += " AND quan_trong = 1";
            }

            baseQuery += " ORDER BY ngay_them DESC";

            return ExecuteQuery(baseQuery, parameterList.ToArray());
        }

        public static bool InsertDocument(string ten, string danhMuc, string dinhDang,
            string duongDan, string ghiChu, double? kichThuoc, bool quanTrong,
            string tags = null)
        {
            string query = @"INSERT INTO tai_lieu
                (ten, danh_muc, dinh_dang, duong_dan, ghi_chu, kich_thuoc, quan_trong, tags)
                VALUES
                (@ten, @danh_muc, @dinh_dang, @duong_dan, @ghi_chu, @kich_thuoc, @quan_trong, @tags)";

            SQLiteParameter[] parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@ten", ten),
                new SQLiteParameter("@danh_muc", string.IsNullOrEmpty(danhMuc) ? DBNull.Value : (object)danhMuc),
                new SQLiteParameter("@dinh_dang", string.IsNullOrEmpty(dinhDang) ? DBNull.Value : (object)dinhDang),
                new SQLiteParameter("@duong_dan", duongDan ?? (object)DBNull.Value),
                new SQLiteParameter("@ghi_chu", string.IsNullOrEmpty(ghiChu) ? DBNull.Value : (object)ghiChu),
                new SQLiteParameter("@kich_thuoc", kichThuoc.HasValue ? (object)kichThuoc.Value : DBNull.Value),
                new SQLiteParameter("@quan_trong", quanTrong ? 1 : 0),
                new SQLiteParameter("@tags", string.IsNullOrEmpty(tags) ? DBNull.Value : (object)tags)
            };

            int result = ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        /// <summary>
        /// Cập nhật tài liệu
        /// </summary>
        public static bool UpdateDocument(int id, string ten, string danhMuc, string dinhDang,
            string duongDan, string ghiChu, double? kichThuoc, bool quanTrong,
            string tags = null)
        {
            string query = @"UPDATE tai_lieu SET
                ten = @ten,
                danh_muc = @danh_muc,
                dinh_dang = @dinh_dang,
                duong_dan = @duong_dan,
                ghi_chu = @ghi_chu,
                kich_thuoc = @kich_thuoc,
                quan_trong = @quan_trong,
                tags = @tags
                WHERE id = @id";

            SQLiteParameter[] parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@id", id),
                new SQLiteParameter("@ten", ten),
                new SQLiteParameter("@danh_muc", string.IsNullOrEmpty(danhMuc) ? DBNull.Value : (object)danhMuc),
                new SQLiteParameter("@dinh_dang", string.IsNullOrEmpty(dinhDang) ? DBNull.Value : (object)dinhDang),
                new SQLiteParameter("@duong_dan", duongDan ?? (object)DBNull.Value),
                new SQLiteParameter("@ghi_chu", string.IsNullOrEmpty(ghiChu) ? DBNull.Value : (object)ghiChu),
                new SQLiteParameter("@kich_thuoc", kichThuoc.HasValue ? (object)kichThuoc.Value : DBNull.Value),
                new SQLiteParameter("@quan_trong", quanTrong ? 1 : 0),
                new SQLiteParameter("@tags", string.IsNullOrEmpty(tags) ? DBNull.Value : (object)tags)
            };

            int result = ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        /// <summary>
        /// Xóa tài liệu
        /// </summary>
        public static bool DeleteDocument(int id)
        {
            string query = "UPDATE tai_lieu SET is_deleted = 1, deleted_at = datetime('now','localtime') WHERE id = @id";

            SQLiteParameter[] parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@id", id)
            };

            int result = ExecuteNonQuery(query, parameters);
            return result > 0;
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
            List<string> allTags = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                string tagsString = row["tags"].ToString();
                string[] tags = tagsString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
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
        public static int CreateCollection(string name, string description = null)
        {
            string query = @"INSERT INTO collections (name, description)
                            VALUES (@name, @description);
                            SELECT last_insert_rowid();";

            SQLiteParameter[] parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@name", name),
                new SQLiteParameter("@description", string.IsNullOrEmpty(description) ? DBNull.Value : (object)description)
            };

            object result = ExecuteScalar(query, parameters);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        /// <summary>
        /// Cập nhật collection
        /// </summary>
        public static bool UpdateCollection(int collectionId, string name, string description = null)
        {
            string query = @"UPDATE collections SET name = @name, description = @description
                            WHERE id = @id";

            SQLiteParameter[] parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@id", collectionId),
                new SQLiteParameter("@name", name),
                new SQLiteParameter("@description", string.IsNullOrEmpty(description) ? DBNull.Value : (object)description)
            };

            return ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Xóa collection
        /// </summary>
        public static bool DeleteCollection(int collectionId)
        {
            // Xóa items trước
            string deleteItems = "DELETE FROM collection_items WHERE collection_id = @id";
            ExecuteNonQuery(deleteItems, new SQLiteParameter[] { new SQLiteParameter("@id", collectionId) });

            // Xóa collection
            string query = "DELETE FROM collections WHERE id = @id";
            SQLiteParameter[] parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@id", collectionId)
            };

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

            SQLiteParameter[] parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@collectionId", collectionId)
            };

            return ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// Thêm tài liệu vào collection
        /// </summary>
        public static bool AddDocumentToCollection(int collectionId, int documentId)
        {
            // Kiểm tra đã tồn tại chưa
            string checkQuery = "SELECT COUNT(*) FROM collection_items WHERE collection_id = @colId AND document_id = @docId";
            SQLiteParameter[] checkParams = new SQLiteParameter[]
            {
                new SQLiteParameter("@colId", collectionId),
                new SQLiteParameter("@docId", documentId)
            };

            object exists = ExecuteScalar(checkQuery, checkParams);
            if (exists != null && Convert.ToInt32(exists) > 0)
            {
                return false; // Đã tồn tại
            }

            string query = "INSERT INTO collection_items (collection_id, document_id) VALUES (@colId, @docId)";
            SQLiteParameter[] parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@colId", collectionId),
                new SQLiteParameter("@docId", documentId)
            };

            return ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Xóa tài liệu khỏi collection
        /// </summary>
        public static bool RemoveDocumentFromCollection(int collectionId, int documentId)
        {
            string query = "DELETE FROM collection_items WHERE collection_id = @colId AND document_id = @docId";
            SQLiteParameter[] parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@colId", collectionId),
                new SQLiteParameter("@docId", documentId)
            };

            return ExecuteNonQuery(query, parameters) > 0;
        }

        #endregion

        /// <summary>
        /// Lấy thống kê số lượng tài liệu theo danh mục
        /// </summary>
        public static DataTable GetStatisticsBySubject()
        {
            string query = @"SELECT COALESCE(NULLIF(danh_muc, ''), 'Chưa phân loại') as danh_muc, COUNT(*) as so_luong
                           FROM tai_lieu
                           WHERE (is_deleted IS NULL OR is_deleted = 0)
                           GROUP BY COALESCE(NULLIF(danh_muc, ''), 'Chưa phân loại')
                           ORDER BY so_luong DESC";

            return ExecuteQuery(query);
        }

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
            object result = ExecuteScalar(query);
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
            object totalResult = ExecuteScalar(totalQuery);
            stats.TotalDocuments = totalResult != null ? Convert.ToInt32(totalResult) : 0;

            // Tài liệu quan trọng
            string importantQuery = "SELECT COUNT(*) FROM tai_lieu WHERE quan_trong = 1 AND (is_deleted IS NULL OR is_deleted = 0)";
            object importantResult = ExecuteScalar(importantQuery);
            stats.ImportantDocuments = importantResult != null ? Convert.ToInt32(importantResult) : 0;

            // Tài liệu chưa có file
            string noFileQuery = "SELECT COUNT(*) FROM tai_lieu WHERE (is_deleted IS NULL OR is_deleted = 0) AND (duong_dan IS NULL OR duong_dan = '')";
            object noFileResult = ExecuteScalar(noFileQuery);
            stats.NoFileDocuments = noFileResult != null ? Convert.ToInt32(noFileResult) : 0;

            // Số danh mục
            string categoryQuery = "SELECT COUNT(DISTINCT danh_muc) FROM tai_lieu WHERE danh_muc IS NOT NULL AND danh_muc != '' AND (is_deleted IS NULL OR is_deleted = 0)";
            object categoryResult = ExecuteScalar(categoryQuery);
            stats.TotalCategories = categoryResult != null ? Convert.ToInt32(categoryResult) : 0;

            // Số collections
            string collectionQuery = "SELECT COUNT(*) FROM collections";
            object collectionResult = ExecuteScalar(collectionQuery);
            stats.TotalCollections = collectionResult != null ? Convert.ToInt32(collectionResult) : 0;

            return stats;
        }

        /// <summary>
        /// Lấy thống kê tài liệu theo ngày (7 ngày gần nhất)
        /// </summary>
        public static DataTable GetDocumentsByDay(int days = 7)
        {
            // SQLite không có CTE recursive như SQL Server, dùng cách khác
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

            SQLiteParameter[] parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@days", days)
            };

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

            SQLiteParameter[] parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@months", months)
            };

            return ExecuteQuery(query, parameters);
        }

        #endregion

        #region Quản lý Danh mục và Định dạng tài liệu

        /// <summary>
        /// Lấy danh sách danh mục DISTINCT kèm số lượng tài liệu
        /// </summary>
        public static DataTable GetDistinctSubjects()
        {
            string query = @"SELECT danh_muc, COUNT(*) as so_luong
                           FROM tai_lieu
                           WHERE danh_muc IS NOT NULL AND danh_muc != ''
                           GROUP BY danh_muc
                           ORDER BY danh_muc";

            return ExecuteQuery(query);
        }

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
        /// Cập nhật tên danh mục
        /// </summary>
        public static bool UpdateSubjectName(string oldName, string newName)
        {
            string query = "UPDATE tai_lieu SET danh_muc = @newName WHERE danh_muc = @oldName";

            SQLiteParameter[] parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@oldName", oldName),
                new SQLiteParameter("@newName", newName)
            };

            int result = ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        /// <summary>
        /// Cập nhật tên định dạng
        /// </summary>
        public static bool UpdateTypeName(string oldName, string newName)
        {
            string query = "UPDATE tai_lieu SET dinh_dang = @newName WHERE dinh_dang = @oldName";

            SQLiteParameter[] parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@oldName", oldName),
                new SQLiteParameter("@newName", newName)
            };

            int result = ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        /// <summary>
        /// Xóa tài liệu có danh mục này
        /// </summary>
        public static bool DeleteDocumentsBySubject(string subjectName)
        {
            string query = "DELETE FROM tai_lieu WHERE danh_muc = @subjectName";

            SQLiteParameter[] parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@subjectName", subjectName)
            };

            int result = ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        /// <summary>
        /// Xóa tài liệu có loại này
        /// </summary>
        public static bool DeleteDocumentsByType(string typeName)
        {
            string query = "DELETE FROM tai_lieu WHERE dinh_dang = @typeName";

            SQLiteParameter[] parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@typeName", typeName)
            };

            int result = ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        #endregion

        #region Personal Notes

        /// <summary>
        /// Lấy ghi chú của tài liệu
        /// </summary>
        public static DataTable GetPersonalNote(int documentId)
        {
            string query = "SELECT * FROM personal_notes WHERE document_id = @documentId";

            SQLiteParameter[] parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@documentId", documentId)
            };

            return ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// Lưu hoặc cập nhật ghi chú
        /// </summary>
        public static bool SavePersonalNote(int documentId, string content)
        {
            // Kiểm tra đã có note chưa
            string checkQuery = "SELECT COUNT(*) FROM personal_notes WHERE document_id = @documentId";
            SQLiteParameter[] checkParams = new SQLiteParameter[]
            {
                new SQLiteParameter("@documentId", documentId)
            };

            object exists = ExecuteScalar(checkQuery, checkParams);
            bool hasNote = exists != null && Convert.ToInt32(exists) > 0;

            if (hasNote)
            {
                // Update
                string updateQuery = @"UPDATE personal_notes
                                      SET content = @content, updated_at = datetime('now', 'localtime')
                                      WHERE document_id = @documentId";
                SQLiteParameter[] updateParams = new SQLiteParameter[]
                {
                    new SQLiteParameter("@documentId", documentId),
                    new SQLiteParameter("@content", string.IsNullOrEmpty(content) ? DBNull.Value : (object)content)
                };
                return ExecuteNonQuery(updateQuery, updateParams) > 0;
            }
            else
            {
                // Insert
                string insertQuery = @"INSERT INTO personal_notes (document_id, content)
                                      VALUES (@documentId, @content)";
                SQLiteParameter[] insertParams = new SQLiteParameter[]
                {
                    new SQLiteParameter("@documentId", documentId),
                    new SQLiteParameter("@content", string.IsNullOrEmpty(content) ? DBNull.Value : (object)content)
                };
                return ExecuteNonQuery(insertQuery, insertParams) > 0;
            }
        }

        /// <summary>
        /// Xóa ghi chú
        /// </summary>
        public static bool DeletePersonalNote(int documentId)
        {
            string query = "DELETE FROM personal_notes WHERE document_id = @documentId";

            SQLiteParameter[] parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@documentId", documentId)
            };

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
            return ExecuteNonQuery(query, new SQLiteParameter[] { new SQLiteParameter("@id", id) }) > 0;
        }

        public static bool PermanentDeleteDocument(int id)
        {
            string query = "DELETE FROM tai_lieu WHERE id = @id";
            return ExecuteNonQuery(query, new SQLiteParameter[] { new SQLiteParameter("@id", id) }) > 0;
        }

        public static int EmptyRecycleBin()
        {
            string query = "DELETE FROM tai_lieu WHERE is_deleted = 1";
            return ExecuteNonQuery(query);
        }

        public static int GetDeletedDocumentCount()
        {
            string query = "SELECT COUNT(*) FROM tai_lieu WHERE is_deleted = 1";
            object result = ExecuteScalar(query);
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

        public static int BulkUpdateSubject(List<int> ids, string subject)
        {
            if (ids == null || ids.Count == 0) return 0;
            string idList = string.Join(",", ids);
            string query = $"UPDATE tai_lieu SET danh_muc = @subject WHERE id IN ({idList})";
            return ExecuteNonQuery(query, new SQLiteParameter[] { new SQLiteParameter("@subject", subject ?? "") });
        }

        public static int BulkToggleImportant(List<int> ids, bool important)
        {
            if (ids == null || ids.Count == 0) return 0;
            string idList = string.Join(",", ids);
            string query = $"UPDATE tai_lieu SET quan_trong = @val WHERE id IN ({idList})";
            return ExecuteNonQuery(query, new SQLiteParameter[] { new SQLiteParameter("@val", important ? 1 : 0) });
        }

        #endregion

        #region Recent Files Methods

        public static void AddRecentFile(int documentId)
        {
            string upsert = @"INSERT OR REPLACE INTO recent_files (document_id, opened_at)
                              VALUES (@docId, datetime('now','localtime'))";
            ExecuteNonQuery(upsert, new SQLiteParameter[] { new SQLiteParameter("@docId", documentId) });

            // Keep only 20 most recent
            string trim = @"DELETE FROM recent_files WHERE id NOT IN
                            (SELECT id FROM recent_files ORDER BY opened_at DESC LIMIT 20)";
            ExecuteNonQuery(trim);
        }

        public static DataTable GetRecentFiles()
        {
            string query = @"SELECT t.id, t.ten, t.danh_muc, t.dinh_dang, t.duong_dan, r.opened_at
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
                new SQLiteParameter[] { new SQLiteParameter("@docId", documentId) });
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
            ExecuteNonQuery(query, new SQLiteParameter[]
            {
                new SQLiteParameter("@d1", lo),
                new SQLiteParameter("@d2", hi),
                new SQLiteParameter("@type", relationType)
            });
        }

        public static DataTable GetRelatedDocuments(int docId)
        {
            string query = @"SELECT t.id, t.ten, t.danh_muc, t.dinh_dang, t.duong_dan, r.relation_type, r.id as relation_id
                             FROM document_relations r
                             INNER JOIN tai_lieu t ON (t.id = CASE WHEN r.doc_id_1 = @docId THEN r.doc_id_2 ELSE r.doc_id_1 END)
                             WHERE (r.doc_id_1 = @docId OR r.doc_id_2 = @docId)
                             AND t.is_deleted = 0
                             ORDER BY t.ten";
            return ExecuteQuery(query, new SQLiteParameter[] { new SQLiteParameter("@docId", docId) });
        }

        public static void RemoveDocumentRelation(int relationId)
        {
            ExecuteNonQuery("DELETE FROM document_relations WHERE id = @id",
                new SQLiteParameter[] { new SQLiteParameter("@id", relationId) });
        }

        /// <summary>
        /// Gợi ý các tài liệu liên quan dựa trên Danh mục hoặc Thẻ gắn
        /// </summary>
        public static DataTable GetSuggestedRelatedDocuments(int docId)
        {
            string query = @"
                SELECT id, ten, danh_muc, dinh_dang, tags
                FROM tai_lieu t
                WHERE id != @docId 
                AND is_deleted = 0
                AND (
                    -- Cùng danh mục
                    danh_muc IN (SELECT danh_muc FROM tai_lieu WHERE id = @docId AND danh_muc IS NOT NULL AND danh_muc != '')
                    OR 
                    -- Hoặc có chung ít nhất 1 tag (giả sử tag cách nhau bởi dấu ;)
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
            
            return ExecuteQuery(query, new SQLiteParameter[] { new SQLiteParameter("@docId", docId) });
        }

        #endregion
    }
}
