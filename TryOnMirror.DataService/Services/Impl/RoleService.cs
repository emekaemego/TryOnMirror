using SymaCord.TryOnMirror.DataAccess.Repositories;

namespace SymaCord.TryOnMirror.DataService.Services.Impl
{
    public class RoleService : IRoleService
    {
        private IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public string[] GetUserRoles(string userName)
        {
            return _roleRepository.GetUserRoles(userName);
        }

        public string[] GetUserRolesByEmail(string userEmail)
        {
            return _roleRepository.GetUserRolesByEmail(userEmail);
        }

        public bool IsUserInRole(string userName, string roleName)
        {
            return _roleRepository.IsUserInRole(userName, roleName);
        }

        public bool IsUserInRoleByEmail(string userEmail, string roleName)
        {
            return _roleRepository.IsUserInRoleByEmail(userEmail, roleName);
        }
    }
}
