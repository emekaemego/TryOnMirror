using System;

namespace SymaCord.TryOnMirror.UI.Web.Utils
{
    public interface IWebContext
    {
        string CurrentUserEmail { get; }
        bool IsAuthenticated { get; }
        void RemoveSession(string key);
        void AddSession(string key, object obj);
        object GetSession(string key);
        void RemoveSessions(string prefix);
        string AnonymouseId { get; }
        int CurrentUserId { get; }
        void SetCookieValue(string key, string value);
        string GetCookieValue(string key);
        void RemoveCookie(string key);
        void SetCookieValue(string key, string value, DateTime expireDate);
    }
}