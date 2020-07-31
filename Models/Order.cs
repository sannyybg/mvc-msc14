using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace muscshop.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter FirstName")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter LastName")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter Address")]
        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        [Required(ErrorMessage = "Please enter Phone")]
        [MinLength(9, ErrorMessage = "Min 9 symbols")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please enter Email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public decimal Total { get; set; }

        public DateTime OrderDate { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }




    }
}