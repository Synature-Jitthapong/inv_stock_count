using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using POSMySQL.POSControl;

namespace InventoryStockCount
{
    public class StockCountHistory : Document
    {
        private string _staffCode;
        private string _staffFirstName;
        private string _staffLastName;
        private string _approveStaffCode;
        private string _approveStaffFirstName;
        private string _approveStaffLastName;
        private string _updateStaffCode;
        private string _updateStaffFirstName;
        private string _updateStaffLastName;

        IFormatProvider dateProvider = System.Globalization.CultureInfo.InvariantCulture;

        public string UpdateStaffLastName
        {
            get { return _updateStaffLastName; }
            set { _updateStaffLastName = value; }
        }
        public string UpdateStaffCode
        {
            get { return _updateStaffCode; }
            set { _updateStaffCode = value; }
        }

        public string UpdateStaffFirstName
        {
            get { return _updateStaffFirstName; }
            set { _updateStaffFirstName = value; }
        }

        public string StaffLastName
        {
            get { return _staffLastName; }
            set { _staffLastName = value; }
        }

        public string StaffFirstName
        {
            get { return _staffFirstName; }
            set { _staffFirstName = value; }
        }

        public string StaffCode
        {
            get { return _staffCode; }
            set { _staffCode = value; }
        }

        public string ApproveStaffCode
        {
            get { return _approveStaffCode; }
            set { _approveStaffCode = value; }
        }

        public string ApproveStaffFirstName
        {
            get { return _approveStaffFirstName; }
            set { _approveStaffFirstName = value; }
        }

        public string ApproveStaffLastName
        {
            get { return _approveStaffLastName; }
            set { _approveStaffLastName = value; }
        }

        public StockCountHistory()
        {

        }

        public List<Material> stockCountMonthlyHistoryDetail(CDBUtil dbUtil, MySqlConnection conn,
           int shopId, int documentId, bool isApprove, int startRec, int limit)
        {
            string joinDocdetail = "";
            if (isApprove)
                joinDocdetail = "docdetail";
            else
                joinDocdetail = "docdetailtemp";

            string sql = @" SELECT a.Remark, a.DocumentID, a.documentStatus, b.ProductID, b.ProductAmount AS MaterialAmount, b.ProductDiscount,
                         b.ProductAmount-b.ProductDiscount AS DiffAmount, b.UnitSmallAmount,
                         b.UnitName, c.MaterialID, c.MaterialName, c.MaterialCode,
                         c.MaterialDeptID, c.MaterialCost, c.MaterialTaxType, c.UnitSmallID, d.UnitLargeID 
                         FROM document a, " + joinDocdetail + @" b, materials c, unitlarge d 
                         WHERE a.DocumentID=b.DocumentID AND a.ShopID=b.ShopID
                         AND a.ShopID= " + shopId + @" AND a.DocumentTypeID=7
                         AND a.documentid = " + documentId + @" AND b.ProductID = c.MaterialID
                         AND b.UnitID=d.UnitLargeID AND c.Deleted=0  ORDER BY c.MaterialCode
                         LIMIT " + startRec + @", " + limit;

            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<Material> materialList = new List<Material>();
            while (reader.Read())
            {
                if (reader["Remark"] != DBNull.Value)
                    Remark = reader.GetString("Remark");

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
                if (reader["UnitSmallID"] != DBNull.Value)
                    material.MaterialUnitSmallId = reader.GetInt32("UnitSmallID");
                if (reader["UnitLargeID"] != DBNull.Value)
                    material.MaterialUnitLargeId = reader.GetInt32("UnitLargeID");
                if (reader["DocumentStatus"] != DBNull.Value)
                    material.DocumentStatus = reader.GetInt32("DocumentStatus");
                if (reader["ProductDiscount"] != DBNull.Value)
                    material.ProductDiscountAmount = reader.GetDecimal("ProductDiscount");
                if (reader["DiffAmount"] != DBNull.Value)
                    material.DiffAmount = reader.GetDecimal("DiffAmount");
                if (reader["UnitSmallAmount"] != DBNull.Value)
                    material.UnitSmallAmount = reader.GetDecimal("UnitSmallAmount");
                materialList.Add(material);
            }
            reader.Close();
            return materialList;
        }

