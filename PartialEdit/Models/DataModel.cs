using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PartialEdit.Models
{
    public class DataModel
    {
        public int count { get; set; }
        public List<Data> entries { get; set; }

        public static DataModel GetData()
        {
            DataModel response = new DataModel();
            string jsonFilePath = @"D:\POC\Practice\PartialEdit\PartialEdit\Models\data.json";
            using (StreamReader r = new StreamReader(jsonFilePath))
            {
                string json = r.ReadToEnd();
                response = JsonConvert.DeserializeObject<DataModel>(json);

                int i = 1;
                foreach (var item in response.entries)
                {
                    item.ID = i;
                    item.HTTPS = false;
                    i++;
                }
            }

            return response;
        }
    }

    public class Data
    {
        public Data()
        {
            this.ID = 1;
        }
        public int ID { get; set; }
        public string API { get; set; }
        public string Description { get; set; }
        public string Auth { get; set; }
        public bool HTTPS { get; set; }
        public string Cors { get; set; }
        public string Link { get; set; }
        public string Category { get; set; }
    }


}