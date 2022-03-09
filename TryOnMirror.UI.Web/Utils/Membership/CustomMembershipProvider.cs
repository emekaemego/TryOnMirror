using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using SymaCord.TryOnMirror.Core;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.DataService.Services;
using SymaCord.TryOnMirror.Entities;
using SymaCord.TryOnMirror.UI.Web.App_Start;
using WebMatrix.WebData;

namespace SymaCord.TryOnMirror.UI.Web.Utils.Membership
{
    public class CustomMembershipProvider : ExtendedMembershipProvider
    {
        private readonly IUserService _userService;
        private ICache _cache;

        public CustomMembershipProvider()
        {
            _userService = WindsorBootstrapper.Container.Resolve<IUserService>();
            _cache = WindsorBootstrapper.Container.Resolve<ICache>();
        }

        #region MembershipProvider

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

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            //var user = _userService.GetUserProfileByUserName(username);

            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out System.Web.Security.MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override System.Web.Security.MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override System.Web.Security.MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(System.Web.Security.MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            var userProfile = _userService.GetUserProfileByUserName(username);
            if (userProfile == null)
            {
                return false;
            }

            var membership = _userService.GetMembership(userProfile.UserId);
            if (membership == null)
            {
                return false;
            }
            //if (!membership.IsConfirmed)
            //{
            //    return false;
            //}
            if (password.VerifyHash(membership.Password, membership.PasswordSalt))
            {
                return true;
            }
            // first once time we can validate through membership ConfirmationToken, 
            // to be logged in immediately after confirmation
            //if (membership.ConfirmationToken != null)
            //{
            //    if (membership.ConfirmationToken == password)
            //    {
            //        membership.ConfirmationToken = null;
            //        this.usersService.Save(membership, add: false);
            //        return true;
            //    }
            //}
            return false;
        }

        public override bool HasLocalAccount(int userId)
        {
            return (_userService.GetMembership(userId) != null);
        }

        #endregion MembershipProvider

        #region ExtendedMembershipProvider

        public override bool ConfirmAccount(string accountConfirmationToken)
        {
            //var membership = this.usersService.GetMembershipByConfirmToken(accountConfirmationToken, withUserProfile: false);
            //if (membership == null)
            //{
            //    throw new Exception("Activation code is incorrect.");
            //}
            //if (membership.IsConfirmed)
            //{
            //    throw new Exception("Your account is already activated.");
            //}
            //membership.IsConfirmed = true;
            //this.usersService.Save(membership, add: false);
            //return true;

            throw new NotImplementedException();
        }

        public override bool ConfirmAccount(string userName, string accountConfirmationToken)
        {
            throw new NotImplementedException();
        }

        public override string CreateAccount(string userName, string password, bool requireConfirmationToken)
        {
            var userProfile = _userService.GetUserProfileByUserName(userName);
            if (userProfile == null)
            {
                throw new Exception("No user with the specified User Name exist");
            }

            string hashedPassword;
            string passwordSalt;

            password.GetHashAndSalt(out hashedPassword, out passwordSalt);

            var membership = new Entities.Membership
            {
                UserId = userProfile.UserId,
                CreateDate = DateTime.UtcNow,
                Password = hashedPassword,
                PasswordSalt = passwordSalt,
                IsConfirmed = true,
                ConfirmationToken = Guid.NewGuid().ToString().ToLower()
            };

            _userService.Save(membership, null);

            //_cache.DeleteItems("userprofile_" + userName + "_");
            //_cache.DeleteItems("membership_" + userName + "_");

            return membership.ConfirmationToken;

            //throw new NotImplementedException();
        }

        public override string CreateUserAndAccount(string email, string password, bool requireConfirmation, 
            IDictionary<string, object> values)
        {
            email = email.Trim().ToLower();

            var userProfile = _userService.GetUserProfile(email);
            if (userProfile != null)
            {
                throw new MembershipCreateUserException(MembershipCreateStatus.DuplicateEmail);
            }

            //var dict = new Dictionary<string, object>(values);

            //object userN = null;

            //var exist = dict.TryGetValue("Keys", out userN);
            //var t = userN.GetType().FullName;

            var newUserProfile = new UserProfile { Email = email, UserName = values["UserName"].ToString() };
            //this.usersService.Save(newUserProfile);

            string hashedPassword;
            string passwordSalt;

            password.GetHashAndSalt(out hashedPassword, out passwordSalt);

            var membership = new Entities.Membership 
            {
                //UserId = newUserProfile.UserId,
                CreateDate = DateTime.UtcNow,
                Password = hashedPassword,
                PasswordSalt = passwordSalt,
                IsConfirmed = true,
                ConfirmationToken = Guid.NewGuid().ToString().ToLower()
            };
            _userService.Save(newUserProfile, null, membership);

            return membership.ConfirmationToken;
        }

        public override bool DeleteAccount(string userName)
        {
            throw new NotImplementedException();
        }

        public override string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow)
        {
            throw new NotImplementedException();
        }

        public override ICollection<OAuthAccountData> GetAccountsForUser(string userName)
        {
            var result = _userService.GetOAuthMembershipsByUserName(userName)
                .Select(x => new OAuthAccountData(x.Provider, x.ProviderUserId)).ToList();

            return result;
            //throw new NotImplementedException();
        }

        public override DateTime GetCreateDate(string userName)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetLastPasswordFailureDate(string userName)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetPasswordChangedDate(string userName)
        {
            throw new NotImplementedException();
        }

        public override int GetPasswordFailuresSinceLastSuccess(string userName)
        {
            throw new NotImplementedException();
        }

        public override int GetUserIdFromPasswordResetToken(string token)
        {
            throw new NotImplementedException();
        }

        public override bool IsConfirmed(string userName)
        {
            throw new NotImplementedException();
        }

        public override bool ResetPasswordWithToken(string token, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override int GetUserIdFromOAuth(string provider, string providerUserId)
        {
            var oAuthMembership = _userService.GetOAuthMembership(provider, providerUserId);
            if (oAuthMembership != null)
            {
                return oAuthMembership.UserId;
            }
            return -1;
        }

        public override void CreateOrUpdateOAuthAccount(string provider, string providerUserId, string userName)
        {
            var userProfile = _userService.GetUserProfileByUserName(userName);
            if (userProfile == null)
            {
                throw new Exception("User profile was not created.");
            }

            var oAuthMembership = new OAuthMembership
                {
                    Provider = provider,
                    ProviderUserId = providerUserId,
                    UserId = userProfile.UserId
                };

            _userService.Save(oAuthMembership, null);
            _cache.DeleteItems("oauthmemberships_" + userName + "_");
        }

        public override string GetUserNameFromId(int userId)
        {
            var userProfile = _userService.GetUserProfile(userId);
            if (userProfile != null)
            {
                return userProfile.UserName;
            }
            return null;
        }

        #endregion ExtendedMembershipProvider

        /*#region Helpers
        private const string salt = "HJIO6589";
        public static string GetHash(string text)
        {
            var buffer = Encoding.UTF8.GetBytes(String.Concat(text, salt));
            var cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            string hash = BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");
            return hash;
        }
        #endregion Helpers*/

    }
}