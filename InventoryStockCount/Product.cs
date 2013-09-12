using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using POSMySQL.POSControl;

namespace InventoryStockCount
{
    [Serializable]
    public class Product
    {
        private int _productId;
        private string _productCode;
        private string _productName;
        private int _vatType;
        private decimal _vatRate;
        private decimal _productPrice;
        private string _productUnitName;

        public Product()
        {

        }

        public int ProductId
        {
            get { return _productId; }
            set { _productId = value; }
        }


        public string ProductCode
        {
            get { return _productCode; }
            set { _productCode = value; }
        }


        public string ProductName
        {
            get { return _productName; }
            set { _productName = value; }
        }

        public int VatType
        {
            get { return _vatType; }
            set { _vatType = value; }
        }


        public decimal VatRate
        {
            get { return _vatRate; }
            set { _vatRate = value; }
        }


        public decimal ProductPrice
        {
            get { return _productPrice; }
            set { _productPrice = value; }
        }

        public string ProductUnitName
        {
            get { return _productUnitName; }
            set { _productUnitName = value; }
        }

        public void ProductData(CDBUtil dbUtil, MySqlConnection conn, int productId)
        {
            string sql = " SELECT a.*, b.* FROM products a " +
                         " JOIN  productprice b " +
                         " ON a.ProductID=b.ProductID " +
                         " WHERE b.MainPrice=1 AND a.ProductID=" + productId +
                         " AND a.Deleted=0";
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            while (reader.Read())
            {
                if (reader["ProductCode"] != DBNull.Value)
                    ProductCode = reader["ProductCode"].ToString();
                if (reader["ProductName"] != DBNull.Value)
                    ProductName = reader["ProductName"].ToString();
                if (reader["ProductVATRate"] != DBNull.Value)
                    VatRate = Convert.ToInt32(reader["ProductVATRate"]);
                if (reader["VATType"] != DBNull.Value)
                    VatType = Convert.ToInt32(reader["VATType"]);
                if (reader["ProductPrice"] != DBNull.Value)
                    ProductPrice = Convert.ToDecimal(reader["ProductPrice"]);
                if (reader["ProductUnitName"] != DBNull.Value)
                    ProductUnitName = reader["ProductUnitName"].ToString();
            }
            reader.Close();
        }

        public virtual void GetProductIDFromBarCode(CDBUtil dbUtil, MySqlConnection conn, string productCode)
        {
            string sqlBarCode = " SELECT pb.ProductID, pb.ProductBarCode FROM productbarcode pb "
                              + " INNER JOIN products p "
                              + " ON(p.ProductID = pb.ProductID) "
                              + " WHERE pb.ProductBarCode = '" + productCode.Trim() + "'";

            string sqlProductCode = " SELECT ProductID, ProductCode FROM products WHERE ProductCode = '" + productCode.Trim() + "'";

            MySqlDataReader reader = dbUtil.sqlRetrive(sqlBarCode, conn);

            if (reader.Read())
            {
                if (reader["ProductID"] != DBNull.Value)
                {
                    ProductId = Convert.ToInt32(reader["ProductID"]);
                }
            }
            else
            {
                MySqlDataReader reader2 = dbUtil.sqlRetrive(sqlProductCode, conn);
                if (reader2.Read())
                {
                    if (reader2["ProductID"] != DBNull.Value)
                    {
                        ProductId = Convert.ToInt32(reader2["ProductID"]);
                    }
                }
                else
                {
                    ProductId = 0;
                }
            }
            reader.Close();
        }
    }
}
