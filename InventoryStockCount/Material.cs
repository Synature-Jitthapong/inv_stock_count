using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POSMySQL.POSControl;
using MySql.Data.MySqlClient;

namespace InventoryStockCount
{
    public class Material : Document
    {
        private int _materialId;
        private int _materialUnit;
        private decimal _materialAmount;
        private string _materialUnitName;
        private int _materialUnitLargeId;
        private int _materialUnitSmallId;
        private int _materialTaxType;
        private int _documentId;
        private decimal _materialCountAmount;
        private DateTime _documentDate;
        private int _isShowAmountInStockCount;
        private decimal _materialDiffAmount;
        private int _documentTypeID;
        private string _documentTypeIdStr;
        private IFormatProvider dateProvider = System.Globalization.CultureInfo.InvariantCulture;
        private decimal _materialRemainAmount;
        private decimal _diffAmount;
        private decimal _materialCurrAmount;
        private int _countTypeId = 3; // monthlystockmaterial
        private int countRow = 0;

        // เอาไว้ยัด List material เขาไปใน material
        private List<Material> matListInMat;

        public List<Material> MatListInMat
        {
            get { return matListInMat; }
            set { matListInMat = value; }
        }

        public int CountRow
        {
            get { return countRow; }
        }

        public decimal MaterialCurrAmount
        {
            get { return _materialCurrAmount; }
            set { _materialCurrAmount = value; }
        }

        public decimal DiffAmount
        {
            get { return _diffAmount; }
            set { _diffAmount = value; }
        }

        public decimal MaterialRemainAmount
        {
            get { return _materialRemainAmount; }
            set { _materialRemainAmount = value; }
        }
        public string DocumentTypeIdStr
        {
            get { return _documentTypeIdStr; }
            set { _documentTypeIdStr = value; }
        }
        public int DocumentTypeID
        {
            get { return _documentTypeID; }
            set { _documentTypeID = value; }
        }

        public decimal MaterialDiffAmount
        {
            get { return _materialDiffAmount; }
            set { _materialDiffAmount = value; }
        }

        public int IsShowAmountInStockCount
        {
            get { return _isShowAmountInStockCount; }
            set { _isShowAmountInStockCount = value; }
        }

        public DateTime DocumentDate
        {
            get { return _documentDate; }
            set { _documentDate = value; }
        }

        public decimal MaterialCountAmount
        {
            get { return _materialCountAmount; }
            set { _materialCountAmount = value; }
        }

        public int DocumentId
        {
            get { return _documentId; }
            set { _documentId = value; }
        }

        public int MaterialTaxType
        {
            get { return _materialTaxType; }
            set { _materialTaxType = value; }
        }

        public int MaterialUnitSmallId
        {
            get { return _materialUnitSmallId; }
            set { _materialUnitSmallId = value; }
        }

        public int MaterialUnitLargeId
        {
            get { return _materialUnitLargeId; }
            set { _materialUnitLargeId = value; }
        }

        public string MaterialUnitName
        {
            get { return _materialUnitName; }
            set { _materialUnitName = value; }
        }

        public decimal MaterialAmount
        {
            get { return _materialAmount; }
            set { _materialAmount = value; }
        }

        public int MaterialUnit
        {
            get { return _materialUnit; }
            set { _materialUnit = value; }
        }

        public int MaterialId
        {
            get { return _materialId; }
            set { _materialId = value; }
        }
        private int _materialDeptId;

        public int MaterialDeptId
        {
            get { return _materialDeptId; }
            set { _materialDeptId = value; }
        }
        private string _materialDeptCode;

        public string MaterialDeptCode
        {
            get { return _materialDeptCode; }
            set { _materialDeptCode = value; }
        }
        private string _materialDeptName;

        public string MaterialDeptName
        {
            get { return _materialDeptName; }
            set { _materialDeptName = value; }
        }
        private string _materialCode;

        public string MaterialCode
        {
            get { return _materialCode; }
            set { _materialCode = value; }
        }
        private string _materialName;

        public string MaterialName
        {
            get { return _materialName; }
            set { _materialName = value; }
        }

        private int _materialGroupId;

        public int MaterialGroupId
        {
            get { return _materialGroupId; }
            set { _materialGroupId = value; }
        }
        private string _materialGroupCode;

        public string MaterialGroupCode
        {
            get { return _materialGroupCode; }
            set { _materialGroupCode = value; }
        }
        private string _materialGroupName;

        public string MaterialGroupName
        {
            get { return _materialGroupName; }
            set { _materialGroupName = value; }
        }
        private int _materialConfig;

        public int MaterialConfig
        {
            get { return _materialConfig; }
            set { _materialConfig = value; }
        }

        private string _countConfigType;

        public string CountConfigType
        {
            get { return _countConfigType; }
            set { _countConfigType = value; }
        }

        private int _stockCountTypeId;
        private string _stockCountName;

        public string StockCountName
        {
            get { return _stockCountName; }
            set { _stockCountName = value; }
        }

        public int StockCountTypeId
        {
            get { return _stockCountTypeId; }
            set { _stockCountTypeId = value; }
        }

        public Material()
        {
            this._documentDate = DateTime.Now;
        }

        public List<Material> ListStockCountMaterialSetting(CDBUtil dbUtil, MySql.Data.MySqlClient.MySqlConnection conn, int langId)
        {
            List<Material> stockCountMaterialSettingList = new List<Material>();
            string sql = "SELECT * FROM stockcountmaterialsetting WHERE LangID=" + langId;
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            while (reader.Read())
            {
                Material m = new Material();
                if (reader["StockCountTypeID"] != DBNull.Value)
                    m.StockCountTypeId = Convert.ToInt32(reader["StockCountTypeID"]);
                if (reader["StockCountName"] != DBNull.Value)
                    m.StockCountName = reader["StockCountName"].ToString();
                stockCountMaterialSettingList.Add(m);
            }
            reader.Close();
            return stockCountMaterialSettingList;
        }

        public Material(int countingType, MySqlConnection conn)
        {
            //string countType = "";
            //switch (countingType)
            //{
            //    case 1:
            //        CountConfigType = "dailystockmaterial";
            //        break;
            //    case 2:
            //        CountConfigType = "weeklystockmaterial";
            //        break;
            //    case 3:
            //        CountConfigType = "monthlystockmaterial";
            //        break;
            //    case 4:
            //        CountConfigType = "stockcountmaterial";
            //        break;
            //}

            string sql = "SELECT StockCountTypeID, StockCountTypeTableName FROM stockcountmaterialsetting WHERE StockCountTypeID=" + countingType;
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                if (reader["StockCountTypeID"] != DBNull.Value)
                    _countTypeId = reader.GetInt32("StockCountTypeID");
                if (reader["StockCountTypeTableName"] != DBNull.Value)
                    CountConfigType = reader.GetString("StockCountTypeTableName");
            }
            reader.Close();
        }