        public List<Material> stockCountMonthlyHistoryDetail(CDBUtil dbUtil, MySqlConnection conn,
            int shopId, int documentId, bool isApprove)
        {
            string joinDocdetail = "";
            if (isApprove)
                joinDocdetail = "docdetail";
            else
                joinDocdetail = "docdetailtemp";

            string sql = @" SELECT a.Remark, a.DocumentID, a.documentStatus, b.ProductID, b.ProductAmount AS MaterialAmount, b.ProductDiscount,
                         b.ProductAmount-b.ProductDiscount AS DiffAmount, b.UnitSmallAmount,
                         b.UnitName, c.MaterialID, c.MaterialName, c.MaterialCode,
                         c.MaterialDeptID, c.MaterialCost, c.MaterialTaxType, c.UnitSmallID, d.UnitLargeID 
                         FROM document a, " + joinDocdetail + @" b, materials c, unitlarge d 
                         WHERE a.DocumentID=b.DocumentID AND a.ShopID=b.ShopID
                         AND a.ShopID= " + shopId + @" AND a.DocumentTypeID=7
                         AND a.documentid = " + documentId + @" AND b.ProductID = c.MaterialID
                         AND b.UnitID=d.UnitLargeID AND c.Deleted=0  ORDER BY c.MaterialCode";

            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<Material> materialList = new List<Material>();
            while (reader.Read())
            {
                if (reader["Remark"] != DBNull.Value)
                    Remark = reader.GetString("Remark");

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
                if (reader["UnitSmallID"] != DBNull.Value)
                    material.MaterialUnitSmallId = reader.GetInt32("UnitSmallID");
                if (reader["UnitLargeID"] != DBNull.Value)
                    material.MaterialUnitLargeId = reader.GetInt32("UnitLargeID");
                if (reader["DocumentStatus"] != DBNull.Value)
                    material.DocumentStatus = reader.GetInt32("DocumentStatus");
                if (reader["ProductDiscount"] != DBNull.Value)
                    material.ProductDiscountAmount = reader.GetDecimal("ProductDiscount");
                if (reader["DiffAmount"] != DBNull.Value)
                    material.DiffAmount = reader.GetDecimal("DiffAmount");
                if (reader["UnitSmallAmount"] != DBNull.Value)
                    material.UnitSmallAmount = reader.GetDecimal("UnitSmallAmount");
                materialList.Add(material);
            }
            reader.Close();
            return materialList;
        }

        public List<Material> stockCountAdjustmentHistoryDetail(CDBUtil dbUtil, MySqlConnection conn, int shopId, int documentId, 
            bool isApprove, int startRec, int limit)
        {
            string joinDocdetail = "";
            if (isApprove)
                joinDocdetail = "docdetail";
            else
                joinDocdetail = "docdetailtemp";

            string sql = @" SELECT a.Remark, a.DocumentID, a.DocumentStatus, b.ProductID, b.ProductAmount AS MaterialAmount, b.ProductDiscount, 
                         b.ProductAmount-b.ProductDiscount AS DiffAmount, b.UnitSmallAmount,
                         b.UnitName, c.MaterialID, c.MaterialName, c.MaterialCode,
                         c.MaterialDeptID, c.MaterialCost, c.MaterialTaxType, c.UnitSmallID, d.UnitLargeID
                         FROM document a, " + joinDocdetail + @" b, materials c, unitlarge d
                         WHERE a.DocumentID=b.DocumentID
                         AND a.ShopID=b.ShopID 
                         AND a.ShopID= " + shopId + @" AND a.DocumentTypeID=57
                         AND a.documentid = " + documentId + @"
                         AND b.ProductID = c.MaterialID " + @"
                         AND b.UnitID=d.UnitLargeID AND c.Deleted=0 ORDER BY c.MaterialCode
                         LIMIT " + startRec + @", " + limit;

            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<Material> materialList = new List<Material>();
            while (reader.Read())
            {
                if (reader["Remark"] != DBNull.Value)
                    Remark = reader.GetString("Remark");

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
                if (reader["UnitSmallID"] != DBNull.Value)
                    material.MaterialUnitSmallId = reader.GetInt32("UnitSmallID");
                if (reader["UnitLargeID"] != DBNull.Value)
                    material.MaterialUnitLargeId = reader.GetInt32("UnitLargeID");
                if (reader["ProductDiscount"] != DBNull.Value)
                    material.ProductDiscountAmount = reader.GetDecimal("ProductDiscount");
                if (reader["DiffAmount"] != DBNull.Value)
                    material.DiffAmount = reader.GetDecimal("DiffAmount");
                if (reader["UnitSmallAmount"] != DBNull.Value)
                    material.UnitSmallAmount = reader.GetDecimal("UnitSmallAmount");
                if (reader["DocumentStatus"] != DBNull.Value)
                    material.DocumentStatus = reader.GetInt32("DocumentStatus");
                materialList.Add(material);
            }
            reader.Close();
            return materialList;
        }

