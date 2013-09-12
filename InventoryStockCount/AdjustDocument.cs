using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using POSMySQL.POSControl;
using POSInventoryPOROModule;

namespace InventoryStockCount
{
    public class AdjustDocument
    {
        private DocDetail docdetail;
        private List<Material> materialAdjustList;
        private List<Material> materialReduceList;

        private int _documentTypeId;
        private int _documentIdRef;
        private DateTime _documentDate;
        IFormatProvider dateProvider = System.Globalization.CultureInfo.InvariantCulture;

        public int DocumentIdRef
        {
            get { return _documentIdRef; }
            set { _documentIdRef = value; }
        }

        public int DocumentTypeId
        {
            get { return _documentTypeId; }
            set { _documentTypeId = value; }
        }

        public AdjustDocument(DateTime documentDate)
        {
            _documentDate = documentDate;
        }

        string refResultText = "";
        // สร้างเอกสารปรับเพิ่ม - ลด
        public bool CreateAdjustMonthlyDocument(CDBUtil dbUtil, MySqlConnection conn, int documentIdRef, List<Material> materialList,
            string remark, int shopId, int staffId)
        {
            materialAdjustList = new List<Material>();
            materialReduceList = new List<Material>();

            // document data
            Document document = new Document();
            for (int i = 0; i <= materialList.Count - 1; i++)
            {
                if (materialList[i].MaterialAmount > 0)
                {
                    Material material = new Material();
                    material.MaterialId = materialList[i].MaterialId;
                    material.MaterialAmount = materialList[i].MaterialAmount;
                    material.MaterialUnitLargeId = materialList[i].MaterialUnitLargeId;
                    materialAdjustList.Add(material);

                }
                else if (materialList[i].MaterialAmount < 0)
                {
                    Material material = new Material();
                    material.MaterialId = materialList[i].MaterialId;
                    material.MaterialAmount = Convert.ToDecimal(materialList[i].MaterialAmount.ToString().Replace("-",""));
                    material.MaterialUnitLargeId = materialList[i].MaterialUnitLargeId;
                    materialReduceList.Add(material);
                }

            }

            document.ShopId = shopId;
            document.DocumentYear = _documentDate.Year;
            document.DocumentMonth = _documentDate.Month;
            document.DocumentDate = _documentDate;
            document.DocumentIdRef = documentIdRef;
            document.DocIdRefShopId = shopId;
            document.InputBy = staffId;
            document.UpdateBy = staffId;
            document.ApproveBy = staffId;
            document.Remark = remark;
            document.DocumentStatus = 2;
            document.ProductLevelId = shopId;
            document.InsertDate = DateTime.Now;

            if (DocumentIdRef == 0 && DocumentTypeId == 22)
            {
                document.FromInvId = shopId;
            }
            else if (DocumentIdRef > 0)
            {
                document.ApproveDate = DateTime.Now;
            }

            if (materialAdjustList.Count > 0)
            {
                document.DocumentTypeId = 22; // ใบเพิ่ม
                document.GetLastDocumentId(dbUtil, conn);
                document.GetLastDocumentNumber(dbUtil, conn);

                if (document.AddAdjustDocument(dbUtil, conn))
                {
                    int[] materialId = new int[materialAdjustList.Count];
                    decimal[] materialCurrAmount = new decimal[materialAdjustList.Count];
                    decimal[] materialAmount = new decimal[materialAdjustList.Count];
                    int[] unitLargeId = new int[materialAdjustList.Count];

                    for (int i = 0; i <= materialAdjustList.Count - 1; i++)
                    {
                        materialId[i] = materialAdjustList[i].MaterialId;
                        materialCurrAmount[i] = materialAdjustList[i].MaterialCurrAmount;
                        materialAmount[i] = materialAdjustList[i].MaterialAmount;
                        unitLargeId[i] = materialAdjustList[i].MaterialUnitLargeId;
                    }

                    if (!POSInventoryPOROModule.POSInventoryPOROModule.StockCount_AddDocDetail(
                        dbUtil, conn, document.DocumentId, shopId, materialId,
                        materialCurrAmount, materialAmount, unitLargeId, true, ref refResultText))
                    {
                        return false;
                    }
                }
                CopyAdjustDocdetailTemp(dbUtil, conn, document.DocumentId, document.DocumentTypeId, shopId, staffId, remark);
            }

            if (materialReduceList.Count > 0)
            {
                document.DocumentTypeId = 23; // ใบลด
                document.GetLastDocumentId(dbUtil, conn);
                document.GetLastDocumentNumber(dbUtil, conn);

                if (document.AddAdjustDocument(dbUtil, conn))
                {
                    int[] materialId = new int[materialReduceList.Count];
                    decimal[] materialCurrAmount = new decimal[materialReduceList.Count];
                    decimal[] materialAmount = new decimal[materialReduceList.Count];
                    int[] unitLargeId = new int[materialReduceList.Count];

                    for (int i = 0; i <= materialReduceList.Count - 1; i++)
                    {
                        materialId[i] = materialReduceList[i].MaterialId;
                        materialCurrAmount[i] = materialReduceList[i].MaterialCurrAmount;
                        materialAmount[i] = materialReduceList[i].MaterialAmount;
                        unitLargeId[i] = materialReduceList[i].MaterialUnitLargeId;
                    }

                    if (!POSInventoryPOROModule.POSInventoryPOROModule.StockCount_AddDocDetail(
                        dbUtil, conn, document.DocumentId, shopId, materialId,
                        materialCurrAmount, materialAmount, unitLargeId, true, ref refResultText))
                    {
                        return false;
                    }
                }
                CopyAdjustDocdetailTemp(dbUtil, conn, document.DocumentId, document.DocumentTypeId, shopId, staffId, remark);
            }
            return true;
        }

