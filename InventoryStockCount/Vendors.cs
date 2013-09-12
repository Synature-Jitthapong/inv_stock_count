/*
 * แก้ไข UpdateVendor Where VendorGroupID AND ShopID // 2012-04-17
 */
using System;
using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using POSMySQL.POSControl;
using System.Globalization;

namespace InventoryStockCount
{
    public class Vendors
    {
        IFormatProvider dateProvider = System.Globalization.CultureInfo.InvariantCulture;
        private int _vendorId;
        private int _vendorGroupId;
        private int _shopId;
        private string _vendorInvName;
        private string _vendorCode;
        private string _vendorName;
        private string _vendorFirstName;
        private string _vendorLastName;
        private string _vendorAddress1;
        private string _vendorAddress2;
        private string _vendorCity;
        private int _vendorProvice;
        private string _vendorZipCode;
        private string _vendorTel;
        private string _vendorMobile;
        private string _vendorFax;
        private string _vendorEmail;
        private string _vendorAdditional;
        private DateTime _insertDate;
        private int _inputBy;
        private DateTime _updateDate;
        private int _deleted;
        private int _vendorTermOfPayMent;
        private int _vendorCreditDay;

        private string _vendorGroupCode;
        private string _vendorGroupName;

        private string _invName;
        private int defaultTaxType;

        public Vendors()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public Vendors(CDBUtil dbUtil)
        {
        }

        public Vendors(int vendorId, int vendorGroupId, int shopId, string vendorCode, string vendorName, string vendorFirstName,
            string vendorLastName, string vendorAddress1, string vendorAddress2, string vendorCity, int vendorProvice, string vendorZipCode,
            string vendorTel, string vendorMobile, string vendorFax, string vendorEmail, string vendorAdditional, DateTime insertDate,
            int inputBy, DateTime updateDate, int vendorTermOfPayMent, int vendorCreditDay, string vendorGroupCode, string vendorGroupName, string invName)
        {
            _vendorId = vendorId;
            _vendorGroupId = vendorGroupId;
            _shopId = shopId;
            _vendorCode = vendorCode;
            _vendorName = vendorName;
            _vendorFirstName = vendorFirstName;
            _vendorLastName = vendorLastName;
            _vendorAddress1 = vendorAddress1;
            _vendorAddress2 = vendorAddress2;
            _vendorCity = vendorCity;
            _vendorProvice = vendorProvice;
            _vendorZipCode = vendorZipCode;
            _vendorTel = vendorTel;
            _vendorMobile = vendorMobile;
            _vendorFax = vendorFax;
            _vendorEmail = vendorEmail;
            _vendorAdditional = vendorAdditional;
            _insertDate = insertDate;
            _inputBy = inputBy;
            _updateDate = updateDate;
            _vendorTermOfPayMent = vendorTermOfPayMent;
            _vendorCreditDay = vendorCreditDay;
            VendorGroupCode = vendorGroupCode;
            VendorGroupName = vendorGroupName;
            _invName = invName;
        }

        public Vendors(int vendorGroupId, string vendorGroupCode, string vendorGroupName)
        {
            _vendorGroupId = vendorGroupId;
            _vendorGroupCode = VendorGroupCode;
            _vendorGroupName = vendorGroupName;
        }

        public int GetMaxVendorGroupId(CDBUtil dbUtil, MySqlConnection conn)
        {
            int maxVendorGroupId = 0;
            string sql = " SELECT MAX(VendorGroupID) AS MaxVendorGroupID FROM vendorgroup WHERE ShopID=" + ShopId;
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            if (reader.Read())
            {
                if (reader["MaxVendorGroupID"] != DBNull.Value)
                    maxVendorGroupId = reader.GetInt32("MaxVendorGroupID") + 1;
            }
            reader.Close();
            return maxVendorGroupId;
        }

