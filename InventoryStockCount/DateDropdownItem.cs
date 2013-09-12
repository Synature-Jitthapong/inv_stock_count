using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace InventoryStockCount
{
    public static class DateDropdownItem
    {
        public static IFormatProvider dateProvider = System.Globalization.CultureInfo.CurrentCulture;

        public static void CreateDateDropdownList(ref DropDownList ddl)
        {
            for (int i = 1; i <= 31; i++)
            {
                ListItem lItem = new ListItem(i.ToString(), i.ToString());
                ddl.Items.Add(lItem);
            }
        }

        public static void CreateMonthDropdownList(ref DropDownList ddl)
        {
            for (int i = 1; i <= 12; i++)
            {
                ListItem lItem = new ListItem(new DateTime(DateTime.Now.Year, i, DateTime.DaysInMonth(DateTime.Now.Year, i)).ToString("MMMM", dateProvider), i.ToString());
                ddl.Items.Add(lItem);
            }
        }

        public static void CreateYearDropdownList(ref DropDownList ddl)
        {
            for (int i = DateTime.Now.Year - 3; i < DateTime.Now.Year + 1; i++)
            {
                ListItem lItem = new ListItem(new DateTime(i, 1, 1).ToString("yyyy", dateProvider), new DateTime(i, 1, 1).ToString("yyyy", System.Globalization.CultureInfo.InvariantCulture));
                ddl.Items.Add(lItem);
            }
        }
    }
}
