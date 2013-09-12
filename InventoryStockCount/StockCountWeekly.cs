using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using POSMySQL.POSControl;
using System.Globalization;

namespace InventoryStockCount
{
    public class StockCountWeekly : StockCountDaily
    {
        string refResultText = "";
        IFormatProvider dateProvider = System.Globalization.CultureInfo.InvariantCulture;

        public StockCountWeekly(MySql.Data.MySqlClient.MySqlConnection conn, int shopId)
        {
            Session s = new Session();
            s.getSessionData(conn, shopId);
            ShopId = shopId;
            DocumentTypeId = 30;
            DocumentDate = DateTime.Now;//s.SessionDate;
        }

        public StockCountWeekly(int shopId)
        {
            ShopId = shopId;
            DocumentTypeId = 30;
        }

        public virtual bool CheckDailyStockDocument(MySqlConnection conn)
        {
            bool isFound = false;
            string strSql = "SELECT COUNT(DocumentID) FROM document " +
                " WHERE ShopID=" + this.ShopId + " AND DocumentTypeID=24 " +
                " AND DocumentDate='" + this.DocumentDate.ToString("yyyy-MM-dd", dateProvider) + "' " +
                " AND DocumentStatus=1";
            MySqlCommand cmd = new MySqlCommand(strSql, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    if(reader.GetInt32(0) > 0)
                        isFound = true;
                }
            }
            return isFound;
        }

        public override bool CheckTransferDocument(CDBUtil dbUtil, MySqlConnection conn, int inv, ref DateTime lastTransferDate)
        {
            return base.CheckTransferDocument(dbUtil, conn, inv, ref lastTransferDate);
        }

        public override bool GetLastCountDate(CDBUtil dbUtil, MySqlConnection conn, int invId, ref DateTime lastCountDate)
        {
            return base.GetLastCountDate(dbUtil, conn, invId, ref lastCountDate);
        }

        public override bool CheckCountStockWeeklyDay(CDBUtil dbUtil, MySqlConnection conn)
        {
            // programpropertyvalue programtype 3 = invent, PropertyID 7 = weekly stock count
            DayOfWeek dayOfWeek = (DayOfWeek)DateTime.Now.DayOfWeek;
            int day = 0;
            switch (dayOfWeek.ToString())
            {
                case "Sunday":
                    day = 0;
                    break;
                case "Monday":
                    day = 1;
                    break;
                case "Tuesday":
                    day = 2;
                    break;
                case "Wednesday":
                    day = 3;
                    break;
                case "Thursday":
                    day = 4;
                    break;
                case "Friday":
                    day = 5;
                    break;
                case "Saturday":
                    day = 6;
                    break;
            }

            string sql = " SELECT b.PropertyValue FROM programproperty a, programpropertyvalue b " +
                         " WHERE a.ProgramTypeID=b.ProgramTypeID AND a.PropertyID=b.PropertyID AND " +
                         " a.ProgramTypeID=3 AND a.PropertyID=7 AND b.PropertyValue=" + day;
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            if (reader.Read())
            {
                reader.Close();
                return false;
            }
            else
            {
                reader.Close();
                sql = " SELECT b.PropertyValue FROM programproperty a, programpropertyvalue b " +
                         " WHERE a.ProgramTypeID=b.ProgramTypeID AND a.PropertyID=b.PropertyID AND " +
                         " a.ProgramTypeID=3 AND a.PropertyID=7 ";
                reader = dbUtil.sqlRetrive(sql, conn);
                if (reader.Read())
                {
                    if (reader["PropertyValue"] != DBNull.Value)
                    {
                        if (reader["PropertyValue"].ToString() == "-1")
                        {
                            reader.Close();
                            return false;
                        }
                        else
                            reader.Close();
                    }
                    else
                    {
                        reader.Close();
                    }
                }
                else
                {
                    reader.Close();
                }
            }
            return true;
        }

