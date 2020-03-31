using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Formatting;

namespace SevenDaysFront.Data
{
    public class MovieService
    {
        // Temporary solution
        private const string URL = "https://sevendayschallengeapi.azurewebsites.net/api/movies";
        private string urlParameters = "?sort=popularity_desc";

        public Task<Movie[]> GetMovieAsync()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);
            Movie[] listMovies = null;

            // List data response.
            HttpResponseMessage response =  client.GetAsync(urlParameters).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                var dataObjects = response.Content.ReadAsAsync<IEnumerable<Movie>>().Result;

                listMovies =  dataObjects.ToArray();
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            //Dispose once all HttpClient calls are complete. 
            client.Dispose();

            return Task.Run(() =>
            {
                return listMovies;
            });
        }
    }
}
