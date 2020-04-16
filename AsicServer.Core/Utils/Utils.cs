using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace AsicServer.Core.Utils
{
    public class Utils
    {
        public static  DataTable GetTableType(IEnumerable<long> ids, string columnName = "stagingId")
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn(columnName, typeof(int)));
            foreach (var id in ids)
            {
                table.Rows.Add(id);
            }
            return table;
        }

        public static Stream GetFile(string name)
        {
            var fileName = Path.Combine(Environment.CurrentDirectory, name);
            var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return stream;
        }

        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
    }
}
