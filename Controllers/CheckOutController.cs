using muscshop.Context;
using muscshop.Helper;
using muscshop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace muscshop.Controllers
{
    public class CheckOutController : Controller
    {
        StoreContext _storeContext = new StoreContext(); 

        const string promoCode = "free";
        public ActionResult Payment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Payment(Order order)
        {
            var promo = Request.Form["PromoCode"];

            if(!string.Equals(promoCode, promo, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("Promocode", "Promo Code isn't valid");
                return View();
            }

            else
            {
                order.Username = User.Identity.Name;
                order.OrderDate = DateTime.Now;

                _storeContext.Orders.Add(order);
                _storeContext.SaveChanges();

                var cart = ShoppingCart.GetCart(HttpContext);

                cart.CreateOrder(order);
            }

            return RedirectToAction("Complete", order);

        }

        public ActionResult Complete(Order order)
        {
            return View(order);
        }
    }

}