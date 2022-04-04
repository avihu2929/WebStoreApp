using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int NumOfProducts { get; set; }
        public DateTime Date { get; set; }
        public int TotalPrice { get; set; }
        public bool Paid { get; set; }
    }
}