        public List<Material> stockCountAdjustmentHistoryDetail(CDBUtil dbUtil, MySqlConnection conn, int shopId, int documentId, bool isApprove)
        {
            string joinDocdetail = "";
            if (isApprove)
                joinDocdetail = "docdetail";
            else
                joinDocdetail = "docdetailtemp";

            string sql = @" SELECT a.Remark, a.DocumentID, a.DocumentStatus, b.ProductID, b.ProductAmount AS MaterialAmount, b.ProductDiscount, 
                         b.ProductAmount-b.ProductDiscount AS DiffAmount, b.UnitSmallAmount,
                         b.UnitName, c.MaterialID, c.MaterialName, c.MaterialCode,
                         c.MaterialDeptID, c.MaterialCost, c.MaterialTaxType, c.UnitSmallID, d.UnitLargeID
                         FROM document a, " + joinDocdetail + @" b, materials c, unitlarge d
                         WHERE a.DocumentID=b.DocumentID
                         AND a.ShopID=b.ShopID 
                         AND a.ShopID= " + shopId + @" AND a.DocumentTypeID=57
                         AND a.documentid = " + documentId + @"
                         AND b.ProductID = c.MaterialID " + @"
                         AND b.UnitID=d.UnitLargeID AND c.Deleted=0 ORDER BY c.MaterialCode";
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<Material> materialList = new List<Material>();
            while (reader.Read())
            {
                if (reader["Remark"] != DBNull.Value)
                    Remark = reader.GetString("Remark");

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
                if (reader["UnitSmallID"] != DBNull.Value)
                    material.MaterialUnitSmallId = reader.GetInt32("UnitSmallID");
                if (reader["UnitLargeID"] != DBNull.Value)
                    material.MaterialUnitLargeId = reader.GetInt32("UnitLargeID");
                if (reader["ProductDiscount"] != DBNull.Value)
                    material.ProductDiscountAmount = reader.GetDecimal("ProductDiscount");
                if (reader["DiffAmount"] != DBNull.Value)
                    material.DiffAmount = reader.GetDecimal("DiffAmount");
                if (reader["UnitSmallAmount"] != DBNull.Value)
                    material.UnitSmallAmount = reader.GetDecimal("UnitSmallAmount");
                if (reader["DocumentStatus"] != DBNull.Value)
                    material.DocumentStatus = reader.GetInt32("DocumentStatus");
                materialList.Add(material);
            }
            reader.Close();
            return materialList;
        }

        public List<Material> stockCountWeeklyHistoryDetail(CDBUtil dbUtil, MySqlConnection conn, int shopId, 
            int documentId, bool isApprove, int startRec, int limit)
        {
            string joinDocdetail = "";
            if (isApprove)
                joinDocdetail = "docdetail";
            else
                joinDocdetail = "docdetailtemp";

            string sql = @" SELECT a.Remark, a.DocumentID, a.DocumentStatus, b.ProductID, b.ProductAmount AS MaterialAmount, b.ProductDiscount, b.ProductAmount-b.ProductDiscount AS DiffAmount,
                         b.UnitName, b.UnitSmallAmount, c.MaterialID, c.MaterialName, c.MaterialCode,
                         c.MaterialDeptID, c.MaterialCost, c.MaterialTaxType, c.UnitSmallID, d.UnitLargeID
                         FROM document a, " + joinDocdetail + @" b, materials c, unitlarge d
                         WHERE a.DocumentID=b.DocumentID
                         AND a.ShopID=b.ShopID
                         AND a.ShopID= " + shopId + @" AND a.DocumentTypeID=30
                         AND a.documentid = " + documentId + @"
                         AND b.ProductID = c.MaterialID " + @"
                         AND b.UnitID=d.UnitLargeID AND c.Deleted=0 ORDER BY c.MaterialCode
                         LIMIT " + startRec + @", " + limit;

            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<Material> materialList = new List<Material>();
            while (reader.Read())
            {
                if (reader["Remark"] != DBNull.Value)
                    Remark = reader.GetString("Remark");

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
                if (reader["UnitSmallID"] != DBNull.Value)
                    material.MaterialUnitSmallId = reader.GetInt32("UnitSmallID");
                if (reader["UnitLargeID"] != DBNull.Value)
                    material.MaterialUnitLargeId = reader.GetInt32("UnitLargeID");
                if (reader["DocumentStatus"] != DBNull.Value)
                    material.DocumentStatus = reader.GetInt32("DocumentStatus");
                if (reader["ProductDiscount"] != DBNull.Value)
                    material.ProductDiscountAmount = reader.GetDecimal("ProductDiscount");
                if (reader["DiffAmount"] != DBNull.Value)
                    material.DiffAmount = reader.GetDecimal("DiffAmount");
                if (reader["UnitSmallAmount"] != DBNull.Value)
                    material.UnitSmallAmount = reader.GetDecimal("UnitSmallAmount");
                materialList.Add(material);
            }
            reader.Close();
            return materialList;
        }

