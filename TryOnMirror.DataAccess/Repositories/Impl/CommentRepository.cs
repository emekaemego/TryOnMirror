using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Core;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataAccess.Repositories.Impl
{
    public class CommentRepository : ICommentRepository
    {
        private ICache _cache;

        public CommentRepository(ICache cache)
        {
            _cache = cache;
        }

        public IEnumerable<Comment> GetComments(int? typeId, int? page, int maxRows)
        {
            string key = "Comments_" + typeId + "_" + page + "_" + maxRows + "_GetComments";

            var result = new List<Comment>();

            if (_cache.Exists(key))
                result = (List<Comment>) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    var oq = from x in dc.Comments select x;

                    if (typeId.HasValue)
                    {
                        oq = oq.Where(x => x.CommentTypeId == typeId.Value);
                    }

                    result = oq.OrderByDescending(x => x.DateCreated).Page(page, maxRows).ToList();

                    _cache.Set(key, result);
                }
            }
            return result;
        }

        public IEnumerable<Comment> GetBookingComments(int bookingId, int? page, int maxRows)
        {
            string key = "Comments_" + bookingId + "_" + page + "_" + maxRows + "_GetComments";

            var result = new List<Comment>();

            if (_cache.Exists(key))
                result = (List<Comment>) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = (from h in dc.HairstyleBookings
                              from c in h.Comments
                              where c.CommentType.TypeName.Equals("Booking")
                              select c).OrderByDescending(x => x.DateCreated)
                        .Page(page, maxRows).ToList();

                    _cache.Set(key, result);
                }
            }
            return result;
        }

        public Comment GetComment(int id)
        {
            string key = "Comment_" + id + "_GetComment";
            Comment result = null;

            if (_cache.Exists(key))
                result = (Comment) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.Comments.FirstOrDefault(x => x.CommentId == id);

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public int Save(Comment comment, IEnumerable<Expression<Func<Comment, object>>> properties)
        {
            using (var dc = new TryOnMirrorEntities())
            {
                if (properties != null)
                {
                    dc.Comments.Attach(comment);
                    ObjectStateEntry entry = ((IObjectContextAdapter)dc).ObjectContext.ObjectStateManager
                        .GetObjectStateEntry(comment);

                    foreach (var selector in properties)
                    {
                        string propertyName = selector.Body.PropertyToString();
                        entry.SetModifiedProperty(propertyName);
                    }
                }
                else
                {
                    var bookings = comment.HairstyleBookings.ToList();
                    comment.HairstyleBookings.Clear();

                    foreach (var b in bookings)
                    {
                        dc.HairstyleBookings.Attach(b);
                        comment.HairstyleBookings.Add(b);
                        ((IObjectContextAdapter) dc).ObjectContext.ObjectStateManager
                            .ChangeObjectState(b, EntityState.Unchanged);
                    }

                    dc.Comments.Add(comment);
                }

                dc.SaveChanges();
            }

            return comment.CommentId;
        }

        public void Delete(int id)
        {
            using (var dc = new TryOnMirrorEntities())
            {
                var comment = new Comment {CommentId = id};
                dc.Comments.Attach(comment);

                ((IObjectContextAdapter) dc).ObjectContext.ObjectStateManager.ChangeObjectState(comment,
                                                                                                EntityState.Deleted);

                dc.SaveChanges();
            }
        }
    }
}