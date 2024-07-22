using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAssignment.Models
{
    public class StemData
    {
        public string MachineKey { get; set; }
        public string StemKey { get; set; }
        public string StemNumber{ get; set; }
        public string receiverPosition { get; set; }
        public string coordinateReferenceSystem { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Altitude { get; set; }

    }
}
