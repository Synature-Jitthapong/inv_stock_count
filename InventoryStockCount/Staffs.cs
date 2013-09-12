using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using POSMySQL.POSControl;

namespace InventoryStockCount
{
    public class Staffs
    {
        private int _staffId;

        public int StaffId
        {
            get { return _staffId; }
            set { _staffId = value; }
        }
        private string _staffCode;

        public string StaffCode
        {
            get { return _staffCode; }
            set { _staffCode = value; }
        }
        private string _staffFirstName;

        public string StaffFirstName
        {
            get { return _staffFirstName; }
            set { _staffFirstName = value; }
        }
        private string _staffLastName;

        public string StaffLastName
        {
            get { return _staffLastName; }
            set { _staffLastName = value; }
        }

        public Staffs()
        {

        }

        public void StaffsInfo(CDBUtil dbUtil, MySqlConnection conn, int staffId)
        {
            string sql = "SELECT * FROM staffs WHERE StaffID=" + staffId;
            MySqlDataReader reader = dbUtil.sqlRetrive(sql, conn);
            while (reader.Read())
            {
                if (reader["StaffID"] != DBNull.Value)
                    StaffId = Convert.ToInt32(reader["StaffID"]);
                if (reader["StaffCode"] != DBNull.Value)
                    StaffCode = reader["StaffCode"].ToString();
                if (reader["StaffFirstName"] != DBNull.Value)
                    StaffFirstName = reader["StaffFirstName"].ToString();
                if (reader["StaffLastName"] != DBNull.Value)
                    StaffLastName = reader["StaffLastName"].ToString();
            }
            reader.Close();
        }
    }
}
