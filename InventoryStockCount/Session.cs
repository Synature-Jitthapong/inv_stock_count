using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventoryStockCount
{
    public class Session
    {
        private DateTime sessionDate;

        public DateTime SessionDate
        {
            get { return sessionDate; }
            set { sessionDate = value; }
        }
        private int isEndday;

        public int IsEndday
        {
            get { return isEndday; }
            set { isEndday = value; }
        }

        public Session()
        {
            sessionDate = DateTime.Now;
        }
 
        public void getSessionData(MySql.Data.MySqlClient.MySqlConnection conn, int productLevelId)
        {
            String strSql = "SELECT SessionDate FROM sessionenddaydetail "
                + " WHERE ProductLevelID=" + productLevelId + " ORDER BY SessionDate DESC LIMIT 1";

            MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(strSql, conn);
            MySql.Data.MySqlClient.MySqlDataReader reader =
                cmd.ExecuteReader();

            if (reader.Read())
            {
                try
                {
                    isEndday = 0;
                    sessionDate = reader.GetDateTime(0);
                }
                catch
                {
                    isEndday = 1;
                }
            }
            reader.Close();
        }
    }
}
