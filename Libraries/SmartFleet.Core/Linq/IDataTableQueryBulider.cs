using System;
using System.Linq.Expressions;

namespace SmartFleet.Core.Linq
{
    interface  IDataTableQueryBulider
    {
       /// <summary>
        /// Determines whether the specified term contains expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="term">The term.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        Expression<Func<T, Boolean>> ContainsExpression<T>(string term, string property);
        /// <summary>
        /// Operationses the expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="term">The term.</param>
        /// <param name="property">The property.</param>
        /// <param name="operation">The operation.</param>
        /// <returns></returns>
        Expression<Func<T, Boolean>> OperationsExpression<T>(string term, string property, ExpressionType operation);
        /// <summary>
        /// Orders the by expression.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TTargetKey">The type of the target key.</typeparam>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        Expression<Func<TSource, TTargetKey>> OrderByExpression<TSource, TTargetKey>(string property);
    }
}
