using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SevenDays.Api.Helpers
{
    /// <summary>
    /// Class that encapsulates the paging response
    /// </summary>
    /// <typeparam name="T">Any object</typeparam>
    public class PagedCollectionResponse<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public Uri NextPage { get; set; }
        public Uri PreviousPage { get; set; }
    }

}
