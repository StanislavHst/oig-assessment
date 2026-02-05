namespace OIG.Assessment.Domain.Common;

public static class Permissions
{
    public const string ViewUserList = "ViewUserList";
    public const string AddUser = "AddUser";
    public const string EditUser = "EditUser";
    public const string DeleteUser = "DeleteUser";
    
    public const string ViewRolesList = "ViewRolesList";
    public const string AddRole = "AddRole";
    public const string EditRole = "EditRole";
    public const string DeleteRole = "DeleteRole";
    
    public const string AddOrganization = "AddOrganization";
    public const string EditOrganization = "EditOrganization";
    public const string DeleteOrganization = "DeleteOrganization";
    public const string AddRootOrganization = "AddRootOrganization";
    
    public static IReadOnlyList<string> GetAll()
    {
        return new[]
        {
            ViewUserList,
            AddUser,
            EditUser,
            DeleteUser,
            ViewRolesList,
            AddRole,
            EditRole,
            DeleteRole,
            AddOrganization,
            EditOrganization,
            DeleteOrganization,
            AddRootOrganization
        };
    }
    
    public static bool IsValid(string permission)
    {
        return GetAll().Contains(permission);
    }
}
