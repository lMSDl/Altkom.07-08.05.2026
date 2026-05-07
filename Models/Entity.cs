namespace Models
{
    public abstract class Entity
    {
        public virtual int Id { get; set; }
        public virtual DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
