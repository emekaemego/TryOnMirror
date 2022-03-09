using System;
using System.Configuration;

namespace SymaCord.TryOnMirror.Core.Util.Impl
{
    public class AppConfiguration : IConfiguration
    {
        public string SiteAddress
        {
            get { throw new NotImplementedException(); }
        }

        public string InfoEmailAddress
        {
            get { throw new NotImplementedException(); }
        }

        public string SaleEmailAddress
        {
            get { throw new NotImplementedException(); }
        }

        public string NoReplyEmailAddress
        {
            get { throw new NotImplementedException(); }
        }

        public int DefaultCacheDurationDays
        {
            get { return (int)getAppSetting(typeof(int), "DefaultCacheDuration_Day"); }
        }

        public int DefaultCacheDurationHours
        {
            get { return (int)getAppSetting(typeof(int), "DefaultCacheDuration_Hours"); }
        }

        public int DefaultCacheDurationMinutes
        {
            get { return (int)getAppSetting(typeof(int), "DefaultCacheDuration_Minutes"); }
        }

        public int NumberOfRecordsInPage
        {
            get { return (int)getAppSetting(typeof(int), "NumberOfRecordsInPage"); }
        }

        public string SiteName
        {
            get { throw new NotImplementedException(); }
        }

        public string ReCaptchaPrivateKey
        {
            get { throw new NotImplementedException(); }
        }

        public string EncDecKey
        {
            get { return getAppSetting(typeof(string), "encdecKey").ToString(); }
        }

        public string SmsUrl
        {
            get { throw new NotImplementedException(); }
        }

        public string SmsAccount
        {
            get { throw new NotImplementedException(); }
        }

        public string SubAccount
        {
            get { throw new NotImplementedException(); }
        }

        public string SubAccountPwd
        {
            get { throw new NotImplementedException(); }
        }

        public string EmailTemplatePath
        {
            get { return getAppSetting(typeof(string), "EmailTemplatePath").ToString(); }
        }

        public string SmsTemplatePath
        {
            get { return getAppSetting(typeof(string), "SmsTemplatePath").ToString(); }
        }

        public bool EmailQueueDirty
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string NotificationEmailAddress
        {
            get { throw new NotImplementedException(); }
        }

        public string SupportEmailAddress
        {
            get { throw new NotImplementedException(); }
        }

        public string GenericErrorMessage
        {
            get { throw new NotImplementedException(); }
        }

        public int MaxPageRows
        {
            get { return (int)getAppSetting(typeof(int), "MaxPageRows"); }
        }
        
        public string Environment
        {
            get { throw new NotImplementedException(); }
        }

        public string[] ReservedNames
        {
            get { throw new NotImplementedException(); }
        }

        public string[] ReservedDomains
        {
            get { throw new NotImplementedException(); }
        }
        
        private static object getAppSetting(Type expectedType, string key)
        {
            string value = ConfigurationManager.AppSettings.Get(key);
            if (value == null)
            {
                //Log.Fatal("Configuration.cs", string.Format("AppSetting: {0} is not configured", key));
                throw new Exception(string.Format("AppSetting: {0} is not configured.", key));
            }

            try
            {
                if (expectedType.Equals(typeof(int)))
                {
                    return int.Parse(value);
                }

                if (expectedType.Equals(typeof(string)))
                {
                    return value;
                }

                throw new Exception("Type not supported.");
            }
            catch (Exception ex)
            {
                //Log.Fatal("Configuration.cs", string.Format("Config key:{0} was expected to be of type {1} but was not.", key, expectedType));
                throw new Exception(string.Format("Config key:{0} was expected to be of type {1} but was not.",
                                                  key, expectedType), ex);
            }
        }

        private static void setAppSetting(string key, string value)
        {
            string appKey = ConfigurationManager.AppSettings.Get(key);
            if (appKey == null)
            {
                throw new ApplicationException("Application configuration error. Please contact your system admin.");
            }
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"]);

            config.AppSettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
        }
    }
}