using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehousePacking.Models
{
    public class RequestInfo
    {
        public List<Container> ContainerInfo { get; set; }
        public List<Cargo> CargoInfo { get; set; }
    }

    public class Container
    {
        public string ContainerName { get; set; }
        public float ContainerWidth { get; set; }
        public float ContainerHeight { get; set; }
        public float ContainerDepth { get; set; }
        public float ContainerMaxWeight { get; set; }
        public int ContainerIndex { get; set; }
    }

    public class Cargo
    {
        public string CargoName { get; set; }
        public float CargoWidth { get; set; }
        public float CargoHeight { get; set; }
        public float CargoDepth { get; set; }
        public float CargoWeight { get; set; }
        public int CargoCount { get; set; }
        public string CargoRotationInfo { get; set; }
        public string CargoColor { get; set; }
    }
}
