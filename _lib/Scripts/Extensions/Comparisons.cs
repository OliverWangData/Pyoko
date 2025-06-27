using System.Collections.Generic;
using System.Linq;

namespace SQLib.Extensions
{
    public static class Comparisons
    {
        // [Strings]
        // ****************************************************************************************************
        public static bool Contains(this string[] array, string check)
        {
            return array.Any(x => check == x);
        }

        public static bool IsIn(this string check, string[] array) => array.Contains(check);
    }
}