        public bool CreateAdjustWeeklyDocument(CDBUtil dbUtil, MySqlConnection conn, int documentIdRef, List<Material> materialList,
            string remark, int shopId, int staffId)
        {


            materialAdjustList = new List<Material>();
            materialReduceList = new List<Material>();

            // document data
            Document document = new Document();
            for (int i = 0; i <= materialList.Count - 1; i++)
            {
                if (materialList[i].MaterialAmount > 0)
                {
                    Material material = new Material();
                    material.MaterialId = materialList[i].MaterialId;
                    material.MaterialAmount = materialList[i].MaterialAmount;
                    material.MaterialUnitLargeId = materialList[i].MaterialUnitLargeId;
                    materialAdjustList.Add(material);

                }
                else if (materialList[i].MaterialAmount < 0)
                {
                    Material material = new Material();
                    material.MaterialId = materialList[i].MaterialId;
                    material.MaterialAmount = Convert.ToDecimal(materialList[i].MaterialAmount.ToString().Replace("-", ""));
                    material.MaterialUnitLargeId = materialList[i].MaterialUnitLargeId;
                    materialReduceList.Add(material);
                }

            }

            document.ShopId = shopId;
            document.DocumentYear = _documentDate.Year;
            document.DocumentMonth = _documentDate.Month;
            document.DocumentDate = _documentDate;
            document.DocumentIdRef = documentIdRef;
            document.DocIdRefShopId = shopId;
            document.InputBy = staffId;
            document.UpdateBy = staffId;
            document.ApproveBy = staffId;
            document.Remark = remark;
            document.DocumentStatus = 2;
            document.ProductLevelId = shopId;
            document.InsertDate = DateTime.Now;

            if (DocumentIdRef == 0 && DocumentTypeId == 31)
            {
                document.FromInvId = shopId;
            }
            else if (DocumentIdRef > 0)
            {
                document.ApproveDate = DateTime.Now;
            }

            if (materialAdjustList.Count > 0)
            {
                document.DocumentTypeId = 31; // ใบเพิ่ม
                document.GetLastDocumentId(dbUtil, conn);
                document.GetLastDocumentNumber(dbUtil, conn);

                if (document.AddAdjustDocument(dbUtil, conn))
                {
                    int[] materialId = new int[materialAdjustList.Count];
                    decimal[] materialCurrAmount = new decimal[materialAdjustList.Count];
                    decimal[] materialAmount = new decimal[materialAdjustList.Count];
                    int[] unitLargeId = new int[materialAdjustList.Count];

                    for (int i = 0; i <= materialAdjustList.Count - 1; i++)
                    {
                        materialId[i] = materialAdjustList[i].MaterialId;
                        materialCurrAmount[i] = materialAdjustList[i].MaterialCurrAmount;
                        materialAmount[i] = materialAdjustList[i].MaterialAmount;
                        unitLargeId[i] = materialAdjustList[i].MaterialUnitLargeId;
                    }

                    if (!POSInventoryPOROModule.POSInventoryPOROModule.StockCount_AddDocDetail(
                        dbUtil, conn, document.DocumentId, shopId, materialId,
                        materialCurrAmount, materialAmount, unitLargeId, true, ref refResultText))
                    {
                        return false;
                    }
                }
                CopyAdjustDocdetailTemp(dbUtil, conn, document.DocumentId, document.DocumentTypeId, shopId, staffId, remark);
            }

            if (materialReduceList.Count > 0)
            {
                document.DocumentTypeId = 32; // ใบลด
                document.GetLastDocumentId(dbUtil, conn);
                document.GetLastDocumentNumber(dbUtil, conn);

                if (document.AddAdjustDocument(dbUtil, conn))
                {
                    int[] materialId = new int[materialReduceList.Count];
                    decimal[] materialCurrAmount = new decimal[materialReduceList.Count];
                    decimal[] materialAmount = new decimal[materialReduceList.Count];
                    int[] unitLargeId = new int[materialReduceList.Count];

                    for (int i = 0; i <= materialReduceList.Count - 1; i++)
                    {
                        materialId[i] = materialReduceList[i].MaterialId;
                        materialCurrAmount[i] = materialReduceList[i].MaterialCurrAmount;
                        materialAmount[i] = materialReduceList[i].MaterialAmount;
                        unitLargeId[i] = materialReduceList[i].MaterialUnitLargeId;
                    }

                    if (!POSInventoryPOROModule.POSInventoryPOROModule.StockCount_AddDocDetail(
                        dbUtil, conn, document.DocumentId, shopId, materialId,
                        materialCurrAmount, materialAmount, unitLargeId, true, ref refResultText))
                    {
                        return false;
                    }
                }
                CopyAdjustDocdetailTemp(dbUtil, conn, document.DocumentId, document.DocumentTypeId, shopId, staffId, remark);
            }
            return true;
        }

