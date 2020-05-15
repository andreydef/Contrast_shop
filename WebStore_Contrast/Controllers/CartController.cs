using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using WebStore_Contrast.Models.Data;
using WebStore_Contrast.Models.ViewModels.Cart;

namespace WebStore_Contrast.Controllers
{
    public class CartController : Controller
    {
        // GET: MainCart
        public ActionResult MainCart()
        {
            // Assign the List<> with type CartVM
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // Check, that the cart is empty 
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "You cart is empty.";
                return View();
            }

            // Add sum and write to ViewBag
            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            // Return List<> in View()
            return View(cart);
        }

        //// GET: MainCart/CartPartial
        //public ActionResult CartPartial()
        //{
        //    // Assign the model CartVM
        //    CartVM model = new CartVM();

        //    // Assign the variable of quantity
        //    int qty = 0;

        //    // Assign the variable of price 
        //    decimal price = 0m;

        //    // Check the session of cart
        //    if (Session["cart"] != null)
        //    {
        //        // Get all quantity og goods and price
        //        var list = (List<CartVM>)Session["cart"];

        //        foreach (var item in list)
        //        {
        //            qty += item.Quantity;
        //            price += item.Quantity * item.Price;
        //        }

        //        model.Quantity = qty;
        //        model.Price = price;
        //    }
        //    else
        //    {
        //        // Or set the quantity and price to 0
        //        model.Quantity = 0;
        //        model.Price = 0m;
        //    }

        //    // Return the View() with model
        //    return View("Cart", model);
        //}

        // GET: MainCart/AddToCartPartial/id
        public ActionResult AddToCartPartial(int id)
        {
            // Assign the List<>, with type CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // Assign the model CartVM
            CartVM model = new CartVM();

            using (Db db = new Db())
            {
                // Get product
                ProductDTO product = db.Products.Find(id);

                // Check, whether the item is already in the cart
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                // If no, add new product to cart
                if (productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                else
                {
                    // If yes, add the item of product 
                    productInCart.Quantity++;
                }
            }

            // Get all quantity, price and add data to model
            int qty = 0;
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            // Save the state of cart to session 
            Session["cart"] = cart;

            // Return the PartialView() with model
            return PartialView("_AddToCartPartial", model);
        }

        // GET: MainCart/IncrementProduct/productId
        public JsonResult IncrementProduct(int productId)
        {
            // Assign List<> CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                // Get the model CartVM from List<>
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                // Add quantity 
                model.Quantity++;

                // Save data 
                var result = new
                {
                    qty = model.Quantity,
                    price = model.Price
                };

                // Return JSON answer with data
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: MainCart/DecrementProduct/productId
        public ActionResult DecrementProduct(int productId)
        {
            // Assign List<> CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                // Get the model CartVM from List<>
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                // Take away quantity (decrement)
                if (model.Quantity > 1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }

                // Save data 
                var result = new
                {
                    qty = model.Quantity,
                    price = model.Price
                };

                // Return JSON answer with data
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: MainCart/RemoveProduct/productId
        public void RemoveProduct(int productId)
        {
            // Assign List<> CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                // Get the model CartVM from List<>
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                // Remode the model
                cart.Remove(model);
            }
        }
    }
}