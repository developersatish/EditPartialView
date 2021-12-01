using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace PartialEdit.Models
{
    public class SearchResultsModel<T>
    {
        private static readonly Dictionary<int, int> _pageSizeList = new Dictionary<int, int>
        {
           {1, 10},
           {2, 20},
           {3, 50},
           {4, 100}
        };

        public IEnumerable<T> Results { get; set; }

        public PagerMode PageDetails { get; set; }

        public int? MatchesFoundCount { get; set; }

        public int PageSizeKey { get; set; }
        public Dictionary<int, int> PageSizeList
        {
            get
            {
                return _pageSizeList;
            }
            set
            {

            }
        }

        public int GetPageSizeWithKey(int key)
        {
            return _pageSizeList[key];
        }

      
    }
}