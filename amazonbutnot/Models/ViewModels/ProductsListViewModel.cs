namespace amazonbutnot.Models.ViewModels
{
    public class ProductsListViewModel
    {
        public IQueryable<Product> Products { get; set;}

        public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();
        public int? CurrentCategoryId { get; internal set; }
    }
}
