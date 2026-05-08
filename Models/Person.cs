namespace Models
{
    public class Person : Entity
    {
        //EF rozpoznaje nazwy pól, które mogą być używane do mapowania właściwości, takie jak:
        //private string firstName;
        private string _firstName;
        //private string m_firstName;

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                FirstNameLength = value.Length;
            }
        }
        public string LastName { get; set; }
        public Address? Address { get; set; }

        public int FirstNameLength { get; private set; }
    }
}
