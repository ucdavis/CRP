using System.Text;

namespace CRP.Tests.Core.Extensions
{
    static class StringExtension
    {
        /// <summary>
        /// Repeats string x times.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="times">The times.</param>
        /// <returns></returns>
        public static string RepeatTimes(this string source, int times)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < times; i++)
            {
                sb.Append(source);
            }
            return sb.ToString();
        }
    }
}
