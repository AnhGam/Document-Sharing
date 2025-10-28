using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace study_document_manager
{
    /// <summary>
    /// Class quản lý kết nối và thao tác với SQL Server Database
    /// </summary>
    public class DatabaseHelper
    {
        // Connection string - đọc từ App.config
        private static string connection_string = GetConnectionStringFromConfig();

        /// <summary>
        /// Đọc connection string từ App.config
        /// </summary>
        private static string GetConnectionStringFromConfig()
        {
            try
            {
                string configPath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                XDocument doc = XDocument.Load(configPath);
                var connectionString = doc.Descendants("connectionStrings")
                    .Descendants("add")
                    .Where(x => x.Attribute("name")?.Value == "DefaultConnection")
                    .Select(x => x.Attribute("connectionString")?.Value)
                    .FirstOrDefault();
                
                return connectionString ?? "Server=DESKTOP-H1DIIG3\\SQL2012;Database=quan_ly_tai_lieu;Integrated Security=True;";
            }
            catch
            {
                // Fallback nếu không đọc được
                return "Server=DESKTOP-H1DIIG3\\SQL2012;Database=quan_ly_tai_lieu;Integrated Security=True;";
            }
        }

        /// <summary>
        /// Lấy connection string hiện tại
        /// </summary>
        public static string ConnectionString
        {
            get { return connection_string; }
            set { connection_string = value; }
        }

        /// <summary>
        /// Kiểm tra kết nối đến database
        /// </summary>
        /// <returns>True nếu kết nối thành công, False nếu thất bại</returns>
        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connection_string))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối database: " + ex.Message + "\n\nConnection string: " + connection_string, 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Thực hiện câu lệnh SELECT và trả về DataTable
        /// </summary>
        /// <param name="query">Câu truy vấn SELECT</param>
        /// <param name="parameters">Dictionary chứa parameters (tùy chọn)</param>
        /// <returns>DataTable chứa kết quả</returns>
        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connection_string))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
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
        /// <param name="query">Câu truy vấn</param>
        /// <param name="parameters">Mảng SqlParameter</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            int affected_rows = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connection_string))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        affected_rows = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thực thi: " + ex.Message, 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return affected_rows;
        }

        /// <summary>
        /// Thực hiện câu lệnh trả về giá trị đơn (COUNT, SUM, v.v.)
        /// </summary>
        /// <param name="query">Câu truy vấn</param>
        /// <param name="parameters">Mảng SqlParameter</param>
        /// <returns>Giá trị đơn (object)</returns>
        public static object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            object result = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connection_string))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
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
            string query = "SELECT * FROM tai_lieu ORDER BY ngay_them DESC";
            return ExecuteQuery(query);
        }

        /// <summary>
        /// Tìm kiếm tài liệu theo từ khóa
        /// </summary>
        public static DataTable SearchDocuments(string keyword)
        {
            string query = @"SELECT * FROM tai_lieu 
                           WHERE ten LIKE @keyword 
                           OR mon_hoc LIKE @keyword 
                           OR ghi_chu LIKE @keyword 
                           ORDER BY ngay_them DESC";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@keyword", "%" + keyword + "%")
            };
            
            return ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// Lọc tài liệu theo môn học và loại
        /// </summary>
        public static DataTable FilterDocuments(string mon_hoc, string loai)
        {
            string query = "SELECT * FROM tai_lieu WHERE 1=1";
            
            if (!string.IsNullOrEmpty(mon_hoc) && mon_hoc != "Tất cả")
            {
                query += " AND mon_hoc = @mon_hoc";
            }
            
            if (!string.IsNullOrEmpty(loai) && loai != "Tất cả")
            {
                query += " AND loai = @loai";
            }
            
            query += " ORDER BY ngay_them DESC";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@mon_hoc", string.IsNullOrEmpty(mon_hoc) ? DBNull.Value : (object)mon_hoc),
                new SqlParameter("@loai", string.IsNullOrEmpty(loai) ? DBNull.Value : (object)loai)
            };
            
            return ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// Thêm tài liệu mới
        /// </summary>
        public static bool InsertDocument(string ten, string mon_hoc, string loai, 
            string duong_dan, string ghi_chu, double? kich_thuoc, string tac_gia, bool quan_trong)
        {
            string query = @"INSERT INTO tai_lieu 
                (ten, mon_hoc, loai, duong_dan, ghi_chu, kich_thuoc, tac_gia, quan_trong) 
                VALUES 
                (@ten, @mon_hoc, @loai, @duong_dan, @ghi_chu, @kich_thuoc, @tac_gia, @quan_trong)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ten", ten),
                new SqlParameter("@mon_hoc", string.IsNullOrEmpty(mon_hoc) ? DBNull.Value : (object)mon_hoc),
                new SqlParameter("@loai", string.IsNullOrEmpty(loai) ? DBNull.Value : (object)loai),
                new SqlParameter("@duong_dan", duong_dan),
                new SqlParameter("@ghi_chu", string.IsNullOrEmpty(ghi_chu) ? DBNull.Value : (object)ghi_chu),
                new SqlParameter("@kich_thuoc", kich_thuoc.HasValue ? (object)kich_thuoc.Value : DBNull.Value),
                new SqlParameter("@tac_gia", string.IsNullOrEmpty(tac_gia) ? DBNull.Value : (object)tac_gia),
                new SqlParameter("@quan_trong", quan_trong)
            };

            int result = ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        /// <summary>
        /// Cập nhật tài liệu
        /// </summary>
        public static bool UpdateDocument(int id, string ten, string mon_hoc, string loai, 
            string duong_dan, string ghi_chu, double? kich_thuoc, string tac_gia, bool quan_trong)
        {
            string query = @"UPDATE tai_lieu SET 
                ten = @ten, 
                mon_hoc = @mon_hoc, 
                loai = @loai, 
                duong_dan = @duong_dan, 
                ghi_chu = @ghi_chu, 
                kich_thuoc = @kich_thuoc, 
                tac_gia = @tac_gia, 
                quan_trong = @quan_trong 
                WHERE id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", id),
                new SqlParameter("@ten", ten),
                new SqlParameter("@mon_hoc", string.IsNullOrEmpty(mon_hoc) ? DBNull.Value : (object)mon_hoc),
                new SqlParameter("@loai", string.IsNullOrEmpty(loai) ? DBNull.Value : (object)loai),
                new SqlParameter("@duong_dan", duong_dan),
                new SqlParameter("@ghi_chu", string.IsNullOrEmpty(ghi_chu) ? DBNull.Value : (object)ghi_chu),
                new SqlParameter("@kich_thuoc", kich_thuoc.HasValue ? (object)kich_thuoc.Value : DBNull.Value),
                new SqlParameter("@tac_gia", string.IsNullOrEmpty(tac_gia) ? DBNull.Value : (object)tac_gia),
                new SqlParameter("@quan_trong", quan_trong)
            };

            int result = ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        /// <summary>
        /// Xóa tài liệu
        /// </summary>
        public static bool DeleteDocument(int id)
        {
            string query = "DELETE FROM tai_lieu WHERE id = @id";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", id)
            };

            int result = ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        /// <summary>
        /// Lấy thống kê số lượng tài liệu theo môn học
        /// </summary>
        public static DataTable GetStatisticsBySubject()
        {
            string query = @"SELECT mon_hoc, COUNT(*) as so_luong 
                           FROM tai_lieu 
                           WHERE mon_hoc IS NOT NULL 
                           GROUP BY mon_hoc 
                           ORDER BY so_luong DESC";
            return ExecuteQuery(query);
        }

        /// <summary>
        /// Lấy thống kê số lượng tài liệu theo loại
        /// </summary>
        public static DataTable GetStatisticsByType()
        {
            string query = @"SELECT loai, COUNT(*) as so_luong 
                           FROM tai_lieu 
                           WHERE loai IS NOT NULL 
                           GROUP BY loai 
                           ORDER BY so_luong DESC";
            return ExecuteQuery(query);
        }

        /// <summary>
        /// Đếm tổng số tài liệu
        /// </summary>
        public static int GetTotalDocumentCount()
        {
            string query = "SELECT COUNT(*) FROM tai_lieu";
            object result = ExecuteScalar(query);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        #region Quản lý Môn học và Loại tài liệu

        /// <summary>
        /// Lấy danh sách môn học DISTINCT kèm số lượng tài liệu
        /// </summary>
        public static DataTable GetDistinctSubjects()
        {
            string query = @"SELECT mon_hoc, COUNT(*) as so_luong 
                           FROM tai_lieu 
                           WHERE mon_hoc IS NOT NULL AND mon_hoc != '' 
                           GROUP BY mon_hoc 
                           ORDER BY mon_hoc";
            return ExecuteQuery(query);
        }

        /// <summary>
        /// Lấy danh sách loại tài liệu DISTINCT kèm số lượng
        /// </summary>
        public static DataTable GetDistinctTypes()
        {
            string query = @"SELECT loai, COUNT(*) as so_luong 
                           FROM tai_lieu 
                           WHERE loai IS NOT NULL AND loai != '' 
                           GROUP BY loai 
                           ORDER BY loai";
            return ExecuteQuery(query);
        }

        /// <summary>
        /// Cập nhật tên môn học (tất cả tài liệu)
        /// </summary>
        public static bool UpdateSubjectName(string oldName, string newName)
        {
            string query = "UPDATE tai_lieu SET mon_hoc = @newName WHERE mon_hoc = @oldName";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@oldName", oldName),
                new SqlParameter("@newName", newName)
            };

            int result = ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        /// <summary>
        /// Cập nhật tên loại tài liệu (tất cả tài liệu)
        /// </summary>
        public static bool UpdateTypeName(string oldName, string newName)
        {
            string query = "UPDATE tai_lieu SET loai = @newName WHERE loai = @oldName";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@oldName", oldName),
                new SqlParameter("@newName", newName)
            };

            int result = ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        /// <summary>
        /// Xóa tất cả tài liệu có môn học này
        /// </summary>
        public static bool DeleteDocumentsBySubject(string subjectName)
        {
            string query = "DELETE FROM tai_lieu WHERE mon_hoc = @subjectName";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@subjectName", subjectName)
            };

            int result = ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        /// <summary>
        /// Xóa tất cả tài liệu có loại này
        /// </summary>
        public static bool DeleteDocumentsByType(string typeName)
        {
            string query = "DELETE FROM tai_lieu WHERE loai = @typeName";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@typeName", typeName)
            };

            int result = ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        #endregion
    }
}
