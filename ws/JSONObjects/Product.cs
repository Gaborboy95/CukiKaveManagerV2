using System;
using System.Collections.Generic;
using System.Text;

namespace CukiKaveManagerV2.ws.JSONObjects
{
    public class Product
    {
        public int price { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public string type { get; set; }
        public DateTime added { get; set; }

        public Product(int _price, string _name, int _id, string _description, string _image, string _type, DateTime _added)
        {
            price = _price;
            name = _name;
            id = _id;
            description = _description;
            image = _image;
            type = _type;
            added = _added;
        }
    }
}
