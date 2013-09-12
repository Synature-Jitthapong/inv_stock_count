using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace InventoryStockCount
{
    public abstract class StockCountBase
    {
        public bool IS_NEWSESSION = false;
        protected MySqlConnection _conn;
        protected IFormatProvider _dateProvider = System.Globalization.CultureInfo.InvariantCulture;
        protected int _shopId;
        protected int _documentTypeId;
        protected DateTime documentDate;

        public virtual int WeeklyCountDay()
        {
            int dayOfWeek = 0; // sunday
            string sql = " SELECT * FROM programpropertyvalue \n" +
                        " WHERE ProgramTypeID=3 AND PropertyID=7";

            MySqlCommand cmd = new MySqlCommand(sql, _conn);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    dayOfWeek = reader.GetInt32("PropertyValue");
                }
            }

            return dayOfWeek;
        }

        /// <summary>Find Sunday date for create firstdate to query stockcountweekly checked approve document </summary>
        public virtual string SundayDate()
        {
            int weeklyCountDay = 0;
            weeklyCountDay = WeeklyCountDay();

            int dayAdd = 0;
            switch (weeklyCountDay)
            {
                case 0:
                    dayAdd = 0;
                    break;

                case 1:
                    dayAdd = -1;
                    break;

                case 2:
                    dayAdd = -2;
                    break;

                case 3:
                    dayAdd = -3;
                    break;

                case 4:
                    dayAdd = -4;
                    break;

                case 5:
                    dayAdd = -5;
                    break;

                case 6:
                    dayAdd = -6;
                    break;
            }

            DateTime dateFrom = DateTime.Now.AddDays(dayAdd);
            return dateFrom.ToString("yyyy-MM-dd", _dateProvider);
        }

        public virtual bool CheckEnddaySession(DateTime documentDate)
        {
            bool isEndday = false;

            string strSql = " SELECT * FROM sessionenddaydetail WHERE \n" +
                 " SessionDate > '" + documentDate.ToString("yyyy-MM-dd", _dateProvider) + "' \n" +
                 " AND ProductLevelID=" + _shopId;

            MySqlCommand cmd = new MySqlCommand(strSql, _conn);
            using(MySqlDataReader reader1 = cmd.ExecuteReader())
            {
                if(reader1.Read())
                {
                    IS_NEWSESSION = true;
                    isEndday = false;
                }else
                {
                    reader1.Close();
                    strSql = " SELECT * FROM sessionenddaydetail WHERE \n" +
                         " SessionDate='" + documentDate.ToString("yyyy-MM-dd", _dateProvider) + "' \n" +
                         " AND ProductLevelID=" + _shopId + " AND IsEndDay=1";

                    cmd = new MySqlCommand(strSql, _conn);
                    using (MySqlDataReader reader2 = cmd.ExecuteReader())
                    {
                        if (reader2.Read())
                        {
                            isEndday = true;
                        }
                    }
                }
            }
            return isEndday;
        }

        public virtual bool CheckEnddaySession()
        {
            bool isEndday = false;
            string strSql = " SELECT * FROM sessionenddaydetail WHERE \n" +
                " SessionDate='" + DateTime.Now.ToString("yyyy-MM-dd", _dateProvider) + "' \n" +
                " AND ProductLevelID=" + _shopId + " AND IsEndDay=1";

            MySqlCommand cmd = new MySqlCommand(strSql, _conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    isEndday = true;
                }
            }
            return isEndday;
        }

        public abstract Document CheckApproveDocument();

        //public virtual bool ApproveStockCount(int documentId, int shopId, int staffId, 
        //    string remark, List<Material> materialList)
        //{
        //    if (materialList.Count > 0)
        //    {
        //        // Gen Diff Doc
        //        //AdjustDocument adjustDocument = new AdjustDocument();
        //        //adjustDocument.CreateAdjustWeeklyDocument(dbUtil, conn, this.DocumentId, materialList, remark, shopId, staffId);
        //    }

        //    MySqlTransaction transaction;
        //    MySqlCommand cmd;

        //    string strSql = " DELETE FROM docdetail \n" +
        //        " WHERE DocumentID=" + documentId + " AND ShopID=" + shopId;

        //    cmd = new MySqlCommand(strSql, _conn);
        //    cmd.ExecuteNonQuery();


        //    strSql = " INSERT INTO docdetail (\n" +
        //        " DocDetailID, DocumentID, ShopID, ProductID, \n " +
        //        " ProductUnit, ProductAmount, ProductDiscount, \n" +
        //        " ProductTaxType, UnitName, UnitSmallAmount, UnitID) \n" +
        //        " SELECT \n " +
        //        " DocDetailID, DocumentID, ShopID, ProductID, \n" + 
        //        " ProductUnit, ProductAmount, ProductDiscount, \n" +
        //        " ProductTaxType, UnitName, UnitSmallAmount, UnitID \n" +
        //        " FROM docdetailtemp WHERE DocumentID=" + documentId + " AND ShopID=" + shopId;

        //    cmd = new MySqlCommand(strSql, _conn);
        //    transaction = _conn.BeginTransaction();
        //    cmd.Transaction = transaction;

        //    try{
        //        cmd.ExecuteNonQuery();
        //        transaction.Commit();

        //        strSql = " UPDATE document SET DocumentStatus=2, \n" +
        //            " UpdateBy=" + staffId + ", ApproveBy=" + staffId + ", \n" +
        //            " UpdateDate='" + DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss", dateProvider) + "', \n" +
        //            " ApproveDate='" + DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss", dateProvider) + "', \n" +
        //            " remark='" + remark + "' \n" +
        //            " WHERE DocumentID=" + documentId + " AND ShopID=" + shopId + "\n" + 
        //            " AND DocumentTypeID=" + _documentTypeId;

        //        cmd = new MySqlCommand(strSql, _conn);
        //        cmd.ExecuteNonQuery();

        //        strSql = "DELETE FROM docdetailtemp WHERE DocumentID=" + documentId + " AND ShopID=" + shopId;

        //        cmd = new MySqlCommand(strSql, _conn);
        //        cmd.ExecuteNonQuery();

        //        return true;

        //    }catch{
        //        transaction.Rollback();
        //    }
        //    return false;
        //}

        //public virtual bool SaveCountData(CDBUtil dbUtil, MySqlConnection conn, int staffId, List<Material> materialList)
        //{
        //    // Create document if no document
        //    if (materialList.Count > 0)
        //    {
        //        // product data
        //        Product product = new Product();
        //        Material material = new Material();
        //        material.DocumentTypeID = DocumentTypeId;
        //        material.DocumentTypeIdStr = DocumentTypeIdStr;

        //        // document data
        //        Document document = new Document();
        //        document.ShopId = ShopId;
        //        document.DocumentTypeId = DocumentTypeId;
        //        document.DocumentYear = DateTime.Now.Year;
        //        document.DocumentMonth = DateTime.Now.Month;
        //        document.GetLastDocumentNumber(dbUtil, conn);
        //        document.DocumentDate = DateTime.Now;
        //        document.Remark = this.Remark.Replace("'", string.Empty).Replace("/", string.Empty).Replace("\\", string.Empty);
        //        document.InputBy = staffId;
        //        document.UpdateBy = staffId;
        //        document.DocumentStatus = 1;
        //        document.ProductLevelId = ShopId;
        //        document.InsertDate = DateTime.Now;
        //        document.GetLastDocumentId(dbUtil, conn);

        //        try
        //        {
        //            if (document.AddDocument(dbUtil, conn))
        //            {
        //                for (int i = 0; i <= materialList.Count - 1; i++)
        //                {
        //                    if (!POSInventoryPOROModule.POSInventoryPOROModule.StockCount_AddDocDetail(dbUtil, conn, document.DocumentId, this.ShopId,
        //                        materialList[i].MaterialId, materialList[i].MaterialCurrAmount, materialList[i].MaterialAmount, materialList[i].MaterialUnitLargeId, true, ref refResultText))
        //                        return false;
        //                }
        //                //MaterialList = material.MaterialWeeklyList(dbUtil, conn, ShopId, document.DocumentId);
        //                return true;
        //            }
        //        }
        //        catch
        //        {

        //        }
        //    }
        //    return false;
        //}
    }
}