        public List<Material> stockCountWeeklyHistoryDetail(CDBUtil dbUtil, MySqlConnection conn, int shopId, int documentId, bool isApprove)
        {
            string joinDocdetail = "";
            if (isApprove)
                joinDocdetail = "docdetail";
            else
                joinDocdetail = "docdetailtemp";

            string sql = @" SELECT a.Remark, a.DocumentID, a.DocumentStatus, b.ProductID, b.ProductAmount AS MaterialAmount, b.ProductDiscount, b.ProductAmount-b.ProductDiscount AS DiffAmount,
                         b.UnitName, b.UnitSmallAmount, c.MaterialID, c.MaterialName, c.MaterialCode,
                         c.MaterialDeptID, c.MaterialCost, c.MaterialTaxType, c.UnitSmallID, d.UnitLargeID
                         FROM document a, " + joinDocdetail + @" b, materials c, unitlarge d
                         WHERE a.DocumentID=b.DocumentID
                         AND a.ShopID=b.ShopID
                         AND a.ShopID= " + shopId + @" AND a.DocumentTypeID=30
                         AND a.documentid = " + documentId + @"
                         AND b.ProductID = c.MaterialID " + @"
                         AND b.UnitID=d.UnitLargeID AND c.Deleted=0 ORDER BY c.MaterialCode";

            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<Material> materialList = new List<Material>();
            while (reader.Read())
            {
                if (reader["Remark"] != DBNull.Value)
                    Remark = reader.GetString("Remark");

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
                if (reader["UnitSmallID"] != DBNull.Value)
                    material.MaterialUnitSmallId = reader.GetInt32("UnitSmallID");
                if (reader["UnitLargeID"] != DBNull.Value)
                    material.MaterialUnitLargeId = reader.GetInt32("UnitLargeID");
                if (reader["DocumentStatus"] != DBNull.Value)
                    material.DocumentStatus = reader.GetInt32("DocumentStatus");
                if (reader["ProductDiscount"] != DBNull.Value)
                    material.ProductDiscountAmount = reader.GetDecimal("ProductDiscount");
                if (reader["DiffAmount"] != DBNull.Value)
                    material.DiffAmount = reader.GetDecimal("DiffAmount");
                if (reader["UnitSmallAmount"] != DBNull.Value)
                    material.UnitSmallAmount = reader.GetDecimal("UnitSmallAmount");
                materialList.Add(material);
            }
            reader.Close();
            return materialList;
        }

        public List<Material> stockCountDailyHistoryDetail(CDBUtil dbUtil, MySqlConnection conn, int shopId, int documentId, 
            bool isApprove, int startRec, int limit)
        {
            string joinDocdetail = "";
            if (isApprove)
                joinDocdetail = "docdetail";
            else
                joinDocdetail = "docdetailtemp";

            string sql = @" SELECT a.Remark, a.DocumentID, a.DocumentStatus, b.ProductID, b.ProductAmount AS MaterialAmount, b.ProductDiscount, b.ProductAmount-b.ProductDiscount AS DiffAmount,
                        b.UnitName, b.UnitSmallAmount, c.MaterialID, c.MaterialName, c.MaterialCode,
                        c.MaterialDeptID, c.MaterialCost, c.MaterialTaxType, c.UnitSmallID, d.UnitLargeID
                        FROM document a, " + joinDocdetail + @" b, materials c, unitlarge d
                        WHERE a.DocumentID=b.DocumentID AND a.ShopID=b.ShopID
                        AND a.ShopID= " + shopId + @" AND a.DocumentTypeID=24
                        AND a.documentid = " + documentId + @"
                        AND b.ProductID = c.MaterialID AND b.UnitID=d.UnitLargeID AND c.Deleted=0 ORDER BY c.MaterialCode
                        LIMIT " + startRec + @", " + limit;

            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<Material> materialList = new List<Material>();
            while (reader.Read())
            {
                if (reader["Remark"] != DBNull.Value)
                    Remark = reader.GetString("Remark");

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
                if (reader["UnitSmallID"] != DBNull.Value)
                    material.MaterialUnitSmallId = reader.GetInt32("UnitSmallID");
                if (reader["UnitLargeID"] != DBNull.Value)
                    material.MaterialUnitLargeId = reader.GetInt32("UnitLargeID");
                if (reader["ProductDiscount"] != DBNull.Value)
                    material.ProductDiscountAmount = reader.GetDecimal("ProductDiscount");
                if (reader["DiffAmount"] != DBNull.Value)
                    material.DiffAmount = reader.GetDecimal("DiffAmount");
                if (reader["DocumentStatus"] != DBNull.Value)
                    material.DocumentStatus = reader.GetInt32("DocumentStatus");
                if (reader["UnitSmallAmount"] != DBNull.Value)
                    material.UnitSmallAmount = reader.GetDecimal("UnitSmallAmount");
                materialList.Add(material);
            }
            reader.Close();
            return materialList;
        }

