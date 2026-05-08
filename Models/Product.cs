using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Product : Entity
    {
        private ILazyLoader? _lazyLoader;

        public Product() { }
        public Product(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }



        public /*virtual*/ string Name { get; set; } = string.Empty;
        public /*virtual*/ decimal Price { get; set; }

        //Lazy loading za pomocą proxy
        //public virtual Order? Order { get; set; }

        //Lazy loading za pomocą ILazyLoader
        public virtual Order? Order
        {
            get
            {
                if (field is null)
                {
                    try
                    {
                        _lazyLoader?.Load(this, ref field);

                    }
                    catch
                    {
                        field = null;
                    }
                }
                return field;
            }

            set
            {
                field = value;
            }
        }

        //odpowiednik IsRowVersion w fluent API
        //[Timestamp]
        //public byte[] Timestamp { get; }


        /*public float Weight { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
        public float Depth { get; set; }*/
        public ProductDetails Details { get; set; } = new ProductDetails();
    }
}