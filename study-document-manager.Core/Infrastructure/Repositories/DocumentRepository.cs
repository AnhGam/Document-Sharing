using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using study_document_manager.Core.Entities;
using study_document_manager.Core.Interfaces;
using study_document_manager.Core.Data;

namespace study_document_manager.Core.Infrastructure.Repositories
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
                KichThuoc = row["kich_thuoc"] != DBNull.Value ? Convert.ToDouble(row["kich_thuoc"]) : (double?)null,
                QuanTrong = Convert.ToInt32(row["quan_trong"]) == 1,
                Tags = row["tags"].ToString()
            };
        }

        private List<Document> ExecuteAndMap(string query, System.Data.SQLite.SQLiteParameter[] parameters = null)
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            List<Document> list = new List<Document>();
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

        public Document GetById(int id)
        {
            string query = "SELECT * FROM tai_lieu WHERE id = @id";
            var parameters = new System.Data.SQLite.SQLiteParameter[] { new System.Data.SQLite.SQLiteParameter("@id", id) };
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

             var parameters = new System.Data.SQLite.SQLiteParameter[] { new System.Data.SQLite.SQLiteParameter("@keyword", "%" + keyword + "%") };
             return ExecuteAndMap(query, parameters);
        }

        public List<Document> Filter(string category, string format)
        {
            string query = "SELECT * FROM tai_lieu WHERE (is_deleted IS NULL OR is_deleted = 0)";
            var parameters = new List<System.Data.SQLite.SQLiteParameter>();

            if (!string.IsNullOrEmpty(format) && format != "Tất cả")
            {
                query += " AND dinh_dang = @dinh_dang";
                parameters.Add(new System.Data.SQLite.SQLiteParameter("@dinh_dang", format));
            }

            query += " ORDER BY ngay_them DESC";
            return ExecuteAndMap(query, parameters.ToArray());
        }

        public List<Document> SearchAdvanced(string keyword, string category, string format, DateTime? fromDate, DateTime? toDate, double? minSize, double? maxSize, bool? isImportant)
        {
            string baseQuery = @"SELECT * FROM tai_lieu WHERE (is_deleted IS NULL OR is_deleted = 0)";
            List<System.Data.SQLite.SQLiteParameter> parameterList = new List<System.Data.SQLite.SQLiteParameter>();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                baseQuery += " AND (ten LIKE @keyword OR ghi_chu LIKE @keyword OR tags LIKE @keyword)";
                parameterList.Add(new System.Data.SQLite.SQLiteParameter("@keyword", "%" + keyword + "%"));
            }

            if (!string.IsNullOrEmpty(format) && format != "Tất cả")
            {
                baseQuery += " AND dinh_dang = @dinh_dang";
                parameterList.Add(new System.Data.SQLite.SQLiteParameter("@dinh_dang", format));
            }

            if (fromDate.HasValue)
            {
                baseQuery += " AND date(ngay_them) >= date(@fromDate)";
                parameterList.Add(new System.Data.SQLite.SQLiteParameter("@fromDate", fromDate.Value.ToString("yyyy-MM-dd")));
            }

            if (toDate.HasValue)
            {
                baseQuery += " AND date(ngay_them) <= date(@toDate)";
                parameterList.Add(new System.Data.SQLite.SQLiteParameter("@toDate", toDate.Value.ToString("yyyy-MM-dd")));
            }

            if (minSize.HasValue)
            {
                baseQuery += " AND kich_thuoc >= @minSize";
                parameterList.Add(new System.Data.SQLite.SQLiteParameter("@minSize", minSize.Value));
            }

            if (maxSize.HasValue)
            {
                baseQuery += " AND kich_thuoc <= @maxSize";
                parameterList.Add(new System.Data.SQLite.SQLiteParameter("@maxSize", maxSize.Value));
            }

            if (isImportant.HasValue && isImportant.Value == true)
            {
                baseQuery += " AND quan_trong = 1";
            }

            baseQuery += " ORDER BY ngay_them DESC";

            return ExecuteAndMap(baseQuery, parameterList.ToArray());
        }

        public bool Add(Document doc)
        {
            return DatabaseHelper.InsertDocument(doc.Ten, "", doc.DinhDang, doc.DuongDan, doc.GhiChu, doc.KichThuoc, doc.QuanTrong, doc.Tags);
        }

        public bool Update(Document doc)
        {
             return DatabaseHelper.UpdateDocument(doc.Id, doc.Ten, "", doc.DinhDang, doc.DuongDan, doc.GhiChu, doc.KichThuoc, doc.QuanTrong, doc.Tags);
        }

        public bool Delete(int id)
        {
            return DatabaseHelper.DeleteDocument(id);
        }

        public List<string> GetDistinctCategories()
        {
            return new List<string>();
        }

        public List<string> GetDistinctFormats()
        {
            var dt = DatabaseHelper.GetDistinctTypes();
             var list = new List<string>();
            foreach(DataRow row in dt.Rows) 
                list.Add(row["dinh_dang"]?.ToString() ?? "");
            return list;
        }

        public List<string> GetDistinctTags()
        {
            return DatabaseHelper.GetDistinctTags();
        }

    }
}