        public List<Material> stockCountDailyHistoryDetail(CDBUtil dbUtil, MySqlConnection conn, int shopId, int documentId, bool isApprove)
        {
            string joinDocdetail = "";
            if (isApprove)
                joinDocdetail = "docdetail";
            else
                joinDocdetail = "docdetailtemp";

            string sql = @" SELECT a.Remark, a.DocumentID, a.DocumentStatus, b.ProductID, b.ProductAmount AS MaterialAmount, b.ProductDiscount, b.ProductAmount-b.ProductDiscount AS DiffAmount,
                        b.UnitName, b.UnitSmallAmount, c.MaterialID, c.MaterialName, c.MaterialCode,
                        c.MaterialDeptID, c.MaterialCost, c.MaterialTaxType, c.UnitSmallID, d.UnitLargeID
                        FROM document a, " + joinDocdetail + @" b, materials c, unitlarge d
                        WHERE a.DocumentID=b.DocumentID AND a.ShopID=b.ShopID
                        AND a.ShopID= " + shopId + @" AND a.DocumentTypeID=24
                        AND a.documentid = " + documentId + @"
                        AND b.ProductID = c.MaterialID AND b.UnitID=d.UnitLargeID AND c.Deleted=0 ORDER BY c.MaterialCode";

            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<Material> materialList = new List<Material>();
            while (reader.Read())
            {
                if (reader["Remark"] != DBNull.Value)
                    Remark = reader.GetString("Remark");

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
                if (reader["UnitSmallID"] != DBNull.Value)
                    material.MaterialUnitSmallId = reader.GetInt32("UnitSmallID");
                if (reader["UnitLargeID"] != DBNull.Value)
                    material.MaterialUnitLargeId = reader.GetInt32("UnitLargeID");
                if (reader["ProductDiscount"] != DBNull.Value)
                    material.ProductDiscountAmount = reader.GetDecimal("ProductDiscount");
                if (reader["DiffAmount"] != DBNull.Value)
                    material.DiffAmount = reader.GetDecimal("DiffAmount");
                if (reader["DocumentStatus"] != DBNull.Value)
                    material.DocumentStatus = reader.GetInt32("DocumentStatus");
                if (reader["UnitSmallAmount"] != DBNull.Value)
                    material.UnitSmallAmount = reader.GetDecimal("UnitSmallAmount");
                materialList.Add(material);
            }
            reader.Close();
            return materialList;
        }

        public List<StockCountHistory> stockCountMonthlyHistory(CDBUtil dbUtil, MySqlConnection conn, int inv, DateTime dateFrom, DateTime dateTo)
        {
            string sql = "SELECT a.DocumentID, a.ShopID, a.DocumentStatus, a.Remark, " +
                         " a.DocumentDate, a.InsertDate, a.UpdateDate, a.ApproveDate, " +
                         " b.StaffCode AS inputStaffCode," +
                         " b.StaffFirstName AS inputStaffFirstName, " +
                         " b.StaffLastName AS inputStaffLastName, " +
                         " c.StaffCode AS approveStaffCode, " +
                         " c.StaffFirstName AS approveStaffFirstName, " +
                         " c.StaffLastName AS approveStaffLastName, " +
                         " d.StaffCode AS updateStaffCode, " +
                         " d.StaffFirstName AS updateStaffFirstName, " +
                         " d.StaffLastName AS updateStaffLastName " +
                         " FROM document a " +
                         " LEFT JOIN staffs b " +
                         " ON a.InputBy=b.StaffID " +
                         " LEFT OUTER JOIN staffs c " +
                         " ON a.ApproveBy = c.StaffID " +
                         " LEFT OUTER JOIN staffs d " + 
                         " ON a.UpdateBy = d.StaffID " +
                         " WHERE shopId=" + inv + " AND documenttypeid=7 " +
                         " AND DocumentDate BETWEEN '" + dateFrom.ToString("yyyy-MM-dd", dateProvider) + "'" +
                         " AND '" + dateTo.ToString("yyyy-MM-dd", dateProvider) + "' " +
                         " ORDER BY a.DocumentDate ";
                
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<StockCountHistory> stockCountHistoryList = new List<StockCountHistory>();
            while (reader.Read())
            {
                StockCountHistory stockCountHistory = new StockCountHistory();
                if (reader["DocumentID"] != DBNull.Value)
                    stockCountHistory.DocumentId = reader.GetInt32("DocumentID");
                if (reader["DocumentDate"] != DBNull.Value)
                    stockCountHistory.DocumentDate = reader.GetDateTime("DocumentDate");
                if (reader["UpdateDate"] != DBNull.Value)
                    stockCountHistory.UpdateDate = reader.GetDateTime("UpdateDate");
                if (reader["ApproveDate"] != DBNull.Value)
                    stockCountHistory.ApproveDate = reader.GetDateTime("ApproveDate");
                if (reader["DocumentStatus"] != DBNull.Value)
                    stockCountHistory.DocumentStatus = reader.GetInt32("DocumentStatus");
                if (reader["inputStaffCode"] != DBNull.Value)
                    stockCountHistory.StaffCode = reader.GetString("inputStaffCode");
                if (reader["inputStaffFirstName"] != DBNull.Value)
                    stockCountHistory.StaffFirstName = reader.GetString("inputStaffFirstName");
                if (reader["inputStaffLastName"] != DBNull.Value)
                    stockCountHistory.StaffLastName = reader.GetString("inputStaffLastName");

                if (reader["upDateStaffCode"] != DBNull.Value)
                    stockCountHistory.UpdateStaffCode = reader.GetString("upDateStaffCode");
                if (reader["upDateStaffFirstName"] != DBNull.Value)
                    stockCountHistory.UpdateStaffFirstName = reader.GetString("upDateStaffFirstName");
                if (reader["upDateStaffLastName"] != DBNull.Value)
                    stockCountHistory.UpdateStaffLastName = reader.GetString("upDateStaffLastName");

                if (reader["approveStaffCode"] != DBNull.Value)
                    stockCountHistory.ApproveStaffCode = reader.GetString("approveStaffCode");
                if (reader["approveStaffFirstName"] != DBNull.Value)
                    stockCountHistory.ApproveStaffFirstName = reader.GetString("ApproveStaffFirstName");
                if (reader["ApproveStaffLastName"] != DBNull.Value)
                    stockCountHistory.ApproveStaffLastName = reader.GetString("ApproveStaffLastName");

                if (reader["ShopID"] != DBNull.Value)
                    stockCountHistory.ShopId = reader.GetInt32("ShopID");
                if (reader["Remark"] != DBNull.Value)
                    stockCountHistory.Remark = reader["Remark"].ToString();

                stockCountHistoryList.Add(stockCountHistory);
            }
            reader.Close();
            return stockCountHistoryList;
        }

