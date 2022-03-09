using System.Data.Objects;
using System.Linq;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataAccess.Repositories.Impl
{
   public class RoleRepository : IRoleRepository
   {
       private ICache _cache;

       public RoleRepository(ICache cache)
       {
           _cache = cache;
       }

       public string[] GetUserRoles(string userName)
       {
           string key = "Roles_" + userName + "_GetUserRoles";
           var result = new string[] { };

           if (_cache.Exists(key))
               result = (string[])_cache.Get(key);
           else
           {
               using (var dc = new TryOnMirrorEntities())
               {
                   //var query = "SELECT VALUE r.RoleName FROM {0}.Roles AS r WHERE EXSTS(SELECT VALUE u "+
                   //    "FROM r.UserProfiles AS u WHERE u.Email=@email)";
                   //var objContext = dc.GetObjectContext();

                   //var oq = new ObjectQuery<string>(string.Format(query, objContext.DefaultContainerName), objContext);
                   var query = from u in dc.UserProfiles
                               where u.UserName == userName
                               from r in u.Roles
                               select r.RoleName;


                   result = query.ToArray();

                   _cache.Set(key, result);
               }
           }

           return result;
       }

       public string[] GetUserRolesByEmail(string userEmail)
       {
           string key = "Roles_" + userEmail + "_GetUserRolesByEmail";
           var result = new string[]{};

           if (_cache.Exists(key))
               result = (string[])_cache.Get(key);
           else
           {
               using (var dc = new TryOnMirrorEntities())
               {
                   //var query = "SELECT VALUE r.RoleName FROM {0}.Roles AS r WHERE EXSTS(SELECT VALUE u "+
                   //    "FROM r.UserProfiles AS u WHERE u.Email=@email)";
                   //var objContext = dc.GetObjectContext();

                   //var oq = new ObjectQuery<string>(string.Format(query, objContext.DefaultContainerName), objContext);
                   var query = from u in dc.UserProfiles
                               where u.Email == userEmail
                               from r in u.Roles
                               select r.RoleName;


                   result = query.ToArray();

                   _cache.Set(key, result);
               }
           }

           return result;
       }

       public bool IsUserInRole(string userName, string roleName)
       {
           string key = "Role_" + userName + "_IsUserInRole";
           var result = false;

           if (_cache.Exists(key))
               result = (bool)_cache.Get(key);
           else
           {
               using (var dc = new TryOnMirrorEntities())
               {
                   var query = "SELECT VALUE COUNT(r.RoleName) FROM {0}.Roles AS r WHERE EXSTS(SELECT VALUE u " +
                       "FROM r.UserProfiles AS u WHERE u.UserName=@userName)";

                   var objContext = dc.GetObjectContext();

                   var oq = new ObjectQuery<int>(string.Format(query, objContext.DefaultContainerName), objContext);
                   oq.Parameters.Add(new ObjectParameter("userName", userName));
                   
                   result = (oq.FirstOrDefault() != 0);

                   _cache.Set(key, result);
               }
           }

           return result;
       }

       public bool IsUserInRoleByEmail(string userEmail, string roleName)
       {
           string key = "Role_" + userEmail + "_IsUserInRoleByEmail";
           var result = false;

           if (_cache.Exists(key))
               result = (bool)_cache.Get(key);
           else
           {
               using (var dc = new TryOnMirrorEntities())
               {
                   var query = "SELECT VALUE COUNT(r.RoleName) FROM {0}.Roles AS r WHERE EXSTS(SELECT VALUE u " +
                       "FROM r.UserProfiles AS u WHERE u.Email=@email)";

                   var objContext = dc.GetObjectContext();

                   var oq = new ObjectQuery<int>(string.Format(query, objContext.DefaultContainerName), objContext);
                   oq.Parameters.Add(new ObjectParameter("email", userEmail));
                   result = (oq.FirstOrDefault() != 0);

                   _cache.Set(key, result);
               }
           }

           return result;
       }

       //public string[] GetUsersInRole(string roleName)
       //{
       //    throw new NotImplementedException();
       //}
   }
}
