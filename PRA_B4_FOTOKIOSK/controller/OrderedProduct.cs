using System;
using System.Collections.Generic;
using System.Text;

namespace PRA_B4_FOTOKIOSK.models
{
    public class OrderedProduct
    {
        public int FotoId { get; set; }
        public string ProductName { get; set; }
        public int Amount { get; set; }
        public double TotalPrice { get; set; }
        public double Price { get; set; }
    }
}