        public List<Material> MaterialDailyList(CDBUtil dbUtil, MySqlConnection conn, int shopId, int startRecord, int limit, string matCode)
        {
            string sql = @" SELECT b.ProductID, b.UnitID, b.ProductAmount AS MaterialAmount, b.ProductDiscountAmount,
                         b.ProductAmount - b.ProductDiscount AS DiffAmount,
                         b.UnitName, c.MaterialID, c.MaterialName, c.MaterialCode,
                         c.MaterialDeptID, c.MaterialCost, c.MaterialTaxType, c.UnitLargeID, c.UnitSmallID 
                         FROM document a, docdetailtemp b, materials c, UnitLarge d
                         WHERE a.DocumentID=b.DocumentID AND a.ShopID=b.ShopID
                         AND a.ShopID= " + shopId + @" AND a.DocumentTypeID=" + DocumentTypeID + @" AND a.documentid = " + DocumentId + @"
                         AND c.MaterialCode LIKE '" + matCode + @"%'
                         AND b.ProductID = c.MaterialID AND b.UnitID=d.UnitLargeID
                         ORDER BY c.MaterialCode LIMIT " + startRecord + ", " + limit;

            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<Material> materialList = new List<Material>();
            while (reader.Read())
            {
                Material material = new Material();
                if (reader["MaterialID"] != DBNull.Value)
                    material.MaterialId = reader.GetInt32("MaterialID");
                if (reader["MaterialCode"] != DBNull.Value)
                    material.MaterialCode = reader.GetString("MaterialCode");
                if (reader["MaterialName"] != DBNull.Value)
                    material.MaterialName = reader.GetString("MaterialName");
                if (reader["MaterialAmount"] != DBNull.Value)
                    material.MaterialAmount = reader.GetDecimal("MaterialAmount");
                if (reader["UnitName"] != DBNull.Value)
                    material.MaterialUnitName = reader.GetString("UnitName");
                if (reader["UnitLargeID"] != DBNull.Value)
                    material.MaterialUnitLargeId = reader.GetInt32("UnitLargeID");
                if (reader["UnitSmallID"] != DBNull.Value)
                    material.MaterialUnitSmallId = reader.GetInt32("UnitSmallID");
                if (reader["DocumentStatus"] != DBNull.Value)
                    material.DocumentStatus = reader.GetInt32("DocumentStatus");
                if (reader["ProductDiscount"] != DBNull.Value)
                    material.ProductDiscountAmount = reader.GetDecimal("ProductDiscount");
                if (reader["DiffAmount"] != DBNull.Value)
                    material.DiffAmount = reader.GetDecimal("DiffAmount");
                materialList.Add(material);
            }
            reader.Close();
            return materialList;
        }
        
        public List<Material> MaterialDailyList(CDBUtil dbUtil, MySqlConnection conn, int shopId)
        {
            string sql = @" SELECT b.ProductID, b.UnitID, b.ProductAmount AS MaterialAmount, b.ProductDiscountAmount,
                         b.ProductAmount - b.ProductDiscount AS DiffAmount,
                         b.UnitName, c.MaterialID, c.MaterialName, c.MaterialCode,
                         c.MaterialDeptID, c.MaterialCost, c.MaterialTaxType, c.UnitLargeID, c.UnitSmallID 
                         FROM document a, docdetailtemp b, materials c, UnitLarge d
                         WHERE a.DocumentID=b.DocumentID AND a.ShopID=b.ShopID
                         AND a.ShopID= " + shopId + @" AND a.DocumentTypeID=" + DocumentTypeID + @" AND a.documentid = " + DocumentId + @" 
                         AND b.ProductID = c.MaterialID AND b.UnitID=d.UnitLargeID ORDER BY c.MaterialCode";

            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<Material> materialList = new List<Material>();
            while (reader.Read())
            {
                Material material = new Material();
                if (reader["MaterialID"] != DBNull.Value)
                    material.MaterialId = reader.GetInt32("MaterialID");
                if (reader["MaterialCode"] != DBNull.Value)
                    material.MaterialCode = reader.GetString("MaterialCode");
                if (reader["MaterialName"] != DBNull.Value)
                    material.MaterialName = reader.GetString("MaterialName");
                if (reader["MaterialAmount"] != DBNull.Value)
                    material.MaterialAmount = reader.GetDecimal("MaterialAmount");
                if (reader["UnitName"] != DBNull.Value)
                    material.MaterialUnitName = reader.GetString("UnitName");
                if (reader["UnitLargeID"] != DBNull.Value)
                    material.MaterialUnitLargeId = reader.GetInt32("UnitLargeID");
                if (reader["UnitSmallID"] != DBNull.Value)
                    material.MaterialUnitSmallId = reader.GetInt32("UnitSmallID");
                if (reader["DocumentStatus"] != DBNull.Value)
                    material.DocumentStatus = reader.GetInt32("DocumentStatus");
                if (reader["ProductDiscount"] != DBNull.Value)
                    material.ProductDiscountAmount = reader.GetDecimal("ProductDiscount");
                if (reader["DiffAmount"] != DBNull.Value)
                    material.DiffAmount = reader.GetDecimal("DiffAmount");
                materialList.Add(material);
            }
            reader.Close();
            return materialList;
        }

        public List<Material> MaterialAdjustmentList(CDBUtil dbUtil, MySqlConnection conn, int shopId, int documentId)
        {
            string sql = " SELECT a.Remark, a.DocumentID, a.DocumentStatus, b.ProductID, b.ProductAmount AS MaterialAmount, b.ProductDiscount, b.ProductAmount - b.ProductDiscount AS DiffAmount, " +
                         " b.UnitID, b.UnitName, b.UnitSmallAmount, c.MaterialID, c.MaterialName, c.MaterialCode, " +
                         " c.MaterialDeptID, c.MaterialCost, " +
                         " c.MaterialTaxType, d.UnitLargeID, c.UnitSmallID " +
                         " FROM document a, docdetailtemp b, materials c, UnitLarge d " +
                         " WHERE a.DocumentID=b.DocumentID AND a.ShopID=b.ShopID " +
                         " AND a.ShopID= " + shopId + " AND a.DocumentTypeID=" + DocumentTypeID +
                         " AND a.documentid = " + documentId +
                         " AND b.ProductID = c.MaterialID AND b.UnitID=d.UnitLargeID " +
                         " ORDER BY c.MaterialCode";

            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<Material> materialList = new List<Material>();
            while (reader.Read())
            {
                Material material = new Material();
                if (reader["DocumentID"] != DBNull.Value)
                    material.DocumentId = reader.GetInt32("DocumentID");
                if (reader["MaterialID"] != DBNull.Value)
                    material.MaterialId = reader.GetInt32("MaterialID");
                if (reader["MaterialCode"] != DBNull.Value)
                    material.MaterialCode = reader.GetString("MaterialCode");
                if (reader["MaterialName"] != DBNull.Value)
                    material.MaterialName = reader.GetString("MaterialName");
                if (reader["MaterialAmount"] != DBNull.Value)
                    material.MaterialAmount = reader.GetDecimal("MaterialAmount");
                if (reader["UnitName"] != DBNull.Value)
                    material.MaterialUnitName = reader.GetString("UnitName");
                if (reader["UnitLargeID"] != DBNull.Value)
                    material.MaterialUnitLargeId = reader.GetInt32("UnitLargeID");
                if (reader["UnitSmallID"] != DBNull.Value)
                    material.MaterialUnitSmallId = reader.GetInt32("UnitSmallID");
                if (reader["DocumentStatus"] != DBNull.Value)
                    material.DocumentStatus = reader.GetInt32("DocumentStatus");
                if (reader["ProductDiscount"] != DBNull.Value)
                    material.ProductDiscountAmount = reader.GetDecimal("ProductDiscount");
                if (reader["DiffAmount"] != DBNull.Value)
                    material.DiffAmount = reader.GetDecimal("DiffAmount");
                if (reader["UnitSmallAmount"] != DBNull.Value)
                    material.UnitSmallAmount = reader.GetDecimal("UnitSmallAmount");
                if (reader["UnitSmallAmount"] != DBNull.Value)
                    material.UnitSmallAmount = reader.GetDecimal("UnitSmallAmount");
                materialList.Add(material);

                if (reader["Remark"] != DBNull.Value)
                    Remark = reader.GetString("Remark");
            }
            reader.Close();
            return materialList;
        }

