using System;
using System.Collections.Generic;
using document_sharing_manager.Core.Domain;

namespace document_sharing_manager.Core.Interfaces
{
    public interface IDashboardContract
    {
        string SearchKeyword { get; }
        string SelectedFormat { get; }
        DateTime? FilterFromDate { get; }
        DateTime? FilterToDate { get; }
        double? FilterMinSize { get; }
        double? FilterMaxSize { get; }
        bool FilterIsImportant { get; }

        event EventHandler SearchRequested;
        event EventHandler FilterApplied;
        event EventHandler RefreshRequested;
        event EventHandler<int> DeleteRequested;

        void SetFormats(List<string> formats);
        void SetDocumentList(List<Document> documents);
        void UpdateStatusCount(int count);
        bool ConfirmDelete();
        void ShowMessage(string message);
        void ShowError(string error);
    }
}
