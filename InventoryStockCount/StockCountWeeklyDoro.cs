using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using MySql.Data.MySqlClient;

namespace InventoryStockCount
{
    /// <summary>StockCountWeekly Doro</summary>
    public class StockCountWeeklyDoro : StockCountBase
    {
        public StockCountWeeklyDoro(MySqlConnection conn, int shopId, int documentTypeId)
        {
            base._conn = conn;
            base._shopId = shopId;
            base._documentTypeId = documentTypeId;
        }

        public override string SundayDate()
        {
            //countstock 25
            DateTime dt = DateTime.Now;
            int dayAdd = 0;

            switch (dt.Day)
            {
                case 25:
                    dayAdd = 0;
                    break;
                case 26:
                    dayAdd = -1;
                    break;
                case 27:
                    dayAdd = -2;
                    break;
                case 28:
                    dayAdd = -3;
                    break;
                case 29:
                    dayAdd = -4;
                    break;
                case 30:
                    dayAdd = -5;
                    break;
                case 31:
                    dayAdd = -6;
                    break;
            }

            DateTime dateFrom = DateTime.Now.AddDays(dayAdd);
            return dateFrom.ToString("yyyy-MM-dd", _dateProvider);
        }

        public override bool CheckEnddaySession()
        {
            return base.CheckEnddaySession();
        }

        public override Document CheckApproveDocument()
        {
            string dateFrom = SundayDate();

            string strSql = " SELECT * FROM document \n" +
                " WHERE ShopID=" + base._shopId + " AND DocumentTypeID=" + base._documentTypeId + "\n" +
                " AND DocumentDate BETWEEN '" + dateFrom + "' AND \n" +
                " '" + DateTime.Now.ToString("yyyy-MM-dd", base._dateProvider) + "' \n" +
                " AND DocumentStatus=1 \n" +
                " ORDER BY DocumentDate DESC LIMIT 1";

            MySqlCommand cmd = new MySqlCommand(strSql, _conn);

            Document doc = new Document();

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    doc.DocumentId = reader.GetInt32("DocumentID");
                    doc.DocumentDate = reader.GetDateTime("DocumentDate");
                    doc.DocumentStatus = reader.GetInt32("DocumentStatus");
                }
                else
                {
                    reader.Close();

                    strSql = " SELECT * FROM document \n" +
                " WHERE ShopID=" + base._shopId + " AND DocumentTypeID=" + base._documentTypeId + "\n" +
                " AND DocumentDate BETWEEN '" + dateFrom + "' AND \n" +
                " '" + DateTime.Now.ToString("yyyy-MM-dd", base._dateProvider) + "' \n" +
                " AND DocumentStatus=2 \n" +
                " ORDER BY DocumentDate DESC LIMIT 1";

                    cmd = new MySqlCommand(strSql, base._conn);

                    using (MySqlDataReader reader2 = cmd.ExecuteReader())
                    {
                        if (reader2.Read())
                        {
                            doc.DocumentId = reader2.GetInt32("DocumentID");
                            doc.DocumentDate = reader2.GetDateTime("DocumentDate");
                            doc.DocumentStatus = reader2.GetInt32("DocumentStatus");
                        }
                    }
                }
            }

            return doc;
        }

    }
}
