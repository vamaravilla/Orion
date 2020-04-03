using Newtonsoft.Json;

namespace SevenDays.Api.Helpers
{
  

    /// <summary>
    /// List of filter parameters specifict for movies
    /// </summary>
    public class FilterModel: FilterModelBase
    {
        public string Title { get; set; }
        public bool IncludeInactive { get; set; }

        public FilterModel(): base()
        {
            this.Limit = 10;  //Max size for each page
        }

        public override object Clone()
        {
            var jsonString = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject(jsonString, this.GetType());
        }

    }

    
}
