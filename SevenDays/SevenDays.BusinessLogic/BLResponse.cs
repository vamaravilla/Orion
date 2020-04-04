using System;
using System.Collections.Generic;
using System.Text;

namespace SevenDays.BusinessLogic
{
    public class BLResult<T> where T : class
    {
        public BLResult()
        {
            Success = false; // Failure is assumed
            Message = string.Empty;
        }
        public bool Success { set; get; }
        public string Message { set; get; }
        public T Item { set; get; }
    }

    public class BLResults<T> where T : class
    {
        public BLResults()
        {
            Success = false; // Failure is assumed
            Message = string.Empty;
        }
        public bool Success { set; get; }
        public string Message { set; get; }
        public IEnumerable<T> Items { get; set; }
    }
    public class BLFResults<T> where T : class
    {
        public BLFResults()
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