        public bool CreateAdjustAdjustmentDocument(CDBUtil dbUtil, MySqlConnection conn, int documentIdRef, List<Material> materialList,
            string remark, int shopId, int staffId)
        {


            materialAdjustList = new List<Material>();
            materialReduceList = new List<Material>();

            // document data
            Document document = new Document();
            for (int i = 0; i <= materialList.Count - 1; i++)
            {
                if (materialList[i].MaterialAmount > 0)
                {
                    Material material = new Material();
                    material.MaterialId = materialList[i].MaterialId;
                    material.MaterialAmount = materialList[i].MaterialAmount;
                    material.MaterialUnitLargeId = materialList[i].MaterialUnitLargeId;
                    materialAdjustList.Add(material);

                }
                else if (materialList[i].MaterialAmount < 0)
                {
                    Material material = new Material();
                    material.MaterialId = materialList[i].MaterialId;
                    material.MaterialAmount = Convert.ToDecimal(materialList[i].MaterialAmount.ToString().Replace("-", ""));
                    material.MaterialUnitLargeId = materialList[i].MaterialUnitLargeId;
                    materialReduceList.Add(material);
                }

            }

            document.ShopId = shopId;
            document.DocumentYear = _documentDate.Year;
            document.DocumentMonth = _documentDate.Month;
            document.DocumentDate = _documentDate;
            document.DocumentIdRef = documentIdRef;
            document.DocIdRefShopId = shopId;
            document.InputBy = staffId;
            document.UpdateBy = staffId;
            document.ApproveBy = staffId;
            document.Remark = remark;
            document.DocumentStatus = 2;
            document.ProductLevelId = shopId;
            document.InsertDate = DateTime.Now;

            if (DocumentIdRef == 0 && DocumentTypeId == 58)
            {
                document.FromInvId = shopId;
            }
            else if (DocumentIdRef > 0)
            {
                document.ApproveDate = DateTime.Now;
            }

            if (materialAdjustList.Count > 0)
            {
                document.DocumentTypeId = 58; // ใบเพิ่ม
                document.GetLastDocumentId(dbUtil, conn);
                document.GetLastDocumentNumber(dbUtil, conn);

                if (document.AddAdjustDocument(dbUtil, conn))
                {
                    int[] materialId = new int[materialAdjustList.Count];
                    decimal[] materialCurrAmount = new decimal[materialAdjustList.Count];
                    decimal[] materialAmount = new decimal[materialAdjustList.Count];
                    int[] unitLargeId = new int[materialAdjustList.Count];

                    for (int i = 0; i <= materialAdjustList.Count - 1; i++)
                    {
                        materialId[i] = materialAdjustList[i].MaterialId;
                        materialCurrAmount[i] = materialAdjustList[i].MaterialCurrAmount;
                        materialAmount[i] = materialAdjustList[i].MaterialAmount;
                        unitLargeId[i] = materialAdjustList[i].MaterialUnitLargeId;
                    }

                    if (!POSInventoryPOROModule.POSInventoryPOROModule.StockCount_AddDocDetail(
                        dbUtil, conn, document.DocumentId, shopId, materialId,
                        materialCurrAmount, materialAmount, unitLargeId, true, ref refResultText))
                    {
                        return false;
                    }
                }
                CopyAdjustDocdetailTemp(dbUtil, conn, document.DocumentId, document.DocumentTypeId, shopId, staffId, remark);
            }

            if (materialReduceList.Count > 0)
            {
                document.DocumentTypeId = 59; // ใบลด
                document.GetLastDocumentId(dbUtil, conn);
                document.GetLastDocumentNumber(dbUtil, conn);

                if (document.AddAdjustDocument(dbUtil, conn))
                {
                    int[] materialId = new int[materialReduceList.Count];
                    decimal[] materialCurrAmount = new decimal[materialReduceList.Count];
                    decimal[] materialAmount = new decimal[materialReduceList.Count];
                    int[] unitLargeId = new int[materialReduceList.Count];

                    for (int i = 0; i <= materialReduceList.Count - 1; i++)
                    {
                        materialId[i] = materialReduceList[i].MaterialId;
                        materialCurrAmount[i] = materialReduceList[i].MaterialCurrAmount;
                        materialAmount[i] = materialReduceList[i].MaterialAmount;
                        unitLargeId[i] = materialReduceList[i].MaterialUnitLargeId;
                    }

                    if (!POSInventoryPOROModule.POSInventoryPOROModule.StockCount_AddDocDetail(
                        dbUtil, conn, document.DocumentId, shopId, materialId,
                        materialCurrAmount, materialAmount, unitLargeId, true, ref refResultText))
                    {
                        return false;
                    }
                }
                CopyAdjustDocdetailTemp(dbUtil, conn, document.DocumentId, document.DocumentTypeId, shopId, staffId, remark);
            }
            return true;
        }

