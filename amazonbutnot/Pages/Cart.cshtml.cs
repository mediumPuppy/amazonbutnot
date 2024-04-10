using amazonbutnot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace amazonbutnot.Pages
{
    public class CartModel : PageModel
    {
        private IProductRepository _prodRepository;

        public Cart Cart { get; set; }

        public CartModel(IProductRepository temp, Cart cartService)
        {
            _prodRepository = temp;
            Cart = cartService;
        }


        public string ReturnUrl { get; set; } = "/";
        public void OnGet(string returnUrl)
        {
            ReturnUrl = returnUrl ?? "/";

        }

        public IActionResult OnPost(int product_ID, string returnUrl)
        {
            Product prod = _prodRepository.Products
                .FirstOrDefault(x => x.product_ID == product_ID);

            if (prod != null)
            {

                Cart.AddItem(prod, 1);

            }

            return RedirectToPage(new { returnUrl = returnUrl });

        }

        public IActionResult OnPostRemove(int product_ID, string returnUrl)
        {
            Cart.RemoveLine(Cart.Lines.First(cl => cl.Product.product_ID == product_ID).Product);
            return RedirectToPage(new { returnUrl = returnUrl });
        }

    }
}
