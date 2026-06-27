namespace FoodRush.Domain.Common;

public abstract class Entity<TId> where TId : notnull
{
    public TId Id { get; protected set; }
    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other || GetType() != other.GetType())
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        if (Id == null || other.Id == null)
        {
            return false;
        }
        return Id.Equals(other.Id);
    }
    public override int GetHashCode()
    {
        return Id?.GetHashCode() ?? base.GetHashCode();
    }
}