﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehousePacking.Models
{
    public class ResponseInfo
    {
    }

    public class ItemInfo
    {
        public string name { get; set; }
        public decimal width { get; set; }
        public decimal height { get; set; }
        public decimal depth { get; set; }
        public List<decimal> position { get; set; }
        public string rotation { get; set; }
    }

    public class BinInfo
    {
        public BasicBinInfo bininfo { get; set; }
        public int binindex { get; set; }
        public List<ItemInfo> fitlist { get; set; }
        public List<ItemInfo> unfitlist { get; set; }
        public decimal binvolusage { get; set; }
        public decimal itemvolusage { get; set; }
    }

    public class BasicBinInfo
    {
        public string name { get; set; }
        public decimal width { get; set; }
        public decimal height { get; set; }
        public decimal depth { get; set; }
    }

}
