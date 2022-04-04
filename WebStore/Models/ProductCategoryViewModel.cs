using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebStore.Models
{
    public class ProductCategoryViewModel
    {
        public List<Product> Products;
        public SelectList Categories;
        public string SelectedCategory { get; set; }
    }
}
