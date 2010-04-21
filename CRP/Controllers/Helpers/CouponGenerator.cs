using System;
using System.Text;

namespace CRP.Controllers.Helpers
{
    public class CouponGenerator
    {
        public static string GenerateCouponCode()
        {
            // default length is 10
            return RandomString(10, false).ToUpper();
        }

        private static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max); 
        }

        /// <summary>
        /// Generates a random string with the given length
        /// </summary>
        /// <param name="size">Size of the string</param>
        /// <param name="lowerCase">If true, generate lowercase string</param>
        /// <returns>Random string</returns>
        private static string RandomString(int size, bool lowerCase)
        { 
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch ;
            for(int i=0; i<size; i++)
            {
            ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65))) ;
            builder.Append(ch); 
            }
            if(lowerCase)
            return builder.ToString().ToLower();
            return builder.ToString();
        }

    }
}
