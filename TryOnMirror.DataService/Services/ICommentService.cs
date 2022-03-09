using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataService.Services
{
    public interface ICommentService
    {
        IEnumerable<Comment> GetComments(int? typeId, int? page, int maxRows);
        IEnumerable<Comment> GetBookingComments(int bookingId, int? page, int maxRows);
        Comment GetComment(int id);
        int Save(Comment comment, IEnumerable<Expression<Func<Comment, object>>> properties);
        void Delete(int id);
    }
}