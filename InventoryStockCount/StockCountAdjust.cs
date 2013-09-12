using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventoryStockCount
{
    public class StockCountAdjust : Document
    {
        public override bool AddDocument(POSMySQL.POSControl.CDBUtil dbUtil, MySql.Data.MySqlClient.MySqlConnection conn)
        {
            return base.AddDocument(dbUtil, conn);
        }
    }
}
