﻿namespace amazonbutnot.Models
{
    public class Cart
    {
        public List<CartLine> Lines { get; set; } = new List<CartLine>();

        public virtual void AddItem(Product prod, int quantity)
        {
            CartLine? line = Lines
                .Where(x => x.Product.product_ID == prod.product_ID)
                .FirstOrDefault();

            //has it already been added to the cart
            if (line == null)
            {
                Lines.Add(new CartLine()
                {
                    Product = prod,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        public virtual void RemoveLine(Product prod) => Lines.RemoveAll(x => x.Product.product_ID == prod.product_ID);

        public virtual void Clear() => Lines.Clear();

        public decimal CalculateTotal() => Lines.Sum(x => x.Product.price * x.Quantity);

       


        public class CartLine
        {
            public int CartLineID { get; set; }
            public Product Product { get; set; }
            public int Quantity { get; set; }

        }
    }
}
