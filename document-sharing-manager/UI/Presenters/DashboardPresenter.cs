using System;
using System.Collections.Generic;
using System.Linq;
using document_sharing_manager.Core.Domain;
using document_sharing_manager.Core.Interfaces;

namespace document_sharing_manager.UI.Presenters
{
    public class DashboardPresenter
    {
        private readonly IDashboardContract _view;
        private readonly IDocumentRepository _repository;

        public DashboardPresenter(IDashboardContract view, IDocumentRepository repository)
        {
            _view = view;
            _repository = repository;

            // Subscribe to events
            _view.SearchRequested += OnSearchRequested;
            _view.FilterApplied += OnFilterApplied;
            _view.RefreshRequested += OnRefreshRequested;
            _view.DeleteRequested += OnDeleteRequested;
            // Add/Edit/Open would typically launch other forms, handled here or in view
        }

        public void Initialize()
        {
            LoadFilterOptions();
            LoadAllDocuments();
        }

        private void LoadFilterOptions()
        {
            // var categories removed

            var formats = _repository.GetDistinctFormats();
            formats.Insert(0, "Tất cả");
            _view.SetFormats(formats);
        }

        private void LoadAllDocuments()
        {
            var docs = _repository.GetAll();
            _view.SetDocumentList(docs);
            _view.UpdateStatusCount(docs.Count);
        }

        private void OnRefreshRequested(object sender, EventArgs e)
        {
            Initialize();
        }

        private void OnSearchRequested(object sender, EventArgs e)
        {
            string keyword = _view.SearchKeyword;
            var docs = _repository.Search(keyword);
            _view.SetDocumentList(docs);
            _view.UpdateStatusCount(docs.Count);
        }

        private void OnFilterApplied(object sender, EventArgs e)
        {
            var docs = _repository.SearchAdvanced(
                _view.SearchKeyword,
                _view.SelectedFormat,
                _view.FilterFromDate,
                _view.FilterToDate,
                _view.FilterMinSize,
                _view.FilterMaxSize,
                _view.FilterIsImportant
            );
            _view.SetDocumentList(docs);
            _view.UpdateStatusCount(docs.Count);
        }

        private void OnDeleteRequested(object sender, int id)
        {
            if (_view.ConfirmDelete())
            {
                if (_repository.Delete(id))
                {
                    _view.ShowMessage("Đã xóa tài liệu thành công.");
                    LoadAllDocuments(); // Reload list
                }
                else
                {
                    _view.ShowError("Không thể xóa tài liệu.");
                }
            }
        }
    }
}

