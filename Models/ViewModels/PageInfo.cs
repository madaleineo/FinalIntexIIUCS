using System;
namespace UtahCarSafety.Models.ViewModels
{
    public class PageInfo
    {
        public int TotalNumRecords { get; set; }

        public int RecordsPerPage { get; set; }

        public int CurrentPage { get; set; }

        //figure out how many pages are needed

        public int TotalPages => (int)Math.Ceiling((double)TotalNumRecords / RecordsPerPage);
    }
}
