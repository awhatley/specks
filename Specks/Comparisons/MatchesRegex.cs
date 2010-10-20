using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Specks.Comparisons
{
    /// <summary>
    /// A specification that is satisfied if the candidate string matches a specified regular expression.
    /// </summary>
    internal class MatchesRegex : Specification<string>
    {
        private static readonly MethodInfo _isMatchMethod = typeof(Regex)
            .GetMethod("IsMatch", new[] { typeof(string), typeof(string), typeof(RegexOptions) });

        private string Pattern { get; set; }
        private RegexOptions Options { get; set; }

        public MatchesRegex(string pattern)
            : this(pattern, RegexOptions.None) { }

        public MatchesRegex(string pattern, RegexOptions options)
        {
            Pattern = pattern;
            Options = options;
        }

        protected override Expression<Func<string, bool>> BuildCriteria()
        {
            var parameter = Expression.Parameter(typeof(string), "x");
            var pattern = Expression.Constant(Pattern, typeof(string));
            var options = Expression.Constant(Options, typeof(RegexOptions));
            var isMatch = Expression.Call(_isMatchMethod, parameter, pattern, options);

            return Expression.Lambda<Func<string, bool>>(isMatch, parameter);
        }
    }
}