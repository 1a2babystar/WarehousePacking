using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehousePacking.Models
{
    public static class PackingAlgorithm
    {
        public static List<BinInfo> BestFit(RequestInfo requestinfo)
        {
            List<Container> containerinfo = requestinfo.ContainerInfo;
            List<Cargo> cargoinfo = requestinfo.CargoInfo;
            List<BinInfo> retlist = new List<BinInfo>();

            Packer packer = new Packer();
            foreach(Container container in containerinfo)
            {
                packer.add_bin(new Bin(container.ContainerName, (decimal)container.ContainerWidth, (decimal)container.ContainerHeight, (decimal)container.ContainerDepth, (decimal)container.ContainerMaxWeight, container.ContainerIndex));
            }

            foreach(Cargo cargo in cargoinfo)
            {
                int count = cargo.CargoCount;
                for(var i = 0; i < count; i++)
                {
                    packer.add_item(new Item(cargo.CargoName, (decimal)cargo.CargoWidth, (decimal)cargo.CargoHeight, (decimal)cargo.CargoDepth, (decimal)cargo.CargoWeight, cargo.CargoRotationInfo));
                }
            }

            packer.pack(true);

            foreach(Bin bin in packer.bins)
            {
                retlist.Add(bin.result);
            }

            return retlist;
        }
    }
}
