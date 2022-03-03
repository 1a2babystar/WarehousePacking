﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehousePacking.Models
{
    public static class PackingAlgorithm
    {
        public static List<BinInfo> BestFit(RequestInfo requestinfo)
        {
            BestFitPacker packer = new BestFitPacker(requestinfo);

            packer.pack(true);

            return packer.Result();
        }
    }

    public class BestFitPacker : Packer, PackerFuncs
    {
        public BestFitPacker(RequestInfo requestinfo) : base(requestinfo) { }
        public void pack(bool bigger_first = false, bool distribute_items = false, int number_of_decimals = Utils.DEFAULT_NUMBER_OF_DECIMALS)
        {
            foreach (Bin bin in this.bins)
            {
                bin.format_numbers(number_of_decimals);
            }

            foreach (Item item in this.items)
            {
                item.format_numbers(number_of_decimals);
            }

            List<Item> tmp = this.items.OrderBy(i => -i.get_volume()).ToList();

            foreach (Bin bin in this.bins)
            {
                foreach (Item item in tmp)
                {
                    this.pack_to_bin(bin, item);
                }

                BinInfo bininfo = new BinInfo();
                List<ItemInfo> fitlist = new List<ItemInfo>();
                List<ItemInfo> unfitlist = new List<ItemInfo>();
                bininfo.bininfo = bin.@string();
                bininfo.binindex = bin.index;

                foreach (Item item in bin.items)
                {
                    fitlist.Add(item.@string());
                }

                foreach (Item item in bin.unfitted_items)
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
                    foreach (Item item in bin.items)
                    {
                        this.items.Remove(item);
                    }
                }
            }
        }
        public void pack_to_bin(Bin bin, Item item)
        {
            if(bin.remain_volume < item.get_volume())
            {
                bin.unfitted_items.Add(item);
                return;
            }

            if (bin.items.Count == 0)
            {
                put_item(bin, item, Utils.START_POSITION);
            }

            foreach (List<decimal> position in bin.can_pos)
            {
                put_item(bin, item, position);
            }
            if (item.candidate.Count == 0)
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
                item.position_p = new List<decimal>(candid.position);
                List<decimal> dim = item.get_dimension();
                item.position_p[0] += dim[0];
                item.position_p[1] += dim[1];
                item.position_p[2] += dim[2];
                bin.can_pos.Remove(candid.position);
                bin.can_pos.Add(new List<decimal> { item.position_p[0], candid.position[1], candid.position[2] });
                bin.can_pos.Add(new List<decimal> { candid.position[0], item.position_p[1], candid.position[2] });
                bin.can_pos.Add(new List<decimal> { candid.position[0], candid.position[1], item.position_p[2] });
                bin.items.Add(item);
                bin.remain_volume -= item.get_volume();
            }
        }

        public bool put_item(Bin bin, Item item, List<decimal> pivot)
        {
            bool fit = false;
            List<decimal> valid_item_position = item.position;
            item.position = pivot;

            foreach (int i in item.rotation_allow)
            {
                item.rotation_type = i;
                List<decimal> dimension = item.get_dimension();

                if (bin.width < pivot[0] + dimension[0] | bin.height < pivot[1] + dimension[1] | bin.depth < pivot[2] + dimension[2])
                {
                    continue;
                }

                fit = true;

                foreach (Item current_item_in_bin in bin.items)
                {
                    if (Utils.intersect(current_item_in_bin, item))
                    {
                        fit = false;
                        break;
                    }
                }

                if (fit)
                {
                    if (bin.get_total_weight() + item.weight > bin.max_weight)
                    {
                        fit = false;
                        return fit;
                    }

                    Candidate candid = new Candidate(0, pivot);
                    if (pivot[0] == 0)
                    {
                        candid.plus_area(dimension[1] * dimension[2]);
                    }
                    else
                    {
                        foreach (Item it in bin.items)
                        {
                            List<decimal> idim = it.get_dimension();
                            if (it.position_p[0] == pivot[0])
                            {
                                decimal tmpx1 = it.position_p[1] - pivot[1];
                                decimal tmpy1 = it.position_p[2] - pivot[2];
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
                        foreach (Item it in bin.items)
                        {
                            List<decimal> idim = it.get_dimension();
                            if (it.position_p[1] == pivot[1])
                            {
                                decimal tmpx1 = it.position_p[0] - pivot[0];
                                decimal tmpy1 = it.position_p[2] - pivot[2];
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
                        candid.plus_area(2 * dimension[0] * dimension[1]);
                    }
                    else
                    {
                        foreach (Item it in bin.items)
                        {
                            List<decimal> idim = it.get_dimension();
                            if (it.position_p[2] == pivot[2])
                            {
                                decimal tmpx1 = it.position_p[0] - pivot[0];
                                decimal tmpy1 = it.position_p[1] - pivot[1];
                                decimal tmpx2 = dimension[0] + pivot[0] - it.position[0];
                                decimal tmpy2 = dimension[1] + pivot[1] - it.position[1];
                                decimal wid = new decimal[] { tmpx1, tmpx2, idim[0], dimension[0] }.Min();
                                decimal hei = new decimal[] { tmpy1, tmpy2, idim[1], dimension[1] }.Min();
                                decimal w = Math.Max(wid, 0);
                                decimal h = Math.Max(hei, 0);
                                candid.plus_area(2 * w * h);
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
}