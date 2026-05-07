namespace Models
{
    public class Product : Entity
    {
        public virtual string Name { get; set; } = string.Empty;
        public virtual decimal Price { get; set; }
        public virtual Order Order { get; set; }
    }
}