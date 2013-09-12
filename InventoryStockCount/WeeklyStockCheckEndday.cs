using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace InventoryStockCount
{
    public class WeeklyStockCheckEndday : StockCountBase
    {

        public WeeklyStockCheckEndday(MySqlConnection conn, int shopId, int documentTypeId)
        {
            _conn = conn;
            _shopId = shopId;
            _documentTypeId = documentTypeId;
            Session s = new Session();
            s.getSessionData(conn, shopId);
            documentDate = s.SessionDate;
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
                        else
                        {
                            doc.DocumentDate = documentDate;
                        }
                    }
                }
            }

            return doc;
        }

        public override bool CheckEnddaySession()
        {
            return base.CheckEnddaySession();
        }

        public override int WeeklyCountDay()
        {
            return base.WeeklyCountDay();
        }
    }
}
