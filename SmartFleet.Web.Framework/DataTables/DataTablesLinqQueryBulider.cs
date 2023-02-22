using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using LinqKit;

namespace SmartFleet.Web.Framework.DataTables
{
    public class DataTablesLinqQueryBulider
    {
        private string[] _types = { "System.String", "System.Int32", "System.Boolean", "System.Byte", "System.SByte", "System.Decimal", "System.Double", "System.Single", "System.UInt32", "System.UInt32", "System.UInt64", "System.Int16", "System.UInt16" }; 
        // ReSharper disable once MethodTooLong
        public static DataTablesQueryModel ParseRequest(HttpRequestBase request)
        {
            var form = request.Form;
            // Retrieve request data
            var draw = int.Parse(form["draw"]);
            var start = int.Parse(form["start"]);
            var length = int.Parse(form["length"]);
            var search = new DataTablesSearchModel
            {
                Value = form["search[value]"],
                Regx = Convert.ToBoolean(form["search[regex]"])
            };
            var o = 0;
            var orders = new List<DataTablesOrderModel>();
            // ReSharper disable once ComplexConditionExpression
            while (!string.IsNullOrEmpty(form["order[" + o + "][column]"]))
            {
                orders.Add(new DataTablesOrderModel
                {
                    Column = Convert.ToInt32(form["order[" + o + "][column]"]),
                    Dir = form["order[" + o + "][dir]"]
                });
                o++;
            }
            var c = 0;
            var columns = new List<DataTablesColumnModel>();
            // ReSharper disable once ComplexConditionExpression
            while (!string.IsNullOrEmpty(form["columns[" + c + "][data]"]))
            {
                columns.Add(new DataTablesColumnModel
                {
                    Data = form["columns[" + c + "][data]"],
                    Name = form["columns[" + c + "][name]"],
                    Orderable = Convert.ToBoolean(form["columns[" + c + "][orderable]"]),
                    Searchable = Convert.ToBoolean(form["columns[" + c + "][searchable]"]),
                    Search = new DataTablesSearchModel()
                    {
                        Value = form["columns[" + c + "][search][value]"],
                        Regx = Convert.ToBoolean(form["columns[" + c + "][search][regex]"])
                    }
                });
                c++;
            }
            if (!columns.Any()) throw new Exception("the names of columns should not be null");
            var result = new DataTablesQueryModel()
            {
                Draw = draw,
                Start = start,
                Length = length,
                Search = search,
                Orders = orders,
                Columns = columns
            };
            return result;
        }
        public DataTablesResultModel<T> BuildQuery<T>(HttpRequestBase request, IQueryable<T> queryable)
        {
            
            var dtParams = ParseRequest(request);
            var result = new DataTablesResultModel<T>();
            result.recordsTotal = queryable.Count();
            // search query
            var predicate = PredicateBuilder.New<T>();
            bool searchHasValue = !string.IsNullOrEmpty(dtParams.Search.Value);
            bool clHasValue = false;

            #region the where clause part

            foreach (var columnModel in dtParams.Columns)
            {
                if (!columnModel.Searchable || string.IsNullOrEmpty(columnModel.Search.Value)) continue;
                GetPredicate(columnModel, predicate, columnModel.Search.Value);
               
                clHasValue = true;
            }
            foreach (var c in dtParams.Columns)
            {
                if (!string.IsNullOrEmpty(dtParams.Search.Value))
                    GetPredicate(c, predicate, dtParams.Search.Value);
            }
            if (searchHasValue || clHasValue)
                queryable = queryable.AsExpandable().Where(predicate);

            #endregion

            #region the order clause part
           
            foreach (var p in dtParams.Orders)
            {
                var c = dtParams.Columns[p.Column];
                if (!c.Orderable) continue;
                PropertyInfo pi = typeof(T).GetProperty(c.Data);
                if (pi == null) continue;
                
                switch (pi.PropertyType.FullName)
                {
                    case"System.String" :
                    {
                        queryable = p.Dir == "asc"
                            ? queryable.OrderBy(OrderByExpression<T, string>(c.Data))
                            : queryable.AsQueryable().OrderByDescending(OrderByExpression<T, string>(c.Data));

                        }
                        break;
                    case "System.DateTime":
                        {
                            queryable = p.Dir == "asc"
                                ? queryable.OrderBy(OrderByExpression<T, DateTime>(c.Data))
                                : queryable.AsQueryable().OrderByDescending(OrderByExpression<T, DateTime>(c.Data));
                        }
                        break;
                    case "System.Nullable<DateTime>":
                        {
                            queryable = p.Dir == "asc"
                                ? queryable.OrderBy(OrderByExpression<T, DateTime?>(c.Data))
                                : queryable.AsQueryable()
                                    .OrderByDescending(OrderByExpression<T, DateTime?>(c.Data));
                        }
                        break;
                    case "System.Int32":
                        {
                            queryable = p.Dir == "asc"
                                ? queryable.OrderBy(OrderByExpression<T, Int32>(c.Data))
                                : queryable.AsQueryable().OrderByDescending(OrderByExpression<T, Int32>(c.Data));
                        }
                        break;
                    case "System.Nullable<Int32>":
                        {
                            queryable = p.Dir == "asc"
                                ? queryable.OrderBy(keySelector: OrderByExpression<T, int?>(c.Data))
                                : queryable.AsQueryable()
                                    .OrderByDescending(OrderByExpression<T, int?>(c.Data));
                        }
                        break;
                    case "System.Int64":
                        {
                            queryable = p.Dir == "asc"
                                ? queryable.OrderBy(OrderByExpression<T, Int64>(c.Data))
                                : queryable.AsQueryable().OrderByDescending(OrderByExpression<T, Int64>(c.Data));
                        }
                        break;
                    case "System.Nullable<Int64>":
                        {
                            queryable = p.Dir == "asc"
                                ? queryable.OrderBy(keySelector: OrderByExpression<T, long?>(c.Data))
                                : queryable.AsQueryable()
                                    .OrderByDescending(OrderByExpression<T, long?>(c.Data));
                        }
                        break;

                    case "System.Decimal":
                        {
                            queryable = p.Dir == "asc"
                                ? queryable.OrderBy(OrderByExpression<T, Decimal>(c.Data))
                                : queryable.AsQueryable().OrderByDescending(OrderByExpression<T, Decimal>(c.Data));
                        }
                        break;
                    case "System.Nullable<Decimal>":
                        {
                            queryable = p.Dir == "asc"
                                ? queryable.OrderBy(OrderByExpression<T, decimal?>(c.Data))
                                : queryable.AsQueryable().OrderByDescending(OrderByExpression<T, Decimal?>(c.Data));
                        }
                        break;
                    case "System.Double":
                        {
                            queryable = p.Dir == "asc"
                                ? queryable.OrderBy(OrderByExpression<T, Double>(c.Data))
                                : queryable.AsQueryable().OrderByDescending(OrderByExpression<T, Double>(c.Data));
                        }
                        break;
                    case "System.Nullable<Double>":
                        {
                            queryable = p.Dir == "asc"
                                ? queryable.OrderBy(OrderByExpression<T, double?>(c.Data))
                                : queryable.AsQueryable().OrderByDescending(OrderByExpression<T, Double?>(c.Data));
                        }
                        break;
                    case "System.Nullable<Guid>":
                        {
                            queryable = p.Dir == "asc"
                                ? queryable.OrderBy(OrderByExpression<T, Guid?>(c.Data))
                                : queryable.AsQueryable().OrderByDescending(OrderByExpression<T, Guid?>(c.Data));
                        }
                        break;
                    case "System.Guid":
                        {
                            queryable = p.Dir == "asc"
                                ? queryable.OrderBy(OrderByExpression<T, Guid>(c.Data))
                                : queryable.AsQueryable().OrderByDescending(OrderByExpression<T, Guid>(c.Data));
                        }
                        break;
                }
            }

            #endregion

            try
            {
                result.draw = dtParams.Draw;
                result.length = dtParams.Length;
                result.data = queryable.AsQueryable().Skip(result.start).Take(result.length);
                result.recordsFiltered = queryable.Count();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
            return result;
        }



        public Expression<Func<T, bool>> BuildExpression<T>(string term, string property)
        {
            PropertyInfo pi = typeof(T).GetProperty(property);
            if (pi == null) return null;
            if (_types.Any(t => t == pi.PropertyType.FullName))
                return (Expression<Func<T, bool>>) GetContainsExpression<T>(term, property);
            
            try
            {
                ParameterExpression lhsParam = Expression.Parameter(typeof(T));
                Expression lhs = Expression.Property(lhsParam, pi);
                if (pi.PropertyType != typeof(DateTime))
                    return (Expression<Func<T, bool>>) GetEnumExpression<T>(term, pi, lhs, lhsParam);

                var value = Convert.ChangeType(term, pi.PropertyType);
                var rf = Expression.Constant(value);
                var bn2 = Expression.Equal(lhs, rf);
                return Expression.Lambda<Func<T, bool>>(bn2, lhsParam);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        // ReSharper disable once TooManyArguments
        private static Expression GetEnumExpression<T>(string term, PropertyInfo pi, Expression lhs,
            ParameterExpression lhsParam)
        {
            var castTo = pi.PropertyType;
            var parsedEnum = Convert.ChangeType(Enum.Parse(castTo, term), castTo);
            var rf1 = Expression.Constant(parsedEnum);
            var bn = Expression.Equal(lhs, rf1);
            return Expression.Lambda<Func<T, bool>>(bn, lhsParam);
        }

        private static Expression GetContainsExpression<T>(string term, string property)
        {
            var param = Expression.Parameter(typeof(T));
            var body = Expression.Call(
                Expression.Call(
                    Expression.Property(param, property),
                    "ToString",
                    null
                ),
                "Contains",
                null,
                Expression.Constant(term)
            );
            var lambda = Expression.Lambda<Func<T, bool>>(body, param);
            return lambda;
        }

        public Expression<Func<T, bool>> OperationsExpression<T>(string term, string property, ExpressionType operation)
        {
            throw new NotImplementedException();
        }

        public Expression<Func<TSource, TTargetKey>> OrderByExpression<TSource, TTargetKey>(string property)
        {
            PropertyInfo pi = typeof(TSource).GetProperty(property);
            ParameterExpression lhsParam = Expression.Parameter(typeof(TSource), "o");
            Expression orderBy = Expression.Property(lhsParam, pi ?? throw new InvalidOperationException());
            var orderByLambda = Expression.Lambda<Func<TSource, TTargetKey>>(orderBy, lhsParam);
            return orderByLambda;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p">The p.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="searchValue"></param>
        private void GetPredicate<T>(DataTablesColumnModel p, ExpressionStarter<T> predicate, string searchValue)
        {
           
            // PropertyInfo pi = typeof(T).GetProperty(p.Data);
            var exp = BuildExpression<T>(searchValue, p.Data);
            if (exp != null)
                predicate.Or(exp);
        }

    }
}