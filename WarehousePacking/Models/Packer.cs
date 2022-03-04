using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehousePacking.Models
{
    interface PackerFuncs
    {
        public void pack(bool bigger_first = false, bool distribute_items = false, int number_of_decimals = Utils.DEFAULT_NUMBER_OF_DECIMALS);
    }
    public class Item
    {
        public string name { get; set; }
        public decimal width { get; set; }
        public decimal height { get; set; }
        public decimal depth { get; set; }
        public decimal weight { get; set; }
        public int rotation_type { get; set; }
        public List<decimal> position { get; set; }
        public List<decimal> position_p { get; set; }
        public List<Candidate> candidate { get; set; }
        public int number_of_decimals { get; set; }
        public int packed { get; set; }
        public int count { get; set; }
        public List<int> rotation_allow { get; set; }

        public Item(string name, decimal width, decimal height, decimal depth, decimal weight, string rotationinfo, int count)
        {
            this.name = name;
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.weight = weight;
            this.rotation_type = 0;
            this.position = Utils.START_POSITION;
            this.candidate = new List<Candidate> { };
            this.number_of_decimals = Utils.DEFAULT_NUMBER_OF_DECIMALS;
            this.packed = 0;
            this.count = count;
            switch (rotationinfo)
            {
                case "width":
                    this.rotation_allow = RotationType.WIDTH;
                    break;
                case "height":
                    this.rotation_allow = RotationType.HEIGHT;
                    break;
                case "depth":
                    this.rotation_allow = RotationType.DEPTH;
                    break;
                default:
                    this.rotation_allow = RotationType.ALL;
                    break;
            }
        }

        public Item ShallowCopy()
        {
            Item ret = (Item)this.MemberwiseClone();
            ret.rotation_type = 0;
            ret.position = Utils.START_POSITION;
            ret.candidate.Clear();
            ret.packed = 0;
            return ret;

        }

        public void format_numbers(int number_of_decimals)
        {
            this.width = Math.Round(this.width, number_of_decimals);
            this.height = Math.Round(this.height, number_of_decimals);
            this.depth = Math.Round(this.depth, number_of_decimals);
            this.weight = Math.Round(this.weight, number_of_decimals);
            this.number_of_decimals = number_of_decimals;
        }

        public string get_rttype()
        {
            switch (this.rotation_type)
            {
                case 1:
                    return "HWD";
                case 2:
                    return "HDW";
                case 3:
                    return "DHW";
                case 4:
                    return "DWH";
                case 5:
                    return "WDH";
                default:
                    return "WHD";
            }
        }

        public ItemInfo @string()
        {
            ItemInfo iteminfo = new ItemInfo();
            List<decimal> dim = this.get_dimension();
            iteminfo.name = this.name;
            iteminfo.width = dim[0];
            iteminfo.height = dim[1];
            iteminfo.depth = dim[2];
            iteminfo.position = this.position;
            iteminfo.rotation = get_rttype();
            return iteminfo;
        }

        public decimal get_volume()
        {
            return Math.Round(this.width * this.height * this.depth, number_of_decimals);
        }

        public List<decimal> get_dimension()
        {
            List<decimal> dimension = new List<decimal> { };
            if (this.rotation_type == RotationType.RT_WHD)
            {
                dimension.Add(this.width);
                dimension.Add(this.height);
                dimension.Add(this.depth);
            }
            else if (this.rotation_type == RotationType.RT_HWD)
            {
                dimension.Add(this.height);
                dimension.Add(this.width);
                dimension.Add(this.depth);
            }
            else if (this.rotation_type == RotationType.RT_HDW)
            {
                dimension.Add(this.height);
                dimension.Add(this.depth);
                dimension.Add(this.width);
            }
            else if (this.rotation_type == RotationType.RT_DHW)
            {
                dimension.Add(this.depth);
                dimension.Add(this.height);
                dimension.Add(this.width);
            }
            else if (this.rotation_type == RotationType.RT_DWH)
            {
                dimension.Add(this.depth);
                dimension.Add(this.width);
                dimension.Add(this.height);
            }
            else if (this.rotation_type == RotationType.RT_WDH)
            {
                dimension.Add(this.width);
                dimension.Add(this.depth);
                dimension.Add(this.height);
            }
            else
            {

            }
            return dimension;
        }
    }

    public class Bin
    {
        public string name { get; set; }
        public decimal width { get; set; }
        public decimal height { get; set; }
        public decimal depth { get; set; }
        public decimal max_weight { get; set; }
        public List<Item> items { get; set; }
        public List<Item> unfitted_items { get; set; }
        public int count { get; set; }
        public bool itemcomplete { get; set; }
        public List<List<decimal>> can_pos { get; set; }
        public int index { get; set; }
        public int number_of_decimals { get; set; }
        public BinInfo result { get; set; }

        public Bin(string name, decimal width, decimal height, decimal depth, decimal max_weight, int index)
        {
            this.name = name;
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.max_weight = max_weight;
            this.index = index;
            this.itemcomplete = false;
            this.items = new List<Item>();
            this.unfitted_items = new List<Item>();
            this.can_pos = new List<List<decimal>>();
            this.number_of_decimals = Utils.DEFAULT_NUMBER_OF_DECIMALS;
        }

        public Bin ShallowCopy()
        {
            Bin ret = (Bin)this.MemberwiseClone();
            ret.items.Clear();
            ret.unfitted_items.Clear();
            ret.count = 0;
            ret.itemcomplete = false;
            ret.can_pos.Clear();
            return ret;
        }

        public void format_numbers(int number_of_decimals)
        {
            this.width = Math.Round(this.width, number_of_decimals);
            this.height = Math.Round(this.height, number_of_decimals);
            this.depth = Math.Round(this.depth, number_of_decimals);
            this.max_weight = Math.Round(this.max_weight, number_of_decimals);
            this.number_of_decimals = number_of_decimals;
        }

        public decimal get_bin_vol_usage()
        {
            decimal total_volume = 0;
            foreach (Item item in this.items)
            {
                total_volume += item.get_volume();
            }
            return total_volume / this.get_volume() * 100;
        }

        public decimal get_item_vol_usage()
        {
            decimal fit_volume = 0;
            decimal total_volume = 0;
            foreach (Item item in this.items)
            {
                fit_volume += item.get_volume();
                total_volume += item.get_volume();
            }
            foreach (Item item in this.unfitted_items)
            {
                total_volume += item.get_volume() * item.count;
            }
            return fit_volume / total_volume * 100;
        }

        public BasicBinInfo @string()
        {
            BasicBinInfo basicbininfo = new BasicBinInfo();
            basicbininfo.name = this.name;
            basicbininfo.width = this.width;
            basicbininfo.height = this.height;
            basicbininfo.depth = this.depth;
            return basicbininfo;
        }

        public decimal get_volume()
        {
            return Math.Round(this.width * this.height * this.depth, this.number_of_decimals);
        }

        public decimal get_total_weight()
        {
            decimal total_weight = 0;
            foreach (Item item in this.items)
            {
                total_weight += item.weight;
            }
            return total_weight;
        }
    }

    public class Packer
    {
        public List<Bin> bins { get; set; }
        public List<Item> items { get; set; }
        public List<Item> unfit_items { get; set; }
        public int total_items { get; set; }
        public Packer(RequestInfo requestinfo)
        {
            this.bins = new List<Bin>();
            this.items = new List<Item>();
            this.unfit_items = new List<Item>();
            this.total_items = 0;
            List<Container> containerinfo = requestinfo.ContainerInfo;
            List<Cargo> cargoinfo = requestinfo.CargoInfo;
            List<BinInfo> retlist = new List<BinInfo>();

            foreach (Container container in containerinfo)
            {
                this.add_bin(new Bin(container.ContainerName, (decimal)container.ContainerWidth, (decimal)container.ContainerHeight, (decimal)container.ContainerDepth, (decimal)container.ContainerMaxWeight, container.ContainerIndex));
            }

            foreach (Cargo cargo in cargoinfo)
            {
                this.add_item(new Item(cargo.CargoName, (decimal)cargo.CargoWidth, (decimal)cargo.CargoHeight, (decimal)cargo.CargoDepth, (decimal)cargo.CargoWeight, cargo.CargoRotationInfo, cargo.CargoCount));
            }
        }
        public void add_bin(Bin bin)
        {
            this.bins.Add(bin);
        }
        public void add_item(Item item)
        {
            this.total_items = this.items.Count + 1;
            this.items.Add(item);
        }
        public List<BinInfo> Result()
        {
            List<BinInfo> retlist = new List<BinInfo>();

            foreach (Bin bin in this.bins)
            {
                retlist.Add(bin.result);
            }

            return retlist;
        }
    }

    public class Candidate
    {
        public decimal redu_area { get; set; }
        public List<decimal> position { get; set; }
        public int rotate { get; set; }
        public Candidate(int redu_area, List<decimal> position)
        {
            this.redu_area = redu_area;
            this.position = position;
            this.rotate = 0;
        }
        public void plus_area(decimal area)
        {
            this.redu_area += area;
        }
        public void show_candidate()
        {
            Console.WriteLine(this.redu_area);
            Console.WriteLine(this.position);
            Console.WriteLine(this.rotate);
            Console.WriteLine("\n");
        }
    }

    public static class Utils
    {
        public const int DEFAULT_NUMBER_OF_DECIMALS = 3;
        public static List<decimal> START_POSITION = new List<decimal> { 0, 0, 0 };
        public static bool rect_intersect(Item item1, Item item2, int x, int y)
        {
            List<decimal> d1 = item1.get_dimension();
            List<decimal> d2 = item2.get_dimension();

            decimal cx1 = item1.position[x] + d1[x] / 2;
            decimal cy1 = item1.position[y] + d1[y] / 2;
            decimal cx2 = item2.position[x] + d2[x] / 2;
            decimal cy2 = item2.position[y] + d2[y] / 2;

            decimal ix = Math.Max(cx1, cx2) - Math.Min(cx1, cx2);
            decimal iy = Math.Max(cy1, cy2) - Math.Min(cy1, cy2);

            return ix < (d1[x] + d2[x]) / 2 & iy < (d1[y] + d2[y]) / 2;
        }

        public static bool intersect(Item item1, Item item2)
        {
            return (rect_intersect(item1, item2, Axis.WIDTH, Axis.HEIGHT) &
                    rect_intersect(item1, item2, Axis.HEIGHT, Axis.DEPTH) &
                    rect_intersect(item1, item2, Axis.WIDTH, Axis.DEPTH));
        }
    }

    public static class Axis
    {
        public const int WIDTH = 0;
        public const int HEIGHT = 1;
        public const int DEPTH = 2;

        public static readonly List<int> ALL = new List<int> { WIDTH, HEIGHT, DEPTH };
    }

    public static class RotationType
    {
        public const int RT_WHD = 0;
        public const int RT_HWD = 1;
        public const int RT_HDW = 2;
        public const int RT_DHW = 3;
        public const int RT_DWH = 4;
        public const int RT_WDH = 5;

        public static readonly List<int> ALL = new List<int> { RT_WHD, RT_HWD, RT_HDW, RT_DHW, RT_DWH, RT_WDH };
        public static readonly List<int> WIDTH = new List<int> { RT_HDW, RT_DHW };
        public static readonly List<int> HEIGHT = new List<int> { RT_DWH, RT_WDH };
        public static readonly List<int> DEPTH = new List<int> { RT_WHD, RT_HWD };
    }
}
