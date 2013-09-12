using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POSMySQL.POSControl;
using MySql.Data.MySqlClient;

namespace InventoryStockCount
{
    public class CancelDocument
    {
        private int _documentId;

        public int DocumentId
        {
            get { return _documentId; }
            set { _documentId = value; }
        }
        private int _shopId;

        public int ShopId
        {
            get { return _shopId; }
            set { _shopId = value; }
        }
        private int _documentTypeId;

        public int DocumentTypeId
        {
            get { return _documentTypeId; }
            set { _documentTypeId = value; }
        }

        private int _staffId;

        public CancelDocument(int documentId, int shopId, int documentTypeId, int staffId)
        {
            this._documentId = documentId;
            this._shopId = shopId;
            this._documentTypeId = documentTypeId;
            this._staffId = staffId;
        }

        public bool CancelStockCountDocument(CDBUtil dbUtil, MySqlConnection conn)
        {
            string sql = "";
            sql = " UPDATE Document SET DocumentStatus = 99, UpdateBy= " + _staffId + 
                " WHERE DocumentID=" + DocumentId +
                " AND ShopID = " + ShopId + " AND DocumentTypeID=" + DocumentTypeId;
            dbUtil.sqlExecute(sql, conn);

            DocDetail docDetail = new DocDetail();
            docDetail.DocumentId = DocumentId;
            docDetail.ShopId = ShopId;
            docDetail.DocumentTypeId = DocumentTypeId;
            docDetail.DeleteDocdetailTemp(dbUtil, conn);
            return true;
        }
    }
}