        public List<Material> MaterialAdjustmentList(CDBUtil dbUtil, MySqlConnection conn, int shopId, int documentId, int startRecord, int limit, string matCode)
        {
            string sql = " SELECT a.Remark, a.DocumentID, a.DocumentStatus, b.ProductID, b.ProductAmount AS MaterialAmount, b.ProductDiscount, b.ProductAmount - b.ProductDiscount AS DiffAmount, " +
                         " b.UnitID, b.UnitName, b.UnitSmallAmount, c.MaterialID, c.MaterialName, c.MaterialCode, " +
                         " c.MaterialDeptID, c.MaterialCost, " +
                         " c.MaterialTaxType, d.UnitLargeID, c.UnitSmallID " +
                         " FROM document a, docdetailtemp b, materials c, UnitLarge d " +
                         " WHERE a.DocumentID=b.DocumentID AND a.ShopID=b.ShopID " +
                         " AND a.ShopID= " + shopId + " AND a.DocumentTypeID=" + DocumentTypeID +
                         " AND a.documentid = " + documentId +
                         " AND c.MaterialCode LIKE '" + matCode + "%' " +
                         " AND b.ProductID = c.MaterialID AND b.UnitID=d.UnitLargeID " +
                         " ORDER BY c.MaterialCode LIMIT " + startRecord + ", " + limit;

            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<Material> materialList = new List<Material>();
            while (reader.Read())
            {
                Material material = new Material();
                if (reader["DocumentID"] != DBNull.Value)
                    material.DocumentId = reader.GetInt32("DocumentID");
                if (reader["MaterialID"] != DBNull.Value)
                    material.MaterialId = reader.GetInt32("MaterialID");
                if (reader["MaterialCode"] != DBNull.Value)
                    material.MaterialCode = reader.GetString("MaterialCode");
                if (reader["MaterialName"] != DBNull.Value)
                    material.MaterialName = reader.GetString("MaterialName");
                if (reader["MaterialAmount"] != DBNull.Value)
                    material.MaterialAmount = reader.GetDecimal("MaterialAmount");
                if (reader["UnitName"] != DBNull.Value)
                    material.MaterialUnitName = reader.GetString("UnitName");
                if (reader["UnitLargeID"] != DBNull.Value)
                    material.MaterialUnitLargeId = reader.GetInt32("UnitLargeID");
                if (reader["UnitSmallID"] != DBNull.Value)
                    material.MaterialUnitSmallId = reader.GetInt32("UnitSmallID");
                if (reader["DocumentStatus"] != DBNull.Value)
                    material.DocumentStatus = reader.GetInt32("DocumentStatus");
                if (reader["ProductDiscount"] != DBNull.Value)
                    material.ProductDiscountAmount = reader.GetDecimal("ProductDiscount");
                if (reader["DiffAmount"] != DBNull.Value)
                    material.DiffAmount = reader.GetDecimal("DiffAmount");
                if (reader["UnitSmallAmount"] != DBNull.Value)
                    material.UnitSmallAmount = reader.GetDecimal("UnitSmallAmount");
                if (reader["UnitSmallAmount"] != DBNull.Value)
                    material.UnitSmallAmount = reader.GetDecimal("UnitSmallAmount");
                materialList.Add(material);

                if (reader["Remark"] != DBNull.Value)
                    Remark = reader.GetString("Remark");
            }
            reader.Close();
            return materialList;
        }

        public List<Material> MaterialWeeklyList(CDBUtil dbUtil, MySqlConnection conn, int shopId, int documentId, int startRecord, int limit, string matCode)
        {
            string sql = " SELECT a.Remark, a.DocumentID, a.DocumentStatus, b.ProductID, b.ProductAmount AS MaterialAmount, b.ProductDiscount, b.ProductAmount - b.ProductDiscount AS DiffAmount, " +
                         " b.UnitID, b.UnitName, b.UnitSmallAmount, c.MaterialID, c.MaterialName, c.MaterialCode, " +
                         " c.MaterialDeptID, c.MaterialCost, " +
                         " c.MaterialTaxType, d.UnitLargeID, c.UnitSmallID " +
                         " FROM document a, docdetailtemp b, materials c, UnitLarge d " +
                         " WHERE a.DocumentID=b.DocumentID AND a.ShopID=b.ShopID " +
                         " AND a.ShopID= " + shopId + " AND a.DocumentTypeID=" + DocumentTypeID +
                         " AND a.documentid = " + documentId +
                         " AND c.MaterialCode LIKE '" + matCode + "%' " +
                         " AND b.ProductID = c.MaterialID AND b.UnitID=d.UnitLargeID " +
                         " ORDER BY c.MaterialCode LIMIT " + startRecord + ", " + limit;

            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<Material> materialList = new List<Material>();
            while (reader.Read())
            {
                Material material = new Material();
                if (reader["DocumentID"] != DBNull.Value)
                    material.DocumentId = reader.GetInt32("DocumentID");
                if (reader["MaterialID"] != DBNull.Value)
                    material.MaterialId = reader.GetInt32("MaterialID");
                if (reader["MaterialCode"] != DBNull.Value)
                    material.MaterialCode = reader.GetString("MaterialCode");
                if (reader["MaterialName"] != DBNull.Value)
                    material.MaterialName = reader.GetString("MaterialName");
                if (reader["MaterialAmount"] != DBNull.Value)
                    material.MaterialAmount = reader.GetDecimal("MaterialAmount");
                if (reader["UnitName"] != DBNull.Value)
                    material.MaterialUnitName = reader.GetString("UnitName");
                if (reader["UnitLargeID"] != DBNull.Value)
                    material.MaterialUnitLargeId = reader.GetInt32("UnitLargeID");
                if (reader["UnitSmallID"] != DBNull.Value)
                    material.MaterialUnitSmallId = reader.GetInt32("UnitSmallID");
                if (reader["DocumentStatus"] != DBNull.Value)
                    material.DocumentStatus = reader.GetInt32("DocumentStatus");
                if (reader["ProductDiscount"] != DBNull.Value)
                    material.ProductDiscountAmount = reader.GetDecimal("ProductDiscount");
                if (reader["DiffAmount"] != DBNull.Value)
                    material.DiffAmount = reader.GetDecimal("DiffAmount");
                if (reader["UnitSmallAmount"] != DBNull.Value)
                    material.UnitSmallAmount = reader.GetDecimal("UnitSmallAmount");
                if (reader["UnitSmallAmount"] != DBNull.Value)
                    material.UnitSmallAmount = reader.GetDecimal("UnitSmallAmount");
                materialList.Add(material);

                if (reader["Remark"] != DBNull.Value)
                    Remark = reader.GetString("Remark");
            }
            reader.Close();
            return materialList;
        }

        public List<Material> MaterialWeeklyList(CDBUtil dbUtil, MySqlConnection conn, int shopId, int documentId)
        {
            string sql = " SELECT a.Remark, a.DocumentID, a.DocumentStatus, b.ProductID, b.ProductAmount AS MaterialAmount, b.ProductDiscount, b.ProductAmount - b.ProductDiscount AS DiffAmount, " +
                         " b.UnitID, b.UnitName, b.UnitSmallAmount, c.MaterialID, c.MaterialName, c.MaterialCode, " +
                         " c.MaterialDeptID, c.MaterialCost, " +
                         " c.MaterialTaxType, d.UnitLargeID, c.UnitSmallID " +
                         " FROM document a, docdetailtemp b, materials c, UnitLarge d " +
                         " WHERE a.DocumentID=b.DocumentID AND a.ShopID=b.ShopID " +
                         " AND a.ShopID= " + shopId + " AND a.DocumentTypeID=" + DocumentTypeID +
                         " AND a.documentid = " + documentId +
                         " AND b.ProductID = c.MaterialID AND b.UnitID=d.UnitLargeID " +
                         " ORDER BY c.MaterialCode";

            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<Material> materialList = new List<Material>();
            while (reader.Read())
            {
                Material material = new Material();
                if (reader["DocumentID"] != DBNull.Value)
                    material.DocumentId = reader.GetInt32("DocumentID");
                if (reader["MaterialID"] != DBNull.Value)
                    material.MaterialId = reader.GetInt32("MaterialID");
                if (reader["MaterialCode"] != DBNull.Value)
                    material.MaterialCode = reader.GetString("MaterialCode");
                if (reader["MaterialName"] != DBNull.Value)
                    material.MaterialName = reader.GetString("MaterialName");
                if (reader["MaterialAmount"] != DBNull.Value)
                    material.MaterialAmount = reader.GetDecimal("MaterialAmount");
                if (reader["UnitName"] != DBNull.Value)
                    material.MaterialUnitName = reader.GetString("UnitName");
                if (reader["UnitLargeID"] != DBNull.Value)
                    material.MaterialUnitLargeId = reader.GetInt32("UnitLargeID");
                if (reader["UnitSmallID"] != DBNull.Value)
                    material.MaterialUnitSmallId = reader.GetInt32("UnitSmallID");
                if (reader["DocumentStatus"] != DBNull.Value)
                    material.DocumentStatus = reader.GetInt32("DocumentStatus");
                if (reader["ProductDiscount"] != DBNull.Value)
                    material.ProductDiscountAmount = reader.GetDecimal("ProductDiscount");
                if (reader["DiffAmount"] != DBNull.Value)
                    material.DiffAmount = reader.GetDecimal("DiffAmount");
                if (reader["UnitSmallAmount"] != DBNull.Value)
                    material.UnitSmallAmount = reader.GetDecimal("UnitSmallAmount");
                if (reader["UnitSmallAmount"] != DBNull.Value)
                    material.UnitSmallAmount = reader.GetDecimal("UnitSmallAmount");
                materialList.Add(material);

                if (reader["Remark"] != DBNull.Value)
                    Remark = reader.GetString("Remark");
            }
            reader.Close();
            return materialList;
        }

