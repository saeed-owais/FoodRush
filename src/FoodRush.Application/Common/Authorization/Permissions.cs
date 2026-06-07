namespace FoodRush.Application.Common.Authorization;

public static class Permissions
{
    public static class Users
    {
        public const string Read = "users:read";
        public const string Create = "users:create";
        public const string Update = "users:update";
        public const string Delete = "users:delete";
    }

    public static class Roles
    {
        public const string Read = "roles:read";
        public const string Create = "roles:create";
        public const string Update = "roles:update";
        public const string Delete = "roles:delete";
    }

    public static class PermissionsManagement
    {
        public const string Assign = "permissions:assign";
        public const string Read = "permissions:read";
        public const string Create = "permissions:create";
        public const string Update = "permissions:update";
        public const string Delete = "permissions:delete";
    }
}