namespace SymaCord.TryOnMirror.DataAccess.Repositories
{
    public interface IRoleRepository
    {
        string[] GetUserRoles(string userName);
        bool IsUserInRole(string userName, string roleName);
        bool IsUserInRoleByEmail(string userEmail, string roleName);
        string[] GetUserRolesByEmail(string userEmail);
    }
}