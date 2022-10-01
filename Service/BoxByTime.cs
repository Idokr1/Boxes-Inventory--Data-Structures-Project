using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class BoxByTime
    {      
        public double X { get; set; }
        public double Y { get; set; }
        public DateTime lastBoughtDate { get; set; }
        public BoxByTime(double x, double y)
        {
            X = x;
            Y = y;
            lastBoughtDate = DateTime.Now;
        }
        public override string ToString()
        {
            return $"The dimensions of the box are: Length / Width: {X}, Height: {Y}";
        }
    }
}
