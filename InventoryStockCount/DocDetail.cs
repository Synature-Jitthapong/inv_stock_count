using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using MySql.Data.MySqlClient;
using POSMySQL.POSControl;

namespace InventoryStockCount
{
    [Serializable]
    public class DocDetail
    {
        #region Property
        private int _docDetailId;
        private int _documentId;
        private int _documentTypeId;
        private int _shopId;
        private int _productId;
        private int _materialId;
        private int _productUnit;
        private decimal _productAmount;
        private decimal _productDiscount;
        private decimal _productTax;
        private decimal _productPricePerUnit;
        private decimal _markUp;
        private decimal _pricePerUnitBeforeMark;
        private int _productTaxType;
        private decimal _productTaxIn;
        private int _staffId;
        private decimal _productDiscountAmount;
        private string _unitName;
        private decimal _unitSmallAmount;
        private int _unitId;
        private int _unitSmallId;
        private decimal _productNetPrice;
        private decimal _otherDiscount;
        private int _adjustLinkGroup;
        #endregion

        public DocDetail()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public DocDetail(CDBUtil dbUtil, MySqlConnection conn, int documentId, int shopId, int productId,
            int materialId, int productUnit, decimal productAmount, decimal productDiscount, decimal productTax,
            decimal productPricePerUnit, decimal markUp, decimal pricePerUnitBeforeMark, int productTaxType,
            decimal productTaxIn, int staffId, decimal productDiscountAmount, string unitName, decimal unitSmallAmount,
            int unitId, int unitSmallId, decimal productNetPrice, decimal otherDiscount, int adjustLinkGroup)
        {
            _documentId = documentId;
            _shopId = shopId;
            _productId = productId;
            _materialId = materialId;
            _productUnit = productUnit;
            _productAmount = productAmount;
            _productDiscount = productDiscount;
            _productTax = productTax;
            _productPricePerUnit = productPricePerUnit;
            _markUp = markUp;
            _pricePerUnitBeforeMark = pricePerUnitBeforeMark;
            _productTaxType = productTaxType;
            _productTaxIn = productTaxIn;
            _staffId = staffId;
            _productDiscountAmount = productDiscountAmount;
            _unitName = unitName;
            _unitSmallAmount = unitSmallAmount;
            _unitId = unitId;
            _unitSmallId = unitSmallId;
            _productNetPrice = productNetPrice;
            _otherDiscount = otherDiscount;
            _adjustLinkGroup = adjustLinkGroup;

            GetLastDocdetailID(dbUtil, conn);
        }


        public int DocDetailId
        {
            get { return _docDetailId; }
            set { _docDetailId = value; }
        }

        public int DocumentId
        {
            get { return _documentId; }
            set { _documentId = value; }
        }


        public int DocumentTypeId
        {
            get { return _documentTypeId; }
            set { _documentTypeId = value; }
        }
        public int ShopId
        {
            get { return _shopId; }
            set { _shopId = value; }
        }

        public int ProductId
        {
            get { return _productId; }
            set { _productId = value; }
        }

        public int MaterialId
        {
            get { return _materialId; }
            set { _materialId = value; }
        }

        public int ProductUnit
        {
            get { return _productUnit; }
            set { _productUnit = value; }
        }

        public decimal ProductAmount
        {
            get { return _productAmount; }
            set { _productAmount = value; }
        }

        public decimal ProductDiscount
        {
            get { return _productDiscount; }
            set { _productDiscount = value; }
        }

        public decimal ProductTax
        {
            get { return _productTax; }
            set { _productTax = value; }
        }

        public decimal ProductPricePerUnit
        {
            get { return _productPricePerUnit; }
            set { _productPricePerUnit = value; }
        }

        public decimal MarkUp
        {
            get { return _markUp; }
            set { _markUp = value; }
        }

        public decimal PricePerUnitBeforeMark
        {
            get { return _pricePerUnitBeforeMark; }
            set { _pricePerUnitBeforeMark = value; }
        }

        public int ProductTaxType
        {
            get { return _productTaxType; }
            set { _productTaxType = value; }
        }

        public decimal ProductTaxIn
        {
            get { return _productTaxIn; }
            set { _productTaxIn = value; }
        }

        public int StaffId
        {
            get { return _staffId; }
            set { _staffId = value; }
        }

        public decimal ProductDiscountAmount
        {
            get { return _productDiscountAmount; }
            set { _productDiscountAmount = value; }
        }

        public string UnitName
        {
            get { return _unitName; }
            set { _unitName = value; }
        }

