using System;
using System.Collections.Generic;
using System.Linq;

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
    }

    internal class Accounting
    {
        private readonly IRepository<Budget> _repo;

        public Accounting(IRepository<Budget> repo)
        {
            _repo = repo;
        }

        public decimal TotalAmount(DateTime start, DateTime end)
        {
            var period = new Period(start, end);

            return period.IsSameMonth()
                ? GetOneMonthAmount(period)
                : GetRangeMonthAmount(start, end);
        }

        private int GetOneMonthAmount(Period period)
        {
            var budget = _repo.GetAll().Get(period.Start);
            if (budget == null)
            {
                return 0;
            }

            return budget.DailyAmount() * period.EffectiveDays();
        }

        private decimal GetRangeMonthAmount(DateTime start, DateTime end)
        {
            var monthCount = end.MonthDifference(start);
            var total = 0;
            for (var index = 0; index <= monthCount; index++)
            {
                if (index == 0)
                {
                    total += GetOneMonthAmount(new Period(start, start.LastDate()));
                }
                else if (index == monthCount)
                {
                    total += GetOneMonthAmount(new Period(end.FirstDate(), end));
                }
                else
                {
                    var now = start.AddMonths(index);
                    total += GetOneMonthAmount(new Period(now.FirstDate(), now.LastDate()));
                }
            }
            return total;
        }
    }

    public static class BudgetExtension
    {
        public static Budget Get(this List<Budget> list, DateTime date)
        {
            return list.FirstOrDefault(r => r.YearMonth == date.ToString("yyyyMM"));
        }
    }

    public static class DateTimeExtension
    {
        public static int MonthDifference(this DateTime lValue, DateTime rValue)
        {
            return (lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year);
        }

        public static DateTime LastDate(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        }

        public static DateTime FirstDate(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }
    }
}