        public List<Material> MaterialMonthlyList(CDBUtil dbUtil, MySqlConnection conn, int shopId, int documentId, int startRecord, int limit, string matCode)
        {
            string sql = @" SELECT a.Remark, a.DocumentID, a.DocumentStatus, b.UnitID, b.ProductID, b.ProductAmount AS MaterialAmount, b.ProductDiscount,
                         b.ProductAmount - b.ProductDiscount AS DiffAmount, b.UnitSmallAmount,
                         b.UnitName, c.MaterialID, c.MaterialName, c.MaterialCode,
                         c.MaterialDeptID, c.MaterialCost, c.MaterialTaxType, d.UnitLargeID, c.UnitSmallID
                         FROM document a, docdetailtemp b, materials c, UnitLarge d
                         WHERE a.DocumentID=b.DocumentID AND a.ShopID=b.ShopID
                         AND a.ShopID= " + shopId + @" AND a.DocumentTypeID=" + DocumentTypeID + @" AND a.documentid = " + documentId + @"
                         AND c.MaterialCode LIKE '" + matCode + "%' " + @"
                         AND b.ProductID = c.MaterialID AND b.UnitID=d.UnitLargeID " + @"
                         ORDER BY c.MaterialCode LIMIT " + startRecord + ", " + limit;

            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<Material> materialList = new List<Material>();
            while (reader.Read())
            {
                Material material = new Material();
                if (reader["DocumentID"] != DBNull.Value)
                    material.DocumentId = reader.GetInt32("DocumentID");
                if (reader["MaterialID"] != DBNull.Value)
                    material.MaterialId = reader.GetInt32("MaterialID");
                if (reader["MaterialCode"] != DBNull.Value)
                    material.MaterialCode = reader.GetString("MaterialCode");
                if (reader["MaterialName"] != DBNull.Value)
                    material.MaterialName = reader.GetString("MaterialName");
                if (reader["MaterialAmount"] != DBNull.Value)
                    material.MaterialAmount = reader.GetDecimal("MaterialAmount");
                if (reader["UnitName"] != DBNull.Value)
                    material.MaterialUnitName = reader.GetString("UnitName");
                if (reader["UnitLargeID"] != DBNull.Value)
                    material.MaterialUnitLargeId = reader.GetInt32("UnitLargeID");
                if (reader["UnitSmallID"] != DBNull.Value)
                    material.MaterialUnitSmallId = reader.GetInt32("UnitSmallID");
                if (reader["DocumentStatus"] != DBNull.Value)
                    material.DocumentStatus = reader.GetInt32("DocumentStatus");
                if (reader["ProductDiscount"] != DBNull.Value)
                    material.ProductDiscountAmount = reader.GetDecimal("ProductDiscount");
                if (reader["DiffAmount"] != DBNull.Value)
                    material.DiffAmount = reader.GetDecimal("DiffAmount");
                if (reader["UnitSmallAmount"] != DBNull.Value)
                    material.UnitSmallAmount = reader.GetDecimal("UnitSmallAmount");
                materialList.Add(material);

                if (reader["Remark"] != DBNull.Value)
                    Remark = reader.GetString("Remark");
            }
            reader.Close();
            return materialList;
        }

        public List<Material> MaterialMonthlyList(CDBUtil dbUtil, MySqlConnection conn, int shopId, int documentId)
        {
            string sql = @" SELECT a.Remark, a.DocumentID, a.DocumentStatus, b.UnitID, b.ProductID, b.ProductAmount AS MaterialAmount, b.ProductDiscount,
                         b.ProductAmount - b.ProductDiscount AS DiffAmount, b.UnitSmallAmount,
                         b.UnitName, c.MaterialID, c.MaterialName, c.MaterialCode,
                         c.MaterialDeptID, c.MaterialCost, c.MaterialTaxType, d.UnitLargeID, c.UnitSmallID
                         FROM document a, docdetailtemp b, materials c, UnitLarge d
                         WHERE a.DocumentID=b.DocumentID AND a.ShopID=b.ShopID
                         AND a.ShopID= " + shopId + @" AND a.DocumentTypeID=" + DocumentTypeID + @" AND a.documentid = " + documentId +
                         @" AND b.ProductID = c.MaterialID AND b.UnitID=d.UnitLargeID ORDER BY c.MaterialCode";

            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<Material> materialList = new List<Material>();
            while (reader.Read())
            {
                Material material = new Material();
                if (reader["DocumentID"] != DBNull.Value)
                    material.DocumentId = reader.GetInt32("DocumentID");
                if (reader["MaterialID"] != DBNull.Value)
                    material.MaterialId = reader.GetInt32("MaterialID");
                if (reader["MaterialCode"] != DBNull.Value)
                    material.MaterialCode = reader.GetString("MaterialCode");
                if (reader["MaterialName"] != DBNull.Value)
                    material.MaterialName = reader.GetString("MaterialName");
                if (reader["MaterialAmount"] != DBNull.Value)
                    material.MaterialAmount = reader.GetDecimal("MaterialAmount");
                if (reader["UnitName"] != DBNull.Value)
                    material.MaterialUnitName = reader.GetString("UnitName");
                if (reader["UnitLargeID"] != DBNull.Value)
                    material.MaterialUnitLargeId = reader.GetInt32("UnitLargeID");
                if (reader["UnitSmallID"] != DBNull.Value)
                    material.MaterialUnitSmallId = reader.GetInt32("UnitSmallID");
                if (reader["DocumentStatus"] != DBNull.Value)
                    material.DocumentStatus = reader.GetInt32("DocumentStatus");
                if (reader["ProductDiscount"] != DBNull.Value)
                    material.ProductDiscountAmount = reader.GetDecimal("ProductDiscount");
                if (reader["DiffAmount"] != DBNull.Value)
                    material.DiffAmount = reader.GetDecimal("DiffAmount");
                if (reader["UnitSmallAmount"] != DBNull.Value)
                    material.UnitSmallAmount = reader.GetDecimal("UnitSmallAmount");
                materialList.Add(material);

                if (reader["Remark"] != DBNull.Value)
                    Remark = reader.GetString("Remark");
            }
            reader.Close();
            return materialList;
        }

