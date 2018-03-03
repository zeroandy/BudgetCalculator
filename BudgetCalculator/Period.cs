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

        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        public bool IsSameMonth()
        {
            return this.Start.Year == this.End.Year && this.Start.Month == this.End.Month;
        }

        public int TotalDays()
        {
            return (this.End - this.Start).Days + 1;
        }

        public int OveralppingDays(Period period)
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
            var effectiveEndDate = End > period.End
                ? period.End
                : End;
            return effectiveEndDate;
        }

        private DateTime EffectiveStartDate(Period period)
        {
            var effectiveStartDate = Start < period.Start
                ? period.Start
                : Start;
            return effectiveStartDate;
        }
    }
}