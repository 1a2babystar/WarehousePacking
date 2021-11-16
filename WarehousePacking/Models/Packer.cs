using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehousePacking.Models
{
    public class Item
    {
        public string name { get; set; }
        public decimal width { get; set; }
        public decimal height { get; set; }
        public decimal depth { get; set; }
        public decimal weight { get; set; }
        public int rotation_type { get; set; }
        public List<decimal> position { get; set; }
        public List<Candidate> candidate { get; set; }
        public int number_of_decimals { get; set; }
        public List<int> rotation_allow { get; set; }

        public Item(string name, decimal width, decimal height, decimal depth, decimal weight, string rotationinfo)
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
            if(this.rotation_type == RotationType.RT_WHD)
            {
                dimension.Add(this.width);
                dimension.Add(this.height);
                dimension.Add(this.depth);
            }
            else if(this.rotation_type == RotationType.RT_HWD)
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
            this.items = new List<Item>();
            this.unfitted_items = new List<Item>();
            this.number_of_decimals = Utils.DEFAULT_NUMBER_OF_DECIMALS;
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
            foreach(Item item in this.items)
            {
                total_volume += item.get_volume();
            }
            return total_volume / this.get_volume() * 100;
        }

        public decimal get_item_vol_usage()
        {
            decimal fit_volume = 0;
            decimal total_volume = 0;
            foreach(Item item in this.items)
            {
                fit_volume += item.get_volume();
                total_volume += item.get_volume();
            }
            foreach(Item item in this.unfitted_items)
            {
                total_volume += item.get_volume();
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
            foreach(Item item in this.items)
            {
                total_weight += item.weight;
            }
            return total_weight;
        }

        public bool put_item(Item item, List<decimal> pivot)
        {
            bool fit = false;
            List<decimal> valid_item_position = item.position;
            item.position = pivot;

            foreach(int i in item.rotation_allow)
            {
                item.rotation_type = i;
                List<decimal> dimension = item.get_dimension();

                if(this.width < pivot[0] + dimension[0] | this.height < pivot[1] + dimension[1] | this.depth < pivot[2] + dimension[2])
                {
                    continue;
                }

                fit = true;

                foreach(Item current_item_in_bin in this.items)
                {
                    if(Utils.intersect(current_item_in_bin, item))
                    {
                        fit = false;
                        break;
                    }
                }

                if (fit)
                {
                    if(this.get_total_weight() + item.weight > this.max_weight)
                    {
                        fit = false;
                        return fit;
                    }

                    Candidate candid = new Candidate(0, pivot);
                    ////////
                    // Console.WriteLine("pivot: " + pivot[0] + " " + pivot[1] + " " + pivot[2]);
                    // Console.WriteLine("rotation: " + i);
                    // Console.WriteLine("+++");
                    ////////
                    if(pivot[0] == 0)
                    {
                        candid.plus_area(dimension[1] * dimension[2]);
                    }
                    else
                    {
                        foreach(Item it in this.items)
                        {
                            List<decimal> idim = it.get_dimension();
                            if(idim[0] + it.position[0] == pivot[0])
                            {
                                decimal tmpx1 = idim[1] + it.position[1] - pivot[1];
                                decimal tmpy1 = idim[2] + it.position[2] - pivot[2];
                                decimal tmpx2 = dimension[1] + pivot[1] - it.position[1];
                                decimal tmpy2 = dimension[2] + pivot[2] - it.position[2];
                                decimal wid = new decimal[] { tmpx1, tmpx2, idim[1], dimension[1] }.Min();
                                decimal hei = new decimal[] { tmpy1, tmpy2, idim[2], dimension[2] }.Min();
                                decimal w = Math.Max(wid, 0);
                                decimal h = Math.Max(hei, 0);
                                candid.plus_area(w * h);
                            }
                        }
                    }

                    if (pivot[1] == 0)
                    {
                        candid.plus_area(dimension[0] * dimension[2]);
                    }
                    else
                    {
                        foreach (Item it in this.items)
                        {
                            List<decimal> idim = it.get_dimension();
                            if (idim[1] + it.position[1] == pivot[1])
                            {
                                decimal tmpx1 = idim[0] + it.position[0] - pivot[0];
                                decimal tmpy1 = idim[2] + it.position[2] - pivot[2];
                                decimal tmpx2 = dimension[0] + pivot[0] - it.position[0];
                                decimal tmpy2 = dimension[2] + pivot[2] - it.position[2];
                                decimal wid = new decimal[] { tmpx1, tmpx2, idim[0], dimension[0] }.Min();
                                decimal hei = new decimal[] { tmpy1, tmpy2, idim[2], dimension[2] }.Min();
                                decimal w = Math.Max(wid, 0);
                                decimal h = Math.Max(hei, 0);
                                candid.plus_area(w * h);
                            }
                        }
                    }

                    if (pivot[2] == 0)
                    {
                        candid.plus_area(dimension[0] * dimension[1]);
                    }
                    else
                    {
                        foreach (Item it in this.items)
                        {
                            List<decimal> idim = it.get_dimension();
                            if (idim[2] + it.position[2] == pivot[2])
                            {
                                decimal tmpx1 = idim[0] + it.position[0] - pivot[0];
                                decimal tmpy1 = idim[1] + it.position[1] - pivot[1];
                                decimal tmpx2 = dimension[0] + pivot[0] - it.position[0];
                                decimal tmpy2 = dimension[1] + pivot[1] - it.position[1];
                                decimal wid = new decimal[] { tmpx1, tmpx2, idim[0], dimension[0] }.Min();
                                decimal hei = new decimal[] { tmpy1, tmpy2, idim[1], dimension[1] }.Min();
                                decimal w = Math.Max(wid, 0);
                                decimal h = Math.Max(hei, 0);
                                candid.plus_area(w * h);
                            }
                        }
                    }

                    candid.position = pivot;
                    candid.rotate = i;
                    item.candidate.Add(candid);
                }
            }
            if (!fit)
            {
                item.position = valid_item_position;
            }
            return fit;
        }
    }

    public class Packer
    {
        public List<Bin> bins { get; set; }
        public List<Item> items { get; set; }
        public List<Item> unfit_items { get; set; }
        public int total_items { get; set; }
        public Packer()
        {
            this.bins = new List<Bin>();
            this.items = new List<Item>();
            this.unfit_items = new List<Item>();
            this.total_items = 0;
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

        public void pack_to_bin(Bin bin, Item item)
        {
            if(bin.items.Count == 0)
            {
                bin.put_item(item, Utils.START_POSITION);
            }

            foreach(int axis in Axis.ALL)
            {
                List<Item> items_in_bin = bin.items;
                foreach(Item ib in items_in_bin)
                {
                    List<decimal> pivot = new List<decimal> { 0, 0, 0 };
                    List<decimal> di = ib.get_dimension();
                    decimal w, h, d;
                    w = di[0];
                    h = di[1];
                    d = di[2];
                    if(axis == Axis.WIDTH)
                    {
                        pivot[0] = ib.position[0] + w;
                        pivot[1] = ib.position[1];
                        pivot[2] = ib.position[2];
                    }
                    else if(axis == Axis.HEIGHT)
                    {
                        pivot[0] = ib.position[0];
                        pivot[1] = ib.position[1] + h;
                        pivot[2] = ib.position[2];
                    }
                    else if(axis == Axis.DEPTH)
                    {
                        pivot[0] = ib.position[0];
                        pivot[1] = ib.position[1];
                        pivot[2] = ib.position[2] + d;
                    }
                    bin.put_item(item, pivot);
                }

            }
            if(item.candidate.Count == 0)
            {
                bin.unfitted_items.Add(item);
            }
            else
            {
                List<Candidate> t = item.candidate.OrderByDescending(c => c.redu_area).ThenBy(c => c.position[2]).ThenBy(c => c.position[0]).ToList();
                Candidate candid = t[0];
                item.rotation_type = candid.rotate;
                item.position = candid.position;
                item.candidate.Clear();
                bin.items.Add(item);
            }
        }

        public void pack(bool bigger_first = false, bool distribute_items = false, int number_of_decimals= Utils.DEFAULT_NUMBER_OF_DECIMALS)
        {
            foreach(Bin bin in this.bins)
            {
                bin.format_numbers(number_of_decimals);
            }

            foreach(Item item in this.items)
            {
                item.format_numbers(number_of_decimals);
            }

            List<Item> tmp = this.items.OrderBy(i => -i.get_volume()).ToList();

            foreach(Bin bin in this.bins)
            {
                foreach(Item item in tmp)
                {
                    this.pack_to_bin(bin, item);
                }

                BinInfo bininfo = new BinInfo();
                List<ItemInfo> fitlist = new List<ItemInfo>();
                List<ItemInfo> unfitlist = new List<ItemInfo>();
                bininfo.bininfo = bin.@string();
                bininfo.binindex = bin.index;

                foreach(Item item in bin.items)
                {
                    fitlist.Add(item.@string());
                }

                foreach(Item item in bin.unfitted_items)
                {
                    unfitlist.Add(item.@string());
                }

                bininfo.fitlist = fitlist;
                bininfo.unfitlist = unfitlist;
                bininfo.binvolusage = bin.get_bin_vol_usage();
                bininfo.itemvolusage = bin.get_item_vol_usage();

                bin.result = bininfo;

                if (distribute_items)
                {
                    foreach(Item item in bin.items)
                    {
                        this.items.Remove(item);
                    }
                }
            }
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