        public decimal GetMaterialDiffAmount(CDBUtil dbUtil, int shopId, int documentId, int materialId)
        {
            decimal diffAmount = 0;
            string sql = "";
            sql = "SELECT SUM(b.ProductAmount * c.MovementInStock) AS diffAmount " +
            " FROM document a " +
            " INNER JOIN docdetail b " +
            " ON a.DocumentID=b.DocumentID AND a.ShopID = b.ShopID " +
            " INNER JOIN documenttype c " +
            " ON a.DocumentTypeID=c.DocumentTypeID AND a.ShopID=c.ShopID " +
            " WHERE a.DocumentIdRef=" + documentId + " AND a.DocIDRefShopID=" + shopId + " AND c.LangID=2 " +
            " AND b.ProductID=" + materialId + " AND a.DocumentTypeID IN (" + DocumentTypeIdStr + ") " +
            " GROUP BY b.ProductID, b.ProductAmount";

            MySqlConnection conn = dbUtil.EstablishConnection();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                if (reader["diffAmount"] != DBNull.Value)
                    diffAmount = reader.GetDecimal("diffAmount");
            }
            reader.Close();
            conn.Close();
            return diffAmount;
        }

        public int CountMaterials(MySqlConnection conn, int documentId, int shopId, bool isApprove)
        {
            string docDetailTable = "docdetail";
            switch (isApprove)
            {
                case true:
                    docDetailTable = "docdetail";
                    break;
                case false:
                    docDetailTable = "docdetailtemp";
                    break;
            }

            string sql = "";
            sql = @" SELECT COUNT(b.DocDetailID)
                    FROM document a, " + docDetailTable + @" b
                    WHERE a.DocumentID=b.DocumentID AND a.ShopID=b.ShopID
                    AND a.ShopID= " + shopId + @" AND a.DocumentTypeID=" + DocumentTypeID + @" 
                    AND a.documentid = " + documentId;

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                try
                {
                    countRow = reader.GetInt32(0);
                }
                catch (Exception)
                {
                    countRow = 0;
                }
            }
            reader.Close();
            return countRow;
        }

        //for create adjust docdetail
        public List<Material> MaterialDailyAdjustList(CDBUtil dbUtil, MySqlConnection conn, int shopId, int documentId)
        {
            string sql = " SELECT a.Remark, a.DocumentID, a.DocumentStatus, b.ProductID, b.ProductAmount - b.ProductDiscount AS MaterialAmount, " +
                         " b.UnitID, b.UnitName, b.UnitSmallAmount, c.MaterialID, c.MaterialName, c.MaterialCode, " +
                         " c.MaterialDeptID, c.MaterialCost, " +
                         " c.MaterialTaxType, d.UnitLargeID, c.UnitSmallID " +
                         " FROM document a, docdetailtemp b, materials c, UnitLarge d " +
                         " WHERE a.DocumentID=b.DocumentID AND a.ShopID=b.ShopID " +
                         " AND a.ShopID= " + shopId + " AND a.DocumentTypeID=" + DocumentTypeID +
                         " AND a.documentid = " + documentId +
                         " AND b.ProductAmount - b.ProductDiscount <> 0 " +
                         " AND b.ProductID = c.MaterialID AND b.UnitID=d.UnitLargeID " +
                         " ORDER BY c.MaterialCode";
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<Material> materialList = new List<Material>();
            while (reader.Read())
            {
                Material material = new Material();
                if (reader["DocumentID"] != DBNull.Value)
                    material.DocumentId = reader.GetInt32("DocumentID");
                if (reader["MaterialID"] != DBNull.Value)
                    material.MaterialId = reader.GetInt32("MaterialID");
                if (reader["MaterialCode"] != DBNull.Value)
                    material.MaterialCode = reader.GetString("MaterialCode");
                if (reader["MaterialName"] != DBNull.Value)
                    material.MaterialName = reader.GetString("MaterialName");
                if (reader["MaterialAmount"] != DBNull.Value)
                    material.MaterialAmount = reader.GetDecimal("MaterialAmount");
                if (reader["UnitName"] != DBNull.Value)
                    material.MaterialUnitName = reader.GetString("UnitName");
                if (reader["UnitLargeID"] != DBNull.Value)
                    material.MaterialUnitLargeId = reader.GetInt32("UnitLargeID");
                if (reader["UnitSmallID"] != DBNull.Value)
                    material.MaterialUnitSmallId = reader.GetInt32("UnitSmallID");
                if (reader["DocumentStatus"] != DBNull.Value)
                    material.DocumentStatus = reader.GetInt32("DocumentStatus");
                if (reader["UnitSmallAmount"] != DBNull.Value)
                    material.UnitSmallAmount = reader.GetDecimal("UnitSmallAmount");
                materialList.Add(material);

                if (reader["Remark"] != DBNull.Value)
                    Remark = reader.GetString("Remark");
            }
            reader.Close();
            return materialList;
        }

        public List<Material> MaterialDailyList(CDBUtil dbUtil, MySqlConnection conn, int shopId, int documentId)
        {
            string sql = " SELECT a.Remark, a.DocumentID, a.DocumentStatus, b.ProductID, b.ProductAmount AS MaterialAmount, b.ProductDiscount, b.ProductAmount - b.ProductDiscount AS DiffAmount, " +
                         " b.UnitID, b.UnitName, b.UnitSmallAmount, c.MaterialID, c.MaterialName, c.MaterialCode, " +
                         " c.MaterialDeptID, c.MaterialCost, " +
                         " c.MaterialTaxType, d.UnitLargeID, c.UnitSmallID " +
                         " FROM document a, docdetailtemp b, materials c, UnitLarge d " +
                         " WHERE a.DocumentID=b.DocumentID AND a.ShopID=b.ShopID " +
                         " AND a.ShopID= " + shopId + " AND a.DocumentTypeID=" + DocumentTypeID +
                         " AND a.documentid = " + documentId +
                         " AND b.ProductID = c.MaterialID AND b.UnitID=d.UnitLargeID " +
                         " ORDER BY c.MaterialCode";
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<Material> materialList = new List<Material>();
            while (reader.Read())
            {
                Material material = new Material();
                if (reader["DocumentID"] != DBNull.Value)
                    material.DocumentId = reader.GetInt32("DocumentID");
                if (reader["MaterialID"] != DBNull.Value)
                    material.MaterialId = reader.GetInt32("MaterialID");
                if (reader["MaterialCode"] != DBNull.Value)
                    material.MaterialCode = reader.GetString("MaterialCode");
                if (reader["MaterialName"] != DBNull.Value)
                    material.MaterialName = reader.GetString("MaterialName");
                if (reader["MaterialAmount"] != DBNull.Value)
                    material.MaterialAmount = reader.GetDecimal("MaterialAmount");
                if (reader["UnitName"] != DBNull.Value)
                    material.MaterialUnitName = reader.GetString("UnitName");
                if (reader["UnitLargeID"] != DBNull.Value)
                    material.MaterialUnitLargeId = reader.GetInt32("UnitLargeID");
                if (reader["UnitSmallID"] != DBNull.Value)
                    material.MaterialUnitSmallId = reader.GetInt32("UnitSmallID");
                if (reader["DocumentStatus"] != DBNull.Value)
                    material.DocumentStatus = reader.GetInt32("DocumentStatus");
                if (reader["ProductDiscount"] != DBNull.Value)
                    material.ProductDiscountAmount = reader.GetDecimal("ProductDiscount");
                if (reader["DiffAmount"] != DBNull.Value)
                    material.DiffAmount = reader.GetDecimal("DiffAmount");
                if (reader["UnitSmallAmount"] != DBNull.Value)
                    material.UnitSmallAmount = reader.GetDecimal("UnitSmallAmount");
                if (reader["UnitSmallAmount"] != DBNull.Value)
                    material.UnitSmallAmount = reader.GetDecimal("UnitSmallAmount");
                materialList.Add(material);

                if (reader["Remark"] != DBNull.Value)
                    Remark = reader.GetString("Remark");
            }
            reader.Close();
            return materialList;
        }

        public void MaterialInfo(CDBUtil dbUtil, MySqlConnection conn, int materialId)
        {
            string sql = @"SELECT a.*,b.*,c.*
                        FROM materials a
                        LEFT JOIN materialdept b
                        ON a.MaterialDeptID=b.materialDeptID
                        LEFT JOIN materialgroup c
                        ON b.MaterialGroupID=c.MaterialGroupID
                        WHERE a.MaterialID=" + materialId + @"
                        AND a.Deleted=0";
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            while (reader.Read())
            {
                if (reader["MaterialID"] != DBNull.Value)
                    MaterialId = reader.GetInt32("MaterialID");
                if (reader["MaterialCode"] != DBNull.Value)
                    MaterialCode = reader.GetString("MaterialCode");
                if (reader["MaterialName"] != DBNull.Value)
                    MaterialName = reader.GetString("MaterialName");
                if (reader["MaterialDeptID"] != DBNull.Value)
                    MaterialDeptId = reader.GetInt32("MaterialDeptID");
                if (reader["MaterialTaxType"] != DBNull.Value)
                    MaterialTaxType = reader.GetInt32("MaterialTaxType");
                if (reader["MaterialGroupID"] != DBNull.Value)
                    MaterialGroupId = reader.GetInt32("MaterialGroupID");
            }
            reader.Close();
        }

        public List<Material> MaterialList(CDBUtil dbUtil, MySqlConnection conn, string matCode, int start, int limit, string configCountTable)
        {
            string sql = "";
            sql = " SELECT a.* FROM materials a " +
                " INNER JOIN " + configCountTable + " b " +
                " ON a.MaterialID=b.MaterialID " +
                " WHERE a.Deleted=0 " +
                " AND (a.MaterialCode LIKE '" + matCode + "%' OR a.MaterialName LIKE '" + matCode + "%') " +
                " GROUP BY a.MaterialID ORDER BY a.MaterialCode LIMIT " + start + ", " + limit;

            List<Material> materialList = new List<Material>();
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            while (reader.Read())
            {
                Material material = new Material();
                material.MaterialId = reader.GetInt32("MaterialID");
                material.MaterialCode = reader.GetString("MaterialCode");
                material.MaterialName = reader.GetString("MaterialName");
                materialList.Add(material);
            }
            reader.Close();
            return materialList;
        }

        public List<Material> MaterialList(CDBUtil dbUtil, MySqlConnection conn, int shopId, int materialGroupId, int materialDeptId)
        {
            string sql = "";
            /*" SELECT a.MaterialID, a.MaterialCode, a.MaterialName, a.MaterialTaxType, b.MaterialDeptID, " +
                         " b.MaterialDeptName, c.MaterialGroupID, c.MaterialGroupName, " +
                         " countConfig.ProductLevelID, countConfig.ShopID, IFNULL(countConfig.MaterialID, 0) AS materialConfig " +
                         " FROM materials a  " +
                         " LEFT JOIN materialdept b " +
                         " ON a.MaterialDeptID=b.MaterialDeptID " +
                         " LEFT JOIN materialgroup c " +
                         " ON b.MaterialGroupID=c.MaterialGroupID " +
                         " LEFT OUTER JOIN " +
                         " (SELECT * FROM " + CountConfigType + " WHERE productlevelid=" + shopId + " AND shopid=" + shopId + ") countConfig " +
                         " ON a.MaterialID=countConfig.MaterialID " +
                         " WHERE ";
            if (materialGroupId != 0)
                sql += " c.MaterialGroupID= " + materialGroupId + " AND ";
            if (materialDeptId != 0)
                sql += " b.MaterialDeptID=" + materialDeptId + " AND ";
            sql += " a.Deleted=0 AND b.Deleted=0 AND c.Deleted=0 ORDER BY a.MaterialCode ";*/
            sql = " SELECT c.MaterialID, c.MaterialCode, c.MaterialName, " +
                " c.MaterialTaxType, b.MaterialDeptID,  b.MaterialDeptName, " +
                " a.MaterialGroupID, a.MaterialGroupName, countConfig.ProductLevelID, countConfig.ShopID, " +
                " IFNULL(countConfig.MaterialID, 0) AS materialConfig  FROM materialgroup a   " +
                " LEFT JOIN materialdept b  ON a.MaterialGroupID=b.MaterialGroupID  " +
                " LEFT JOIN materials c  ON b.MaterialDeptID=c.MaterialDeptID  " +
                " LEFT OUTER JOIN  (SELECT * FROM " + CountConfigType +
                " WHERE productlevelid=" + shopId + " AND shopid=" + shopId + ") countConfig  " +
                " ON c.MaterialID=countConfig.MaterialID " +
                " WHERE ";
            if (materialGroupId != 0)
                sql += " a.MaterialGroupID= " + materialGroupId + " AND ";
            if (materialDeptId != 0)
                sql += " b.MaterialDeptID=" + materialDeptId + " AND  ";
            sql += " c.Deleted=0  ORDER BY c.MaterialCode";

            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<Material> listMaterial = new List<Material>();
            /*int matDeptId = 0;
            int matGroupId = 0;*/
            while (reader.Read())
            {
                /*if (matGroupId != reader.GetInt32("MaterialGroupID"))
                {
                    Material material = new Material();
                    material.MaterialGroupId = reader.GetInt32("MaterialGroupID");
                    material.MaterialDeptId = reader.GetInt32("MaterialDeptID");
                    material.MaterialCode = "Group : " + reader.GetString("MaterialGroupName");
                    material.MaterialName = "";
                    listMaterial.Add(material);

                    if (matDeptId != reader.GetInt32("MaterialDeptID"))
                    {
                        material = new Material();
                        material.MaterialGroupId = reader.GetInt32("MaterialGroupID");
                        material.MaterialDeptId = reader.GetInt32("MaterialDeptID");
                        material.MaterialCode = "Dept : " + reader.GetString("MaterialDeptName");
                        material.MaterialName = "";
                        listMaterial.Add(material);

                        material = new Material();
                        if (reader["MaterialID"] != DBNull.Value)
                            material.MaterialId = reader.GetInt32("MaterialID");
                        if (reader["MaterialDeptID"] != DBNull.Value)
                            material.MaterialDeptId = reader.GetInt32("MaterialDeptID");
                        if (reader["MaterialGroupID"] != DBNull.Value)
                            material.MaterialGroupId = reader.GetInt32("MaterialGroupID");
                        if (reader["MaterialCode"] != DBNull.Value)
                            material.MaterialCode = reader.GetString("MaterialCode");
                        if (reader["MaterialName"] != DBNull.Value)
                            material.MaterialName = reader.GetString("MaterialName");
                        if (reader["materialConfig"] != DBNull.Value)
                            material.MaterialConfig = reader.GetInt32("materialConfig");
                        if (reader["MaterialTaxType"] != DBNull.Value)
                            material.MaterialTaxType = reader.GetInt32("MaterialTaxType");
                        listMaterial.Add(material);
                    }
                }
                else
                {*/
                Material material = new Material();
                if (reader["MaterialID"] != DBNull.Value)
                    material.MaterialId = reader.GetInt32("MaterialID");
                if (reader["MaterialDeptID"] != DBNull.Value)
                {
                    material.MaterialDeptId = reader.GetInt32("MaterialDeptID");
                    material.MaterialDeptName = reader.GetString("MaterialDeptName");
                }
                if (reader["MaterialGroupID"] != DBNull.Value)
                {
                    material.MaterialGroupId = reader.GetInt32("MaterialGroupID");
                    material.MaterialGroupName = reader.GetString("MaterialGroupName");
                }
                if (reader["MaterialCode"] != DBNull.Value)
                    material.MaterialCode = reader.GetString("MaterialCode");
                if (reader["MaterialName"] != DBNull.Value)
                    material.MaterialName = reader.GetString("MaterialName");
                if (reader["materialConfig"] != DBNull.Value)
                    material.MaterialConfig = reader.GetInt32("materialConfig");
                if (reader["MaterialTaxType"] != DBNull.Value)
                    material.MaterialTaxType = reader.GetInt32("MaterialTaxType");
                listMaterial.Add(material);
                /*}
                matGroupId = reader.GetInt32("MaterialGroupID");
                matDeptId = reader.GetInt32("MaterialDeptID");*/
            }
            reader.Close();
            return listMaterial;
        }

        public List<Material> listMaterialGroup(CDBUtil dbUtil, MySqlConnection conn)
        {
            string sql = "SELECT * FROM materialgroup WHERE Deleted=0";
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);

            List<Material> listMaterialGroup = new List<Material>();
            while (reader.Read())
            {
                Material material = new Material();
                if (reader["MaterialGroupID"] != DBNull.Value)
                    material.MaterialGroupId = reader.GetInt32("MaterialGroupID");
                if (reader["MaterialGroupCode"] != DBNull.Value)
                    material.MaterialGroupCode = reader.GetString("MaterialGroupCode");
                if (reader["MaterialGroupName"] != DBNull.Value)
                    material.MaterialGroupName = reader.GetString("MaterialGroupName");

                listMaterialGroup.Add(material);
            }
            reader.Close();
            return listMaterialGroup;
        }

        public List<Material> listMaterialDept(CDBUtil dbUtil, MySqlConnection conn, int materialGroupId)
        {
            string sql = "SELECT * FROM materialdept WHERE Deleted=0";
            if (materialGroupId != 0)
                sql += " AND MaterialGroupID=" + materialGroupId;

            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);

            List<Material> listMaterialDept = new List<Material>();
            while (reader.Read())
            {
                Material material = new Material();
                if (reader["MaterialDeptID"] != DBNull.Value)
                    material.MaterialDeptId = reader.GetInt32("MaterialDeptID");
                if (reader["MaterialDeptCode"] != DBNull.Value)
                    material.MaterialDeptCode = reader.GetString("MaterialDeptCode");
                if (reader["MaterialDeptName"] != DBNull.Value)
                    material.MaterialDeptName = reader.GetString("MaterialDeptName");
                listMaterialDept.Add(material);
            }
            reader.Close();
            return listMaterialDept;
        }

        public int countMaterialSetting(MySqlConnection conn)
        {
            int count = 0;
            string strSql = "SELECT COUNT(*) FROM " + _countConfigType;
            MySqlCommand cmd = new MySqlCommand(strSql, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    count = reader.GetInt32(0);
                }
            }
            return count;
        }

        public bool saveCountConfig(CDBUtil dbUtil, MySqlConnection conn, int productlevelId,
            int shopId, List<Material> materialList, int materialGroupId, int materialDeptId)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlDel = new StringBuilder();

            try
            {
                if (materialList.Count > 0)
                {
                    List<Material> matListForDel = MaterialList(dbUtil, conn, shopId, materialGroupId, materialDeptId);
                    string materialId = "(";
                    int count = 0;
                    foreach (Material mat in matListForDel)
                    {
                        materialId += mat.MaterialId;
                        if (count < matListForDel.Count - 1)
                            materialId += ",";
                        count++;
                    }
                    materialId += ")";
                    string strSql = "DELETE FROM " + CountConfigType + " WHERE ProductLevelID=" + productlevelId +
                                     " AND ShopID=" + shopId + " AND MaterialID IN " + materialId;
                    dbUtil.sqlExecute(strSql, conn);

                    
                    if (CountConfigType.Equals("stockcountmaterial", StringComparison.InvariantCultureIgnoreCase))
                    {
                        sql.Append("INSERT INTO " + CountConfigType + " (StockCountTypeID, ProductLevelID, ShopID, MaterialID)  VALUES ");
                    }
                    else
                    {
                        sql.Append("INSERT INTO " + CountConfigType + " (ProductLevelID, ShopID, MaterialID)  VALUES ");
                    }

                    for (int i = 0; i <= materialList.Count - 1; i++)
                    {
                        //sqlDel.Append(" DELETE FROM " + CountConfigType + " WHERE ProductLevelID=" + productlevelId +
                        //               " AND ShopID=" + shopId + " AND MaterialID=" + materialList[i].MaterialId + ";");

                        if (CountConfigType.Equals("stockcountmaterial", StringComparison.InvariantCultureIgnoreCase))
                        {
                            sql.Append(" (" + _countTypeId + ", " + productlevelId + ", " + shopId + ", " + materialList[i].MaterialId + ")");

                        }
                        else
                        {
                            sql.Append("(" + productlevelId + ", " + shopId + ", " + materialList[i].MaterialId + ")");
                        }
                        if (i < materialList.Count - 1)
                            sql.Append(",");
                    }
                    //first delete old data
                    //dbUtil.sqlExecute(sqlDel.ToString(), conn);
                    dbUtil.sqlExecute(sql.ToString(), conn);

                }
                else
                {
                    dbUtil.sqlExecute(" DELETE FROM " + CountConfigType + " WHERE ProductLevelID=" + productlevelId +
                                       " AND ShopID=" + shopId, conn);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Material> ListCountDiffMaterial(CDBUtil dbUtil, MySqlConnection conn, DateTime dateFrom,
          DateTime dateTo, int inv, int docType, string matCode)
        {
            string sql = "";
            sql = " SELECT a.DocumentDate, b.ProductDiscount, SUM(b.ProductAmount) AS ProductAmount, " +
                 " b.ProductAmount-ProductDiscount AS ProductAdjAmount, c.MaterialID, " +
                    " c.MaterialCode, c.MaterialName, b.UnitName, d.UnitLargeID " +
                    " FROM document a " +
                    " INNER JOIN docdetail b " +
                    " ON a.DocumentID=b.DocumentID AND a.ShopID=b.ShopID " +
                    " INNER JOIN materials c " +
                    " ON b.ProductID=c.MaterialID " +
                    " INNER JOIN unitlarge d " +
                    " ON b.UnitID=d.UnitLargeID " +
                    " WHERE a.DocumentTypeID=" + docType + " AND a.ShopID=" + inv +
                    " AND a.DocumentDate BETWEEN {d '" + dateFrom.ToString("yyyy'-'MM'-'dd", dateProvider) + "'}  " +
                    " AND {d '" + dateTo.ToString("yyyy'-'MM'-'dd", dateProvider) + "'} " +
                    " AND b.ProductAmount - b.ProductDiscount <> 0 AND (c.MaterialCode LIKE '" + matCode + "%' OR c.MaterialName LIKE '" + matCode + "%' )" +
                    " GROUP BY b.ProductID ";
            List<Material> materialCountDiffList = new List<Material>();
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            while (reader.Read())
            {
                Material material = new Material();
                material.DocumentDate = reader.GetDateTime("DocumentDate");
                material.MaterialId = reader.GetInt32("MaterialID");
                material.MaterialCode = reader.GetString("MaterialCode");
                material.MaterialName = reader.GetString("MaterialName");
                material.ProductDiscount = reader.GetDecimal("ProductAmount");
                material.MaterialAmount = reader.GetDecimal("ProductAdjAmount");
                material.MaterialUnitLargeId = reader.GetInt32("UnitLargeID");
                material.MaterialUnitName = reader.GetString("UnitName");
                materialCountDiffList.Add(material);
            }
            reader.Close();
            return materialCountDiffList;
        }

        protected List<Material> QueryCountDiffMaterial(CDBUtil dbUtil,
            MySqlConnection conn, DateTime dateFrom, DateTime dateTo,
            int inv, int docType, string groupBy)
        {
            string sql = "";
            sql = " SELECT a.DocumentDate, b.ProductAmount-ProductDiscount AS ProductAmount, c.MaterialID, " +
                    " c.MaterialCode, c.MaterialName, b.UnitName, d.UnitLargeID " +
                    " FROM document a " +
                    " INNER JOIN docdetail b " +
                    " ON a.DocumentID=b.DocumentID AND a.ShopID=b.ShopID " +
                    " INNER JOIN materials c " +
                    " ON b.ProductID=c.MaterialID " +
                    " INNER JOIN unitlarge d " +
                    " ON b.UnitID=d.UnitLargeID " +
                    " WHERE a.DocumentTypeID=" + docType + " AND a.ShopID=" + inv +
                    " AND a.DocumentDate BETWEEN {d '" + dateFrom.ToString("yyyy'-'MM'-'dd", dateProvider) + "'}  " +
                    " AND {d '" + dateTo.ToString("yyyy'-'MM'-'dd", dateProvider) + "'} " +
                    " AND b.ProductAmount - b.ProductDiscount <> 0 " +
                    " GROUP BY " + groupBy;

            List<Material> materialCountDiffList = new List<Material>();
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);

            while (reader.Read())
            {
                Material material = new Material();
                material.DocumentDate = reader.GetDateTime("DocumentDate");
                material.MaterialId = reader.GetInt32("MaterialID");
                material.MaterialCode = reader.GetString("MaterialCode");
                material.MaterialName = reader.GetString("MaterialName");
                material.MaterialUnitLargeId = reader.GetInt32("UnitLargeID");
                material.MaterialUnitName = reader.GetString("UnitName");
                material.MaterialAmount = reader.GetDecimal("ProductAmount");

                materialCountDiffList.Add(material);
            }
            reader.Close();

            return materialCountDiffList;
        }

        public List<Material> ListCountDiffMaterial(CDBUtil dbUtil, MySqlConnection conn, DateTime dateFrom,
            DateTime dateTo, int inv, int docType)
        {
            List<Material> materialCountDiffList = new List<Material>();
            materialCountDiffList = QueryCountDiffMaterial(dbUtil, conn, dateFrom,
                dateTo, inv, docType, "b.ProductID");

            List<Material> materialCountDiffListByDate = new List<Material>();
            materialCountDiffListByDate = QueryCountDiffMaterial(dbUtil, conn, dateFrom,
                dateTo, inv, docType, "a.DocumentDate, b.ProductID");

            List<Material> matList = new List<Material>();
            for (int i = 0; i < materialCountDiffList.Count; i++)
            {
                Material material = new Material();
                material.DocumentDate = materialCountDiffList[i].DocumentDate;
                material.MaterialId = materialCountDiffList[i].MaterialId;
                material.MaterialCode = materialCountDiffList[i].MaterialCode;
                material.MaterialName = materialCountDiffList[i].MaterialName;
                material.MaterialUnitLargeId = materialCountDiffList[i].MaterialUnitLargeId;
                material.MaterialUnitName = materialCountDiffList[i].MaterialUnitName;

                int startDay = dateFrom.Day;
                int endDay = dateTo.Day;

                material.matListInMat = new List<Material>();
                for (int j = startDay; j <= endDay; j++)
                {
                    Material matInList = new Material();
                    matInList.MaterialAmount = 0;
                    for (int k = 0; k < materialCountDiffListByDate.Count; k++)
                    {
                        if (j == materialCountDiffListByDate[k].DocumentDate.Day &&
                            materialCountDiffList[i].MaterialId == materialCountDiffListByDate[k].MaterialId)
                        {
                            matInList.MaterialAmount = materialCountDiffListByDate[k].MaterialAmount;
                        }
                    }
                    material.matListInMat.Add(matInList);
                }

                matList.Add(material);
            }
            return matList;
        }

        protected bool ReadMaterialIfNotFoundInShowOrHideStockAmount(CDBUtil dbUtil, MySqlConnection conn, int productLevelId, int materialGroupId)
        {
            bool found = false;
            string sql = "SELECT c.MaterialGroupID FROM materials a " +
                    " INNER JOIN materialdept b ON a.MaterialDeptID=b.MaterialDeptID " +
                    " INNER JOIN materialgroup c ON b.MaterialGroupID=c.MaterialGroupID " +
                    " WHERE a.MaterialID = " + productLevelId;
            MySqlCommand cmd = new MySqlCommand();
            cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                if (reader["MaterialGroupID"] != DBNull.Value)
                    this.MaterialGroupId = reader.GetInt32("MaterialGroupID");
                found = true;
            }
            reader.Close();
            return found;
        }

        public bool FindMaterialThatShowOrHideAmount(CDBUtil dbUtil, MySqlConnection conn, int productLevelId, int materialGroupId)
        {
            bool found = false;
            string sql = "";
            sql = "SELECT * FROM materialgroupsettingstockcountamount WHERE ProductLevelID=" + productLevelId +
                " AND MaterialGroupID=" + materialGroupId;
            MySqlCommand cmd = new MySqlCommand(sql, conn);

            MySqlDataReader reader;
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    if (reader["MaterialGroupID"] != DBNull.Value)
                        this.MaterialGroupId = reader.GetInt32("MaterialGroupID");
                    found = true;
                }

                if (!reader.Read())
                {
                    reader.Close();
                    //found = ReadMaterialIfNotFoundInShowOrHideStockAmount(dbUtil, conn, productLevelId, materialGroupId);
                }
                reader.Close();
            }
            catch
            {
                // if materialgroupsettingstockcountamount dose not exits 
                found = ReadMaterialIfNotFoundInShowOrHideStockAmount(dbUtil, conn, productLevelId, materialGroupId);
            }
            return found;
        }

        public bool saveMaterialGroupCountConfig(CDBUtil dbUtil, MySqlConnection conn, int productlevelId, int shopId, List<Material> materialList)
        {
            if (productlevelId == 0)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" DELETE FROM materialgroupsettingstockcountamount");
                dbUtil.sqlExecute(sql.ToString(), conn);

                sql = new StringBuilder();
                StringBuilder sqlInsert = new StringBuilder();
                MySqlDataReader reader = dbUtil.sqlRetrive("SELECT * FROM ProductLevel WHERE Deleted=0", conn);

                while (reader.Read())
                {
                    sqlInsert.Append("INSERT INTO materialgroupsettingstockcountamount (ProductLevelID, ShopID, MaterialGroupID) VALUES ");

                    for (int i = 0; i <= materialList.Count - 1; i++)
                    {
                        sql.Append(" DELETE FROM materialgroupsettingstockcountamount WHERE ProductLevelID=" +
                            reader.GetInt32("ProductLevelID") + " AND ShopID=" + reader.GetInt32("ProductLevelID") +
                            " AND MaterialGroupID=" + materialList[i].MaterialGroupId + ";");

                        /*sql.Append("INSERT INTO materialgroupsettingstockcountamount (ProductLevelID, ShopID, MaterialGroupID) " +
                                " VALUES (" + readeterialGroupId + ");");*/
                        sqlInsert.Append("(" + reader.GetInt32("ProductLevelID") + ", " + reader.GetInt32("ProductLevelID") + ", " +
                                materialList[i].MaterialGroupId + ")");
                        if (i < materialList.Count - 1)
                            sqlInsert.Append(", ");
                    }
                    sqlInsert.Append(";");
                }
                reader.Close();

                sql.Append(sqlInsert);
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql.ToString();
                cmd.Connection = conn;

                MySqlTransaction transaction;

                try
                {
                    cmd.ExecuteNonQuery();
                    //transaction.Commit();
                }
                catch
                {
                    //transaction.Rollback();
                }
            }
            else
            {
                string sql = "";
                sql = " DELETE FROM materialgroupsettingstockcountamount WHERE ProductLevelID=" + productlevelId + " AND " +
                        " ShopID=" + shopId;
                dbUtil.sqlExecute(sql, conn);

                for (int i = 0; i <= materialList.Count - 1; i++)
                {
                    sql = " DELETE FROM materialgroupsettingstockcountamount WHERE ProductLevelID=" + productlevelId + " AND " +
                        " ShopID=" + shopId + " AND MaterialGroupID=" + materialList[i].MaterialGroupId;
                    dbUtil.sqlExecute(sql, conn);

                    sql = "INSERT INTO materialgroupsettingstockcountamount (ProductLevelID, ShopID, MaterialGroupID) " +
                            " VALUES (" + productlevelId + ", " + shopId + ", " + materialList[i].MaterialGroupId + ")";
                    dbUtil.sqlExecute(sql, conn);
                }
            }
            return true;
        }

        public int GetMaterialId(CDBUtil dbUtil, MySqlConnection conn, string materialCode)
        {
            int materialId = 0;
            MySqlDataReader reader = dbUtil.sqlRetrive("SELECT * FROM materials WHERE MaterialCode='" + materialCode + "' AND Deleted=0", conn);
            if (reader.Read())
            {
                if (reader["MaterialID"] != DBNull.Value)
                    materialId = reader.GetInt32("MaterialID");
            }
            reader.Close();
            return materialId;
        }
    }
}
