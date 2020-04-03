using System;
using System.Collections.Generic;
using System.Text;

namespace SevenDays.BusinessLogic
{
    public class BLResult<T> where T : class
    {
        public bool Success { set; get; }
        public string Message { set; get; }
        public T Item { set; get; }
    }

    public class BLResults<T> where T : class
    {
        public bool Success { set; get; }
        public string Message { set; get; }
        public IEnumerable<T> Items { get; set; }
    }
}
