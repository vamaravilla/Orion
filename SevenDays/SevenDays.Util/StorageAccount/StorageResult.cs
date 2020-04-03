using System;
using System.Collections.Generic;
using System.Text;

namespace SevenDays.Util
{
    /// <summary>
    /// Class to return the result of the storage account process
    /// </summary>
    public class StorageResult
    {
        public StorageResult()
        {
            Uri = string.Empty;
            Success = false; // Failure is assumed
            Message = string.Empty;
        }
        // URL to image access
        public string Uri { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
