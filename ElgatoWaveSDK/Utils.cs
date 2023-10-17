using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace ElgatoWaveSDK
{
    internal static class Utils
    {
        public static dynamic ToDynamic<TOne, TTwo>(Dictionary<TOne,TTwo> dict) where TOne : notnull
        {
            dynamic? eo = dict.Aggregate(new ExpandoObject() as IDictionary<TOne, TTwo>, (a, p) => { a?.Add(p); return a; });
            return eo ?? new { };
        }
    }
}
