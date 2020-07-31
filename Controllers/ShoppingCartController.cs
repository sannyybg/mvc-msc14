using muscshop.Context;
using muscshop.filters;
using muscshop.Helper;
using muscshop.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace muscshop.Controllers
{
    [SessionFilter]
    public class ShoppingCartController : Controller
    {
        private StoreContext _storeContext = new StoreContext();
        public ActionResult Index()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            var cartViewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };
            

            return View(cartViewModel);
        }

        [HttpPost]
        public ActionResult AddToCart(int id)
        {
            var album = _storeContext.Albums.Single(x => x.AlbumId == id);
                
            var currentcart = ShoppingCart.GetCart(this.HttpContext);

            currentcart.AddToCart(album);
            

            return RedirectToAction("Index");
        }

        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = ShoppingCart.GetCart(HttpContext);

            ViewData["Cartsummary"] = cart.GetCount();

            return PartialView();
        }

        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            var cart = ShoppingCart.GetCart(HttpContext);

            var albumTitle = _storeContext.Carts.Include("Album").Single(x => x.RecordId == id).Album.Title;

            var itemcount = cart.RemoveFromCart(id);

            var removedalbum = new ShoppingCartRemoveViewModel();

            removedalbum.Message = albumTitle + "is removed from cart";
            removedalbum.CartCount = cart.GetCount();
            removedalbum.CartTotal = cart.GetTotal();
            removedalbum.DeletedId = id;
            removedalbum.ItemCount = itemcount;

            return Json(removedalbum);

        }
    }
}