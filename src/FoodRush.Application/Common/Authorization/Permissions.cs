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

    public static class RolePermissions
    {
        public const string Read = "rolepermissions:read";
        public const string Assign = "rolepermissions:assign";
        public const string Remove = "rolepermissions:remove";
    }

    public static class UserRoles
    {
        public const string Read = "userroles:read";
        public const string Assign = "userroles:assign";
        public const string Remove = "userroles:remove";
    }

    public static class UserPermissions
    {
        public const string Read = "userpermissions:read";
        public const string Assign = "userpermissions:assign";
        public const string Remove = "userpermissions:remove";
    }

}