        public decimal UnitSmallAmount
        {
            get { return _unitSmallAmount; }
            set { _unitSmallAmount = value; }
        }

        public int UnitId
        {
            get { return _unitId; }
            set { _unitId = value; }
        }

        public int UnitSmallId
        {
            get { return _unitSmallId; }
            set { _unitSmallId = value; }
        }

        public decimal ProductNetPrice
        {
            get { return _productNetPrice; }
            set { _productNetPrice = value; }
        }

        public decimal OtherDiscount
        {
            get { return _otherDiscount; }
            set { _otherDiscount = value; }
        }

        public int AdjustLinkGroup
        {
            get { return _adjustLinkGroup; }
            set { _adjustLinkGroup = value; }
        }

        public void clearDocDetail(MySqlConnection conn, int documentId, int shopId)
        {
            MySqlCommand cmd = new MySqlCommand("DELETE FROM docdetailtemp WHERE DocumentID=" +
             documentId + " AND ShopID=" + shopId, conn);
            cmd.ExecuteNonQuery();
        }

        public void getLastDocdetailID(CDBUtil dbUtil, MySqlConnection conn)
        {
            string strSql = "SELECT MAX(DocDetailID) AS LastDocDetailID FROM docdetail " +
                " WHERE DocumentID=" + DocumentId + " AND ShopID=" + ShopId;

            MySqlDataReader reader = dbUtil.sqlRetrive(strSql, conn);
            int lastDocDetailID = 0;
            while (reader.Read())
            {
                if (reader["LastDocDetailID"] != DBNull.Value)
                    lastDocDetailID = reader.GetInt32(0);
            }
            reader.Close();
            this.DocDetailId = lastDocDetailID + 1;
        }

        public void GetLastDocdetailID(CDBUtil dbUtil, MySqlConnection conn)
        {
            string strSql = "SELECT MAX(DocDetailID) AS LastDocDetailID FROM docdetailtemp " +
                " WHERE DocumentID=" + DocumentId + " AND ShopID=" + ShopId;

            MySqlDataReader reader = dbUtil.sqlRetrive(strSql, conn);
            int lastDocDetailID = 0;
            while (reader.Read())
            {
                if (reader["LastDocDetailID"] != DBNull.Value)
                    lastDocDetailID = reader.GetInt32(0);
            }
            reader.Close();
            this.DocDetailId = lastDocDetailID + 1;
        }

        public bool DeleteDocDetail(CDBUtil dbUtil, MySqlConnection conn)
        {
            if (dbUtil.sqlExecute(" DELETE FROM docdetail WHERE DocDetailID = " +
                DocDetailId + " AND  DocumentID=" + DocumentId, conn) > 0)
                return true;
            return true;
        }

        public bool DeleteDocdetailTemp(CDBUtil dbUtil, MySqlConnection conn)
        {
            dbUtil.sqlExecute("DELETE FROM docdetailtemp  WHERE " +
            " DocumentID=" + DocumentId + " AND ShopID=" + ShopId +
            " AND DocumentTypeID=" + DocumentTypeId, conn);

            return true;
        }

        public virtual bool AddDocDetailTemp(CDBUtil dbUtil, MySqlConnection conn)
        {
            string sql = " INSERT INTO docdetailtemp (DocDetailID, DocumentID, ShopID, DocumentTypeID, ProductID, " +
                " ProductUnit, ProductAmount, ProductTaxType, UnitName, UnitSmallAmount, UnitID) " +
                " VALUES(" + DocDetailId + ", " + DocumentId + ", " + ShopId + ", " + DocumentTypeId + ", " + ProductId + ", " +
                ProductUnit + ", " + ProductAmount + ", " + ProductTaxType + ", '" + UnitName + "', " + UnitSmallAmount + ", " + UnitId + ")";
            int exec = dbUtil.sqlExecute(sql, conn);
            return true;
        }

        public virtual bool AddDocDetail(CDBUtil dbUtil, MySqlConnection conn)
        {
            string sql = " INSERT INTO docdetail (DocDetailID, DocumentID, ShopID, ProductID, " +
                " ProductUnit, ProductAmount, ProductTaxType, UnitName, UnitSmallAmount, UnitID) " +
                " VALUES(" + DocDetailId + ", " + DocumentId + ", " + ShopId + ", " + ProductId + ", " +
                ProductUnit + ", " + ProductAmount + ", " + ProductTaxType + ", '" + UnitName + "', " + 
                UnitSmallAmount + ", " + UnitId + ")";
            int exec = dbUtil.sqlExecute(sql, conn);
            return true;
        }
    }
}