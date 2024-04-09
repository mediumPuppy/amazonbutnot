namespace amazonbutnot.Models
{
    public interface IProductRepository
    {
        public IQueryable<Product> Products { get; }
        public IQueryable<Customer> Customers { get; }
        public IQueryable<Country> Countries { get; }
        public IQueryable<Order> Orders { get; }

        public IQueryable<LineItem> LineItems { get; }
    }

}
