using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korbit.Models
{
    public static class SqlHelpers
    {
        public static string ValidText(string text)
        {
            return "'" + text.Replace("'", "''") + "'";
        }
        public static string FromBoolean(bool value)
        {
            return value ? "1" : "0";
        }
    }
}
