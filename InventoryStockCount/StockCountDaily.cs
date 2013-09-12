using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using POSMySQL.POSControl;
using POSInventoryPOROModule;
using System.Globalization;

namespace InventoryStockCount
{
    public class StockCountDaily
    {
        private int _shopId;
        private List<Material> _materialList;
        private DateTime _documentDate;
        private int _documentId;
        private int _documentTypeId;
        private string _remark;
        string refResultText;
        private string _documentTypeIdStr;
        private IFormatProvider dateProvider = System.Globalization.CultureInfo.InvariantCulture;
        private int _documentStatus;
        private string _documentStatusStr;

        public string DocumentStatusStr
        {
            get { return _documentStatusStr; }
            set { _documentStatusStr = value; }
        }

        public int DocumentStatus
        {
            get { return _documentStatus; }
            set { _documentStatus = value; }
        }

        public string DocumentTypeIdStr
        {
            get { return _documentTypeIdStr; }
            set { _documentTypeIdStr = value; }
        }

        public string RefResultText
        {
            get { return refResultText; }
            set { refResultText = value; }
        }

        public string Remark
        {
            get { return _remark; }
            set { _remark = value; }
        }


        public int DocumentTypeId
        {
            get { return _documentTypeId; }
            set { _documentTypeId = value; }
        }

        public int DocumentId
        {
            get { return _documentId; }
            set { _documentId = value; }
        }

        public DateTime DocumentDate
        {
            get { return _documentDate; }
            set { _documentDate = value; }
        }

        public List<Material> MaterialList
        {
            get { return _materialList; }
            set { _materialList = value; }
        }

        public int ShopId
        {
            get { return _shopId; }
            set { _shopId = value; }
        }

        // get date from session
        public StockCountDaily()
        {
            _documentTypeId = 24;
            _documentTypeIdStr = "18,19";
            _documentDate = DateTime.Now;
            _documentStatus = 2;
            _documentStatusStr = "1,2";
        }

        public StockCountDaily(int shopId)
        {
            _shopId = shopId;
            _documentTypeId = 24;
            _documentTypeIdStr = "18,19";
            _documentDate = DateTime.Now;
            _documentStatus = 2;
            _documentStatusStr = "1,2";
        }

        public StockCountDaily(MySql.Data.MySqlClient.MySqlConnection conn, int shopId)
        {
            Session s = new Session();
            s.getSessionData(conn, shopId);

            _shopId = shopId;
            _documentTypeId = 24;
            _documentTypeIdStr = "18,19";
            _documentDate = DateTime.Now;//s.SessionDate;
            _documentStatus = 2;
            _documentStatusStr = "1,2";

        }

        public StockCountDaily(DateTime documentDate)
        {
            _documentTypeId = 24;
            _documentTypeIdStr = "18,19";
            _documentDate = documentDate;
            _documentStatus = 2;
            _documentStatusStr = "1,2";
        }

        public bool CheckOvertimeOfThisDocument(MySqlConnection conn, int documentTypeId, DateTime documentDate)
        {
            Boolean found = false;
            MySqlCommand cmd = new MySqlCommand("SELECT COUNT(DocumentID) FROM document " +
                " WHERE DocumentStatus = 1 AND DocumentTypeID = " + documentTypeId + " AND " +
                " DocumentDate > '" + documentDate.ToString("yyyy-MM-dd", dateProvider) + "'", conn);

            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                if (reader.GetInt32(0) > 0)
                    found = true;
            }
            reader.Close();
            return found;
        }
        
        public virtual bool CheckWeeklyStockDocument(MySqlConnection conn)
        {
            bool isFound = false;
            string strSql = "SELECT COUNT(DocumentID) FROM document " +
                " WHERE ShopID=" + this.ShopId + " AND DocumentTypeID=30 " + 
                " AND DocumentDate='" + this.DocumentDate.ToString("yyyy-MM-dd", dateProvider) + "' " +
                " AND DocumentStatus=1";
            MySqlCommand cmd = new MySqlCommand(strSql,conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if(reader.Read())
                {
                    if (reader.GetInt32(0) > 0)
                        isFound = true;
                }
            }
            return isFound;
        }