        public List<StockCountHistory> stockCountAdjustmentHistory(CDBUtil dbUtil, MySqlConnection conn, int inv, DateTime dateFrom, DateTime dateTo)
        {
            string sql = "SELECT a.DocumentID, a.ShopID, a.DocumentStatus, a.Remark, " +
                         " a.DocumentDate, a.InsertDate, a.UpdateDate, a.ApproveDate, " +
                         " b.StaffCode AS inputStaffCode," +
                         " b.StaffFirstName AS inputStaffFirstName, " +
                         " b.StaffLastName AS inputStaffLastName, " +
                         " c.StaffCode AS approveStaffCode, " +
                         " c.StaffFirstName AS approveStaffFirstName, " +
                         " c.StaffLastName AS approveStaffLastName, " +
                         " d.StaffCode AS updateStaffCode, " +
                         " d.StaffFirstName AS updateStaffFirstName, " +
                         " d.StaffLastName AS updateStaffLastName " +
                         " FROM document a " +
                         " LEFT JOIN staffs b " +
                         " ON a.InputBy=b.StaffID " +
                         " LEFT OUTER JOIN staffs c " +
                         " ON a.ApproveBy = c.StaffID " +
                         " LEFT OUTER JOIN staffs d " +
                         " ON a.UpdateBy = d.StaffID " +
                         " WHERE shopId=" + inv + " AND documenttypeid=57 " +
                         " AND DocumentDate BETWEEN '" + dateFrom.ToString("yyyy-MM-dd", dateProvider) + "'" +
                         " AND '" + dateTo.ToString("yyyy-MM-dd", dateProvider) + "'";
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<StockCountHistory> stockCountHistoryList = new List<StockCountHistory>();
            while (reader.Read())
            {
                StockCountHistory stockCountHistory = new StockCountHistory();
                if (reader["DocumentID"] != DBNull.Value)
                    stockCountHistory.DocumentId = reader.GetInt32("DocumentID");
                if (reader["DocumentDate"] != DBNull.Value)
                    stockCountHistory.DocumentDate = reader.GetDateTime("DocumentDate");
                if (reader["UpdateDate"] != DBNull.Value)
                    stockCountHistory.UpdateDate = reader.GetDateTime("UpdateDate");
                if (reader["ApproveDate"] != DBNull.Value)
                    stockCountHistory.ApproveDate = reader.GetDateTime("ApproveDate");
                if (reader["DocumentStatus"] != DBNull.Value)
                    stockCountHistory.DocumentStatus = reader.GetInt32("DocumentStatus");
                if (reader["inputStaffCode"] != DBNull.Value)
                    stockCountHistory.StaffCode = reader.GetString("inputStaffCode");
                if (reader["inputStaffFirstName"] != DBNull.Value)
                    stockCountHistory.StaffFirstName = reader.GetString("inputStaffFirstName");
                if (reader["inputStaffLastName"] != DBNull.Value)
                    stockCountHistory.StaffLastName = reader.GetString("inputStaffLastName");

                if (reader["upDateStaffCode"] != DBNull.Value)
                    stockCountHistory.UpdateStaffCode = reader.GetString("upDateStaffCode");
                if (reader["upDateStaffFirstName"] != DBNull.Value)
                    stockCountHistory.UpdateStaffFirstName = reader.GetString("upDateStaffFirstName");
                if (reader["upDateStaffLastName"] != DBNull.Value)
                    stockCountHistory.UpdateStaffLastName = reader.GetString("upDateStaffLastName");

                if (reader["approveStaffCode"] != DBNull.Value)
                    stockCountHistory.ApproveStaffCode = reader.GetString("approveStaffCode");
                if (reader["approveStaffFirstName"] != DBNull.Value)
                    stockCountHistory.ApproveStaffFirstName = reader.GetString("ApproveStaffFirstName");
                if (reader["ApproveStaffLastName"] != DBNull.Value)
                    stockCountHistory.ApproveStaffLastName = reader.GetString("ApproveStaffLastName");

                if (reader["ShopID"] != DBNull.Value)
                    stockCountHistory.ShopId = reader.GetInt32("ShopID");
                if (reader["Remark"] != DBNull.Value)
                    stockCountHistory.Remark = reader["Remark"].ToString();

                stockCountHistoryList.Add(stockCountHistory);
            }
            reader.Close();
            return stockCountHistoryList;
        }

