using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using MySql.Data.MySqlClient;
using POSMySQL.POSControl;
using System.Globalization;
using POSInventoryPOROModule;

namespace InventoryStockCount
{
    [Serializable]
    public class Document : DocDetail
    {
        #region Field
        private int _documentID;
        private int _shopID;
        private int _vendorID;
        private int _vendorGroupID;
        private int _vendorShopID;
        private int _documentTypeID;
        private int _documentYear;
        private int _documentMonth;
        private int _documentNumber;
        private DateTime _documentDate;
        private string _documentMonthStr;
        private int _inputBy;
        private int _updateBy;
        private int _approveBy;
        private int _voidBy;
        private int _receiveBy;
        private int _documentStatus;
        private int _invoiceRef;
        private int _documentIDRef;
        private int _docIDRefShopID;
        private int _productLevelID;
        private int _toInvID;
        private int _fromInvID;
        private int _isSmallUnit;
        private string _remark;
        private int _termOfPayment;
        private DateTime _dueDate;
        private int _documentHeader;
        private int _creditDay;
        private DateTime _insertDate;
        private DateTime _updateDate;
        private DateTime _approveDate;
        private DateTime _cancelDate;
        private int _newSend;
        private int _currentAccessStaff;
        private decimal _otherPercentDiscount;
        private decimal _otherAmountDiscount;
        private int _documentVATType;
        private decimal _vatPercent;
        private decimal _productAmount;
        private int _foundDocument;


        public decimal ProductAmount
        {
            get { return _productAmount; }
            set { _productAmount = value; }
        }
        private string _documentTypeHeader;
        #endregion

        private string resultText = "";
        private IFormatProvider dateProvider = System.Globalization.CultureInfo.InvariantCulture;
        public Document()
        {

        }

        public Document(CDBUtil dbUtil, MySqlConnection conn, int shopId, int vendorId,
            int vendorGroupId, int vendorShopId, int documentTypeId, int documentYear,
            int documentMonth, DateTime documentDate, int inputBy,
            int updateBy, int approveBy, int voidBy, int receiptBy, int documentStatus,
            int invoiceRef, int documentIdRef, int docIdRefShopId, int productLevelId,
            int toInvId, int fromInvId, int isSmallUnit, string remark, int termOfPayment,
            DateTime dueDate, int documentHeader, int creditDay, DateTime insertDate,
            DateTime updateDate, DateTime approveDate, DateTime cancelDate, int newSend,
            int currentStaffAccess, decimal otherPercentDiscount,
            decimal otherAmountDiscount, int documentVatType, decimal vatPercent)
        {
            _shopID = shopId;
            _vendorID = vendorId;
            _vendorGroupID = vendorGroupId;
            _vendorShopID = vendorShopId;
            _documentTypeID = documentTypeId;
            _documentYear = documentYear;
            _documentMonth = documentMonth;
            _documentDate = documentDate;
            _inputBy = inputBy;
            _updateBy = updateBy;
            _approveBy = approveBy;
            _voidBy = voidBy;
            _receiveBy = receiptBy;
            _documentStatus = documentStatus;
            _invoiceRef = invoiceRef;
            _documentIDRef = documentIdRef;
            _docIDRefShopID = docIdRefShopId;
            _productLevelID = productLevelId;
            _toInvID = toInvId;
            _fromInvID = fromInvId;
            _isSmallUnit = isSmallUnit;
            _remark = remark;
            _termOfPayment = termOfPayment;
            _dueDate = dueDate;
            _documentHeader = documentHeader;
            _creditDay = creditDay;
            _insertDate = insertDate;
            _updateDate = updateDate;
            _approveDate = approveDate;
            _cancelDate = cancelDate;
            _newSend = newSend;
            _currentAccessStaff = currentStaffAccess;
            _otherPercentDiscount = otherPercentDiscount;
            _otherAmountDiscount = otherAmountDiscount;
            _documentVATType = documentVatType;
            _vatPercent = vatPercent;

            GetLastDocumentId(dbUtil, conn);
            GetLastDocumentNumber(dbUtil, conn);
        }

