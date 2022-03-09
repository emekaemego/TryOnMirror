using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SymaCord.TryOnMirror.Core.Util.Impl;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.Core
{
    public static class Extensions
    {
        public static void GetHashAndSalt(this string data, out string hash, out string salt)
        {
            Cryptography.GetHashAndSalt(data, out hash, out salt);
        }

        public static bool VerifyHash(this string data, string hash, string salt)
        {
            return Cryptography.VerifyHash(data, hash, salt);
        }

        public static string ConvertToString(this int[] ids, string seperator)
        {
            if (ids != null && ids.Count() > 0)
            {
                string result = "";
                if (string.IsNullOrEmpty(seperator)) seperator = ",";

                for (int i = 0; i < ids.Count(); i++)
                {
                    result += ids[i] + seperator;
                }
                //remove the last seperator from the list
                result = result.Remove(result.Length - 1);
                return result;
            }

            return "";
        }

        public static string ConvertToString(this string[] text, string seperator, bool encloseWithQuote)
        {
            if (text != null && text.Count() > 0)
            {
                string result = "";
                string quote = "";

                if (string.IsNullOrEmpty(seperator)) seperator = ", "; else seperator = seperator + " ";
                if (encloseWithQuote) quote = "'";

                if (text.Count() == 1 && !string.IsNullOrEmpty(text[0]))
                    return quote + text[0] + quote;

                for (int i = 0; i < text.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(text[i]))
                        result += quote + text[i] + quote + seperator;
                }
                //remove the last seperator from the list
                if (encloseWithQuote)
                    result = result.Remove(result.Length - 3);
                else
                    result = result.Remove(result.Length - 2);

                return result;
            }

            return "";
        }

        public static string PropertyToString(this Expression selector)
        {
            if (selector is UnaryExpression)
            {
                UnaryExpression unex = (UnaryExpression)selector;
                if (unex.NodeType == ExpressionType.Convert)
                {
                    Expression ex = unex.Operand;
                    MemberExpression mex = (MemberExpression)ex;
                    return mex.Member.Name;
                }
            }

            if (selector.NodeType == ExpressionType.MemberAccess)
            {
                return ((selector as MemberExpression).Member as PropertyInfo).Name;
            }

            throw new InvalidOperationException();
        }

        public static string AppendClause(this string query, bool whereNearOpenBracket=false)
        {
            if (!string.IsNullOrEmpty(query))
            {
                if (whereNearOpenBracket)
                {
                    if (query.ToLower().Contains(" where (") || query.ToLower().Contains(" where("))
                        return query += " AND ";
                }
                else if (query.ToLower().Contains(" where "))
                {
                    return query += " AND ";
                }

                return query += " WHERE ";
            }

            return query += "";
        }

        public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int? page, int pageSize)
        {
            if (page.HasValue)
                return source.Skip((page.Value - 1) * pageSize).Take(pageSize);

            return source.Take(pageSize);
        }

        public static int SmsMessageCount(this string text)
        {
            int length = text.Length;

            var result = (length + 160 - 1) / 160;

            return result;
        }

        /// <summary>
        /// Converts a single DbDataRwcord object into something else.
        /// The destination type must have a default constructor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="record"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(this DbDataRecord record)
        {
            T item = Activator.CreateInstance<T>();
            for (int f = 0; f < record.FieldCount; f++)
            {
                PropertyInfo p = item.GetType().GetProperty(record.GetName(f));

                var fieldType = record.GetFieldType(f);

                if (p != null && (p.PropertyType == fieldType || (p.PropertyType.IsValueType && 
                    !(Nullable.GetUnderlyingType(p.PropertyType) == null))))
                {
                    if (record.GetValue(f) != DBNull.Value)
                    {
                        p.SetValue(item, record.GetValue(f), null);
                    }
                    else
                    {
                        p.SetValue(item, null, null);
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Converts a list of DbDataRecord to a list of something else.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> ConvertTo<T>(this List<DbDataRecord> list)
        {
            List<T> result = (List<T>)Activator.CreateInstance<List<T>>();

            list.ForEach(rec =>
            {
                result.Add(rec.ConvertTo<T>());
            });

            return result;
        }

        public static Point[] GetRectPoints(this Rectangle rect)
        {
            var points = new Point[] {};

            if (rect != Rectangle.Empty)
            {
                points[0] = new Point(rect.X, rect.Bottom/2); //right points
                points[1] = new Point(rect.Right/2, rect.Y); // top points
                points[2] = new Point(rect.Right, rect.Height/2); //right points
                points[3] = new Point(rect.Right/2, rect.Bottom); //bottom points
            }

            return points;
        }

        public static Point[] ToPointsArray(this int[] points)
        {
            List<Point> plist = new List<Point>();

            for (int i = 0; i < points.Length; i += 2)
            {
                plist.Add(new Point(points[i], points[i + 1]));
            }

            return plist.ToArray();
        }

        public static Coord[] ToCoordsArray(this int[] points)
        {
            var plist = new List<Coord>();

            for (int i = 0; i < points.Length; i += 2)
            {
                plist.Add(new Coord(points[i], points[i + 1]));
            }

            return plist.ToArray();
        }

        public static CvSearchCoordsResult ToCoordResult(this CvSearchResult searchResult)
        {
            var coordResult = new CvSearchCoordsResult
                {
                    Id = searchResult.Id,
                    Coords = new Coord[searchResult.Points.Length]
                };

            for(var i =0; i < searchResult.Points.Length; i++)
            {
                coordResult.Coords[i] = new Coord(searchResult.Points[i].X, searchResult.Points[i].Y);
            }

            return coordResult;
        }

        public static List<CvSearchCoordsResult> ToCoordResults(this List<CvSearchResult> searchResults)
        {
            var coordResults = new List<CvSearchCoordsResult>();

            foreach(var r in searchResults)
            {
                if(r.Points != null)
                {
                    var coordResult = new CvSearchCoordsResult
                    {
                        Id = r.Id,
                        Coords = new Coord[r.Points.Length]
                    };

                    for (var i = 0; i < r.Points.Length; i++)
                    {
                        coordResult.Coords[i] = new Coord(r.Points[i].X, r.Points[i].Y);
                    }

                    coordResults.Add(coordResult);
                }
            }

            return coordResults;
        }
    }
}
