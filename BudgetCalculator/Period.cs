using System;

namespace BudgetCalculator
{
    public class Period
    {
        public Period(DateTime start, DateTime end)
        {
            if (start > end)
            {
                throw new ArgumentException();
            }
            Start = start;
            End = end;
        }

        public DateTime End { get; private set; }
        public DateTime Start { get; private set; }

        public int OverlappingDays(Period period)
        {
            if (End < period.Start)
            {
                return 0;
            }

            if (Start > period.End)
            {
                return 0;
            }

            var effectiveStartDate = EffectiveStartDate(period);
            var effectiveEndDate = EffectiveEndDate(period);

            return new Period(effectiveStartDate, effectiveEndDate).TotalDays();
        }

        private DateTime EffectiveEndDate(Period period)
        {
            return End > period.End ? period.End : End;
        }

        private DateTime EffectiveStartDate(Period period)
        {
            return Start < period.Start ? period.Start : Start;
        }

        private int TotalDays()
        {
            return (this.End - this.Start).Days + 1;
        }
    }
}