        public int GetMaxVendorId(CDBUtil dbUtil, MySqlConnection conn)
        {
            // get max vendorId from IdRange tabel
            int maxVendorId = 0;
            string sql = " SELECT MAX(VendorID) AS MaxVendorID FROM vendors WHERE ShopID=" + ShopId;

            //ShopId = ShopId == 1 ? 0 : ShopId;
            IdRange idRange = new IdRange(ShopId, 15);
            idRange.getIdRange(conn);

            using (MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn))
            {
                if (reader.Read())
                {
                    if (reader["MaxVendorID"] != DBNull.Value)
                    {
                        maxVendorId = reader.GetInt32("MaxVendorID") + 1;
                        if (!(maxVendorId >= idRange.MinId && maxVendorId <= idRange.MaxId))
                        {
                            maxVendorId = idRange.MinId;
                        }
                    }
                    else
                    {
                        maxVendorId = idRange.MinId;
                    }
                }
                else
                {
                    maxVendorId = idRange.MinId;
                }
            }
            return maxVendorId;
        }

        public bool UpdateVendorGroup(CDBUtil dbUtil, MySqlConnection conn)
        {
            string sql = " UPDATE vendorgroup SET ShopID=" + ShopId + ", VendorGroupCode='" + VendorGroupCode + "', " +
                " VendorGroupName='" + VendorGroupName + "' WHERE VendorGroupID=" + VendorGroupId +
                " AND ShopID=" + ShopId;
            if (dbUtil.sqlExecute(sql, conn) > 0)
                return true;
            return false;
        }

        public bool AddVendorGroup(CDBUtil dbUtil, MySqlConnection conn)
        {
            GetMaxVendorGroupId(dbUtil, conn);
            string sql = " INSERT INTO vendorgroup (VendorGroupID, ShopID, VendorGroupCode, VendorGroupName) " +
                         " VALUES (" + VendorGroupId + ", " + ShopId + ", '" + VendorGroupCode + "', '" + VendorGroupName + "')";
            if (dbUtil.sqlExecute(sql, conn) > 0)
                return true;
            return false;
        }

        public bool DeleteVendor(CDBUtil dbUtil, MySqlConnection conn, int vendorId)
        {
            string sql = "";
            sql = "UPDATE vendors SET Deleted=1 WHERE VendorID=" + vendorId;
            dbUtil.sqlExecute(sql, conn);
            return true;
        }

        public bool DeleteVendorGroup(CDBUtil dbUtil, MySqlConnection conn, int vendorGroupId)
        {
            string sql = "";
            sql = " UPDATE vendorgroup SET Deleted=1 WHERE VendorGroupID=" + vendorGroupId + " AND ShopID=" + ShopId;
            dbUtil.sqlExecute(sql, conn);
            return true;
        }

        public bool UpdateVendor(CDBUtil dbUtil, MySqlConnection conn)
        {
            string sql = " UPDATE vendors SET " +
                " VendorName='" + VendorName + "', VendorCode='" + VendorCode + "', VendorFirstName='" + VendorFirstName + "', " +
                " VendorLastName='" + VendorLastName + "', VendorAddress1='" + VendorAddress1 + "', VendorAddress2='" + VendorAddress2 + "', " +
                " VendorCity='" + VendorCity + "', VendorProvince=" + VendorProvice + ", VendorZipCode='" + VendorZipCode + "', VendorTelephone='" + VendorTel + "', " +
                " VendorMobile='" + VendorMobile + "', VendorFax='" + VendorFax + "', VendorEmail='" + VendorEmail + "'," +
                " VendorAdditional='" + VendorAdditional + "', DefaultTaxType=" + DefaultTaxType + ", InsertDate='" + DateTime.Now.ToString("yyyy'-'MM'-'dd", dateProvider) + "', InputBy=" + InputBy + ", " +
                " UpdateDate='" + DateTime.Now.ToString("yyyy'-'MM'-'dd", dateProvider) + "', VendorTermOfPayment=" + VendorTermOfPayMent + ", VendorCreditDay=" + VendorCreditDay +
                " WHERE VendorID=" + VendorId + " AND VendorGroupID=" + VendorGroupId + " AND ShopID=" + ShopId;
            if (dbUtil.sqlExecute(sql, conn) > 0)
                return true;
            return false;
        }

