namespace FoodRush.Domain.Interfaces
{
    public interface IConcurrencyAware
    {
        byte[] RowVersion { get; set; }
    }
}
