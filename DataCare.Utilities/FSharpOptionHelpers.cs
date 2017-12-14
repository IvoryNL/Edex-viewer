namespace DataCare.Utilities
{
    using System;
    using System.Linq;
    using FSharpx;
    using Microsoft.FSharp.Core;

    public static class FSharpOptionHelpers
    {
        public static bool HasSomeValue<T>(this FSharpOption<T> value)
        {
            return value.Match(v => !v.Equals(default(T)), false);
        }
    }
}