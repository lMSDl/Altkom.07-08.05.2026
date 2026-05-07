namespace Models
{
    public class Order : Entity
    {
        public string Name { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public ICollection<Product> Products { get; set; } = [];
    }
}
