using CukiKaveManagerV2.ws.JSONObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CukiKaveManagerV2.ws
{
    class cukiAPI
    {
        public static string server = "http://localhost";
        const uint PORT = 80;

        //URLS
        const string URL_GETPRODUCTS = "/getProducts";
        const string URL_DELETEPRODUCT = "/products/delete";

        public static string hash;

        static readonly HttpClient client = new HttpClient();

        public async static Task<List<Product>> GetProducts()
        {
            var result = await client.GetAsync(server + URL_GETPRODUCTS);
            if (result.IsSuccessStatusCode)
            {
                string readString = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Product>>(readString);
            }
            
            return null;
        }

        /// <summary>
        /// Deletes a message from the database
        /// </summary>
        /// <returns>True on success</returns>
        public async static Task<bool> DeleteProduct(int id)
        {
            var parameters = new Dictionary<string, string>() { { "hash", hash }, { "id", id.ToString() } };
            var content = new FormUrlEncodedContent(parameters);
            var result = await client.PostAsync(server + URL_DELETEPRODUCT, content);
            return result.IsSuccessStatusCode;
        }
    }
}
