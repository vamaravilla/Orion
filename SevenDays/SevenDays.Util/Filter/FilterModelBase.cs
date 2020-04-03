using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

}
