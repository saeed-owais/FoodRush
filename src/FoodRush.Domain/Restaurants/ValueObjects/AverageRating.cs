namespace FoodRush.Domain.Restaurants.ValueObjects;

public sealed record AverageRating
{
    private readonly double _value;
    public double Value => _value;
    private AverageRating(double value)
    {
        _value = value;
    }

    public static AverageRating Zero()
    {
        return new AverageRating(0);
    }

    internal static AverageRating FromReviews(IEnumerable<int> ratings)
    {
        var average = ratings.Average();

        return new AverageRating(average);
    }
}