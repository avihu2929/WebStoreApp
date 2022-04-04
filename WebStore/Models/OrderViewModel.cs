using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebStore.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public int Num { get; set; }
        public string ProductName { get; set; }
        public int Price { get; set; }
    }
}
