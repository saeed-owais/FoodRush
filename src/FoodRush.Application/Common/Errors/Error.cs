namespace FoodRush.Application.Common.Errors
{
    public record Error
    {
        public string Code { get; }
        public string Description { get; }
        public ErrorType ErrorType { get; }
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
        public static readonly Error NullValue = new("NullValue", "Value is null.", ErrorType.Failure);

        public Error(string code, string description, ErrorType errorType)
        {
            Code = code;
            Description = description;
            ErrorType = errorType;
        }

        public static Error Conflict(string code, string description) => new(code, description, ErrorType.Conflict);
        public static Error Failure(string code, string description) => new(code, description, ErrorType.Failure);
        public static Error NotFound(string code, string description) => new(code, description, ErrorType.NotFound);
        public static Error Problem(string code, string description) => new(code, description, ErrorType.Problem);
        public static Error Unauthorized(string code, string description) => new(code, description, ErrorType.Unauthorized);
        public static Error Validation(string code, string description) => new(code, description, ErrorType.Validation);
    }
}
