// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace RedScooby.Helpers
{
    public class ExpressionHelpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
        {
            if (memberExpression == null) throw new ArgumentNullException("memberExpression");

            var expressionBody = (MemberExpression) memberExpression.Body;
            return expressionBody.Member.Name;
        }
    }
}
