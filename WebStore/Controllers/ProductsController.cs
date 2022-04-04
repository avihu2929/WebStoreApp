using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebStore.Data;
using WebStore.Models;

namespace WebStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // responsible for search view
        public async Task<IActionResult> Index(string SelectedCategory, string SearchString)
        {
            //get all categories
            var categories = from c in _context.Category
                        select c;
            categories = categories.Where(c => c.Title == SelectedCategory);
            int catId = 0;


            //catId = categories.ToList().First().Id;


            if (categories.Any())
            {
                catId = categories.First().Id;
            }
    
            IQueryable<string> CategoryQuery = from c in _context.Category orderby c.Title select c.Title;
            var products = from P in _context.Product select P;

            if (!string.IsNullOrEmpty(SearchString))
            {
                products = products.Where(s => s.Title.Contains(SearchString));
            }
            if (!string.IsNullOrEmpty(SelectedCategory))
            {
                products = products.Where(d => d.CategoryId == catId);
            }

            var toReturn = new ProductCategoryViewModel();
            toReturn.Categories = new SelectList(await CategoryQuery.Distinct().ToListAsync());
            toReturn.Products = await products.ToListAsync();
            toReturn.SelectedCategory = SelectedCategory;
            ViewData["Cat"] = SelectedCategory;
            return View(toReturn);
        }

        //responsible to add a single order to the cart
        [HttpPost]
        public async Task<IActionResult> InsertToCart(int pid, int num, int price)
        {
            //get user data
            string userName = HttpContext.User.Identity.Name;
            var users = from o in _context.Users
                        select o;
            users = users.Where(p => p.UserName == userName);
            string userId = users.First().Id;
            //get all user's orders
            var orders = from o in _context.Order
                         select o;
            orders = orders.Where(p => p.ProductId == pid && p.UserId == userId && p.Paid == false);



            //if there are open orders, update 
            if (orders.Any())
            {
                (from p in _context.Order
                 where p.Id == orders.First().Id
                 select p).ToList()
                    .ForEach(x =>
                    {
                        x.NumOfProducts += num;
                        x.TotalPrice += num * price;
                    });

                _context.SaveChanges();
                return Redirect("/Orders/Index");

            }
            else //otherwise create new order
            {
                
                Order ord = new Order
                {

                    UserId = userId,

                    NumOfProducts = num,
                    ProductId = pid
                    ,
                    TotalPrice = num * price,
                    Paid = false

                };
                if (num != 0)
                {
                    // Add the new object to the Orders collection.
                    _context.Order.Add(ord);

                    // Submit the change to the database.
                    try
                    {
                        _context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        // Make some adjustments.
                        // ...
                        // Try again.
                        _context.SaveChanges();
                    }
                    return Redirect("/Orders/Index");
                }
            }



            return Content("ERROR: Must have at least 1 item");

        }

        //show all the categories 
        public async Task<IActionResult> ByCategory(int? id)
        {

            var products = from p in _context.Product
                           select p;
            products = products.Where(p => p.CategoryId.Equals(id));

            var categoryTitle = from c in _context.Category
                           select c;
            categoryTitle = categoryTitle.Where(c => c.Id.Equals(id));
         
            ViewData["Message"] =  categoryTitle.First().Title;
            return View(await products.ToListAsync());

        }
        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoryId,Title,Price")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,Title,Price")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.SingleOrDefaultAsync(m => m.Id == id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
