
namespace Models
{
    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

        public Coordinates Coordinates { get; set; }
    }
}
