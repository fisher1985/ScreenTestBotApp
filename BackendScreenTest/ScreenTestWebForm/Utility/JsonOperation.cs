using ScreenTestWebForm.CarModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScreenTestWebForm.Utility
{
    public class JsonOperation
    {
        public void saveAsJsonFile(List<CarModel> lst, string url)
        {
            string json = JsonSerializer.Serialize(lst);
            File.WriteAllText(url, json);
        }
        public String toJsonString(List<CarModel> lst)
        {
            String json = JsonSerializer.Serialize(lst);
            return json;
        }
    }
}
