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
        //public static string server = "http://localhost"; //DEBUG LOCALHOST
        public static string server = "http://cukikave.herokuapp.com"; //MAIN SERVER
        const uint PORT = 80;

        //URLS
        const string URL_GETPRODUCTS = "/getProducts";
        const string URL_DELETEPRODUCT = "/products/delete";
        const string URL_CREATEPRODUCT = "/products/create";
        const string URL_UPDATEPRODUCT = "/products/update";

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

        public async static Task<bool> SendNewProduct(Product _prd)
        {
            var parameters = new Dictionary<string, string>() { { "hash", hash } };
            parameters.Add("name", _prd.name);
            parameters.Add("price", _prd.price.ToString());
            parameters.Add("image", _prd.image);
            parameters.Add("type", _prd.type);
            parameters.Add("description", _prd.description);
            parameters.Add("added", _prd.added.ToString());

            var content = new FormUrlEncodedContent(parameters);
            var result = await client.PostAsync(server + URL_CREATEPRODUCT, content);
            return result.IsSuccessStatusCode;
        }

        public async static Task<bool> UpdateProduct(Product _prd)
        {
            var parameters = new Dictionary<string, string>() { { "hash", hash } };
            parameters.Add("id", _prd.id.ToString());
            parameters.Add("name", _prd.name);
            parameters.Add("price", _prd.price.ToString());
            parameters.Add("image", _prd.image);
            parameters.Add("type", _prd.type);
            parameters.Add("description", _prd.description);
            parameters.Add("added", _prd.added.ToString());

            var content = new FormUrlEncodedContent(parameters);
            var result = await client.PostAsync(server + URL_UPDATEPRODUCT, content);
            return result.IsSuccessStatusCode;
        }
    }
}
