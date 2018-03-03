using System;

namespace BudgetCalculator
{
    internal class Period
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

        public int EffectiveDays()
        {
            return (this.End - this.Start).Days + 1;
        }

        public int MonthCount()
        {
            var monthCount = End.MonthDifference(Start);
            return monthCount;
        }
    }
}