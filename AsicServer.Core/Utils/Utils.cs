using System;
using System.Collections.Generic;
using System.Data;
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

    }
}