        public virtual bool CheckDailyDocIsApprove(CDBUtil dbUtil, MySqlConnection conn)
        {
            string sql = " SELECT DocumentDate, DocumentID FROM document WHERE documenttypeid=30 AND shopid=" + ShopId +
                         " AND DocumentStatus=1 AND DocumentDate='" + this.DocumentDate.ToString("yyyy'-'MM'-'dd", dateProvider) + "'";
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            if (reader.Read())
            {
                DocumentDate = reader.GetDateTime("DocumentDate");
                DocumentId = reader.GetInt32("DocumentID");
                reader.Close();
                return false; // Approved
            }
            else
            {
                sql = " SELECT DocumentDate, DocumentID FROM document WHERE documenttypeid=30 AND shopid=" + ShopId +
                         " AND DocumentStatus=2 AND DocumentDate='" + this.DocumentDate.ToString("yyyy'-'MM'-'dd", dateProvider) + "'";
                reader.Close();
                reader = dbUtil.sqlRetrive(sql, conn);
                if (reader.Read())
                {
                    DocumentDate = reader.GetDateTime("DocumentDate");
                    DocumentId = reader.GetInt32("DocumentID");
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

        public virtual bool ApproveStockCount(CDBUtil dbUtil, MySqlConnection conn, int shopId, int staffId, string remark, List<Material> materialList)
        {
            if (materialList.Count > 0)
            {
                // Gen Diff Doc
                AdjustDocument adjustDocument = new AdjustDocument(this.DocumentDate);
                adjustDocument.CreateAdjustWeeklyDocument(dbUtil, conn, this.DocumentId, materialList, remark, shopId, staffId);
            }

            dbUtil.sqlExecute(" DELETE FROM docdetail WHERE DocumentID=" + this.DocumentId + " AND ShopID=" + shopId, conn);
            string sql = " INSERT INTO docdetail (DocDetailID, DocumentID, ShopID, ProductID, ProductUnit, ProductAmount, ProductDiscount," +
                        " ProductTaxType, UnitName, UnitSmallAmount, UnitID) " +
                        " SELECT DocDetailID, DocumentID, ShopID, ProductID, ProductUnit, ProductAmount, ProductDiscount," +
                        " ProductTaxType, UnitName, UnitSmallAmount, UnitID " +
                        " FROM docdetailtemp WHERE DocumentID=" + this.DocumentId + " AND ShopID=" + shopId;
            if (dbUtil.sqlExecute(sql, conn) > 0)
            {
                dbUtil.sqlExecute("UPDATE document SET DocumentStatus=2, ApproveBy=" + staffId + ", " +
                     " UpdateDate='" + DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss", dateProvider) + "', " +
                     " ApproveDate='" + DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss", dateProvider) + "', " +
                     " remark='" + remark + "' " +
                     " WHERE DocumentID=" + this.DocumentId + " AND ShopID=" + shopId + " AND DocumentTypeID=30", conn);
                dbUtil.sqlExecute("DELETE FROM docdetailtemp WHERE DocumentID=" + this.DocumentId + " AND ShopID=" + shopId, conn);
                return true;
            }
            return false;
        }

        public virtual bool SaveCountData(CDBUtil dbUtil, MySqlConnection conn, int staffId, List<Material> materialList)
        {
            // Create document if no document
            // product data
            Product product = new Product();
            Material material = new Material();
            material.DocumentTypeID = DocumentTypeId;
            material.DocumentTypeIdStr = DocumentTypeIdStr;

            // document data
            Document document = new Document();
            document.ShopId = ShopId;
            document.DocumentTypeId = DocumentTypeId;
            document.DocumentYear = DateTime.Now.Year;
            document.DocumentMonth = DateTime.Now.Month;
            document.GetLastDocumentNumber(dbUtil, conn);
            document.DocumentDate = this.DocumentDate;
            if(this.Remark != null)
                document.Remark = this.Remark.Replace("'", string.Empty).Replace("/", string.Empty).Replace("\\", string.Empty);
            document.InputBy = staffId;
            document.UpdateBy = staffId;
            document.DocumentStatus = 1;
            document.ProductLevelId = ShopId;
            document.InsertDate = DateTime.Now;
            document.GetLastDocumentId(dbUtil, conn);

            if (document.AddDocument(dbUtil, conn))
            {
                this.DocumentId = document.DocumentId;
                int[] materialId = new int[materialList.Count];
                decimal[] materialCurrAmount = new decimal[materialList.Count];
                decimal[] materialAmount = new decimal[materialList.Count];
                int[] unitLargeId = new int[materialList.Count];
                for (int i = 0; i <= materialList.Count - 1; i++)
                {
                    materialId[i] = materialList[i].MaterialId;
                    materialCurrAmount[i] = materialList[i].MaterialCurrAmount;
                    materialAmount[i] = materialList[i].MaterialAmount;
                    unitLargeId[i] = materialList[i].MaterialUnitLargeId;
                }

                if (!POSInventoryPOROModule.POSInventoryPOROModule.StockCount_AddDocDetail(dbUtil,
                    conn, document.DocumentId, document.ShopId, materialId, materialCurrAmount,
                    materialAmount, unitLargeId, true, ref refResultText))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public List<Document> GetStockCountDiffByEachMaterial(CDBUtil dbUtil, MySqlConnection conn, DateTime dateFrom,
           DateTime dateTo, int materialID, int invId, int langId)
        {
            List<Document> documentList = new List<Document>();
            string sql = " SELECT a.DocumentID, a.ApproveDate, b.Productid, SUM(b.ProductAmount * c.MovementInStock) AS DiffAmount, " +
                            " c.DocumentTypeHeader, c.DocumentTypeName " +
                            " FROM document a " +
                            " INNER JOIN docdetail b " +
                            " ON a.DocumentID=b.DocumentID AND a.shopid=b.ShopID " +
                            " INNER JOIN documenttype c " +
                            " ON a.DocumentTypeID=c.DocumentTypeID AND a.ShopID=c.ShopID " +
                            " WHERE a.shopid=" + invId + " AND a.DocumentDate BETWEEN {d '" + dateFrom.ToString("yyyy'-'MM'-'dd", dateProvider) + "'}  " +
                            " AND {d '" + dateTo.ToString("yyyy'-'MM'-'dd", dateProvider) + "'} AND a.DocumentTypeID IN(31,32) AND b.ProductID= " + materialID +
                            " AND c.LangID=" + langId + " GROUP BY a.documentdate, b.ProductID, b.ProductAmount ";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Document document = new Document();
                document.DocumentId = reader.GetInt32("DocumentID");
                document.DocumentDate = reader.GetDateTime("ApproveDate");
                document.ProductAmount = reader.GetDecimal("DiffAmount");
                documentList.Add(document);
            }
            reader.Close();
            return documentList;
        }
    }
}
