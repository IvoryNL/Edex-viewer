using System;
using Microsoft.FSharp.Core;
using FSharpx;
using NodaTime;
using System.Globalization;

namespace DataCare.Utilities
{
    public static class LocalDateExtensions
    {
        public static DateTime ToDateTime(this LocalDate date)
        {
            return date.AtMidnight().ToDateTimeUnspecified();
        }

        public static FSharpOption<DateTime> ToDateTime(this FSharpOption<LocalDate> date)
        {
            if (date.HasValue())
            {
                return date.Value.AtMidnight().ToDateTimeUnspecified().ToFSharpOption();
            }
            return FSharpOption<DateTime>.None;
        }

        public static string ToShortDateString(this LocalDate date)
        {
            return date.ToString("d", CultureInfo.CurrentCulture.DateTimeFormat);
        }

        public static LocalDate ToDay()
        {
            return DateTimeExtensions.Now.ToLocalDate();
        }
        
    }
}