        public bool AddVendor(CDBUtil dbUtil, MySqlConnection conn)
        {
            GetMaxVendorId(dbUtil, conn);
            string sql = " INSERT INTO vendors (VendorID, VendorGroupID, ShopID, VendorName, VendorCode, " +
                         " VendorFirstName, VendorLastName, VendorAddress1, VendorAddress2, VendorCity, VendorProvince, " +
                         " VendorZipCode, VendorTelephone, VendorMobile, VendorFax, VendorEmail, VendorAdditional, " +
                         " DefaultTaxType, InsertDate, InputBy, UpdateDate, VendorTermOfPayment, VendorCreditDay) " +
                         " VALUES (" +
                         VendorId + ", " + VendorGroupId + ", " + ShopId + ", '" + VendorName + "', '" + VendorCode + "', '" +
                         VendorFirstName + "', '" + VendorLastName + "', '" + VendorAddress1 + "', '" + VendorAddress2 + "', '" + VendorCity + "', " +
                         VendorProvice + ", '" + VendorZipCode + "', '" + VendorTel + "', '" + VendorMobile + "', '" + VendorFax + "', '" +
                         VendorEmail + "', '" + VendorAdditional + "', " + DefaultTaxType + ", '" + DateTime.Now.ToString("yyyy'-'MM'-'dd HH:mm:ss", dateProvider) + "', " +
                         InputBy + ", '" + DateTime.Now.ToString("yyyy'-'MM'-'dd HH:mm:ss", dateProvider) + "', " +
                         VendorTermOfPayMent + ", " + VendorCreditDay +
                         " )";
            if (dbUtil.sqlExecute(sql, conn) > 0)
                return true;
            return false;
        }

        public int GetVendor(CDBUtil dbUtil, MySqlConnection conn, int vendorId)
        {
            string sql = "SELECT * FROM vendors WHERE VendorID=" + vendorId + " AND Deleted=0";
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            int i = 0;
            while (reader.Read())
            {
                if (reader["VendorID"] != DBNull.Value)
                    VendorId = reader.GetInt32("VendorID");
                if (reader["VendorGroupID"] != DBNull.Value)
                    VendorGroupId = reader.GetInt32("VendorGroupID");
                if (reader["ShopID"] != DBNull.Value)
                    ShopId = reader.GetInt32("ShopID");
                if (reader["VendorName"] != DBNull.Value)
                    VendorName = reader.GetString("VendorName");
                if (reader["VendorCode"] != DBNull.Value)
                    VendorCode = reader.GetString("VendorCode");
                if (reader["VendorFirstName"] != DBNull.Value)
                    VendorFirstName = reader.GetString("VendorFirstName");
                if (reader["VendorLastName"] != DBNull.Value)
                    VendorLastName = reader.GetString("VendorLastName");
                if (reader["VendorAddress1"] != DBNull.Value)
                    VendorAddress1 = reader.GetString("VendorAddress1");
                if (reader["VendorCity"] != DBNull.Value)
                    VendorCity = reader.GetString("VendorCity");
                if (reader["VendorProvince"] != DBNull.Value)
                    VendorProvice = reader.GetInt32("VendorProvince");
                if (reader["VendorZipCode"] != DBNull.Value)
                    VendorZipCode = reader.GetString("VendorZipCode");
                if (reader["VendorTelephone"] != DBNull.Value)
                    VendorTel = reader.GetString("VendorTelephone");
                if (reader["VendorMobile"] != DBNull.Value)
                    VendorMobile = reader.GetString("VendorMobile");
                if (reader["VendorFax"] != DBNull.Value)
                    VendorFax = reader.GetString("VendorFax");
                if (reader["VendorEmail"] != DBNull.Value)
                    VendorEmail = reader.GetString("VendorEmail");
                if (reader["VendorAdditional"] != DBNull.Value)
                    VendorAdditional = reader.GetString("VendorAdditional");
                if (reader["InsertDate"] != DBNull.Value)
                    InsertDate = reader.GetDateTime("InsertDate");
                if (reader["InputBy"] != DBNull.Value)
                    InputBy = reader.GetInt32("InputBy");
                if (reader["UpdateDate"] != DBNull.Value)
                    UpdateDate = reader.GetDateTime("UpdateDate");
                if (reader["VendorTermOfPayment"] != DBNull.Value)
                    VendorTermOfPayMent = reader.GetInt32("VendorTermOfPayment");
                if (reader["VendorCreditDay"] != DBNull.Value)
                    VendorCreditDay = reader.GetInt32("VendorCreditDay");
                if (reader["DefaultTaxType"] != DBNull.Value)
                    DefaultTaxType = reader.GetInt32("DefaultTaxType");
                i++;
            }
            reader.Close();
            return i;
        }

