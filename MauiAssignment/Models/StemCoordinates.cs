using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAssignment.Models
{
    public class StemCoordinates
    {
        public string receiverPosition { get; set; }
        public string coordinateReferenceSystem { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Altitude { get; set; }
    }
}
