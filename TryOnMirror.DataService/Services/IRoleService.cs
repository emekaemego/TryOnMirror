namespace SymaCord.TryOnMirror.DataService.Services
{
    public interface IRoleService
    {
        string[] GetUserRoles(string userName);
        bool IsUserInRole(string userName, string roleName);
        string[] GetUserRolesByEmail(string userEmail);
        bool IsUserInRoleByEmail(string userEmail, string roleName);
    }
}