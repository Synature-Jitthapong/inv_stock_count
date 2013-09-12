using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using POSMySQL.POSControl;
using MySql.Data.MySqlClient;
using POSInventoryPOROModule;

namespace InventoryStockCount
{
    [Serializable]
    public class DataForCrystalReport : DataTableForCrytalReportDocumentPreview
    {
        public DataForCrystalReport()
        {
               
        }
    }
}
