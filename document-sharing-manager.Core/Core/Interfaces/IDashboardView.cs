using System;
using System.Collections.Generic;
using document_sharing_manager.Core.Entities;

namespace document_sharing_manager.Core.Interfaces
{
    public interface IDashboardView
    {
        // Properties
        string SearchKeyword { get; }
        string SelectedFormat { get; }
        DateTime? FilterFromDate { get; }
        DateTime? FilterToDate { get; }
        double? FilterMinSize { get; }
        double? FilterMaxSize { get; }
        bool FilterIsImportant { get; }

        // Data Sources
        void SetDocumentList(List<Document> documents);
        void SetFormats(List<string> formats);
        void UpdateStatusCount(int count);

        // Events
        event EventHandler SearchRequested;
        event EventHandler FilterApplied;
        event EventHandler RefreshRequested;
        event EventHandler<int> DeleteRequested;


        event EventHandler<string> OpenFileRequested;
        event EventHandler ExportRequested;
        event EventHandler StatisticsRequested;

        // UI Feedback
        void ShowMessage(string message);
        void ShowError(string message);
        bool ConfirmDelete();
    }
}

