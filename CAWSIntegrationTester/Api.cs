using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CAWSIntegrationTester
{
   class Api<T>
   {
      private readonly Uri _uri;
      private readonly string _token;
      private readonly HttpClient _client;

      public Api(HttpClient client, Uri uri, string token)
      {
         _client = client;
         _uri = uri;
         _token = token;

         _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
      }

      public async Task<IEnumerable<T>> Get()
      {
         T[] items = null;
         
         var response = await _client.GetAsync(_uri);
         if (response.IsSuccessStatusCode)
         {
            items = await response.Content.ReadAsAsync<T[]>();
         }

         return items;
      }


   }
}
