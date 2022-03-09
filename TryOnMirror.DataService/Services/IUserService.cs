using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataService.Services
{
    public interface IUserService
    {
        UserProfile GetUserProfile(int userId);
        UserProfile GetUserProfile(string email);
        UserProfile GetUserProfileByUserName(string username);
        OAuthMembership GetOAuthMembership(string provider, string providerUserId);
        IEnumerable<OAuthMembership> GetOAuthMembershipsByUserName(string userName);
        Membership GetMembership(int userId, bool withUserProfile=false);
        Membership GetMembershipByVerificationToken(string token, bool withUserProfile=false);

        int Save(UserProfile userProfile, IEnumerable<Expression<Func<UserProfile, object>>> properties,
                                 Membership membership);

        int Save(UserProfile userProfile, IEnumerable<Expression<Func<UserProfile, object>>> properties);

        int Save(Membership membership, IEnumerable<Expression<Func<Membership, object>>> properties,
                                 UserProfile userProfile);

        int Save(Membership membership, IEnumerable<Expression<Func<Membership, object>>> properties);

        int Save(OAuthMembership oAuthMembership, IEnumerable<Expression<Func<OAuthMembership, object>>> properties,
                                 UserProfile userProfile);

        int Save(OAuthMembership oAuthMembership, IEnumerable<Expression<Func<OAuthMembership, object>>> properties);
    }
}