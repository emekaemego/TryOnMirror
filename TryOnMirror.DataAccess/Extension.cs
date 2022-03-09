using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;

namespace SymaCord.TryOnMirror.DataAccess
{
    public static class Extension
    {
        public static ObjectContext GetObjectContext(this DbContext context)
        {
            var objectContext = ((IObjectContextAdapter) context).ObjectContext;

            return objectContext;
        }
    }
}
