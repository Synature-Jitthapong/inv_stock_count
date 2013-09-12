using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace InventoryStockCount
{
    public class DailyStockFixCheckEnddayBeforeApprove : StockCountBase
    {
        public DailyStockFixCheckEnddayBeforeApprove(MySqlConnection conn, int shopId, int documentTypeId)
        {
            base._conn = conn;
            base._shopId = shopId;
            base._documentTypeId = documentTypeId;
        }

        public override bool CheckEnddaySession()
        {
            return base.CheckEnddaySession();
        }

        public override Document CheckApproveDocument()
        {
            throw new NotImplementedException();
        }
    }
}
