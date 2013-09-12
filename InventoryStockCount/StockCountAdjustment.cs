using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Globalization;
using POSMySQL.POSControl;

namespace InventoryStockCount
{
    public class StockCountAdjustment : StockCountDaily
    {
        string refResultText = "";
        IFormatProvider dateProvider = System.Globalization.CultureInfo.InvariantCulture;
        public StockCountAdjustment(int shopId)
        {
            ShopId = shopId;
            DocumentTypeId = 57;
            DocumentTypeIdStr = "58,59";
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

            string sql = @" SELECT * FROM programpropertyvalue " +
            @" WHERE ProgramTypeID=3 AND PropertyID=7 AND PropertyValue=" + day;
            
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            if (reader.Read())
            {
                reader.Close();
                return false;
            }
            else
            {
            
            }
            return true;
        }

        public virtual bool CheckDailyDocIsApprove(CDBUtil dbUtil, MySqlConnection conn)
        {
            string sql = " SELECT DocumentDate, DocumentID FROM document WHERE documenttypeid=" + this.DocumentTypeId + " AND shopid=" + ShopId +
                         " AND DocumentStatus=1 AND DocumentDate='" + DateTime.Now.ToString("yyyy'-'MM'-'dd", dateProvider) + "'";
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
                sql = " SELECT DocumentDate, DocumentID FROM document WHERE documenttypeid=" + this.DocumentTypeId + " AND shopid=" + ShopId +
                         " AND DocumentStatus=2 AND DocumentDate='" + DateTime.Now.ToString("yyyy'-'MM'-'dd", dateProvider) + "'";
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
                adjustDocument.CreateAdjustAdjustmentDocument(dbUtil, conn, this.DocumentId, materialList, remark, shopId, staffId);
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
                     " WHERE DocumentID=" + this.DocumentId + " AND ShopID=" + shopId + " AND DocumentTypeID=" + this.DocumentTypeId, conn);
                dbUtil.sqlExecute("DELETE FROM docdetailtemp WHERE DocumentID=" + this.DocumentId + " AND ShopID=" + shopId, conn);
                return true;
            }
            return false;
        }

        public virtual bool SaveCountData(CDBUtil dbUtil, MySqlConnection conn, int staffId, List<Material> materialList)
        {
            // Create document if no document
            if (materialList.Count > 0)
            {
                // product data
                Product product = new Product();
                Material material = new Material();
                material.DocumentTypeID = DocumentTypeId;
                material.DocumentTypeIdStr = DocumentTypeIdStr;

                // document data
                Document document = new Document();
                document.ShopId = ShopId;
                document.DocumentTypeId = this.DocumentTypeId;
                document.DocumentYear = DateTime.Now.Year;
                document.DocumentMonth = DateTime.Now.Month;
                document.GetLastDocumentNumber(dbUtil, conn);
                document.DocumentDate = DateTime.Now;
                document.InputBy = staffId;
                document.UpdateBy = staffId;
                document.Remark = this.Remark.Replace("'", string.Empty).Replace("/", string.Empty).Replace("\\", string.Empty);
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

                    //for (int i = 0; i <= materialList.Count - 1; i++)
                    //{
                    //    if (!POSInventoryPOROModule.POSInventoryPOROModule.StockCount_AddDocDetail(dbUtil, conn, document.DocumentId, this.ShopId,
                    //        materialList[i].MaterialId, materialList[i].MaterialCurrAmount, materialList[i].MaterialAmount, materialList[i].MaterialUnitLargeId, true, ref refResultText))
                    //        return false;
                    //}
                    ////MaterialList = material.MaterialAdjustmentList(dbUtil, conn, ShopId, document.DocumentId);
                    //return true;
                }
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
                            " AND {d '" + dateTo.ToString("yyyy'-'MM'-'dd", dateProvider) + "'} AND a.DocumentTypeID IN(58,59) AND b.ProductID= " + materialID +
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
