using FoodRush.Domain.Interfaces;

namespace FoodRush.Domain.Common
{
    public abstract class BaseEntity
      : IAuditable,
        ISoftDeletable,
        IConcurrencyAware
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}
