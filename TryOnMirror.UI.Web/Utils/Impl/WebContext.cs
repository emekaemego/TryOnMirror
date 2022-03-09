using System;
using System.Web;
using System.Web.SessionState;
using System.Collections.Generic;
using System.Collections;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.DataService.Services;

namespace SymaCord.TryOnMirror.UI.Web.Utils.Impl
{
    public class WebContext : IWebContext
    {
        private IConfiguration _config;
        private HttpSessionState _session;
        private readonly HttpCookieCollection _cookieCollection;
        private IUserService _userService;

        public WebContext(IConfiguration config, IUserService userService)
        {
            _config = config;
            _session = HttpContext.Current.Session;
            _cookieCollection = HttpContext.Current.Request.Cookies;
            _userService = userService;
        }

        public string CurrentUserEmail
        {
            get { return HttpContext.Current.User.Identity.Name; }
        }

        public string CurrentUserName
        {
            get { return HttpContext.Current.User.Identity.Name; }
        }

        public bool IsAuthenticated
        {
            get { return HttpContext.Current.User.Identity.IsAuthenticated; }
        }

        public void RemoveSession(string key)
        {
            _session.Remove(key);
        }

        public void AddSession(string key, object obj)
        {
            _session[key] = obj;
        }

        public object GetSession(string key)
        {
            return _session[key];
        }

        public void RemoveSessions(string prefix)
        {
            prefix = prefix.ToLower();
            List<string> itemsToRemove = new List<string>();

            var enumerator = (IDictionaryEnumerator) _session.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (enumerator.Key.ToString().ToLower().StartsWith(prefix))
                    itemsToRemove.Add(enumerator.Key.ToString());
            }

            foreach (string itemToRemove in itemsToRemove)
                _session.Remove(itemToRemove);
        }

        public string AnonymouseId
        {
            get { return HttpContext.Current.Request.AnonymousID; }
        }

        public int CurrentUserId
        {
            get { return _userService.GetUserProfileByUserName(HttpContext.Current.User.Identity.Name).UserId; }
        }

        public void SetCookieValue(string key, string value)
        {
            var cookie = new HttpCookie(key, value);
            //HttpContext.Current.Response.Cookies.Remove(key);
            HttpContext.Current.Response.SetCookie(cookie);
        }

        public void SetCookieValue(string key, string value, DateTime expireDate)
        {
            var cookie = new HttpCookie(key, value) {Expires = expireDate};
            //HttpContext.Current.Response.Cookies.Remove(key);
            HttpContext.Current.Response.SetCookie(cookie);
        }

        public void RemoveCookie(string key)
        {
            HttpContext.Current.Response.Cookies.Remove(key);
        }

        public string GetCookieValue(string key)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(key);

            return cookie != null ? cookie.Value : null;
        }
    }

    public class CookieKeys
    {
        public static string NewModelTrace = "NewModelTrace";

        public static string TempFileName = "TempFileName";

        public static string LatestTrace = "LatestTrace";

        public static string TraceTempFileName = "TraceTempFileName";

        public static string HairTempFileName = "HairTempFileName";

        public static string GlassTempFileName = "GlassTempFileName";

        public static string ContactTempFileName = "ContactTempFileName";

        public static string GlassCatId = "EyeSunglassCatId";

        public static string ExternalProviderUserEmail = "ExternalProviderUserEmail";
    }
}