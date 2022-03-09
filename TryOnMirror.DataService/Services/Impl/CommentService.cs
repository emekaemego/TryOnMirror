using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.DataAccess.Repositories;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataService.Services.Impl
{
   public class CommentService : ICommentService
   {
       private ICommentRepository _repository;
       private ICache _cache;

       public CommentService(ICommentRepository repository, ICache cache)
       {
           _repository = repository;
           _cache = cache;
       }

       public IEnumerable<Comment> GetComments(int? typeId, int? page, int maxRows)
       {
           return _repository.GetComments(typeId, page, maxRows);
       }

       public IEnumerable<Comment> GetBookingComments(int bookingId, int? page, int maxRows)
       {
           return _repository.GetBookingComments(bookingId, page, maxRows);
       }

       public Comment GetComment(int id)
       {
           return _repository.GetComment(id);
       }
       
       public int Save(Comment comment, IEnumerable<Expression<Func<Comment, object>>> properties)
       {
           var result = _repository.Save(comment, properties);

           _cache.DeleteItems("comment_" + comment.CommentId + "_");

           return result;
       }

      public void Delete(int id)
      {
          _repository.Delete(id);
          _cache.DeleteItems("comment_" + id + "_");
      }
   }
}
