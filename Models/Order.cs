using System.Collections.ObjectModel;

namespace Models
{
    public class Order : Entity
    {
        public virtual string Name { get; set; } = string.Empty;
        public virtual DateTime OrderDate { get; set; } = DateTime.Now;
        public virtual ICollection<Product> Products { get; set; } = new ObservableCollection<Product>();
    }
}
