using System;
using System.Collections.Generic;
using System.Text;

namespace FQCS.DeviceAdmin.Business
{
    public interface IPaginator
    {
        int TotalPages { get; set; }
        int CurrentPage { get; set; }
        int ItemsPerPage { get; set; }
        int PagesPerLoad { get; set; }
        IList<int> VisiblePages { get; set; }
        bool IsActivePage(int page);
        T ReturnIf<T>(int page, T valIfPageActive, T valIfPageNoneActive);
        void CountPage(int totalItems);
    }

    public class Paginator : IPaginator
    {
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int ItemsPerPage { get; set; } = 12;
        public int PagesPerLoad { get; set; } = 5;
        public IList<int> VisiblePages { get; set; } = new List<int>();

        public void CountPage(int totalItems)
        {
            TotalPages = totalItems / ItemsPerPage + 1;
            TotalPages = (totalItems % ItemsPerPage) == 0 ?
                TotalPages - 1 : TotalPages;

            var diff = PagesPerLoad / 2;
            var lastVisiblePage = CurrentPage + diff;
            var firstVisiblePage = CurrentPage - diff + (PagesPerLoad % 2 == 0 ? 1 : 0);
            if (lastVisiblePage > TotalPages)
            {
                firstVisiblePage -= lastVisiblePage - TotalPages;
                lastVisiblePage = TotalPages;
            }
            if (firstVisiblePage < 1)
            {
                lastVisiblePage += 1 - firstVisiblePage;
                firstVisiblePage = 1;
            }
            firstVisiblePage = firstVisiblePage > 0 ? firstVisiblePage : 1;
            lastVisiblePage = lastVisiblePage <= TotalPages ? lastVisiblePage : TotalPages;

            for (var i = firstVisiblePage; i <= lastVisiblePage; i++)
                VisiblePages.Add(i);
        }

        public bool IsActivePage(int page)
        {
            return CurrentPage == page;
        }

        public T ReturnIf<T>(int page, T valIfPageActive, T valIfPageNoneActive)
        {
            return IsActivePage(page) ? valIfPageActive : valIfPageNoneActive;
        }
    }
}
