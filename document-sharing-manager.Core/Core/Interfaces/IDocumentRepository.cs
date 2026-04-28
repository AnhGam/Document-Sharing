using System;
using System.Collections.Generic;
using document_sharing_manager.Core.Entities;

namespace document_sharing_manager.Core.Interfaces
{
    public interface IDocumentRepository
    {
        List<Document> GetAll();
        Document GetById(int id);
        List<Document> Search(string keyword);
        List<Document> Filter(string category, string format);
        List<Document> SearchAdvanced(string keyword, string category, string format, DateTime? fromDate, DateTime? toDate, double? minSize, double? maxSize, bool? isImportant);
        bool Add(Document document);
        bool Update(Document document);
        bool Delete(int id);

        List<string> GetDistinctCategories();
        List<string> GetDistinctFormats();
        List<string> GetDistinctTags();
    }
}

