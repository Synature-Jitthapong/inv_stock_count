using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POSMySQL.POSControl;
using MySql.Data.MySqlClient;

namespace InventoryStockCount
{
    public class ProductLevel
    {
        int _productLevelId;
        string _productLevelCode;
        string _productLevelName;
        DateTime _startShopDate;
        DateTime _endShopDate;
        string _ipAddress;
        string _databaseName;

        public string DatabaseName
        {
            get { return _databaseName; }
            set { _databaseName = value; }
        }

        public string IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; }
        }

        public int ProductLevelId
        {
            get { return _productLevelId; }
            set { _productLevelId = value; }
        }

        public string ProductLevelCode
        {
            get { return _productLevelCode; }
            set { _productLevelCode = value; }
        }

        public string ProductLevelName
        {
            get { return _productLevelName; }
            set { _productLevelName = value; }
        }
        public DateTime StartShopDate
        {
            get { return _startShopDate; }
            set { _startShopDate = value; }
        }

        public DateTime EndShopDate
        {
            get { return _endShopDate; }
            set { _endShopDate = value; }
        }

        public ProductLevel()
        {

        }

        public ProductLevel GetProductLevel(MySqlConnection conn, int dbShopId)
        {
            ProductLevel productLevel = new ProductLevel();
            MySqlCommand cmd = new MySqlCommand("select distinct inventoryid,inventoryname from inventoryview where inventoryid=" + dbShopId, conn);

            MySqlDataReader reader = cmd.ExecuteReader();
            if(reader.Read())
            {
                productLevel.ProductLevelId     = reader.GetInt32("InventoryID");
                productLevel.ProductLevelName   = reader.GetString("InventoryName");
            }
            reader.Close();
            return productLevel;
        }

        public void ProductLevelInfo(MySqlConnection conn, int productLevelId)
        {
            string sql = "select * from productlevel where ProductLevelID=" + productLevelId + " AND deleted=0";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                if (reader["ProductLevelID"] != null)
                    ProductLevelId = reader.GetInt32("ProductLevelID");
                if (reader["ProductLevelCode"] != null)
                    ProductLevelCode = reader.GetString("ProductLevelCode");
                if (reader["ProductLevelName"] != null)
                    ProductLevelName = reader.GetString("ProductLevelName");
                if (reader["IPAddress"] != null)
                    IpAddress = reader.GetString("IPAddress");
                if (reader["DatabaseName"] != null)
                    DatabaseName = reader.GetString("DatabaseName");
            }
            reader.Close();
        }

        public List<ProductLevel> ListInv(CDBUtil dbUtil, MySqlConnection conn)
        {
            string sql = "";
            sql = " SELECT * FROM ProductLevel WHERE Deleted = 0 AND IsInv=1 ";
            MySqlDataReader reader;
            reader = dbUtil.sqlRetrive(sql, conn);
            List<ProductLevel> invList = new List<ProductLevel>();
            while (reader.Read())
            {
                ProductLevel invInfo = new ProductLevel();
                if (reader["ProductLevelID"] != DBNull.Value)
                    invInfo.ProductLevelId = reader.GetInt32("ProductLevelID");
                if (reader["ProductLevelCode"] != DBNull.Value)
                    invInfo.ProductLevelCode = reader.GetString("ProductLevelCode");
                if (reader["ProductLevelName"] != DBNull.Value)
                    invInfo.ProductLevelName = reader.GetString("ProductLevelName");
                invList.Add(invInfo);
            }
            reader.Close();
            return invList;
        }

        public List<ProductLevel> ListInv(CDBUtil dbUtil, MySqlConnection conn, bool isStockCountConfig)
        {
            string sql = "";
            sql = " SELECT * FROM ProductLevel WHERE Deleted = 0 ";
            MySqlDataReader reader;
            reader = dbUtil.sqlRetrive(sql, conn);
            List<ProductLevel> invList = new List<ProductLevel>();
            while (reader.Read())
            {
                ProductLevel invInfo = new ProductLevel();
                if (reader["ProductLevelID"] != DBNull.Value)
                    invInfo.ProductLevelId = reader.GetInt32("ProductLevelID");
                if (reader["ProductLevelCode"] != DBNull.Value)
                    invInfo.ProductLevelCode = reader.GetString("ProductLevelCode");
                if (reader["ProductLevelName"] != DBNull.Value)
                    invInfo.ProductLevelName = reader.GetString("ProductLevelName");
                invList.Add(invInfo);
            }
            reader.Close();
            return invList;
        }

        public List<ProductLevel> ListInv(CDBUtil dbUtil, MySqlConnection conn, int staffRoleId)
        {
            bool isHQ = false;
            string sql = "";

            // Check shop is hq
            sql = " SELECT * FROM property WHERE HeadOrBranch=1";
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            if (reader.Read())
            {
                isHQ = true;
            }
            else
            {
                isHQ = false;
            }
            reader.Close();

            sql = "";
            if (isHQ)
            {
                sql = " SELECT * FROM ProductLevel WHERE Deleted = 0 AND IsInv=1 ";
            }
            else
            {
                sql = " SELECT * FROM ProductLevel WHERE Deleted = 0 AND IsInv=1 AND ProductLevelID <> 1  ";
            }

            System.Data.DataTable dtResult = dbUtil.List("SELECT * FROM staffroleviewinventory WHERE StaffRoleID=" + staffRoleId, conn);
            if (dtResult.Rows.Count > 0)
            {
                sql += " AND ProductLevelID IN ( ";
                for (int i = 0; i <= dtResult.Rows.Count - 1; i++)
                {
                    sql += dtResult.Rows[i]["ProductLevelID"].ToString();
                    if (i < dtResult.Rows.Count - 1)
                        sql += " , ";
                }
                sql += " ) ";
            }
            reader = dbUtil.sqlRetrive(sql, conn);
            List<ProductLevel> invList = new List<ProductLevel>();
            while (reader.Read())
            {
                ProductLevel invInfo = new ProductLevel();
                if (reader["ProductLevelID"] != DBNull.Value)
                    invInfo.ProductLevelId = reader.GetInt32("ProductLevelID");
                if (reader["ProductLevelCode"] != DBNull.Value)
                    invInfo.ProductLevelCode = reader.GetString("ProductLevelCode");
                if (reader["ProductLevelName"] != DBNull.Value)
                    invInfo.ProductLevelName = reader.GetString("ProductLevelName");
                invList.Add(invInfo);
            }
            reader.Close();
            return invList;
        }

    }
}
