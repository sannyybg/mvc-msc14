﻿using muscshop.Context;
using muscshop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace muscshop.Helper
{
    public class ShoppingCart
    {
        private StoreContext _storeContext = new StoreContext();

        private string ShoppingCartId { get; set; }

        public const string CartSessionKey = "CartId";

        public static string GetCartId (HttpContextBase httpContext)
        {
            if (httpContext.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrEmpty(httpContext.User.Identity.Name))
                {
                    httpContext.Session[CartSessionKey] = httpContext.User.Identity.Name;
                }

                else
                {
                    var temporaryCartId = Guid.NewGuid();
                    httpContext.Session[CartSessionKey] = Guid.NewGuid().ToString();
                }
            }

            return httpContext.Session[CartSessionKey].ToString();
        }

        public decimal GetTotal()
        {
            var total = (from cartItems in _storeContext.Carts 
                         where cartItems.CartId == this.ShoppingCartId 
                         select (int?) cartItems.Count * cartItems.Album.Price).Sum();

            return 12;
        }

        public List<Cart> GetCartItems()
        {
            var result = _storeContext.Carts.Include("Album").Where(x => x.CartId == this.ShoppingCartId).ToList();

            return result;
        }

        public static ShoppingCart GetCart(HttpContextBase httpContext)
        {
            var cart = new ShoppingCart();

            cart.ShoppingCartId = GetCartId(httpContext);
            return cart;
        }

        public void AddToCart(Album album)
        {
            var cartItem = _storeContext.Carts.SingleOrDefault(x=>x.CartId == ShoppingCartId && x.AlbumId == album.AlbumId);

            if(cartItem == null)
            {
                cartItem = new Cart
                {
                    AlbumId = album.AlbumId,
                    CartId = ShoppingCartId,
                    Count = 1,
                    CreateDate = DateTime.Now
                };

                _storeContext.Carts.Add(cartItem);
            }

            else
            {
                cartItem.Count++;
            }

            _storeContext.SaveChanges();
        }

        public int GetCount()
        {
            int? count = (from cartItems in _storeContext.Carts
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Count).Sum();

            return count ?? 0;
        }

        public void MigrateCart(string username)
        {
            var guestcart = _storeContext.Carts.Where(x => x.CartId == ShoppingCartId).ToList();
            var usercart = _storeContext.Carts.Where(x => x.CartId == username).ToList();

            foreach (Cart item in guestcart)
            {
                var album = usercart.Where(x => x.AlbumId == item.AlbumId).FirstOrDefault();

                if (album != null)
                {
                    album.Count += item.Count;
                    _storeContext.Carts.Remove(item);
                    
                }
                else
                {
                    item.CartId = username;
                }
            }

            _storeContext.SaveChanges();
        }

        public int RemoveFromCart(int id)
        {
            var cartItem = _storeContext.Carts.Single(x => x.CartId == ShoppingCartId && x.RecordId == id);

            int itemCount = 0;

            if(cartItem.Count > 1)
            {
                cartItem.Count--;
                itemCount = cartItem.Count;
            }

            else
            {
                _storeContext.Carts.Remove(cartItem);
            }

            _storeContext.SaveChanges();

            return itemCount;
        }

        public void CreateOrder(Order order)
        {
            double orderTotal = 0;

            var cartItems = GetCartItems();

            foreach(var item in cartItems)
            {
                var orderDetail = new OrderDetail()
                {
                    AlbumId = item.AlbumId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Album.Price,
                    Quantity = item.Count
                };

                orderTotal += item.Count * item.Album.Price;

                

                _storeContext.OrderDetails.Add(orderDetail);
            }

            order.Total = (decimal)orderTotal;

            foreach(var item in cartItems)
            {
                _storeContext.Carts.Remove(item);
            }

            _storeContext.SaveChanges();


            
        }
    }
}