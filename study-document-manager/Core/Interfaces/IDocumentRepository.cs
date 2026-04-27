using System;
using System.Collections.Generic;
using study_document_manager.Core.Entities;

namespace study_document_manager.Core.Interfaces
{
    public interface IDocumentRepository
    {
        List<StudyDocument> GetAll();
        StudyDocument GetById(int id);
        List<StudyDocument> Search(string keyword);
        List<StudyDocument> Filter(string category, string format);
        List<StudyDocument> SearchAdvanced(string keyword, string category, string format, DateTime? fromDate, DateTime? toDate, double? minSize, double? maxSize, bool? isImportant);
        bool Add(StudyDocument document);
        bool Update(StudyDocument document);
        bool Delete(int id);

        List<string> GetDistinctCategories();
        List<string> GetDistinctFormats();
        List<string> GetDistinctTags();
    }
}
