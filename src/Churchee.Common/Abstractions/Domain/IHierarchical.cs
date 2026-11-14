namespace Churchee.Common.Abstractions.Entities
{
    public interface IHierarchical<T>
    {
        Guid? ParentId { get; set; }

        ICollection<T> Children { get; set; }
    }
}
