using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using System.Configuration;

namespace SevenDaysFront.Data
{
    public class MovieService
    {

        public Task<Movie[]> GetMovieAsync(string basUrl)
        {
            
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{basUrl}/api/movies");
            Movie[] listMovies = null;

            // List data response.
            HttpResponseMessage response =  client.GetAsync(string.Empty).Result;  // Blocking call!
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
