using System.ComponentModel;

namespace Models
{
    public abstract class Entity : INotifyPropertyChanged
    {
        public /*virtual*/ int Id { get; set; }
        //public /*virtual*/ DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