        public List<StockCountHistory> stockCountWeeklyHistory(CDBUtil dbUtil, MySqlConnection conn, int inv, DateTime dateFrom, DateTime dateTo)
        {
            string sql = "SELECT a.DocumentID, a.ShopID, a.DocumentStatus, a.Remark, " +
                         " a.DocumentDate, a.InsertDate, a.UpdateDate, a.ApproveDate, " +
                         " b.StaffCode AS inputStaffCode," +
                         " b.StaffFirstName AS inputStaffFirstName, " +
                         " b.StaffLastName AS inputStaffLastName, " +
                         " c.StaffCode AS approveStaffCode, " +
                         " c.StaffFirstName AS approveStaffFirstName, " +
                         " c.StaffLastName AS approveStaffLastName, " +
                         " d.StaffCode AS updateStaffCode, " + 
                         " d.StaffFirstName AS updateStaffFirstName, " +
                         " d.StaffLastName AS updateStaffLastName " +
                         " FROM document a " +
                         " LEFT JOIN staffs b " +
                         " ON a.InputBy=b.StaffID " +
                         " LEFT OUTER JOIN staffs c " +
                         " ON a.ApproveBy = c.StaffID " +
                         " LEFT OUTER JOIN staffs d " +
                         " ON a.UpdateBy = d.StaffID " +
                         " WHERE shopId=" + inv + " AND documenttypeid=30 " +
                         " AND DocumentDate BETWEEN '" + dateFrom.ToString("yyyy-MM-dd", dateProvider) + "'" +
                         " AND '" + dateTo.ToString("yyyy-MM-dd", dateProvider) + "' " +
                         " ORDER BY a.DocumentDate ";
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<StockCountHistory> stockCountHistoryList = new List<StockCountHistory>();
            while (reader.Read())
            {
                StockCountHistory stockCountHistory = new StockCountHistory();
                if (reader["DocumentID"] != DBNull.Value)
                    stockCountHistory.DocumentId = reader.GetInt32("DocumentID");
                if (reader["DocumentDate"] != DBNull.Value)
                    stockCountHistory.DocumentDate = reader.GetDateTime("DocumentDate");
                if (reader["UpdateDate"] != DBNull.Value)
                    stockCountHistory.UpdateDate = reader.GetDateTime("UpdateDate");
                if (reader["ApproveDate"] != DBNull.Value)
                    stockCountHistory.ApproveDate = reader.GetDateTime("ApproveDate");
                if (reader["DocumentStatus"] != DBNull.Value)
                    stockCountHistory.DocumentStatus = reader.GetInt32("DocumentStatus");
                if (reader["inputStaffCode"] != DBNull.Value)
                    stockCountHistory.StaffCode = reader.GetString("inputStaffCode");
                if (reader["inputStaffFirstName"] != DBNull.Value)
                    stockCountHistory.StaffFirstName = reader.GetString("inputStaffFirstName");
                if (reader["inputStaffLastName"] != DBNull.Value)
                    stockCountHistory.StaffLastName = reader.GetString("inputStaffLastName");

                if (reader["updateStaffCode"] != DBNull.Value)
                    stockCountHistory.UpdateStaffCode = reader.GetString("updateStaffCode");
                if (reader["updateStaffFirstName"] != DBNull.Value)
                    stockCountHistory.UpdateStaffFirstName = reader.GetString("updateStaffFirstName");
                if (reader["updateStaffLastName"] != DBNull.Value)
                    stockCountHistory.UpdateStaffLastName = reader.GetString("updateStaffLastName");

                if (reader["approveStaffCode"] != DBNull.Value)
                    stockCountHistory.ApproveStaffCode = reader.GetString("approveStaffCode");
                if (reader["approveStaffFirstName"] != DBNull.Value)
                    stockCountHistory.ApproveStaffFirstName = reader.GetString("ApproveStaffFirstName");
                if (reader["ApproveStaffLastName"] != DBNull.Value)
                    stockCountHistory.ApproveStaffLastName = reader.GetString("ApproveStaffLastName");

                if (reader["ShopID"] != DBNull.Value)
                    stockCountHistory.ShopId = reader.GetInt32("ShopID");
                if (reader["Remark"] != DBNull.Value)
                    stockCountHistory.Remark = reader["Remark"].ToString();

                stockCountHistoryList.Add(stockCountHistory);
            }
            reader.Close();
            return stockCountHistoryList;
        }