        // Adjust daily stock document 
        public bool CreateAdjustDailyDocument(CDBUtil dbUtil, MySqlConnection conn, int documentIdRef, List<Material> materialList,
           string remark, int shopId, int staffId)
        {

            materialAdjustList = new List<Material>();
            materialReduceList = new List<Material>();

            // document data
            Document document = new Document();
            for (int i = 0; i <= materialList.Count - 1; i++)
            {
                if (materialList[i].MaterialAmount > 0)
                {
                    Material material = new Material();
                    material.MaterialId = materialList[i].MaterialId;
                    material.MaterialAmount = materialList[i].MaterialAmount;
                    material.MaterialUnitLargeId = materialList[i].MaterialUnitLargeId;
                    materialAdjustList.Add(material);

                }
                else if (materialList[i].MaterialAmount < 0)
                {
                    Material material = new Material();
                    material.MaterialId = materialList[i].MaterialId;
                    material.MaterialAmount = Convert.ToDecimal(materialList[i].MaterialAmount.ToString().Replace("-", ""));
                    material.MaterialUnitLargeId = materialList[i].MaterialUnitLargeId;
                    materialReduceList.Add(material);
                }

            }

            document.ShopId = shopId;
            document.DocumentYear = _documentDate.Year;
            document.DocumentMonth = _documentDate.Month;
            document.DocumentDate = _documentDate;
            document.DocumentIdRef = documentIdRef;
            document.DocIdRefShopId = shopId;
            document.InputBy = staffId;
            document.UpdateBy = staffId;
            document.ApproveBy = staffId;
            document.Remark = remark;
            document.DocumentStatus = 2;
            document.ProductLevelId = shopId;
            document.InsertDate = DateTime.Now;

            if (DocumentIdRef == 0 && DocumentTypeId == 18)
            {
                document.FromInvId = shopId;
            }
            else if (DocumentIdRef > 0)
            {
                document.ApproveDate = DateTime.Now;
            }

            if (materialAdjustList.Count > 0)
            {
                document.DocumentTypeId = 18; // ใบเพิ่ม
                document.GetLastDocumentId(dbUtil, conn);
                document.GetLastDocumentNumber(dbUtil, conn);

                if (document.AddAdjustDocument(dbUtil, conn))
                {
                    int[] materialId = new int[materialAdjustList.Count];
                    decimal[] materialCurrAmount = new decimal[materialAdjustList.Count];
                    decimal[] materialAmount = new decimal[materialAdjustList.Count];
                    int[] unitLargeId = new int[materialAdjustList.Count];
                   
                    for (int i = 0; i <= materialAdjustList.Count - 1; i++)
                    {
                        materialId[i] = materialAdjustList[i].MaterialId;
                        materialCurrAmount[i] = materialAdjustList[i].MaterialCurrAmount;
                        materialAmount[i] = materialAdjustList[i].MaterialAmount;
                        unitLargeId[i] = materialAdjustList[i].MaterialUnitLargeId;
                    }

                    if (!POSInventoryPOROModule.POSInventoryPOROModule.StockCount_AddDocDetail(
                        dbUtil, conn, document.DocumentId, shopId, materialId,
                        materialCurrAmount, materialAmount, unitLargeId, true, ref refResultText))
                    {
                        return false;
                    }
                }
                CopyAdjustDocdetailTemp(dbUtil, conn, document.DocumentId, document.DocumentTypeId, shopId, staffId, remark);
            }

            if (materialReduceList.Count > 0)
            {
                document.DocumentTypeId = 19; // ใบลด
                document.GetLastDocumentId(dbUtil, conn);
                document.GetLastDocumentNumber(dbUtil, conn);

                if (document.AddAdjustDocument(dbUtil, conn))
                {
                    int[] materialId = new int[materialReduceList.Count];
                    decimal[] materialCurrAmount = new decimal[materialReduceList.Count];
                    decimal[] materialAmount = new decimal[materialReduceList.Count];
                    int[] unitLargeId = new int[materialReduceList.Count];

                    for (int i = 0; i <= materialReduceList.Count - 1; i++)
                    {
                        materialId[i] = materialReduceList[i].MaterialId;
                        materialCurrAmount[i] = materialReduceList[i].MaterialCurrAmount;
                        materialAmount[i] = materialReduceList[i].MaterialAmount;
                        unitLargeId[i] = materialReduceList[i].MaterialUnitLargeId;
                    }

                    if (!POSInventoryPOROModule.POSInventoryPOROModule.StockCount_AddDocDetail(
                        dbUtil, conn, document.DocumentId, shopId, materialId,
                        materialCurrAmount, materialAmount, unitLargeId, true, ref refResultText))
                    {
                        return false;
                    }
                }
                CopyAdjustDocdetailTemp(dbUtil, conn, document.DocumentId, document.DocumentTypeId, shopId, staffId, remark);
            }
            return true;
        }

