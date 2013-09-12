using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace InventoryStockCount
{
    public class StockCountMonthly : StockCountDaily
    {
        IFormatProvider dateProvider = System.Globalization.CultureInfo.InvariantCulture;
        public  StockCountMonthly(int shopId)
        {
            ShopId = shopId;
            DocumentTypeId = 7;
            DocumentTypeIdStr = "22,23";
        }
        public StockCountMonthly(int shopId, DateTime documentDate)
        {
            ShopId = shopId;
            DocumentTypeId = 7;
            DocumentTypeIdStr = "22,23";
            DocumentDate = documentDate;
        }

        public override bool CheckTransferDocument(POSMySQL.POSControl.CDBUtil dbUtil, MySql.Data.MySqlClient.MySqlConnection conn, int inv, ref DateTime lastTransferDate)
        {
            return base.CheckTransferDocument(dbUtil, conn, inv, ref lastTransferDate);
        }

        public override bool GetLastCountDate(POSMySQL.POSControl.CDBUtil dbUtil, MySql.Data.MySqlClient.MySqlConnection conn, int invId, ref DateTime lastCountDate)
        {
            return base.GetLastCountDate(dbUtil, conn, invId, ref lastCountDate);
        }

        public override bool SaveCountData(POSMySQL.POSControl.CDBUtil dbUtil, MySql.Data.MySqlClient.MySqlConnection conn, int staffId, List<Material> materialList)
        {
            //this.DocumentId = base.DocumentId;
            return base.SaveCountData(dbUtil, conn, staffId, materialList);
        }

        public List<Document> checkReceiptDocument(MySqlConnection conn, 
            int year, int month, int langId)
        {
            bool found = false;
            String strSql = "SELECT a.documentid, a.documenttypeid, b.documenttypename " +
                            " FROM document a " +
                            " LEFT OUTER JOIN documenttype b " +
                            " ON a.DocumentTypeID = b.DocumentTypeID " +
                            " AND a.shopid=b.ShopID " +
                            " WHERE a.ShopID=" + this.ShopId + " AND a.DocumentTypeID IN (2, 3, 25, 39) " +
                            " AND a.documentstatus=1 AND a.documentyear = " + year +
                            " AND a.documentmonth=" + month + " AND b.LangID=" + langId +
                            " GROUP BY a.DocumentTypeID ";
            MySqlCommand cmd = new MySqlCommand(strSql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            List<Document> docList = new List<Document>();
            while(reader.Read())
            {
                Document doc = new Document();
                doc.FoundDocument       = 1;
                doc.DocumentId          = reader.GetInt32("DocumentID");
                doc.DocumentTypeHeader  = reader.GetString("DocumentTypeName");
                docList.Add(doc);
            }
            reader.Close();
            return docList;
        }

        public void UpdateApproveByStockDocument(MySqlConnection conn, int docYear, int docMonth, int staffId)
        {
            String strSql = " UPDATE Document SET ApproveBy=" + staffId + 
                ", ApproveDate='" + DateTime.Now.ToString("yyyy-MM-dd", dateProvider) + "' " +                
                " WHERE DocumentTypeID=7 AND DocumentYear=" + docYear + " AND DocumentMonth=" + docMonth +
                " AND ShopID=" + ShopId;

            MySqlCommand cmd = new MySqlCommand(strSql, conn);
            cmd.ExecuteNonQuery();
        }

        public virtual bool ApproveStockCount(POSMySQL.POSControl.CDBUtil dbUtil, MySql.Data.MySqlClient.MySqlConnection conn, int shopId, int staffId, string remark, List<Material> materialList)
        {
            if (materialList.Count > 0)
            {
                // Gen Diff Doc
                AdjustDocument adjustDocument = new AdjustDocument(this.DocumentDate);
                adjustDocument.CreateAdjustMonthlyDocument(dbUtil, conn, this.DocumentId, materialList, remark, shopId, staffId);
            }

            dbUtil.sqlExecute(" DELETE FROM docdetail WHERE DocumentID=" + this.DocumentId + " AND ShopID=" + shopId, conn);
            string sql = " INSERT INTO docdetail (DocDetailID, DocumentID, ShopID, ProductID, ProductUnit, ProductAmount, " +
                          " ProductTaxType, UnitName, UnitSmallAmount, UnitID) " +
                          " SELECT DocDetailID, DocumentID, ShopID, ProductID, ProductUnit, ProductAmount, " +
                          " ProductTaxType, UnitName, UnitSmallAmount, UnitID " +
                          " FROM docdetailtemp WHERE DocumentID=" + this.DocumentId + " AND ShopID=" + shopId + " AND DocumentTypeID= " + this.DocumentTypeId;
            if (dbUtil.sqlExecute(sql, conn) > 0)
            {
                dbUtil.sqlExecute("UPDATE document SET DocumentStatus=2, UpdateBy=" + staffId + ", ApproveBy=" + staffId + ", " +
                     " UpdateDate='" + DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss", dateProvider) + "', " +
                     " ApproveDate='" + DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss", dateProvider) + "'" +
                     " WHERE DocumentID=" + this.DocumentId + " AND ShopID=" + shopId + " AND DocumentTypeID=" + this.DocumentTypeId, conn);
                dbUtil.sqlExecute("DELETE FROM docdetailtemp WHERE DocumentID=" + this.DocumentId + " AND ShopID=" + shopId + " AND DocumentTypeID=" + this.DocumentTypeId, conn);
                return true;
            }
            return false;
        }

        public bool CheckMonthlyDocIsApprove(POSMySQL.POSControl.CDBUtil dbUtil, MySql.Data.MySqlClient.MySqlConnection conn)
        {
            this.DocumentTypeId = 7;
            string sql = " SELECT DocumentDate, DocumentID, DocumentStatus FROM document WHERE documenttypeid=" + this.DocumentTypeId +
                         " AND shopid=" + this.ShopId +
                         " AND DocumentStatus=1 AND DocumentDate = '" +
                         this.DocumentDate.ToString("yyyy-MM-dd", dateProvider) + "'";
            MySql.Data.MySqlClient.MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            if (reader.Read())
            {
                this.DocumentDate = reader.GetDateTime("DocumentDate");
                this.DocumentId = reader.GetInt32("DocumentID");
                this.DocumentStatus = reader.GetInt32("DocumentStatus");
                reader.Close();
                return false; // Approved
            }
            else
            {
                sql = " SELECT DocumentDate, DocumentID, DocumentStatus FROM document WHERE documenttypeid=" + this.DocumentTypeId +
                      " AND shopid=" + this.ShopId +
                      " AND DocumentStatus=2 AND DocumentDate = '" +
                      this.DocumentDate.ToString("yyyy-MM-dd", dateProvider) + "' ";
                reader.Close();
                reader = dbUtil.sqlRetrive(sql, conn);
                if (reader.Read())
                {
                    this.DocumentDate = reader.GetDateTime("DocumentDate");
                    this.DocumentId = reader.GetInt32("DocumentID");
                    this.DocumentStatus = reader.GetInt32("DocumentStatus");
                    reader.Close();
                    return true; // Not Approve
                }
                else
                {
                    reader.Close();
                }
            }
            return false;
        }

        // overload 2/9/2011 fix load history in new month
        public bool CheckMonthlyDocIsApprove(POSMySQL.POSControl.CDBUtil dbUtil, MySql.Data.MySqlClient.MySqlConnection conn,
            DateTime dateFrom, DateTime dateTo)
        {
            this.DocumentTypeId = 7;
            string sql = " SELECT DocumentDate, DocumentID FROM document WHERE documenttypeid=" + this.DocumentTypeId + " AND shopid=" + this.ShopId +
                         " AND DocumentStatus=1 AND DocumentDate BETWEEN '" + dateFrom.ToString("yyyy-MM-dd", dateProvider) + "' AND '" + dateTo.ToString("yyyy-MM-dd", dateProvider) + "'";
            MySql.Data.MySqlClient.MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            if (reader.Read())
            {
                this.DocumentDate = reader.GetDateTime("DocumentDate");
                this.DocumentId = reader.GetInt32("DocumentID");
                reader.Close();
                return false; // Approved
            }
            else
            {
                sql = " SELECT DocumentDate, DocumentID FROM document WHERE documenttypeid=" + this.DocumentTypeId + " AND shopid=" + this.ShopId +
                         " AND DocumentStatus=2 AND DocumentDate BETWEEN '" + dateFrom.ToString("yyyy-MM-dd", dateProvider) + "' AND '" + dateTo.ToString("yyyy-MM-dd", dateProvider) + "'";
                reader.Close();
                reader = dbUtil.sqlRetrive(sql, conn);
                if (reader.Read())
                {
                    this.DocumentDate = reader.GetDateTime("DocumentDate");
                    this.DocumentId = reader.GetInt32("DocumentID");
                    reader.Close();
                    return true; // Not Approve
                }
                else
                {
                    reader.Close();
                }
            }
            return false;
        }
    }
}
