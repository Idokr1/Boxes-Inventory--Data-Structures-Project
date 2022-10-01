using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class DataY : IComparable<DataY>
    {
        public double Y { get; set; }
        public int Amount { get; set; }
        public DoubleLinkedList<BoxByTime>.Node Node { get; set; }
        public DataY(double y, int amount)
        {
            Y = y;
            Amount = amount;
        }

        public int CompareTo(DataY other)
        {
            if (other != null)
                return this.Y.CompareTo(other.Y);
            else
                throw new ArgumentException("Object is not a DataY");
        }
    }
}
