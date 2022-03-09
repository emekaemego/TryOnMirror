using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.DataAccess.Repositories;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataService.Services.Impl
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;
        private ICache _cache;

        public UserService(IUserRepository userRepository, ICache cache)
        {
            _userRepository = userRepository;
            _cache = cache;
        }

        public UserProfile GetUserProfile(int userId)
        {
            return _userRepository.GetUserProfile(userId);
        }

        public UserProfile GetUserProfile(string email)
        {
            return _userRepository.GetUserProfile(email);
        }

        public UserProfile GetUserProfileByUserName(string username)
        {
            return _userRepository.GetUserProfileByUserName(username);
        }

        public Membership GetMembership(int userId, bool withUserProfile=false)
        {
            return _userRepository.GetMembership(userId, withUserProfile);
        }

        public Membership GetMembershipByVerificationToken(string token, bool withUserProfile=false)
        {
            return _userRepository.GetMembershipByVerificationToken(token, withUserProfile);
        }

        public OAuthMembership GetOAuthMembership(string provider, string providerUserId)
        {
            return _userRepository.GetOAuthMembership(provider, providerUserId);
        }

        public IEnumerable<OAuthMembership> GetOAuthMembershipsByUserName(string userName)
        {
            return _userRepository.GetOAuthMembershipsByUserName(userName);
        }

        public int Save(UserProfile userProfile, IEnumerable<Expression<Func<UserProfile, object>>> properties,
                        Membership membership)
        {
            var id = _userRepository.Save(userProfile, properties, membership);

            _cache.DeleteItems("userprofile_" + userProfile.UserId + "_");
            _cache.DeleteItems("membership_" + userProfile.UserId + "_");
            _cache.DeleteItems("oauthmemberships_" + userProfile.UserName + "_");

            return id;
        }

        public int Save(UserProfile userProfile, IEnumerable<Expression<Func<UserProfile, object>>> properties)
        {
            var id = _userRepository.Save(userProfile, properties);

            _cache.DeleteItems("userprofile_" + userProfile.UserId + "_");
            _cache.DeleteItems("userprofile_" + userProfile.Email + "_");
            _cache.DeleteItems("userprofile_" + userProfile.UserName + "_");
            _cache.DeleteItems("oauthmemberships_" + userProfile.UserName + "_");

            return id;
        }

        public int Save(Membership membership, IEnumerable<Expression<Func<Membership, object>>> properties,
                        UserProfile userProfile)
        {
            var id = _userRepository.Save(membership, properties, userProfile);

            _cache.DeleteItems("userprofile_" + membership.UserId + "_");
            _cache.DeleteItems("membership_" + membership.UserId + "_");

            return id;
        }

        public int Save(Membership membership, IEnumerable<Expression<Func<Membership, object>>> properties)
        {
            var id = _userRepository.Save(membership, properties);

            _cache.DeleteItems("userprofile_" + membership.UserId + "_");
            _cache.DeleteItems("membership_" + membership.UserId + "_");

            return id;
        }

        public int Save(OAuthMembership oAuthMembership,
                        IEnumerable<Expression<Func<OAuthMembership, object>>> properties,
                        UserProfile userProfile)
        {
            var id = _userRepository.Save(oAuthMembership, properties, userProfile);

            _cache.DeleteItems("userprofile_" + oAuthMembership.UserId + "_");
            _cache.DeleteItems("oauthmembership_" + oAuthMembership.UserId + "_");
            _cache.DeleteItems("oauthmemberships_" + oAuthMembership.UserId + "_");

            return id;
        }

        public int Save(OAuthMembership oAuthMembership,
                        IEnumerable<Expression<Func<OAuthMembership, object>>> properties)
        {
            var id = _userRepository.Save(oAuthMembership, properties);

            _cache.DeleteItems("userprofile_" + oAuthMembership.UserId + "_");
            _cache.DeleteItems("oauthmembership_" + oAuthMembership.UserId + "_");
            _cache.DeleteItems("oauthmemberships_" + oAuthMembership.UserId+"_");

            return id;
        }
    }
}