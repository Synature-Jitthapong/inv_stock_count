using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventoryStockCount
{
    public class MaterialCountData : Material
    {
        private Material[] _stockCountData;

        public Material[] StockCountData
        {
            set { _stockCountData = value; }
            get { return _stockCountData; }
        }
    }
}
