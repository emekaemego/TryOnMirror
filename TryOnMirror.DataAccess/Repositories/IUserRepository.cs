using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataAccess.Repositories
{
    public interface IUserRepository
    {
        UserProfile GetUserProfile(int userId);
        UserProfile GetUserProfile(string email);
        UserProfile GetUserProfileByUserName(string username);
        OAuthMembership GetOAuthMembership(string provider, string providerUserId);
        Membership GetMembership(int userId, bool withUserProfile);
        Membership GetMembershipByVerificationToken(string token, bool withUserProfile);
        IEnumerable<OAuthMembership> GetOAuthMembershipsByUserName(string userName);

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