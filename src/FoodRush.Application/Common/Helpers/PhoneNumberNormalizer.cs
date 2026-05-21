namespace FoodRush.Application.Common.Helpers
{
    internal static class PhoneNumberNormalizer
    {
        public static string Normalize(string phoneNumber)
        {
            return phoneNumber
            .Replace(" ", "")
            .Replace("-", "")
            .Trim();
        }
    }
}