        protected bool CopyAdjustDocdetailTemp(CDBUtil dbUtil, MySqlConnection conn, int documentId, int documentTypeId, int shopId, int staffId, string remark)
        {

            dbUtil.sqlExecute(" DELETE FROM docdetail WHERE DocumentID=" + documentId + " AND ShopID=" + shopId, conn);
            string sql = " INSERT INTO docdetail (DocDetailID, DocumentID, ShopID, ProductID, ProductUnit, ProductAmount, " +
                         " ProductTaxType, UnitName, UnitSmallAmount, UnitID) " +
                         " SELECT DocDetailID, DocumentID, ShopID, ProductID, ProductUnit, ProductAmount, " +
                         " ProductTaxType, UnitName, UnitSmallAmount, UnitID " +
                         " FROM docdetailtemp WHERE DocumentID=" + documentId + " AND ShopID=" + shopId;
            if (dbUtil.sqlExecute(sql, conn) > 0)
            {
                dbUtil.sqlExecute("UPDATE document SET DocumentStatus=2, UpdateBy=" + staffId + ", ApproveBy=" + staffId + ", " +
                     " UpdateDate='" + DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss", dateProvider) + "', " +
                     " ApproveDate='" + DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss", dateProvider) + "', " +
                     " remark='" + remark + "' " +
                     " WHERE DocumentID=" + documentId + " AND ShopID=" + shopId + " AND DocumentTypeID=" + documentTypeId, conn);
                dbUtil.sqlExecute("DELETE FROM docdetailtemp WHERE DocumentID=" + documentId + " AND ShopID=" + shopId, conn);
                return true;
            }
            return false;
        }
    }
}
