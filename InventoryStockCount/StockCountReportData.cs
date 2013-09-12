using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using POSInventoryStockCardModule;
using POSMySQL.POSControl;
using MySql.Data.MySqlClient;

namespace InventoryStockCount
{
    public class StockCountReportData
    {
        public DataSet GetDataSetReport(CDBUtil dbUtil, MySqlConnection conn, int productLevelId,
            int shopId, string countType)
        {
            DataTable dtStockCountMonthlyMaterials = new DataTable("dtStockCountMaterials");
            dtStockCountMonthlyMaterials.Columns.Add("MaterialDeptName");
            dtStockCountMonthlyMaterials.Columns.Add("MaterialCode");
            dtStockCountMonthlyMaterials.Columns.Add("MaterialName");
            dtStockCountMonthlyMaterials.Columns.Add("MaterialUnitLargeName");

            DataTable dtMaterialDept = GetMaterialDept(dbUtil, conn, productLevelId, shopId, countType);
            DataTable dtMaterials = GetMaterials2(dbUtil, conn, productLevelId, shopId, countType); //GetMaterials(dbUtil, conn, productLevelId, shopId, countType);
            if (dtMaterialDept.Rows.Count > 0)
            {
                string expression = "";
                DataRow[] foundRow;
                for (int i = 0; i < dtMaterialDept.Rows.Count; i++)
                {
                    // expression
                    expression = "MaterialDeptID='" +
                        dtMaterialDept.Rows[i]["MaterialDeptID"].ToString() + "'";
                    foundRow = dtMaterials.Select(expression);
                    if (foundRow.Length > 0)
                    {
                        // row materialdeptname
                        DataRow rowMaterialDept = dtStockCountMonthlyMaterials.NewRow();
                        rowMaterialDept["MaterialDeptName"] = dtMaterialDept.Rows[i]["MaterialGroupName"] +
                            " | " + dtMaterialDept.Rows[i]["MaterialDeptName"].ToString();
                        dtStockCountMonthlyMaterials.Rows.Add(rowMaterialDept);

                        for (int j = 0; j < foundRow.Length; j++)
                        {
                            DataRow row = dtStockCountMonthlyMaterials.NewRow();
                            row["MaterialCode"]             = foundRow[j]["MaterialCode"].ToString();
                            row["MaterialName"]             = foundRow[j]["MaterialName"].ToString();
                            row["MaterialUnitLargeName"]    = foundRow[j]["StockCountUnitName"];
                            dtStockCountMonthlyMaterials.Rows.Add(row);
                        }
                    }
                }
            }

            DataSet dsReport = new DataSet();
            dsReport.Tables.Add(dtStockCountMonthlyMaterials);
            return dsReport;
        }

        protected DataTable GetMaterialDept(CDBUtil dbUtil, MySqlConnection conn, int productLevelId,
            int shopId, string countType)
        {
            string sql = "";
            DataTable dtMaterialDept = new DataTable();
            sql = "SELECT a.MaterialDeptID, a.MaterialDeptName, " +
                  " b.materialgroupid, b.MaterialGroupName " +
                  " FROM materialdept a " +
                  " LEFT JOIN materialgroup b " +
                  " ON a.MaterialGroupID = b.MaterialGroupID ";
            //sql = " SELECT a.*, b.*, c.*, d.*, e.*, f.*, g.* FROM materialgroup g " +
            //    " INNER JOIN materialdept a " +
            //    " ON g.MaterialGroupID=a.MaterialGroupID "  +               
            //    " INNER JOIN materials b " +
            //    " ON a.MaterialDeptID=b.MaterialDeptID " +
            //    " INNER JOIN unitsmall c " +
            //    " ON b.UnitSmallID=c.UnitSmallID " +
            //    " INNER JOIN unitratio d " +
            //    " ON c.UnitSmallID = d.UnitLargeID " +
            //    " INNER JOIN unitlarge e " +
            //    " ON d.UnitLargeID = e.UnitLargeID " +
            //    " INNER JOIN " + countType + " f " +
            //    " ON b.MaterialID=f.MaterialID " +
            //    " WHERE b.Deleted = 0 AND f.ProductLevelID=" + productLevelId +
            //    " AND f.ShopID=" + shopId + " GROUP BY g.MaterialGroupID ORDER BY b.MaterialCode, b.MaterialDeptID";
            return dtMaterialDept = dbUtil.List(sql, conn);
        }

        protected DataTable GetMaterials(CDBUtil dbUtil, MySqlConnection conn, int productLevelId,
            int shopId, string countType)
        {
            string sql = "";

            DataTable dtMaterials = new DataTable();
            //sql = " SELECT a.*, b.*, c.*, d.*, e.*, f.* FROM materialdept a " +
            //    " LEFT JOIN materials b " +
            //    " ON a.MaterialDeptID=b.MaterialDeptID " +
            //    " LEFT JOIN unitsmall c " +
            //    " ON b.UnitSmallID=c.UnitSmallID " +
            //    " LEFT JOIN unitratio d " +
            //    " ON c.UnitSmallID = d.UnitLargeID " +
            //    " LEFT JOIN unitlarge e " +
            //    " ON d.UnitLargeID = e.UnitLargeID " +
            //    " LEFT JOIN " + countType + " f " +
            //    " ON b.MaterialID=f.MaterialID " +
            //    " WHERE b.Deleted = 0 AND f.ProductLevelID=" + productLevelId +
            //    " AND f.ShopID=" + shopId + " ORDER BY b.MaterialCode, b.MaterialDeptID ";
            sql = " SELECT a.*, b.*, c.*, d.*, e.* FROM materials a " +
              " INNER JOIN unitsmall b " +
              " ON a.UnitSmallID=b.UnitSmallID " +
              " INNER JOIN unitratio c " +
              " ON b.UnitSmallID = c.UnitLargeID " +
              " INNER JOIN unitlarge d " +
              " ON c.UnitLargeID = d.UnitLargeID " +
              " INNER JOIN " + countType + " e " +
              " ON a.MaterialID=e.MaterialID " +
              " WHERE a.Deleted = 0 AND e.ProductLevelID=" + productLevelId +
              " AND e.ShopID=" + shopId + " ORDER BY a.MaterialCode, a.MaterialDeptID ";
            return dtMaterials = dbUtil.List(sql, conn);
        }

        public DataTable GetMaterials2(CDBUtil dbUtil, MySqlConnection conn,
             int productLevelId, int invId, string countConfigTableName)
        {
            DateTime dateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime dateTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DataTable dtStockCountColumn = StockCardModule.StockCountColumn(dbUtil, conn);
            DataTable dtStockCountData = new DataTable();

            if(countConfigTableName == "dailystockmaterial")
            {
                dtStockCountData =
                   StockCardModule.DailyStockCountViewDetail(dbUtil, conn, dtStockCountColumn,
                                                              invId, DateTime.Now, 1, 1, 1, 1, "1");
            }
            else if (countConfigTableName == "weeklystockmaterial")
            {
                dtStockCountData =
                    StockCardModule.WeeklyStockCountViewDetail(dbUtil, conn, dtStockCountColumn,
                                                               invId, dateFrom, dateTo, 1, 1, 1, 1, "1");
            }else if(countConfigTableName == "monthlystockmaterial")
            {
                dtStockCountData =
                    StockCardModule.MonthlyStockCountViewDetail(dbUtil, conn, dtStockCountColumn,
                                                                invId, dateFrom, dateTo, 1, 1, 1, 1, "1");
            }

            return dtStockCountData;
        }
    }
}
