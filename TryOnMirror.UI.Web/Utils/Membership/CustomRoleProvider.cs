using System;
using System.Web.Security;
using SymaCord.TryOnMirror.DataService.Services;
using SymaCord.TryOnMirror.UI.Web.App_Start;

namespace SymaCord.TryOnMirror.UI.Web.Utils.Membership
{
    public class CustomRoleProvider : RoleProvider
    {
        private readonly IRoleService _roleService;

        public CustomRoleProvider()
        {
            _roleService = WindsorBootstrapper.Container.Resolve<IRoleService>();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string userName)
        {
            return _roleService.GetUserRoles(userName);
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string userEmail, string roleName)
        {
            return _roleService.IsUserInRole(userEmail, roleName);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}