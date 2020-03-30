using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SevenDays.Api.Helpers
{
    /// <summary>
    /// Filter parameters base
    /// </summary>
    public abstract class FilterModelBase : ICloneable
    {
        public int Page { get; set; }
        public int Limit { get; set; }

        public FilterModelBase()
        {
            this.Page = 1;
            this.Limit = 100;
        }

        public abstract object Clone();
    }

    /// <summary>
    /// List of filter parameters specifict for movies
    /// </summary>
    public class FilterModel: FilterModelBase
    {
        public string Title { get; set; }
        public bool IncludeInactive { get; set; }

        public FilterModel(): base()
        {
            this.Limit = 3;
        }

        public override object Clone()
        {
            var jsonString = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject(jsonString, this.GetType());
        }

    }

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
