using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebStore.Data;

using WebStore.Models;

namespace WebStore
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        //machinelearning







        // Responsible for the view of cart
        public async Task<IActionResult> Index()
        {
           //get user name
            string userName = HttpContext.User.Identity.Name;
            //use user name to get user id
            var users = from o in _context.Users
                select o;
            users = users.Where(p => p.UserName == userName);

            //get all orders of the user that didnt paid
            var orders = from o in _context.Order
                select o;
            string id = users.First().Id;
            orders = orders.Where(p => p.UserId == id && p.Paid == false);

            //put all the orders in a list
            List<OrderViewModel> lovm = new List<OrderViewModel>();
            float pid=0;
            int price = 0;
            foreach (var item in orders)
            {
                var products = from p in _context.Product
                    select p;
                products = products.Where(p => p.Id.Equals(item.ProductId));
                pid = products.First().Id;
              
                OrderViewModel ovm = new OrderViewModel
                {
                    Id = item.Id,
                    Num = item.NumOfProducts,
                    Price = item.TotalPrice,
                    ProductName = products.First().Title
                };

                lovm.Add(ovm);
            }
            //if there are no orders put a single item in the list that says "cart is empty"
            if (!orders.Any())
            {
                List<OrderViewModel> empty = new List<OrderViewModel>();
                OrderViewModel emptyCart = new OrderViewModel
                {

                    ProductName = "Your cart is empty"
                };
                empty.Add(emptyCart);
                return View(empty);
            }

            //machine learning to predict a popular product
            string predictedProduct = Program.Predict(pid,(float)lovm[0].Num,0,1.8f);
         
            //search the database for information about the predicted product
            var products2 = from o in _context.Product
                select o;
            products2 = products2.Where(p => p.Id == Convert.ToInt32(predictedProduct));
            //send the data to the view
            ViewData["PredictionID"] = predictedProduct;
            ViewData["PredictionPrice"] = products2.First().Price;
            ViewData["PredictionTitle"] = products2.First().Title;
            return View(lovm.ToList());
        }

        //responsible to set orders to paid
        public async Task<IActionResult> Pay()
        {
            //get user data
            string userName = HttpContext.User.Identity.Name;
            var users = from o in _context.Users
                select o;
            users = users.Where(p => p.UserName == userName);

            //search for user's orders that didnt paid and set them to paid
            (from p in _context.Order
                    where p.UserId == users.First().Id
                    select p).ToList()
                .ForEach(x =>
                {
                    x.Paid = true;
                });

            _context.SaveChanges();
            return Redirect("/Orders/Index");
        }


        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .SingleOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,ProductId,NumOfProducts,Date,TotalPrice")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.SingleOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ProductId,NumOfProducts,Date,TotalPrice")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var products = from p in _context.Product
                                select p;
                    products = products.Where(p => p.Id == order.ProductId);
                    int price = products.First().Price;
                    order.TotalPrice = order.NumOfProducts * price;
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .SingleOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.SingleOrDefaultAsync(m => m.Id == id);
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}
