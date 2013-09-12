using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace InventoryStockCount
{
    public class IdRange
    {
        private int productLevelId;
        private int idRnageTypeId;
        private int minId;
        private int maxId;
        private DateTime updateDate;

        public IdRange(int shopId, int idRangeTypeId)
        {
            productLevelId = shopId;
            this.idRnageTypeId = idRangeTypeId;
        }

        public void getIdRange(MySqlConnection conn){
            string sql = "SELECT MinID, MaxID FROM IDRange WHERE ProductLevelID=" + productLevelId 
                + " AND IDRangeTypeID=" + idRnageTypeId;

            MySqlCommand cmd = new MySqlCommand(sql, conn);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    minId = reader.GetInt32(0);
                    maxId = reader.GetInt32(1);
                }
            }
        }

        public int IdRnageTypeId
        {
            get { return idRnageTypeId; }
            set { idRnageTypeId = value; }
        }

        public int ProductLevelId
        {
            get { return productLevelId; }
            set { productLevelId = value; }
        }
        public int MinId
        {
            get { return minId; }
            set { minId = value; }
        }
        public int MaxId
        {
            get { return maxId; }
            set { maxId = value; }
        }
        public DateTime UpdateDate
        {
            get { return updateDate; }
            set { updateDate = value; }
        }
    }
}
