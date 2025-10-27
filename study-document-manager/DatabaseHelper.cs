using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace study_document_manager
{
    /// <summary>
    /// Class qu?n lý k?t n?i và thao tác v?i SQL Server Database
    /// </summary>
    public class DatabaseHelper
    {
        // Connection string - ??c t? App.config
        private static string connection_string = GetConnectionStringFromConfig();

        /// <summary>
        /// ??c connection string t? App.config
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
                // Fallback n?u không ??c ???c
                return "Server=DESKTOP-H1DIIG3\\SQL2012;Database=quan_ly_tai_lieu;Integrated Security=True;";
            }
        }

        /// <summary>
        /// L?y connection string hi?n t?i
        /// </summary>
        public static string ConnectionString
        {
            get { return connection_string; }
            set { connection_string = value; }
        }

        /// <summary>
        /// Ki?m tra k?t n?i ??n database
        /// </summary>
        /// <returns>True n?u k?t n?i thành công, False n?u th?t b?i</returns>
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
                MessageBox.Show("L?i k?t n?i database: " + ex.Message + "\n\nConnection string: " + connection_string, 
                    "L?i", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Th?c hi?n câu l?nh SELECT và tr? v? DataTable
        /// </summary>
        /// <param name="query">Câu truy v?n SELECT</param>
        /// <param name="parameters">Dictionary ch?a parameters (tùy ch?n)</param>
        /// <returns>DataTable ch?a k?t qu?</returns>
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
                MessageBox.Show("L?i truy v?n: " + ex.Message, 
                    "L?i", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        /// <summary>
        /// Th?c hi?n câu l?nh INSERT, UPDATE, DELETE
        /// </summary>
        /// <param name="query">Câu truy v?n</param>
        /// <param name="parameters">M?ng SqlParameter</param>
        /// <returns>S? dòng b? ?nh h??ng</returns>
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
                MessageBox.Show("L?i th?c thi: " + ex.Message, 
                    "L?i", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return affected_rows;
        }

        /// <summary>
        /// Th?c hi?n câu l?nh tr? v? giá tr? ??n (COUNT, SUM, v.v.)
        /// </summary>
        /// <param name="query">Câu truy v?n</param>
        /// <param name="parameters">M?ng SqlParameter</param>
        /// <returns>Giá tr? ??n (object)</returns>
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
                MessageBox.Show("L?i th?c thi: " + ex.Message, 
                    "L?i", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }

        /// <summary>
        /// L?y t?t c? tài li?u
        /// </summary>
        public static DataTable GetAllDocuments()
        {
            string query = "SELECT * FROM tai_lieu ORDER BY ngay_them DESC";
            return ExecuteQuery(query);
        }

        /// <summary>
        /// Tìm ki?m tài li?u theo t? khóa
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
        /// L?c tài li?u theo môn h?c và lo?i
        /// </summary>
        public static DataTable FilterDocuments(string mon_hoc, string loai)
        {
            string query = "SELECT * FROM tai_lieu WHERE 1=1";
            
            if (!string.IsNullOrEmpty(mon_hoc) && mon_hoc != "T?t c?")
            {
                query += " AND mon_hoc = @mon_hoc";
            }
            
            if (!string.IsNullOrEmpty(loai) && loai != "T?t c?")
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
        /// Thêm tài li?u m?i
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
        /// C?p nh?t tài li?u
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
        /// Xóa tài li?u
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
        /// L?y th?ng kê s? l??ng tài li?u theo môn h?c
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
        /// L?y th?ng kê s? l??ng tài li?u theo lo?i
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
        /// ??m t?ng s? tài li?u
        /// </summary>
        public static int GetTotalDocumentCount()
        {
            string query = "SELECT COUNT(*) FROM tai_lieu";
            object result = ExecuteScalar(query);
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }
}
