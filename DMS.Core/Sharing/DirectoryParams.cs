using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Core.Sharing
{
    public class DirectoryParams
    {
        public int MaxPageSize { get; set; } = 15;

        private int _pageSize = 5;

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value > MaxPageSize ? MaxPageSize : value; }
        }

        public string Sort { get; set; }

        public int? WorkspaceId { get; set; }

        public int PageNumber { get; set; } = 1;

        public string Search { get; set; }
    }
}
