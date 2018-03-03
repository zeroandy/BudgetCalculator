using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetCalculator
{
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

            var budgets = _repo.GetAll();

            return period.IsSameMonth()
                ? GetOneMonthAmount(period, budgets)
                : GetRangeMonthAmount(period, budgets);
        }

        private int GetOneMonthAmount(Period period, List<Budget> budgets)
        {
            var budget = budgets.Get(period.Start);
            if (budget == null)
            {
                return 0;
            }

            return budget.DailyAmount() * period.EffectiveDays();
        }

        private decimal GetRangeMonthAmount(Period period, List<Budget> budgets)
        {
            var monthCount = period.MonthCount();
            var total = 0;
            for (var index = 0; index <= monthCount; index++)
            {
                var effectivePeriod = EffectivePeriod(period, index, monthCount);
                total += GetOneMonthAmount(effectivePeriod, budgets);
            }
            return total;
        }

        private static Period EffectivePeriod(Period period, int index, int monthCount)
        {
            Period effectivePeriod = null;
            if (index == 0)
            {
                effectivePeriod = new Period(period.Start, period.Start.LastDate());
            }
            else if (index == monthCount)
            {
                effectivePeriod = new Period(period.End.FirstDate(), period.End);
            }
            else
            {
                var now = period.Start.AddMonths(index);
                effectivePeriod = new Period(now.FirstDate(), now.LastDate());
            }
            return effectivePeriod;
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