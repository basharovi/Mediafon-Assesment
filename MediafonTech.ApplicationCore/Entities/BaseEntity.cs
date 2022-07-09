
namespace MediafonTech.ApplicationCore.Entities
{
    public abstract class BaseEntity<T>
    {
        public virtual T Id { get; set; }
    }
}