        public Vendors getVendorGroupShopId(MySqlConnection con, int vendorGroupId, string vendorGroupName)
        {
            Vendors vendor = new Vendors();
            string strSql = "SELECT ShopID FROM VendorGroup WHERE VendorGroupID=" + vendorGroupId + " AND VendorGroupName='" + vendorGroupName + "'";
            MySqlCommand cmd = new MySqlCommand(strSql,con);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if(reader.Read())
                {
                    vendor.ShopId = reader.GetInt32(0);
                }
            }
            return vendor;
        }

        public Vendors getEachVendorGroup(MySqlConnection conn, int shopId)
        {
            Vendors vendor = new Vendors();
            string strSql = "SELECT a.*, b.ProductLevelName FROM vendorgroup a " +
                " LEFT JOIN productlevel b " + 
                " ON a.ShopId=b.ProductLevelID " +
                " WHERE a.ShopID=" + shopId + " AND a.Deleted=0";

            MySqlCommand cmd = new MySqlCommand(strSql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                if (reader["VendorGroupID"] != DBNull.Value)
                    vendor.VendorGroupId = reader.GetInt32("VendorGroupID");
                if (reader["ShopID"] != DBNull.Value)
                    vendor.ShopId = reader.GetInt32("ShopID");
                if (reader["VendorGroupCode"] != DBNull.Value)
                    vendor.VendorGroupCode = reader.GetString("VendorGroupCode");
                if (reader["VendorGroupName"] != DBNull.Value)
                    vendor.VendorGroupName = reader.GetString("VendorGroupName");
                if (reader["ProductLevelName"] != DBNull.Value)
                    vendor.VendorInvName = reader.GetString("ProductLevelName");
            }
            reader.Close();
            return vendor;
        }

        public List<Vendors> ListVendorGroup(CDBUtil dbUtil, MySqlConnection conn)
        {
            string sql = " SELECT a.*, b.ProductLevelName FROM vendorgroup a, productlevel b WHERE a.ShopID=b.ProductLevelID AND a.Deleted=0";
           // if (ShopId != 0)
            //    sql += " AND ShopID=" + ShopId;
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            List<Vendors> vendorList = new List<Vendors>();
            while (reader.Read())
            {
                Vendors vendor = new Vendors();
                if (reader["VendorGroupID"] != DBNull.Value)
                    vendor.VendorGroupId = reader.GetInt32("VendorGroupID");
                if (reader["ShopID"] != DBNull.Value)
                    vendor.ShopId = reader.GetInt32("ShopID");
                if (reader["VendorGroupCode"] != DBNull.Value)
                    vendor.VendorGroupCode = reader.GetString("VendorGroupCode");
                if (reader["VendorGroupName"] != DBNull.Value)
                    vendor.VendorGroupName = reader.GetString("VendorGroupName");
                if (reader["ProductLevelName"] != DBNull.Value)
                    vendor.VendorInvName = reader.GetString("ProductLevelName");
                vendorList.Add(vendor);
            }
            reader.Close();
            return vendorList;
        }

        public List<Vendors> GetVendorGroup(CDBUtil dbUtil, MySqlConnection conn)
        {
            string strSql = "SELECT * FROM vendorgroup WHERE Deleted=0";
            if (ShopId != 1) strSql += " AND ShopID=" + ShopId;

            List<Vendors> listVendorGroup = new List<Vendors>();
            MySqlDataReader reader = dbUtil.sqlRetrive(strSql, conn);
            while (reader.Read())
            {
                if (reader["VendorGroupID"] != DBNull.Value)
                {
                    listVendorGroup.Add(new Vendors(Convert.ToInt32(reader["VendorGroupID"]),
                                                    reader["VendorGroupCode"].ToString(),
                                                    reader["VendorGroupName"].ToString()));
                }
            }
            reader.Close();
            return listVendorGroup;
        }

        public Vendors getEachVendor(MySqlConnection conn, int vendorId)
        {
            Vendors vendor = new Vendors();
            string strSql = "SELECT a.*, b.*, c.ProductLevelName FROM vendors a LEFT JOIN vendorgroup b " +
                  " ON (a.VendorGroupID=b.VendorGroupID AND a.ShopID=b.ShopID) " +
                  " LEFT JOIN productlevel c ON (a.ShopID=c.ProductLevelID) " +
                  " WHERE a.VendorID=" + vendorId + " AND a.Deleted=0 and b.Deleted=0";

            MySqlCommand cmd = new MySqlCommand(strSql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                if (reader["VendorID"] != DBNull.Value)
                    vendor.VendorId = reader.GetInt32("VendorID");
                if (reader["VendorGroupID"] != DBNull.Value)
                    vendor.VendorGroupId = reader.GetInt32("VendorGroupID");
                if (reader["ShopID"] != DBNull.Value)
                    vendor.ShopId = reader.GetInt32("ShopID");
                if (reader["VendorCode"] != DBNull.Value)
                    vendor.VendorCode = reader.GetString("VendorCode");
                if (reader["VendorName"] != DBNull.Value)
                    vendor.VendorName = reader.GetString("VendorName");
                if (reader["VendorFirstName"] != DBNull.Value)
                    vendor.VendorFirstName = reader.GetString("VendorFirstName");
                if (reader["VendorLastName"] != DBNull.Value)
                    vendor.VendorLastName = reader.GetString("VendorLastName");
                if (reader["VendorAddress1"] != DBNull.Value)
                    vendor.VendorAddress1 = reader.GetString("VendorAddress1");
                if (reader["VendorAddress2"] != DBNull.Value)
                    vendor.VendorAddress2 = reader.GetString("VendorAddress2");
                if (reader["VendorCity"] != DBNull.Value)
                    vendor.VendorCity = reader.GetString("VendorCity");
                if (reader["VendorProvince"] != DBNull.Value)
                    vendor.VendorProvice = reader.GetInt32("VendorProvince");
                if (reader["VendorZipCode"] != DBNull.Value)
                    vendor.VendorZipCode = reader.GetString("VendorZipCode");
                if (reader["VendorTelephone"] != DBNull.Value)
                    vendor.VendorTel = reader.GetString("VendorTelephone");
                if (reader["VendorMobile"] != DBNull.Value)
                    vendor.VendorMobile = reader.GetString("VendorMobile");
                if (reader["VendorFax"] != DBNull.Value)
                    vendor.VendorFax = reader.GetString("VendorFax");
                if (reader["VendorEmail"] != DBNull.Value)
                    vendor.VendorEmail = reader.GetString("VendorEmail");
                if (reader["VendorAdditional"] != DBNull.Value)
                    vendor.VendorAdditional = reader.GetString("VendorAdditional");
                if (reader["InsertDate"] != DBNull.Value)
                    vendor.InsertDate = reader.GetDateTime("InsertDate");
                if (reader["VendorFirstName"] != DBNull.Value)
                    vendor.VendorFirstName = reader.GetString("VendorFirstName");
                if (reader["InputBy"] != DBNull.Value)
                    vendor.InputBy = reader.GetInt32("InputBy");
                if (reader["UpdateDate"] != DBNull.Value)
                    vendor.UpdateDate = reader.GetDateTime("UpdateDate");
                if (reader["VendorTermOfPayment"] != DBNull.Value)
                    vendor.VendorTermOfPayMent = reader.GetInt32("VendorTermOfPayment");
                if (reader["VendorCreditDay"] != DBNull.Value)
                    vendor.VendorCreditDay = reader.GetInt32("VendorCreditDay");
                if (reader["VendorGroupCode"] != DBNull.Value)
                    vendor.VendorGroupCode = reader.GetString("VendorGroupCode");
                if (reader["VendorGroupName"] != DBNull.Value)
                    vendor.VendorGroupName = reader.GetString("VendorGroupName");
                if (reader["ProductLevelName"] != DBNull.Value)
                    vendor.VendorInvName = reader.GetString("ProductLevelName");
                try
                {
                    if (reader["DefaultTaxType"] != DBNull.Value)
                        vendor.DefaultTaxType = reader.GetInt32("DefaultTaxType");
                }
                catch
                {

                }
            }
            reader.Close();
            return vendor;
        }

        public List<Vendors> GetVendors(CDBUtil dbUtil, MySqlConnection conn, int vendorGroupId)
        {
            string strSql = "SELECT a.*, b.*, c.ProductLevelName FROM vendors a JOIN vendorgroup b " +
                   " ON (a.VendorGroupID=b.VendorGroupID AND a.ShopID=b.ShopID) " +
                   " JOIN productlevel c ON (a.ShopID=c.ProductLevelID) WHERE a.Deleted=0 and b.Deleted=0";
            if (vendorGroupId > 0) strSql += " AND b.VendorGroupID=" + vendorGroupId + " AND b.ShopID=" + ShopId;
            //if(ShopId != 1) strSql += " AND a.ShopID = " + ShopId; // hq เห็นหมด

            MySqlDataReader reader = dbUtil.sqlRetrive(strSql, conn);
            List<Vendors> listVendor = new List<Vendors>();
            while (reader.Read())
            {
                Vendors vendor = new Vendors();
                if (reader["VendorID"] != DBNull.Value)
                    vendor.VendorId = reader.GetInt32("VendorID");
                if (reader["VendorGroupID"] != DBNull.Value)
                    vendor.VendorGroupId = reader.GetInt32("VendorGroupID"); 
                if (reader["ShopID"] != DBNull.Value)
                    vendor.ShopId = reader.GetInt32("ShopID");
                if (reader["VendorCode"] != DBNull.Value)
                    vendor.VendorCode = reader.GetString("VendorCode");
                if (reader["VendorName"] != DBNull.Value)
                    vendor.VendorName = reader.GetString("VendorName");
                if (reader["VendorFirstName"] != DBNull.Value)
                    vendor.VendorFirstName = reader.GetString("VendorFirstName");
                if (reader["VendorLastName"] != DBNull.Value)
                    vendor.VendorLastName = reader.GetString("VendorLastName");
                if (reader["VendorAddress1"] != DBNull.Value)
                    vendor.VendorAddress1 = reader.GetString("VendorAddress1");
                if (reader["VendorAddress2"] != DBNull.Value)
                    vendor.VendorAddress2 = reader.GetString("VendorAddress2");
                if (reader["VendorCity"] != DBNull.Value)
                    vendor.VendorCity = reader.GetString("VendorCity");
                if (reader["VendorProvince"] != DBNull.Value)
                    vendor.VendorProvice = reader.GetInt32("VendorProvince");
                if (reader["VendorZipCode"] != DBNull.Value)
                    vendor.VendorZipCode = reader.GetString("VendorZipCode");
                if (reader["VendorTelephone"] != DBNull.Value)
                    vendor.VendorTel = reader.GetString("VendorTelephone");
                if (reader["VendorMobile"] != DBNull.Value)
                    vendor.VendorMobile = reader.GetString("VendorMobile");
                if (reader["VendorFax"] != DBNull.Value)
                    vendor.VendorFax = reader.GetString("VendorFax");
                if (reader["VendorEmail"] != DBNull.Value)
                    vendor.VendorEmail = reader.GetString("VendorEmail");
                if (reader["VendorAdditional"] != DBNull.Value)
                    vendor.VendorAdditional = reader.GetString("VendorAdditional");
                if (reader["InsertDate"] != DBNull.Value)
                    vendor.InsertDate = reader.GetDateTime("InsertDate");
                if (reader["VendorFirstName"] != DBNull.Value)
                    vendor.VendorFirstName = reader.GetString("VendorFirstName");
                if (reader["InputBy"] != DBNull.Value)
                    vendor.InputBy = reader.GetInt32("InputBy");
                if (reader["UpdateDate"] != DBNull.Value)
                    vendor.UpdateDate = reader.GetDateTime("UpdateDate");
                if (reader["VendorTermOfPayment"] != DBNull.Value)
                    vendor.VendorTermOfPayMent = reader.GetInt32("VendorTermOfPayment");
                if (reader["VendorCreditDay"] != DBNull.Value)
                    vendor.VendorCreditDay = reader.GetInt32("VendorCreditDay");
                if (reader["VendorGroupCode"] != DBNull.Value)
                    vendor.VendorGroupCode = reader.GetString("VendorGroupCode");
                if (reader["VendorGroupName"] != DBNull.Value)
                    vendor.VendorGroupName = reader.GetString("VendorGroupName");
                if (reader["ProductLevelName"] != DBNull.Value)
                    vendor.VendorInvName = reader.GetString("ProductLevelName");

                listVendor.Add(vendor);
            }
            reader.Close();
            return listVendor;
        }

        public int VendorId
        {
            get { return _vendorId; }
            set { _vendorId = value; }
        }

        public int VendorGroupId
        {
            get { return _vendorGroupId; }
            set { _vendorGroupId = value; }
        }

        public int ShopId
        {
            get { return _shopId; }
            set { _shopId = value; }
        }

        public string VendorCode
        {
            get { return _vendorCode; }
            set { _vendorCode = value; }
        }

        public string VendorName
        {
            get { return _vendorName; }
            set { _vendorName = value; }
        }

        public string VendorInvName
        {
            get { return _vendorInvName; }
            set { _vendorInvName = value; }
        }
        public string VendorFirstName
        {
            get { return _vendorFirstName; }
            set { _vendorFirstName = value; }
        }

        public string VendorLastName
        {
            get { return _vendorLastName; }
            set { _vendorLastName = value; }
        }

        public string VendorAddress1
        {
            get { return _vendorAddress1; }
            set { _vendorAddress1 = value; }
        }

        public string VendorAddress2
        {
            get { return _vendorAddress2; }
            set { _vendorAddress2 = value; }
        }

        public string VendorCity
        {
            get { return _vendorCity; }
            set { _vendorCity = value; }
        }

        public int VendorProvice
        {
            get { return _vendorProvice; }
            set { _vendorProvice = value; }
        }

        public string VendorZipCode
        {
            get { return _vendorZipCode; }
            set { _vendorZipCode = value; }
        }

        public string VendorTel
        {
            get { return _vendorTel; }
            set { _vendorTel = value; }
        }

        public string VendorMobile
        {
            get { return _vendorMobile; }
            set { _vendorMobile = value; }
        }

        public string VendorFax
        {
            get { return _vendorFax; }
            set { _vendorFax = value; }
        }

        public string VendorEmail
        {
            get { return _vendorEmail; }
            set { _vendorEmail = value; }
        }

        public string VendorAdditional
        {
            get { return _vendorAdditional; }
            set { _vendorAdditional = value; }
        }

        public DateTime InsertDate
        {
            get { return _insertDate; }
            set { _insertDate = value; }
        }

        public int InputBy
        {
            get { return _inputBy; }
            set { _inputBy = value; }
        }

        public DateTime UpdateDate
        {
            get { return _updateDate; }
            set { _updateDate = value; }
        }

        public int Deleted
        {
            get { return _deleted; }
            set { _deleted = value; }
        }

        public int VendorTermOfPayMent
        {
            get { return _vendorTermOfPayMent; }
            set { _vendorTermOfPayMent = value; }
        }

        public int VendorCreditDay
        {
            get { return _vendorCreditDay; }
            set { _vendorCreditDay = value; }
        }

        public string VendorGroupCode
        {
            get { return _vendorGroupCode; }
            set { _vendorGroupCode = value; }
        }

        public string VendorGroupName
        {
            get { return _vendorGroupName; }
            set { _vendorGroupName = value; }
        }

        public string InvName
        {
            get { return _invName; }
            set { _invName = value; }
        }

        public int DefaultTaxType
        {
            get { return defaultTaxType; }
            set { defaultTaxType = value; }
        }
    }
}