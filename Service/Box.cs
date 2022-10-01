using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Box
    {
        public DataX DataX { get; set; }
        public DataY DataY { get; set; }
        public int AvailableAmount { get; set; }
        public int AmountBeforePurchase { get; set; }

        public Box(DataX dataX, DataY dataY, int amountBeforePurchase, int newAmount)
        {
            DataX = dataX;
            DataY = dataY;
            AmountBeforePurchase = amountBeforePurchase;
            AvailableAmount = newAmount;            
        }
        public override string ToString()
        {
            return $"The dimensions of the box are: Length / Width: {DataX.X}, Height: {DataY.Y} and we can offer you {AmountBeforePurchase - AvailableAmount} units\n";            
        }
    }
}
