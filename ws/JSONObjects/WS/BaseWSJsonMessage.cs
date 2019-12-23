using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CukiKaveManagerV2.JSONObjects.WS
{
    public class BaseWSJsonMessage
    {
        public string type { get; set; }
        public JObject product { get; set; }
    }
}
