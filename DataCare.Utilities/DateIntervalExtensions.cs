using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;

namespace DataCare.Utilities
{
    public static class DateIntervalExtensions
    {

        public static bool Overlaps(this DateInterval dateinterval, DateInterval range)
        {
            Contract.Requires(range != null);

            return dateinterval.Contains(range.Start) ||
                dateinterval.Contains(range.End) ||
                range.Contains(dateinterval.Start) ||
                range.Contains(dateinterval.End);
        }

        public static DateInterval CreatePeriodeFrom(LocalDate start, LocalDate end)
        {
            if (start > end)
            {
                return new DateInterval(end, start);
            }
            return new DateInterval(start, end);
        }

        /// <summary>
        /// Wordt de gegeven periode geheel afgedekt door de periodes
        /// </summary>
        /// <param name="periods"></param>
        /// <param name="periodToCheck"></param>
        /// <returns></returns>
        public static bool FullRange(DateInterval periodToCheck, List<DateInterval> periods)
        {
            return GetFirstFreeRange(periodToCheck, periods) == null;
        }

        /// <summary>
        /// Als de gegeven periode geheel wordt afgedekt door de periodes geef dan null terug,
        /// zoniet geef dan de eerste vrije periode terug.
        /// </summary>
        /// <param name="periodToCheck"></param>
        /// <param name="periods"></param>
        /// <returns></returns>
        public static DateInterval GetFirstFreeRange(DateInterval periodToCheck, List<DateInterval> periods)
        {
            if (!periods.Any())
            {
                return periodToCheck;
            }

            var periodsThatApply = periods.Where(p => periodToCheck.Overlaps(p)).ToList();
            if (!periodsThatApply.Any())
            {
                return periodToCheck;
            }

            periodsThatApply.Sort(new OnStartDate());
            if (periodsThatApply.First().Start > periodToCheck.Start)
            {
                return new DateInterval(periodToCheck.Start, periodsThatApply.First().Start.PlusDays(-1));
            }

            var currentPeriod = periodsThatApply.First();
            foreach (var period in periodsThatApply.Skip(1))
            {
                if (Connected(currentPeriod, period))
                {
                    currentPeriod = period;
                }
                else
                {
                    if (!currentPeriod.Contains(period))
                    {
                        return new DateInterval(currentPeriod.End.PlusDays(1), period.Start.PlusDays(-1));
                    }
                }
            }

            if (currentPeriod.End < periodToCheck.End)
            {
                return new DateInterval(currentPeriod.End.PlusDays(1), periodToCheck.End);
            }

            // Gehele periode afgedekt.
            return null;
        }

        public static bool Contains(this DateInterval currentPeriod, DateInterval period)
        {
            return currentPeriod.Start <= period.Start && currentPeriod.End >= period.End;
        }

        private static bool Connected(DateInterval currentPeriod, DateInterval period)
        {
            if (currentPeriod.End <= period.End)
            {
                if (period.Start > currentPeriod.End.PlusDays(1))
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private class OnStartDate : IComparer<DateInterval>
        {
            // Sorteer op volgorde van LowerBound.
            public int Compare(DateInterval x, DateInterval y)
            {
                if (y.Start > x.Start)
                {
                    return -1;
                }

                if (y.Start < x.Start)
                {
                    return 1;
                }

                return 0;
            }
        }
    }
}
