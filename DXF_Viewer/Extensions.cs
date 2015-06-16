using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DXF.Extensions
{
   public static class Extensions
   {
      public static Double ConvertToDoubleWithCulture(this string val)
      {
         string decSep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
         if (decSep != ".")
         {
            val = val.Replace(".", decSep);
         }
         return Convert.ToDouble(val);
      }
   }
}
