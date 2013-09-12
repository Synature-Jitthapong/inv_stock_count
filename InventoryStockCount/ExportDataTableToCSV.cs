using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;

namespace InventoryStockCount
{
    public static class ExportDataTableToCSV
    {
        public static void ExportDataToCSV(DataTable dtData, string exportFileName)
        {
            DataTable table = dtData;
            HttpContext context = HttpContext.Current;
            context.Response.ContentType = "text/csv";
            context.Response.Charset = "windows-874";
            context.Response.ContentEncoding = System.Text.Encoding.GetEncoding(874);
            context.Response.AppendHeader("Content-Disposition", "attachment; filename=" + exportFileName + ".csv");

            if (table.Rows.Count > 0)
            {
                /*foreach (DataColumn column in table.Columns)
                {
                    context.Response.Write("'" + column.ColumnName + ",");
                }*/
                for (int i = 0; i <= table.Columns.Count - 1; i++)
                {
                    context.Response.Write(table.Columns[i].ColumnName);
                    if (i < table.Columns.Count - 1)
                        context.Response.Write(",");
                }
                context.Response.Write(Environment.NewLine);
                foreach (DataRow row in table.Rows)
                {
                    for (int i = 0; i <= table.Columns.Count - 1; i++)
                    {
                        context.Response.Write(row[i].ToString().Replace(",", string.Empty));
                        if (i < table.Columns.Count - 1)
                            context.Response.Write(",");
                    }
                    context.Response.Write(Environment.NewLine);
                }
            }
            context.Response.End();
        }
    }
}
