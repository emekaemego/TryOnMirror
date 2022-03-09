namespace SymaCord.TryOnMirror.Core.Util
{
    public interface IConfiguration
    {
        string SiteAddress { get; }
        string InfoEmailAddress { get; }
        string SaleEmailAddress { get; }
        string NoReplyEmailAddress { get; }
        int DefaultCacheDurationDays { get; }
        int DefaultCacheDurationHours { get; }
        int DefaultCacheDurationMinutes { get; }
        int NumberOfRecordsInPage { get; }
        string SiteName { get; }
        string ReCaptchaPrivateKey { get; }
        string EncDecKey { get; }
        string SmsUrl { get; }
        string SmsAccount { get; }
        string SubAccount { get; }
        string SubAccountPwd { get; }
        string EmailTemplatePath { get; }
        string SmsTemplatePath { get; }
        bool EmailQueueDirty { get; set; }
        string NotificationEmailAddress { get; }
        string SupportEmailAddress { get; }
        string GenericErrorMessage { get; }
        int MaxPageRows { get; }
        string Environment { get; }
        string[] ReservedNames { get; }
        string[] ReservedDomains { get; }
    }
}