using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Core;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataAccess.Repositories.Impl
{
    public class UserRepository : IUserRepository
    {
        private ICache _cache;

        public UserRepository(ICache cache)
        {
            _cache = cache;
        }

        public UserProfile GetUserProfile(int userId)
        {
            string key = "UserProfile_" + userId + "_GetUserProfile";
            UserProfile result = null;

            if (_cache.Exists(key))
                result = (UserProfile) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    var oq = from x in dc.UserProfiles where x.UserId == userId select x;
                    result = oq.FirstOrDefault();

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public UserProfile GetUserProfile(string email)
        {
            string key = "UserProfile_" + email + "_GetUserProfile";
            UserProfile result = null;

            if (_cache.Exists(key))
                result = (UserProfile) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.UserProfiles.FirstOrDefault(x => x.Email.Equals(email));

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public UserProfile GetUserProfileByUserName(string username)
        {
            string key = "UserProfile_" + username + "_GetUserProfileByUserName";
            UserProfile result = null;

            if (_cache.Exists(key))
                result = (UserProfile)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.UserProfiles.FirstOrDefault(x => x.UserName.Equals(username));

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public Membership GetMembership(int userId, bool withUserProfile=false)
        {
            string key = "Membership_" + userId + "_GetMembership";
            Membership result = null;

            if (_cache.Exists(key))
                result = (Membership)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.Memberships.FirstOrDefault(x => x.UserId == userId);

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public Membership GetMembershipByVerificationToken(string token, bool withUserProfile=false)
        {
            string key = "Membership_" + token + "_GetMembership";
            Membership result = null;

            if (_cache.Exists(key))
                result = (Membership)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.Memberships.FirstOrDefault(x => x.PasswordVerificationToken.Equals(token));

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public OAuthMembership GetOAuthMembership(string provider, string providerUserId)
        {
            string key = "OAuthMembership_" + provider + "_" + providerUserId + "_GetOAuthMembership";
            OAuthMembership result = null;

            if (_cache.Exists(key))
                result = (OAuthMembership)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.OAuthMemberships.FirstOrDefault(x => x.Provider.Equals(provider) &&
                                                                     x.ProviderUserId.Equals(providerUserId));

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public IEnumerable<OAuthMembership> GetOAuthMembershipsByUserName(string userName)
        {
            string key = "OAuthMemberships_" + userName + "_GetOAuthMembershipsByUserName";
            IEnumerable<OAuthMembership> result = null;

            if (_cache.Exists(key))
                result = (IEnumerable<OAuthMembership>)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.OAuthMemberships.Where(x => x.UserProfile.UserName.Equals(userName, 
                        StringComparison.OrdinalIgnoreCase)).ToList();

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public int Save(UserProfile userProfile, IEnumerable<Expression<Func<UserProfile, object>>> properties,
                        Membership membership)
        {
            using (var dc = new TryOnMirrorEntities())
            {

                if (userProfile.UserId == 0)
                {
                    dc.UserProfiles.Add(userProfile);
                }
                else
                {
                    if (properties != null)
                    {
                        dc.UserProfiles.Attach(userProfile);
                        ObjectStateEntry entry = ((IObjectContextAdapter) dc).ObjectContext.ObjectStateManager
                            .GetObjectStateEntry(userProfile);

                        foreach (var selector in properties)
                        {
                            string propertyName = selector.Body.PropertyToString();
                            entry.SetModifiedProperty(propertyName);
                        }
                    }
                }

                if (membership != null && membership.UserId == 0)
                {
                    membership.UserId = userProfile.UserId;
                    dc.Memberships.Add(membership);
                }

                dc.SaveChanges();
            }

            return userProfile.UserId;
        }
        
        public int Save(UserProfile userProfile, IEnumerable<Expression<Func<UserProfile, object>>> properties)
        {
            return Save(userProfile, properties, null);
        }

        public int Save(Membership membership, IEnumerable<Expression<Func<Membership, object>>> properties,
                        UserProfile userProfile)
        {
            using (var dc = new TryOnMirrorEntities())
            {

                if (userProfile.UserId == 0)
                {
                    dc.UserProfiles.Add(userProfile);
                }

                if (membership != null && membership.UserId == 0)
                {
                    membership.UserId = userProfile.UserId;
                    dc.Memberships.Add(membership);
                }
                else
                {
                    if (properties != null)
                    {
                        dc.Memberships.Attach(membership);
                        ObjectStateEntry entry = ((IObjectContextAdapter)dc).ObjectContext.ObjectStateManager
                            .GetObjectStateEntry(userProfile);

                        foreach (var selector in properties)
                        {
                            string propertyName = selector.Body.PropertyToString();
                            entry.SetModifiedProperty(propertyName);
                        }
                    }
                }

                dc.SaveChanges();
            }

            return membership.UserId;
        }

        public int Save(Membership membership, IEnumerable<Expression<Func<Membership, object>>> properties)
        {
            using (var dc = new TryOnMirrorEntities())
            {
                if (properties == null)
                {
                    dc.Memberships.Add(membership);
                }
                else
                {
                    dc.Memberships.Attach(membership);
                    ObjectStateEntry entry = ((IObjectContextAdapter) dc).ObjectContext.ObjectStateManager
                        .GetObjectStateEntry(membership);

                    foreach (var selector in properties)
                    {
                        string propertyName = selector.Body.PropertyToString();
                        entry.SetModifiedProperty(propertyName);
                    }
                }

                dc.SaveChanges();
            }

            return membership.UserId;
        }

        public int Save(OAuthMembership oAuthMembership, IEnumerable<Expression<Func<OAuthMembership, object>>> properties,
                        UserProfile userProfile)
        {
            using (var dc = new TryOnMirrorEntities())
            {

                if (userProfile != null && userProfile.UserId == 0)
                {
                    dc.UserProfiles.Add(userProfile);
                }

                if (oAuthMembership != null && oAuthMembership.UserId == 0)
                {
                    //oAuthMembership.UserId = userProfile.UserId;
                    dc.OAuthMemberships.Add(oAuthMembership);
                }
                else
                {
                    if (properties != null)
                    {
                        dc.OAuthMemberships.Attach(oAuthMembership);
                        ObjectStateEntry entry = ((IObjectContextAdapter)dc).ObjectContext.ObjectStateManager
                            .GetObjectStateEntry(userProfile);

                        foreach (var selector in properties)
                        {
                            string propertyName = selector.Body.PropertyToString();
                            entry.SetModifiedProperty(propertyName);
                        }
                    }
                }

                dc.SaveChanges();
            }

            return oAuthMembership.UserId;
        }

        public int Save(OAuthMembership oAuthMembership, IEnumerable<Expression<Func<OAuthMembership, object>>> properties)
        {
            using (var dc = new TryOnMirrorEntities())
            {
                //if (oAuthMembership.UserId == 0)
                //{
                    //oAuthMembership.UserId = userProfile.UserId;
                    dc.OAuthMemberships.Add(oAuthMembership);
                //}
                //else
                //{
                //    if (properties != null)
                //    {
                //        dc.OAuthMemberships.Attach(oAuthMembership);
                //        ObjectStateEntry entry = ((IObjectContextAdapter)dc).ObjectContext.ObjectStateManager
                //            .GetObjectStateEntry(oAuthMembership);

                //        foreach (var selector in properties)
                //        {
                //            string propertyName = selector.Body.PropertyToString();
                //            entry.SetModifiedProperty(propertyName);
                //        }
                //    }
                //}

                dc.SaveChanges();
            }

            return oAuthMembership.UserId;
        }
    }
}