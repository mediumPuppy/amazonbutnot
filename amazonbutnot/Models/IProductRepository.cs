namespace amazonbutnot.Models
{
    public interface IProductRepository
    {
        public IQueryable<AspNetUser> AspNetUsers { get; }
        public IQueryable<Product> Products { get; }
        public IQueryable<Country> Countries { get; }
        public IQueryable<Order> Orders { get; }

        public IQueryable<LineItem> LineItems { get; }

        Product GetProductById(int productId);
        void UpdateProduct(Product product);
        public void AddProduct(Product product);
        void DeleteProduct(Product product);
        public void AddOrder(Order order);
    }

}
