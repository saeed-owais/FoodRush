using FoodRush.Domain.Interfaces;

namespace FoodRush.Domain.Common
{
    public abstract class BaseEntity
      : IAuditable,
        ISoftDeletable,
        IConcurrencyAware
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();

        public DateTime CreatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Guid? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public Guid? DeletedBy { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}
