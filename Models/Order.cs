using NetTopologySuite.Geometries;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Order : Entity
    {
        //odpowiednik IsConcurrencyToken w fluent API
        //[ConcurrencyCheck]
        public /*virtual*/ string Name
        {
            get => field;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = string.Empty;
        public /*virtual*/ DateTime OrderDate { get; set; } = DateTime.Now;
        public virtual ICollection<Product> Products { get; set; } = new ObservableCollection<Product>();

        public float Value { get; set; }
        public float Tax { get; set; }

        //public float TotalValue => Value * (1+ Tax);
        public float TotalValue { get; }

        public bool IsExpired { get; }

        public OrderType Type { get; set; } = OrderType.Standard;
        public OrderParameters Parameters { get; set; } = OrderParameters.None;

        public Point? DeliveryPoint { get; set; }
    }
}