        public int FoundDocument
        {
            get { return _foundDocument; }
            set { _foundDocument = value; }
        }

        public int DocumentId
        {
            get { return _documentID; }
            set { _documentID = value; }
        }

        public int ShopId
        {
            get { return _shopID; }
            set { _shopID = value; }
        }

        public int VendorId
        {
            get { return _vendorID; }
            set { _vendorID = value; }
        }

        public int VendorGroupId
        {
            get { return _vendorGroupID; }
            set { _vendorGroupID = value; }
        }

        public int VendorShopId
        {
            get { return _vendorShopID; }
            set { _vendorShopID = value; }
        }

        public int DocumentTypeId
        {
            get { return _documentTypeID; }
            set { _documentTypeID = value; }
        }

        public int DocumentYear
        {
            get { return _documentYear; }
            set { _documentYear = value; }
        }

        public int DocumentMonth
        {
            get { return _documentMonth; }
            set { _documentMonth = value; }
        }

        public int DocumentNumber
        {
            get { return _documentNumber; }
            set { _documentNumber = value; }
        }

        public DateTime DocumentDate
        {
            get { return _documentDate; }
            set { _documentDate = value; }
        }

        public int InputBy
        {
            get { return _inputBy; }
            set { _inputBy = value; }
        }

        public int UpdateBy
        {
            get { return _updateBy; }
            set { _updateBy = value; }
        }

        public int ApproveBy
        {
            get { return _approveBy; }
            set { _approveBy = value; }
        }

        public int VoidBy
        {
            get { return _voidBy; }
            set { _voidBy = value; }
        }

        public int ReceiveBy
        {
            get { return _receiveBy; }
            set { _receiveBy = value; }
        }

        public int DocumentStatus
        {
            get { return _documentStatus; }
            set { _documentStatus = value; }
        }

        public int InvoiceRef
        {
            get { return _invoiceRef; }
            set { _invoiceRef = value; }
        }

        public int DocumentIdRef
        {
            get { return _documentIDRef; }
            set { _documentIDRef = value; }
        }

        public int DocIdRefShopId
        {
            get { return _docIDRefShopID; }
            set { _docIDRefShopID = value; }
        }

        public int ProductLevelId
        {
            get { return _productLevelID; }
            set { _productLevelID = value; }
        }

        public int ToInvId
        {
            get { return _toInvID; }
            set { _toInvID = value; }
        }

        public int FromInvId
        {
            get { return _fromInvID; }
            set { _fromInvID = value; }
        }

        public int IsSmallUnit
        {
            get { return _isSmallUnit; }
            set { _isSmallUnit = value; }
        }

        public string Remark
        {
            get { return _remark; }
            set { _remark = value; }
        }

        public int TermOfPayment
        {
            get { return _termOfPayment; }
            set { _termOfPayment = value; }
        }

        public DateTime DueDate
        {
            get { return _dueDate; }
            set { _dueDate = value; }
        }

        public int DocumentHeader
        {
            get { return _documentHeader; }
            set { _documentHeader = value; }
        }

        public int CreditDay
        {
            get { return _creditDay; }
            set { _creditDay = value; }
        }

        public DateTime InsertDate
        {
            get { return _insertDate; }
            set { _insertDate = value; }
        }

        public DateTime UpdateDate
        {
            get { return _updateDate; }
            set { _updateDate = value; }
        }

        public DateTime ApproveDate
        {
            get { return _approveDate; }
            set { _approveDate = value; }
        }

        public DateTime CancelDate
        {
            get { return _cancelDate; }
            set { _cancelDate = value; }
        }

        public int NewSend
        {
            get { return _newSend; }
            set { _newSend = value; }
        }

        public int CurrentAccessStaff
        {
            get { return _currentAccessStaff; }
            set { _currentAccessStaff = value; }
        }

        public decimal OtherPercentDiscount
        {
            get { return _otherPercentDiscount; }
            set { _otherPercentDiscount = value; }
        }

