
namespace CRP.Core.Helpers
{
    static class BooleanExtension
    {
        /// <summary>
        /// Coverts a boolean to an int of 0 or 1.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns></returns>
        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }
    }
}
