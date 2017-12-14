namespace DataCare.Utilities
{
    using System;
    using System.Linq;
    using Microsoft.FSharp.Core;
    using NodaTime;
    using FSharpx;
    using System.Globalization;

    public static class DateTimeExtensions
    {

        public static LocalDate ToLocalDate(this DateTime date)
        {
            return LocalDateTime.FromDateTime(date).Date;
        }

        public static FSharpOption<LocalDate> ToLocalDate(this FSharpOption<DateTime> date)
        {
            if (date.HasSomeValue())
            {
                return date.Value.ToLocalDate().ToFSharpOption();
            }
            return FSharpOption<LocalDate>.None;
        }

        private static IDateTimeNowProvider dateTimeNowProvider = new DateTimeNowProvider();

        public static DateTime Now
        {
            get { return dateTimeNowProvider.Now(); }
        }

        public static void SetNowProvider(IDateTimeNowProvider newProvider)
        {
            dateTimeNowProvider = newProvider;
        }
    }

    public interface IDateTimeNowProvider
    {
        DateTime Now();
    }

    public class DateTimeNowProvider : IDateTimeNowProvider
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }

    public class FakeDateTimeNowProvider : IDateTimeNowProvider
    {
        private DateTime? currentTime = null;

        public void AdvanceClock(TimeSpan span)
        {
            currentTime = CurrentTime + span;
        }

        public DateTime CurrentTime
        {
            get
            {
                return currentTime ?? DateTime.Now;
            }

            set
            {
                if (currentTime != value)
                {
                    currentTime = value;
                }
            }
        }

        public DateTime Now()
        {
            return CurrentTime;
        }
    }
}