        public decimal OtherAmountDiscount
        {
            get { return _otherAmountDiscount; }
            set { _otherAmountDiscount = value; }
        }

        public int DocumentVatType
        {
            get { return _documentVATType; }
            set { _documentVATType = value; }
        }

        public decimal VAtPercent
        {
            get { return _vatPercent; }
            set { _vatPercent = value; }
        }

        public string DocumentTypeHeader
        {
            get { return _documentTypeHeader; }
            set { _documentTypeHeader = value; }
        }

        public string DocumentMonthStr
        {
            get { return _documentMonthStr; }
            set { _documentMonthStr = value; }
        }

        public bool DocumentData(MySqlConnection conn, int documentId, int shopId)
        {
            bool foundDocument = false;
            string sql = "SELECT * FROM document WHERE DocumentID=" + documentId + " AND ShopID=" + shopId;
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    if (reader["DocumentID"] != DBNull.Value)
                        _documentID = reader.GetInt32("DocumentID");
                    if (reader["ShopID"] != DBNull.Value)
                        _shopID = reader.GetInt32("ShopID");
                    if (reader["DocumentDate"] != DBNull.Value)
                        _documentDate = reader.GetDateTime("DocumentDate");

                    foundDocument = true;
                }
            }
            return foundDocument;
        }

        public bool CheckStockCountDocument(MySqlConnection conn)
        {
            bool isFound = false;
            string sql = " SELECT * FROM document \n " +
                    " WHERE documenttypeid IN (7,24,30,57) \n " +
                    " AND DocumentDate='" + DateTime.Now.ToString("yyyy-MM-dd", dateProvider) + "' \n " +
                    " AND DocumentStatus=1";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    isFound = true;
                }
            }
            return isFound;
        }

        public List<Document> ListDocType10(CDBUtil dbUtil, MySqlConnection conn)
        {
            string sql = " SELECT * FROM document " +
                " WHERE ShopID= " + ProductLevelId + " AND DocumentTypeID = 10 " +
                " AND documentstatus=2";
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<Document> docList = new List<Document>();
            while (reader.Read())
            {
                Document document = new Document();
                if (reader["DocumentID"] != DBNull.Value)
                    document.DocumentId = Convert.ToInt32(reader["DocumentID"]);
                if (reader["DocumentYear"] != DBNull.Value)
                {
                    document.DocumentYear = Convert.ToInt32(reader["DocumentYear"]);
                    document.DocumentMonth = Convert.ToInt32(reader["DocumentMonth"]);
                    document.DocumentMonthStr = Convert.ToDateTime(new DateTime(Convert.ToInt32(reader["DocumentYear"]),
                        Convert.ToInt32(reader["DocumentMonth"]), 1)).ToString("MMMM");
                }
                docList.Add(document);
            }
            reader.Close();
            return docList;
        }

        public int GetStartSaleMonth(CDBUtil dbUtil, MySqlConnection conn)
        {
            int month = DateTime.Now.Month;
            string sql = " SELECT MIN(DocumentMonth) AS DocumentMonth FROM document " +
                " WHERE ShopID= " + ProductLevelId + " AND DocumentTypeID = 20 " +
                " AND DocumentYear = " + DocumentYear + " AND documentstatus=2";
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            while (reader.Read())
            {
                if (reader["DocumentMonth"] != DBNull.Value)
                    month = Convert.ToInt32(reader["DocumentMonth"]);
            }
            reader.Close();
            return month;
        }

        public int GetLastEndMonth(CDBUtil dbUtil, MySqlConnection conn)
        {
            int month = 0;
            string sql = " SELECT MAX(DocumentMonth) AS DocumentMonth FROM document " +
                " WHERE ShopID= " + ProductLevelId + " AND DocumentTypeID = 10 " +
                " AND DocumentYear = " + DocumentYear + " AND documentstatus=2";
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            while (reader.Read())
            {
                if (reader["DocumentMonth"] != DBNull.Value)
                    month = Convert.ToInt32(reader["DocumentMonth"]);
            }
            reader.Close();
            return month;
        }

        public DateTime GetLastTransferDocument(CDBUtil dbUtil, MySqlConnection conn, int invId)
        {
            string sql = "SELECT * FROM document WHERE ShopID=" + invId + " AND DocumentTypeID=10 ORDER BY DocumentDate DESC LIMIT 1";
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            if (reader.Read())
            {
                if (reader["DocumentID"] != DBNull.Value)
                    this.DocumentDate = reader.GetDateTime("DocumentDate");
            }
            reader.Close();
            return this.DocumentDate;
        }

        public bool CheckTransferStockOfThisShop(CDBUtil dbUtil, MySqlConnection conn)
        {
            string sql = " SELECT * FROM document WHERE " +
                " ShopID= " + ProductLevelId + " AND DocumentTypeID = 10 " +
                " AND DocumentMonth = " + DocumentMonth +
                " AND DocumentYear = " + DocumentYear + " AND DocumentStatus = 2";
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            if (reader.Read())
            {
                reader.Close();
                return true;
            }
            return false;
        }

        //protected void AddMaxDocumentID(CDBUtil dbUtil, MySqlConnection conn)
        //{
        //    bool isMaxDocumentID = false;
        //    string sql = "";
        //    sql = "SELECT MaxDocumentID FROM maxdocumentid WHERE ShopID=" + this.ShopId + " AND IsDocumentOrBatch=1 ";
        //    MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
        //    if (reader.Read())
        //    {
        //        if (reader["MaxDocumentID"] != DBNull.Value)
        //            if (reader.GetInt32("MaxDocumentID") > this.DocumentId)
        //                isMaxDocumentID = true;
        //    }
        //    reader.Close();
        //    if (isMaxDocumentID == false)
        //    {
        //        sql = " UPDATE maxdocumentid SET MaxDocumentID=" + this.DocumentId + " WHERE ShopID=" + this.ShopId + " AND IsDocumentOrBatch=1";
        //        dbUtil.sqlExecute(sql, conn);
        //    }
        //}

        public void GetLastDocumentId(CDBUtil dbUtil, MySqlConnection conn)
        {
            int documentid = 0;
            if (POSInventoryPOROModule.POSInventoryPOROModule.GetNewDocumentID(dbUtil, conn, this.ShopId, ref documentid, ref resultText))
                this.DocumentId = documentid;
        }

        //public void GetLastDocumentIdFromTableDocument(CDBUtil dbUtil, MySqlConnection conn)
        //{
        //    MySqlDataReader reader = dbUtil.sqlRetrive("SELECT MAX(DocumentID) AS MaxDocumentID FROM document WHERE ShopID="
        //        + ShopId, conn);
        //    if (reader.Read())
        //    {
        //        if (reader["MaxDocumentID"] != DBNull.Value)
        //            this.DocumentId = Convert.ToInt32(reader["MaxDocumentID"]) + 1;
        //    }
        //    else
        //    {
        //        this.DocumentId = 1;
        //    }
        //    reader.Close();
        //}

        protected void AddMaxDocumentNumber(CDBUtil dbUtil, MySqlConnection conn)
        {
            dbUtil.sqlExecute("DELETE FROM maxdocumentnumber " +
                " WHERE ShopID=" + this.ShopId + " AND DocType= " + this.DocumentTypeId +
                " AND DocumentYear= " + this.DocumentYear +
                " AND DocumentMonth=" + this.DocumentMonth, conn);

            dbUtil.sqlExecute(" INSERT INTO maxdocumentnumber " +
                " VALUES(" + this.ShopId + ", " + this.DocumentTypeId + ", " +
                " 0, " + this.DocumentYear + ", " + this.DocumentMonth + ", " +
                this.DocumentNumber + ")", conn);
        }

        public void GetLastDocumentNumber(CDBUtil dbUtil, MySqlConnection conn)
        {
            //MySqlDataReader reader = dbUtil.sqlRetrive(" SELECT MaxDocumentNumber FROM maxdocumentnumber WHERE " +
            //    " ShopID=" + this.ShopId + " AND DocType=" + this.DocumentTypeId + " AND DocumentYear = " + this.DocumentYear +
            //    " AND DocumentMonth = " + this.DocumentMonth, conn);
            //if (reader.Read())
            //{
            //    if (reader["MaxDocumentNumber"] != DBNull.Value)
            //        this.DocumentNumber = reader.GetInt32(0) + 1;
            //}
            //else
            //{
            //reader.Close();

            MySqlDataReader reader = dbUtil.sqlRetrive("SELECT MAX(DocumentNumber) AS DocumentNumber FROM document " +
            " WHERE ShopID=" + this.ShopId + " AND DocumentTypeID=" + this.DocumentTypeId +
            " AND DocumentYear=" + this.DocumentYear + " AND DocumentMonth=" + this.DocumentMonth, conn);
            if (reader.Read())
            {
                if (reader["DocumentNumber"] != DBNull.Value)
                    this.DocumentNumber = reader.GetInt32(0) + 1;
                else
                    this.DocumentNumber = 1;
            }
            else
            {
                this.DocumentNumber = 1;
            }
            reader.Close();
            //}

            //if (!reader.IsClosed)
            //    reader.Close();
        }

        //public bool IfHaveCountStockDailyDocument(CDBUtil dbUtil, MySqlConnection conn, int shopId)
        //{
        //    string sql = "SELECT * FROM document WHERE DocumentID=" + DocumentId + " AND shopid=" + shopId 
        //        + " AND documenttypeid=24 AND documentstatus=1";
        //    MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
        //    if (reader.Read())
        //    {
        //        reader.Close();
        //        return true;
        //    }
        //    else
        //        reader.Close();
        //    return false;
        //}

        public virtual bool AddAdjustDocument(CDBUtil dbUtil, MySqlConnection conn)
        {
            string sql = " INSERT INTO document (DocumentID, ShopID, DocumentTypeID, DocumentYear, DocumentMonth, " +
                " DocumentNumber, DocumentDate, InputBy, UpdateBy, ApproveBy, DocumentStatus, DocumentIDRef, " +
                " DocIDRefShopID, ProductLevelID, ToInvID, FromInvID, Remark, InsertDate, UpdateDate, ApproveDate) " +
                " VALUES(" + DocumentId + ", " + ShopId + ", " + DocumentTypeId + ", " +
                DocumentYear + ", " + DocumentMonth + ", " + DocumentNumber + ", '" +
                DocumentDate.ToString("yyyy'-'MM'-'dd", dateProvider) + "', " +
                InputBy + ", " + UpdateBy + ", " + ApproveBy + ", " + DocumentStatus + ", " + DocumentIdRef + ", " +
                DocIdRefShopId + ", " + ProductLevelId + ", " + ToInvId + ", " + FromInvId + ", '" + Remark + "', '" +
                DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss", dateProvider) + "', '" +
                DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss", dateProvider) + "', '" +
                DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss", dateProvider) + "')";

            try
            {
                int exec = dbUtil.sqlExecute(sql, conn);
                if (exec > 0)
                {
                    AddMaxDocumentNumber(dbUtil, conn);
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        public bool UpdateDocument(CDBUtil dbUtil, MySqlConnection conn)
        {
            string sql = "UPDATE document SET UpdateDate='" + DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss", dateProvider) + "', " +
                   " Remark = '" + Remark + "', UpdateBy=" + UpdateBy +
                   " WHERE DocumentID=" + DocumentId + " AND ShopID=" + ShopId;
            dbUtil.sqlExecute(sql, conn);
            return true;
        }

        public virtual bool AddDocument(CDBUtil dbUtil, MySqlConnection conn)
        {
            string sql = "";
            if (this.DocumentTypeId == 7 || this.DocumentTypeId == 10)
            {
                sql = " SELECT * FROM document WHERE ShopID=" + ShopId +
                " AND DocumentTypeID = " + DocumentTypeId + " AND DocumentYear=" +
                this.DocumentYear + " AND DocumentMonth = " + this.DocumentMonth +
                " AND DocumentStatus=1";
            }
            else if (this.DocumentTypeId == 57)
            {
                StockCountWeeklyDoro scd = new StockCountWeeklyDoro(conn, ShopId, 57);

                string dateFrom = scd.SundayDate();

                sql = " SELECT * FROM document \n" +
                    " WHERE ShopID=" + ShopId + " AND DocumentTypeID=" + DocumentTypeId + "\n" +
                    " AND DocumentDate BETWEEN '" + dateFrom + "' AND \n" +
                    " '" + DateTime.Now.ToString("yyyy-MM-dd", dateProvider) + "' \n" +
                    " AND DocumentStatus=1";
            }
            else
            {
                sql = " SELECT * FROM document WHERE ShopID=" + ShopId +
                      " AND DocumentTypeID = " + DocumentTypeId +
                      " AND DocumentStatus=1  AND DocumentDate=" +
                      " {d '" + this.DocumentDate.ToString("yyyy'-'MM'-'dd", dateProvider) + "'} " +
                      " AND DocumentStatus=1";
            }
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            if (reader.Read())
            {
                // Add Docdetail
                if (reader["DocumentID"] != DBNull.Value)
                    DocumentId = Convert.ToInt32(reader["DocumentID"]);
                if (reader["Remark"] != DBNull.Value)
                    Remark = reader.GetString("Remark");

                reader.Close();

                sql = "UPDATE document SET UpdateDate='" + DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss", dateProvider) + "', " +
                    " Remark = '" + Remark + "' WHERE DocumentID=" + DocumentId + " AND ShopID=" + ShopId;
                dbUtil.sqlExecute(sql, conn);

                sql = "DELETE FROM docdetail WHERE DocumentID=" + DocumentId + " AND ShopID=" + ShopId;
                dbUtil.sqlExecute(sql, conn);

                sql = "DELETE FROM docdetailtemp WHERE DocumentID=" + DocumentId + " AND ShopID=" + ShopId;
                dbUtil.sqlExecute(sql, conn);

                return true;
            }
            else
            {
                reader.Close();
                sql = " INSERT INTO document (DocumentID, ShopID, " +
                    " DocumentTypeID, DocumentYear, DocumentMonth, DocumentNumber, DocumentDate, InputBy, " +
                    " UpdateBy, DocumentStatus, Remark, DocumentIDRef, DocIDRefShopID, ProductLevelID, InsertDate, UpdateDate) " +
                    " VALUES(" + DocumentId + ", " + ShopId + ", " + DocumentTypeId + ", " +
                    DocumentYear + ", " + DocumentMonth + ", " + DocumentNumber + ", '" +
                    DocumentDate.ToString("yyyy'-'MM'-'dd", dateProvider) + "', " +
                    InputBy + ", " + UpdateBy + ", 1, '" + Remark + "', " + DocumentIdRef + ", " + DocIdRefShopId + ", " + ProductLevelId + ", '" +
                    DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss", dateProvider) + "', '" +
                    DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss", dateProvider) + "')";

                try
                {
                    int exec = dbUtil.sqlExecute(sql, conn);
                    if (exec > 0)
                    {
                        // add maxdocumentnumber
                        AddMaxDocumentNumber(dbUtil, conn);
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public bool CancelPrevStockCountDoc(MySqlConnection conn, int shopId, int docTypeId, 
            DateTime docDate)
        {
            bool isSuccess = false;
            String strSql = "UPDATE document SET " +
                " DocumentStatus=99, UpdateDate=NOW(), " +
                " CancelDate=NOW(), Remark='ยกเลิกเอกสารนี้ เนื่องจากมีการนับหลังจากนี้แล้ว' " +
                " WHERE ShopID=" + shopId + 
                " AND DocumentTypeID=" + docTypeId + 
                " AND DocumentStatus=1 " +
                " AND DocumentDate < '" + docDate.ToString("yyyy'-'MM'-'dd", dateProvider) + "'";
            MySqlCommand cmd = new MySqlCommand(strSql, conn);
            if (cmd.ExecuteNonQuery() > 0) isSuccess = true;

            return isSuccess;
        }
    }
}