        public List<StockCountHistory> stockCountDailyHistory(CDBUtil dbUtil, MySqlConnection conn, int inv, DateTime dateFrom, DateTime dateTo)
        {
            string sql = "SELECT a.DocumentID, a.ShopID, a.DocumentStatus, a.Remark, " +
                         " a.DocumentDate, a.InsertDate, a.UpdateDate, a.ApproveDate, " +
                         " b.StaffCode AS inputStaffCode," +
                         " b.StaffFirstName AS inputStaffFirstName, " +
                         " b.StaffLastName AS inputStaffLastName, " +
                         " c.StaffCode AS approveStaffCode, " +
                         " c.StaffFirstName AS approveStaffFirstName, " +
                         " c.StaffLastName AS approveStaffLastName, " +
                         " d.StaffCode AS updateStaffCode, " +
                         " d.StaffFirstName AS updateStaffFirstName, " +
                         " d.StaffLastName AS updateStaffLastName " +
                         " FROM document a " +
                         " LEFT JOIN staffs b " +
                         " ON a.InputBy=b.StaffID " +
                         " LEFT OUTER JOIN staffs c " +
                         " ON a.ApproveBy = c.StaffID " +
                         " LEFT OUTER JOIN staffs d " +
                         " ON a.UpdateBy = d.StaffID " +
                         " WHERE shopId=" + inv + " AND documenttypeid=24 " +
                         " AND DocumentDate BETWEEN '" + dateFrom.ToString("yyyy-MM-dd", dateProvider) + "'" +
                         " AND '" + dateTo.ToString("yyyy-MM-dd", dateProvider) + "' " + 
                         " ORDER BY a.DocumentDate ";
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<StockCountHistory> stockCountHistoryList = new List<StockCountHistory>();
            while (reader.Read())
            {
                StockCountHistory stockCountHistory = new StockCountHistory();
                if (reader["DocumentID"] != DBNull.Value)
                    stockCountHistory.DocumentId = reader.GetInt32("DocumentID");
                if (reader["DocumentDate"] != DBNull.Value)
                    stockCountHistory.DocumentDate = reader.GetDateTime("DocumentDate");
                if (reader["UpdateDate"] != DBNull.Value)
                    stockCountHistory.UpdateDate = reader.GetDateTime("UpdateDate");
                if (reader["ApproveDate"] != DBNull.Value)
                    stockCountHistory.ApproveDate = reader.GetDateTime("ApproveDate");
                if (reader["DocumentStatus"] != DBNull.Value)
                    stockCountHistory.DocumentStatus = reader.GetInt32("DocumentStatus");
                if (reader["upDateStaffCode"] != DBNull.Value)
                    stockCountHistory.UpdateStaffCode = reader.GetString("upDateStaffCode");
                if (reader["upDateStaffFirstName"] != DBNull.Value)
                    stockCountHistory.UpdateStaffFirstName = reader.GetString("upDateStaffFirstName");
                if (reader["upDateStaffLastName"] != DBNull.Value)
                    stockCountHistory.UpdateStaffLastName = reader.GetString("upDateStaffLastName");

                if (reader["inputStaffCode"] != DBNull.Value)
                    stockCountHistory.StaffCode = reader.GetString("inputStaffCode");
                if (reader["inputStaffFirstName"] != DBNull.Value)
                    stockCountHistory.StaffFirstName = reader.GetString("inputStaffFirstName");
                if (reader["inputStaffLastName"] != DBNull.Value)
                    stockCountHistory.StaffLastName = reader.GetString("inputStaffLastName");

                if (reader["approveStaffCode"] != DBNull.Value)
                    stockCountHistory.ApproveStaffCode = reader.GetString("approveStaffCode");
                if (reader["approveStaffFirstName"] != DBNull.Value)
                    stockCountHistory.ApproveStaffFirstName = reader.GetString("ApproveStaffFirstName");
                if (reader["ApproveStaffLastName"] != DBNull.Value)
                    stockCountHistory.ApproveStaffLastName = reader.GetString("ApproveStaffLastName");

                if (reader["ShopID"] != DBNull.Value)
                    stockCountHistory.ShopId = reader.GetInt32("ShopID");
                if (reader["Remark"] != DBNull.Value)
                    stockCountHistory.Remark = reader["Remark"].ToString();

                stockCountHistoryList.Add(stockCountHistory);
            }
            reader.Close();
            return stockCountHistoryList;
        }
    }
}
