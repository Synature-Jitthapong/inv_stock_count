using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using POSMySQL.POSControl;

namespace InventoryStockCount
{
    public class StockCountMonthlyFixChkEnddayBeforeApprove : StockCountBase
    {
        public StockCountMonthlyFixChkEnddayBeforeApprove(MySqlConnection conn, int shopId, int documentTypeId)
        {
            base._conn = conn;
            base._shopId = shopId;
            base._documentTypeId = documentTypeId;
        }

        public bool CheckSessionEnddayDetail(DateTime finalDateOfMonth)
        {
            bool isEndday = false;
            string strSql = " SELECT * FROM sessionenddaydetail WHERE \n" +
                     " SessionDate='" + finalDateOfMonth.ToString("yyyy-MM-dd", _dateProvider) + "' \n" +
                     " AND ProductLevelID=" + _shopId + " AND IsEndDay=1";

            MySqlCommand cmd = new MySqlCommand(strSql, _conn);
            using (MySqlDataReader reader2 = cmd.ExecuteReader())
            {
                if (reader2.Read())
                {
                    isEndday = true;
                }
            }
            return isEndday;
        }

        public override bool CheckEnddaySession()
        {
            bool isEndday = false;
            StockCountMonthly monthlyStock = new StockCountMonthly(base._shopId);

            DateTime monthlyCountDate = DateTime.Now;
            monthlyStock.GetLastCountDate(new POSMySQL.POSControl.CDBUtil(),
            base._conn, base._shopId, ref monthlyCountDate);

            DateTime finalDateOfMonth = DateTime.Now;//new DateTime(monthlyCountDate.Year, monthlyCountDate.Month,
            //DateTime.DaysInMonth(monthlyCountDate.Year, monthlyCountDate.Month));

            string strSql = "";

            strSql = " SELECT  COUNT(TransactionID) FROM ordertransaction " +
                " WHERE ShopID=" + _shopId + " AND SaleDate='" + finalDateOfMonth.ToString("yyyy-MM-dd", _dateProvider) + "' ";

            MySqlCommand cmd = new MySqlCommand(strSql, _conn);

            int countRow = Convert.ToInt32(cmd.ExecuteScalar());
            if (countRow > 0)
            {
                isEndday = CheckSessionEnddayDetail(finalDateOfMonth);
            }
            else
            {
                // ยกยอดครั้งแรก
                DateTime fromDate = new DateTime(monthlyCountDate.Year, monthlyCountDate.Month, 1);

                strSql = " SELECT  COUNT(TransactionID) FROM ordertransaction " +
                " WHERE ShopID=" + _shopId + " AND SaleDate BETWEEN '" + fromDate.ToString("yyyy-MM-dd", _dateProvider) + "'" +
                " AND '" + finalDateOfMonth.ToString("yyyy-MM-dd", _dateProvider) + "'";

                cmd = new MySqlCommand(strSql, _conn);

                countRow = Convert.ToInt32(cmd.ExecuteScalar());
                if (countRow > 0)
                {
                    isEndday = CheckSessionEnddayDetail(finalDateOfMonth);
                }
                else
                {
                    isEndday = true; // ยกยอดเดือนใหม่
                }
            }
            return isEndday;
        }

        public bool CheckEnddaySession(bool chkForApprove, DateTime documentDate)
        {
            bool isEndday = false;

            StockCountMonthly stockCountMonthly = new StockCountMonthly(_shopId);
            if (!stockCountMonthly.CheckTransferDocument(_conn, _shopId))
            {
                isEndday = true;
            }
            else
            {
                DateTime finalDateOfMonth = documentDate;
                isEndday = CheckSessionEnddayDetail(finalDateOfMonth);
            }
            return isEndday;
        }

        public override Document CheckApproveDocument()
        {
            throw new NotImplementedException();
        }
    }
}
