namespace ServiceManagementSystem.Core.Entities
{
    public class Product 
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int ServiceProviderId { get; set; }
        public ServiceProvider ServiceProvider { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