        public virtual bool CheckTransferDocument(MySqlConnection conn, int inv)
        {
            bool isTransfer = false;
            string sql = "SELECT COUNT(DocumentID) FROM document WHERE ShopID=" + inv + " AND DocumentTypeID=10 " +
                         " AND DocumentStatus=" + DocumentStatus;

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                if(reader.GetInt32(0) > 0)
                    isTransfer = true;
            }
            reader.Close();
            return isTransfer;
        }

        public virtual bool CheckTransferDocument(CDBUtil dbUtil, MySqlConnection conn, int inv, ref DateTime lastTransferDate)
        {
            // หาเอกสารยกยอดของเดือนปัจจุบัน
            bool isTransfer = false;
            string sql = "SELECT * FROM document WHERE ShopID=" + inv + " AND DocumentTypeID=10 " +
                " AND DocumentStatus=" + DocumentStatus + " AND DocumentMonth=" + DateTime.Now.Month + " AND DocumentYear=" + DateTime.Now.ToString("yyyy", dateProvider);

            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            if (reader.Read())
            {
                if (reader["DocumentDate"] != DBNull.Value)
                    lastTransferDate = reader.GetDateTime("DocumentDate");
                if (reader["Remark"] != DBNull.Value)
                    this.Remark = reader.GetString("Remark");
                isTransfer = true;
            }
            reader.Close();

            //ถ้าไม่เจอ หาล่าสุด
            if (isTransfer == false)
            {
                sql = "SELECT * FROM document WHERE ShopID=" + inv + " AND DocumentTypeID=" +
                      " 10 AND DocumentStatus=" + DocumentStatus + " ORDER BY DocumentDate DESC LIMIT 1";

                reader = dbUtil.sqlRetrive(sql, conn);
                if (reader.Read())
                {
                    if (reader["DocumentDate"] != DBNull.Value)
                        lastTransferDate = reader.GetDateTime("DocumentDate");

                    isTransfer = true;
                }
            }
            reader.Close();
            return isTransfer;
        }

        public virtual bool GetLastCountDate(CDBUtil dbUtil, MySqlConnection conn, int invId, ref DateTime lastCountDate)
        {
            bool isCount = false;
            string sql = " SELECT * FROM document WHERE ShopID=" + invId + " AND  DocumentTypeID=" + this.DocumentTypeId +
                " AND DocumentStatus IN (" + DocumentStatusStr + ") ORDER BY DocumentDate DESC LIMIT 1";
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            if (reader.Read())
            {
                if (reader["DocumentDate"] != DBNull.Value)
                    lastCountDate = reader.GetDateTime("DocumentDate");
                if (reader["Remark"] != DBNull.Value)
                    this.Remark = reader.GetString("Remark");
                isCount = true;
            }
            reader.Close();
            return isCount;
        }

        public virtual bool CheckCountStockWeeklyDay(CDBUtil dbUtil, MySqlConnection conn)
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
                return true;
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
            return false;
        }

        public virtual bool CheckDailyDocIsApprove(CDBUtil dbUtil, MySqlConnection conn, int documentId)
        {
            string sql = " SELECT * FROM document WHERE documentId=" + documentId +
                            " AND documenttypeid=" + this.DocumentTypeId + " AND shopid=" + this.ShopId +
                            " AND DocumentStatus=1 ";

            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            if (reader.Read())
            {
                if (reader["DocumentDate"] != DBNull.Value)
                    this.DocumentDate = reader.GetDateTime("DocumentDate");
                if (reader["DocumentID"] != DBNull.Value)
                    this.DocumentId = reader.GetInt32("DocumentID");
                if (reader["Remark"] != DBNull.Value)
                    this.Remark = reader.GetString("Remark");
                reader.Close();
                return false; // Approved
            }
            else
            {
                sql = " SELECT * FROM document WHERE documentId=" + documentId +
                        " AND documenttypeid=" + this.DocumentTypeId + " AND shopid=" + this.ShopId +
                        " AND DocumentStatus=2 ";
                reader.Close();
                reader = dbUtil.sqlRetrive(sql, conn);
                if (reader.Read())
                {
                    if (reader["DocumentDate"] != DBNull.Value)
                        this.DocumentDate = reader.GetDateTime("DocumentDate");
                    if (reader["DocumentID"] != DBNull.Value)
                        this.DocumentId = reader.GetInt32("DocumentID");
                    if (reader["Remark"] != DBNull.Value)
                        this.Remark = reader.GetString("Remark");
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

        // check working document from operationdate 
        public virtual bool GetWorkingStockDocument(CDBUtil dbUtil, MySqlConnection conn, DateTime operationDate)
        {
            Boolean isApprove = true;
            string sql = " SELECT * FROM document " +
                   " WHERE documenttypeid=" + this.DocumentTypeId + " AND shopid=" + this.ShopId +
                   " AND DocumentStatus=1 " +
                   " AND DocumentDate='" + operationDate.ToString("yyyy'-'MM'-'dd", dateProvider) + "'";
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            if (reader.Read())
            {
                if (reader["DocumentDate"] != DBNull.Value)
                    this.DocumentDate = reader.GetDateTime("DocumentDate");
                if (reader["DocumentID"] != DBNull.Value)
                    this.DocumentId = reader.GetInt32("DocumentID");
                if (reader["Remark"] != DBNull.Value)
                    this.Remark = reader.GetString("Remark");
                if (reader["DocumentStatus"] != DBNull.Value)
                    this.DocumentStatus = reader.GetInt32("DocumentStatus");
                isApprove = false;
            }
            reader.Close();
            return isApprove;
        }

        public virtual bool GetWorkingStockDocument(CDBUtil dbUtil, MySqlConnection conn)
        {
            /* ตรวจสอบว่า มีเอกสารการนับที่ยังไม่อนุมัติหรือป่าว โดยเอาตัวล่าสุดเสมอ
             * 
             */
            Boolean isApprove = true;
            string sql = " SELECT * FROM document " +
                   " WHERE documenttypeid=" + this.DocumentTypeId + " AND shopid=" + this.ShopId +
                   " AND DocumentStatus=1 AND DocumentMonth=" + this.DocumentDate.Month + 
                   " AND DocumentYear=" + this.DocumentDate.Year + //" ORDER BY documentdate DESC LIMIT 1";
                   " AND DocumentDate='" + this.DocumentDate.ToString("yyyy'-'MM'-'dd", dateProvider) + "'";
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            if (reader.Read())
            {
                if (reader["DocumentDate"] != DBNull.Value)
                    this.DocumentDate = reader.GetDateTime("DocumentDate");
                if (reader["DocumentID"] != DBNull.Value)
                    this.DocumentId = reader.GetInt32("DocumentID");
                if (reader["Remark"] != DBNull.Value)
                    this.Remark = reader.GetString("Remark");
                if (reader["DocumentStatus"] != DBNull.Value)
                    this.DocumentStatus = reader.GetInt32("DocumentStatus");
                isApprove = false;
            }
            reader.Close();
            return isApprove;
        }

        public virtual bool CheckDailyDocIsApprove(CDBUtil dbUtil, MySqlConnection conn)
        {
            string sql = " SELECT * FROM document WHERE documenttypeid=" + this.DocumentTypeId + " AND shopid=" + this.ShopId +
                         " AND DocumentStatus=1 AND DocumentDate='" + this.DocumentDate.ToString("yyyy'-'MM'-'dd", dateProvider) + "'";
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            if (reader.Read())
            {
                if (reader["DocumentDate"] != DBNull.Value)
                    this.DocumentDate = reader.GetDateTime("DocumentDate");
                if (reader["DocumentID"] != DBNull.Value)
                    this.DocumentId = reader.GetInt32("DocumentID");
                if (reader["Remark"] != DBNull.Value)
                    this.Remark = reader.GetString("Remark");
                if (reader["DocumentStatus"] != DBNull.Value)
                    this.DocumentStatus = reader.GetInt32("DocumentStatus");
                reader.Close();
                return false; // Approved
            }
            else
            {
                sql = " SELECT * FROM document WHERE documenttypeid=" + this.DocumentTypeId + " AND shopid=" + this.ShopId +
                         " AND DocumentStatus=2 AND DocumentDate='" + this.DocumentDate.ToString("yyyy'-'MM'-'dd", dateProvider) + "'";
                reader.Close();
                reader = dbUtil.sqlRetrive(sql, conn);
                if (reader.Read())
                {
                    if (reader["DocumentDate"] != DBNull.Value)
                        this.DocumentDate = reader.GetDateTime("DocumentDate");
                    if (reader["DocumentID"] != DBNull.Value)
                        this.DocumentId = reader.GetInt32("DocumentID");
                    if (reader["Remark"] != DBNull.Value)
                        this.Remark = reader.GetString("Remark");
                    if (reader["DocumentStatus"] != DBNull.Value)
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

        public virtual bool ApproveStockCount(CDBUtil dbUtil, MySqlConnection conn,
            int shopId, int staffId, string remark, List<Material> materialList)
        {
            if (materialList.Count > 0)
            {
                // Gen Diff Doc
                AdjustDocument adjustDocument = new AdjustDocument(this.DocumentDate);
                adjustDocument.CreateAdjustDailyDocument(dbUtil, conn, this.DocumentId, materialList, remark, shopId, staffId);
            }

            dbUtil.sqlExecute(" DELETE FROM docdetail WHERE DocumentID=" + this.DocumentId + " AND ShopID=" + shopId, conn);
            string sql = " INSERT INTO docdetail (DocDetailID, DocumentID, ShopID, ProductID, ProductUnit, ProductAmount, ProductDiscount," +
                         " ProductTaxType, UnitName, UnitSmallAmount, UnitID) " +
                         " SELECT DocDetailID, DocumentID, ShopID, ProductID, ProductUnit, ProductAmount, ProductDiscount, " +
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

        public bool UpdateStockCount(CDBUtil dbUtil, MySqlConnection conn, int docId, int shopId, int updateStaffId)
        {
            Document document = new Document();
            document.ShopId = shopId;
            document.DocumentId = docId;
            document.Remark = Remark;
            document.UpdateBy = updateStaffId;

            return document.UpdateDocument(dbUtil, conn);
        }

        public virtual bool SaveCountData(CDBUtil dbUtil, MySqlConnection conn,
            int staffId, List<Material> materialList)
        {
            // Create document if no document
            // product data
            Product product = new Product();
            Material material = new Material();
            material.DocumentTypeID = DocumentTypeId;
            material.DocumentTypeIdStr = DocumentTypeIdStr;

            // document data
            Document document = new Document();
            document.ShopId = this.ShopId;
            document.DocumentTypeId = this.DocumentTypeId;
            document.DocumentYear = this.DocumentDate.Year;
            document.DocumentMonth = this.DocumentDate.Month;
            document.GetLastDocumentNumber(dbUtil, conn);
            document.DocumentDate = this.DocumentDate;
            if (this.Remark != null)
                document.Remark = this.Remark.Replace("'", string.Empty).Replace("/", string.Empty).Replace("\\", string.Empty);
            document.InputBy = staffId;
            document.UpdateBy = staffId;
            document.DocumentStatus = 1;
            document.ProductLevelId = this.ShopId;
            document.InsertDate = this.DocumentDate;
            document.GetLastDocumentId(dbUtil, conn);
            this.DocumentId = document.DocumentId;

            if (document.AddDocument(dbUtil, conn))
            {
                this.DocumentId = document.DocumentId;
                int[] materialId = new int[materialList.Count];
                decimal[] materialCurrAmount = new decimal[materialList.Count];
                decimal[] materialAmount = new decimal[materialList.Count];
                int[] unitLargeId = new int[materialList.Count];
                for (int i = 0; i < materialList.Count; i++)
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

        public decimal GetMaterialSumByCountDate(CDBUtil dbUtil, MySqlConnection conn,
            DateTime documentDate, int materialId, int invId)
        {
            string sql = "";
            sql = " SELECT SUM(b.ProductAmount) AS DiffAmount " +
                    " FROM document a " +
                    " INNER JOIN docdetail b " +
                    " ON a.DocumentID=b.DocumentID AND a.ShopID=b.ShopID " +
                    " WHERE a.DocumentTypeID = " + this.DocumentTypeId + " AND b.ProductID=" + materialId + " AND " +
                    " a.DocumentDate={d '" + documentDate.ToString("yyyy'-'MM'-'dd", dateProvider) + "'} " +
                    " AND a.ShopID=" + invId + " GROUP BY b.ProductID, b.ProductAmount, a.DocumentDate";
            decimal diffAmount = 0;
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            if (reader.Read())
            {
                diffAmount = reader.GetDecimal("DiffAmount");
            }
            reader.Close();
            return diffAmount;
        }

        public decimal GetMaterialSumAll(CDBUtil dbUtil, MySqlConnection conn,
            DateTime dateFrom, DateTime dateTo, int materialId, int invId)
        {
            string strDocumentTypeId = "";
            if (this.DocumentTypeId == 24)
                strDocumentTypeId = "(18,19)";
            else if (DocumentTypeId == 30)
                strDocumentTypeId = "(31,32)";
            else if (DocumentTypeId == 57)
                strDocumentTypeId = "(58,59)";

            string sql = "";
            sql = " SELECT SUM(b.ProductAmount) * c.MovementInStock AS DiffAmount " +
                    " FROM document a " +
                    " INNER JOIN docdetail b " +
                    " ON a.DocumentID=b.DocumentID AND a.ShopID=b.ShopID " +
                    " INNER JOIN documenttype c " +
                    " ON a.DocumentTypeID=c.DocumentTypeID AND a.ShopID=c.ShopID " +
                    " WHERE a.DocumentTypeID IN " + strDocumentTypeId + " AND b.ProductID=" + materialId + " AND " +
                    " a.DocumentDate BETWEEN {d '" + dateFrom.ToString("yyyy'-'MM'-'dd", dateProvider) + "'} " +
                    " AND {d '" + dateTo.ToString("yyyy'-'MM'-'dd", dateProvider) + "'} " +
                    " AND a.ShopID=" + invId + " AND c.LangID=2 GROUP BY b.ProductID";
            decimal diffAmount = 0;
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            if (reader.Read())
            {
                diffAmount = reader.GetDecimal("DiffAmount");
            }
            reader.Close();
            return diffAmount;
        }

        //public List<Document> GetStockCountByEachMaterial(CDBUtil dbUtil, MySqlConnection conn, DateTime dateFrom,
        //    DateTime dateTo, int materialID, int invId, int langId)
        //{
        //    List<Document> documentList = new List<Document>();
        //    string sql = " SELECT a.DocumentID, a.ApproveDate, a.DocumentDate, b.Productid, SUM(b.ProductAmount) AS DiffAmount, " +
        //                    " c.DocumentTypeHeader, c.DocumentTypeName " +
        //                    " FROM document a " +
        //                    " INNER JOIN docdetail b " +
        //                    " ON a.DocumentID=b.DocumentID AND a.shopid=b.ShopID " +
        //                    " INNER JOIN documenttype c " +
        //                    " ON a.DocumentTypeID=c.DocumentTypeID AND a.ShopID=c.ShopID " +
        //                    " WHERE a.shopid=" + invId + " AND a.DocumentDate BETWEEN {d '" + dateFrom.ToString("yyyy'-'MM'-'dd", dateProvider) + "'}  " +
        //                    " AND {d '" + dateTo.ToString("yyyy'-'MM'-'dd", dateProvider) + "'} AND a.DocumentTypeID =" + this.DocumentTypeId +
        //                    " AND b.ProductID= " + materialID +
        //                    " AND c.LangID=" + langId + " GROUP BY a.documentdate, b.ProductID, b.ProductAmount ";
        //    MySqlCommand cmd = new MySqlCommand(sql, conn);
        //    MySqlDataReader reader = cmd.ExecuteReader();
        //    while (reader.Read())
        //    {
        //        Document document = new Document();
        //        document.DocumentId = reader.GetInt32("DocumentID");
        //        document.DocumentDate = reader.GetDateTime("DocumentDate");
        //        document.ProductAmount = reader.GetDecimal("DiffAmount");
        //        documentList.Add(document);
        //    }
        //    reader.Close();
        //    return documentList;
        //}

        public List<Document> GetStockCountDiffByEachMaterial(CDBUtil dbUtil, MySqlConnection conn, DateTime dateFrom,
            DateTime dateTo, int materialID, int invId, int langId)
        {
            string strDocumentTypeId = "";
            if (this.DocumentTypeId == 24)
                strDocumentTypeId = "(18,19)";
            else if (DocumentTypeId == 30)
                strDocumentTypeId = "(31,32)";
            else if (DocumentTypeId == 57)
                strDocumentTypeId = "(58,59)";

            List<Document> documentList = new List<Document>();
            string sql = " SELECT a.DocumentID, a.DocumentDate, b.Productid, b.ProductAmount AS DiffAmount, " +
                            " c.DocumentTypeHeader, c.DocumentTypeName " +
                            " FROM document a " +
                            " INNER JOIN docdetail b " +
                            " ON a.DocumentID=b.DocumentID AND a.shopid=b.ShopID " +
                            " INNER JOIN documenttype c " +
                            " ON a.DocumentTypeID=c.DocumentTypeID AND a.ShopID=c.ShopID " +
                            " WHERE a.shopid=" + invId + " AND a.DocumentDate BETWEEN {d '" + dateFrom.ToString("yyyy'-'MM'-'dd", dateProvider) + "'}  " +
                            " AND {d '" + dateTo.ToString("yyyy'-'MM'-'dd", dateProvider) + "'} AND a.DocumentTypeID IN " + strDocumentTypeId + " AND b.ProductID= " + materialID +
                            " AND c.LangID=" + langId + " GROUP BY a.documentdate, b.ProductID";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Document document = new Document();
                document.DocumentId = reader.GetInt32("DocumentID");
                document.DocumentDate = reader.GetDateTime("DocumentDate");
                document.ProductAmount = reader.GetDecimal("DiffAmount");
                documentList.Add(document);
            }
            reader.Close();
            return documentList;
        }

        public decimal GetStockCountDiffByEachMaterial(CDBUtil dbUtil, MySqlConnection conn, DateTime countDate,
           int materialID, int invId, int langId)
        {
            string strDocumentTypeId = "";
            if (this.DocumentTypeId == 24)
                strDocumentTypeId = "(18,19)";
            else if (DocumentTypeId == 30)
                strDocumentTypeId = "(31,32)";
            else if (DocumentTypeId == 57)
                strDocumentTypeId = "(58,59)";

            List<Document> documentList = new List<Document>();
            string sql = " SELECT a.DocumentID, a.DocumentDate, b.Productid, b.ProductAmount AS DiffAmount, " +
                            " c.DocumentTypeHeader, c.DocumentTypeName " +
                            " FROM document a " +
                            " INNER JOIN docdetail b " +
                            " ON a.DocumentID=b.DocumentID AND a.shopid=b.ShopID " +
                            " INNER JOIN documenttype c " +
                            " ON a.DocumentTypeID=c.DocumentTypeID AND a.ShopID=c.ShopID " +
                            " WHERE a.shopid=" + invId + " AND a.DocumentDate = " +
                            "{d '" + countDate.ToString("yyyy'-'MM'-'dd", dateProvider) + "'}  " +
                            " AND a.DocumentTypeID IN " + strDocumentTypeId + " AND b.ProductID= " + materialID +
                            " AND c.LangID=" + langId + " GROUP BY a.documentdate, b.ProductID";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            decimal diffAmount = 0;
            while (reader.Read())
            {
                if (reader["DiffAmount"] != DBNull.Value)
                    diffAmount = reader.GetDecimal("DiffAmount");
            }
            reader.Close();
            return diffAmount;
        }
    }
}
