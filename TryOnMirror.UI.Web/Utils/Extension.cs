using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;

namespace SymaCord.TryOnMirror.UI.Web.Utils
{
    public static class Extension
    {
        public static void InjectProperties(this IKernel kernel, object target)
        {
            var type = target.GetType();

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.CanWrite && kernel.HasComponent(property.PropertyType))
                {
                    var value = kernel.Resolve(property.PropertyType);

                    try
                    {
                        property.SetValue(target, value, null);
                    }

                    catch (Exception ex)
                    {
                        var message =
                            string.Format(
                                "Error setting property {0} on type {1}, See inner exception for more information.",
                                property.Name, type.FullName);

                        throw new ComponentActivatorException(message, ex);
                    }
                }
            }
        }

        public static string Partial(this Controller controller, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData,
                                                  controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public static bool Contains(this string[] source, string[] checkFor)
        {
            var result = false;

            foreach (var c in checkFor)
            {
                result = source.Contains(c, StringComparer.InvariantCultureIgnoreCase);
                if (result) break;
            }

            return result;
        }

        public static bool Contains(this string[] source, string checkFor)
        {
            var result = false;

            var splits = checkFor.Split(',');


            foreach (var s in splits)
            {
                result = source.Contains(s.TrimEnd(), StringComparer.InvariantCultureIgnoreCase);
                if (result) break;
            }

            return result;
        }

        public static bool Contains(this string source, string[] checkFor)
        {
            var result = false;

            foreach (var s in checkFor)
            {
                result = checkFor.Contains(source);
                if (result) break;
            }

            return result;
        }

        public static int ValidateMaxRows(this int? pageSize)
        {
            return pageSize.ValidateMaxRows(30);
        }

        public static int ValidateMaxRows(this int? pageSize, int count)
        {
            pageSize = count;

            if (pageSize.Value > 100)
                pageSize = 30;

            int maxRows = pageSize.Value;

            return maxRows;
        }

        public static DateTime UtcToLocal(this DateTime date, string timeZone = "W. Central Africa Standard Time")
        {
            // user-specified time zone
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZone);

            // an UTC DateTime
            var utcTime = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second,
                                       DateTimeKind.Utc);

            var offset = tzi.GetUtcOffset(utcTime);
            var zoneDateTime = utcTime.Add(offset);

            return zoneDateTime;
        }
    }
}