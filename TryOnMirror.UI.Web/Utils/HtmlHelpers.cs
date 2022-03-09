using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AForge.Imaging;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.UI.Web.Utils
{
    public static class HtmlHelpers
    {
        public static string ModelImage(this HtmlHelper htmlHelper, string fileName, bool thumbnail)
        {
            var requestContext = htmlHelper.ViewContext.RequestContext;
            fileName += thumbnail ? "_t" : string.Empty;
            fileName += ".jpg";

            var virPath =
                Helper.RelativeFromAbsolutePath(Helper.ConstructFilePath(fileName, Helper.ModelPhotoeFilePath, false),
                                                requestContext.HttpContext.Request);

            var url = new UrlHelper(requestContext);

            //if (!requestContext.HttpContext.Request.IsLocal)
            //    return url.Content("~" + virPath);

            return url.Content(virPath);
        }

        public static string PhotoImage(this HtmlHelper htmlHelper, string fileName, bool thumbnail)
        {
            var requestContext = htmlHelper.ViewContext.RequestContext;
            fileName += thumbnail ? "_t" : string.Empty;
            fileName += ".jpg";

            var virPath =
                Helper.RelativeFromAbsolutePath(Helper.ConstructFilePath(fileName, Helper.PhotoFilePath, false),
                                                requestContext.HttpContext.Request);

            var url = new UrlHelper(requestContext);

            //if (!requestContext.HttpContext.Request.IsLocal)
            //    return url.Content("~" + virPath);

            return url.Content(virPath);
        }

        public static string SharedMakeoverImage(this HtmlHelper htmlHelper, string fileName)
        {
            var requestContext = htmlHelper.ViewContext.RequestContext;
            fileName += ".jpg";

            var virPath = Helper.RelativeFromAbsolutePath
                (Helper.ConstructFilePath(fileName, Helper.MakeoverImageFilePath, false),
                 requestContext.HttpContext.Request);

            var url = new UrlHelper(requestContext);

            return url.Content(virPath);
        }

        public static string MakeoverImage(this HtmlHelper htmlHelper, string fileName, bool thumbnail)
        {
            var requestContext = htmlHelper.ViewContext.RequestContext;
            fileName += thumbnail ? "_150" : "_o";
            fileName += ".jpg";

            var virPath =
                Helper.RelativeFromAbsolutePath(Helper.ConstructFilePath(fileName, Helper.MakeoverImageFilePath, false),
                                                requestContext.HttpContext.Request);

            var url = new UrlHelper(requestContext);

            //if (!requestContext.HttpContext.Request.IsLocal)
            //    return url.Content("~" + virPath);

            return url.Content(virPath);
        }

        public static string HairstyleImage(this HtmlHelper htmlHelper, string fileName, bool thumbnail)
        {
            fileName += thumbnail ? "_t" : string.Empty;
            fileName += ".jpg";
            var requestContext = htmlHelper.ViewContext.RequestContext;

            var virPath =
                Helper.RelativeFromAbsolutePath(Helper.ConstructFilePath(fileName, Helper.HairstyleFilePath, false),
                                                requestContext.HttpContext.Request);

            var url = new UrlHelper(requestContext);

            //if(!requestContext.HttpContext.Request.IsLocal)
            //    return url.Content("~" + virPath);

            return url.Content(virPath);
        }

        //public static string Image(this HtmlHelper htmlHelper, string fileName, string ext, string parentPath, string size)
        //{
        //    fileName += !string.IsNullOrEmpty(size) ? "_" + size : string.Empty;
        //    fileName += ext;

        //    var file = Helper.ConstructFilePath(fileName, Helper.HairstyleFilePath, false);
        //    int sz = 0;

        //    if(!File.Exists(file) && int.TryParse(size, out sz))
        //    {
        //        var img = new KalikoImage(file);
        //    }

        //    var virPath = Helper.RelativeFromAbsolutePath(file, htmlHelper.ViewContext.RequestContext.HttpContext.Request);

        //    var url = new UrlHelper(htmlHelper.ViewContext.RequestContext);
        //    return url.Content(virPath);
        //}

        public static string EyeWearImage(this HtmlHelper htmlHelper, string fileName, bool thumbnail)
        {
            var requestContext = htmlHelper.ViewContext.RequestContext;
            fileName += thumbnail ? "_t" : string.Empty;
            fileName += ".jpg";

            var virPath =
                Helper.RelativeFromAbsolutePath(Helper.ConstructFilePath(fileName, Helper.EyeWearFilePath, false),
                                                requestContext.HttpContext.Request);

            var url = new UrlHelper(requestContext);

            //if (!requestContext.HttpContext.Request.IsLocal)
            //    return url.Content("~" + virPath);

            return url.Content(virPath);
        }

        public static string LimitString(this HtmlHelper htmlHelper, string text, int lenght)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var result = (text.Length > lenght) ? text.Substring(0, lenght) : text;
                return result;
            }

            return null;
        }

        public static MvcHtmlString Timeago(this HtmlHelper helper, DateTime datetime)
        {
            datetime = ConvertUtcDateToLocalTimeZone("W. Central Africa Standard Time", datetime);
            var tag = new TagBuilder("abbr");
            tag.AddCssClass("timeago");
            tag.Attributes.Add("title", datetime.ToString("MMM dd, yyyy h:mm tt"));
            tag.SetInnerText(datetime.ToString());

            return MvcHtmlString.Create(tag.ToString());
        }

        public static MvcHtmlString BookingStatusIcon(this HtmlHelper helper, int status)
        {
            var icon = string.Empty;
            var stat = (HairBookingStatus) status;

            switch (stat)
            {
                case HairBookingStatus.Pending:
                    icon = "icon-info-2";
                    break;
                case HairBookingStatus.Approved:
                    icon = "icon-checkmark-2";
                    break;
                case HairBookingStatus.Done:
                    icon = "icon-happy";
                    break;
                case HairBookingStatus.Cancelled:
                    icon = "icon-cancel-4 ";
                    break;
            }

            var tag = new TagBuilder("i");
            tag.AddCssClass(icon);
            tag.Attributes.Add("title", stat.ToString());

            return MvcHtmlString.Create(tag.ToString());
        }

        public static string StatusText(this HtmlHelper helper, int statusId)
        {
            var result = string.Empty;
            var stat = (HairBookingStatus) statusId;

            switch (stat)
            {
                case HairBookingStatus.Pending:
                case HairBookingStatus.Approved:
                case HairBookingStatus.Done:
                case HairBookingStatus.Cancelled:
                    result = stat.ToString();
                    break;
            }

            return result;
        }

        public static string UserTimeZone(this HtmlHelper htmlHelper)
        {
            return htmlHelper.ViewContext.Controller.ViewBag.TimeZone;
        }

        private static DateTime ConvertUtcDateToLocalTimeZone(string timeZoneInfoId, DateTime date)
        {
            // user-specified time zone
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneInfoId);

            // an UTC DateTime
            var utcTime = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second,
                                       DateTimeKind.Utc);

            var offset = tzi.GetUtcOffset(utcTime);
            var zoneDateTime = utcTime.Add(offset);

            return zoneDateTime;
        }

        public static MvcHtmlString MenuItem(this HtmlHelper htmlHelper, string text, string action, string controller,
                                             string area = null)
        {
            var li = new TagBuilder("li");
            var routeData = htmlHelper.ViewContext.RouteData;

            var currentAction = routeData.GetRequiredString("action");
            var currentController = routeData.GetRequiredString("controller");
            var currentArea = routeData.DataTokens["area"] as string;

            if (string.Equals(currentAction, action, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(currentController, controller, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(currentArea, area, StringComparison.OrdinalIgnoreCase))
            {
                li.AddCssClass("active");
            }

            li.InnerHtml = htmlHelper.ActionLink(text, action, controller, new {area}, null).ToHtmlString();
            return MvcHtmlString.Create(li.ToString());
        }

        public static RGB HSL2RGB(this HtmlHelper htmlHelper, int h, float s, float l)
        {
            var rgb = new RGB();
            HSL.ToRGB(new HSL(h, s, l), rgb);

            return rgb;
        }

        public static string HSL2Hex(this HtmlHelper htmlHelper, int h, float s, float l)
        {
            var rgb = new RGB();
            HSL.ToRGB(new HSL(h, s, l), rgb);

            return "#" + rgb.Red.ToString("X2") + rgb.Green.ToString("X2") + rgb.Blue.ToString("X2");
        }
    }
}