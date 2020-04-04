using System;
using System.Collections.Generic;
using System.Text;

namespace SevenDays.DataAccess
{
    public class DBResult<T> where T : class
    {
        public DBResult()
        {
            Success = false; // Failure is assumed
            Message = string.Empty;
        }

        public bool Success { set; get; }
        public string Message { set; get; }
        public T Item { set; get; }
    }

    public class DBResults<T> where T : class
    {
        public DBResults()
        {
            Success = false; // Failure is assumed
            Message = string.Empty;
        }
        public bool Success { set; get; }
        public string Message { set; get; }
        public IEnumerable<T> Items { get; set; }
    }

    public class DBFResults<T> where T : class
    {
        public DBFResults()
        {
            Success = false; // Failure is assumed
            Message = string.Empty;
        }
        public bool Success { set; get; }
        public string Message { set; get; }
        public IEnumerable<T> Items { get; set; }
        public int CountNextFilter { get; set; }
    }

}
