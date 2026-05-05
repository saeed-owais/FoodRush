using FoodRush.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRush.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
