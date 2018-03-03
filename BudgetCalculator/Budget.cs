using System;

namespace BudgetCalculator
{
    public class Budget
    {
        public int Amount { get; set; }
        public string YearMonth { get; set; }

        private DateTime FirstDay
        {
            get
            {
                return DateTime.ParseExact(YearMonth + "01", "yyyyMMdd", null);
            }
        }

        private DateTime LastDay
        {
            get
            {
                return DateTime.ParseExact(YearMonth + TotalDays, "yyyyMMdd", null);
            }
        }

        private int TotalDays
        {
            get
            {
                var firstDay = FirstDay;
                return DateTime.DaysInMonth(firstDay.Year, firstDay.Month);
            }
        }

        public int EffectiveAmountOfBudget(Period period)
        {
            return DailyAmount() * period.OverlappingDays(new Period(FirstDay, LastDay));
        }

        private int DailyAmount()
        {
            return Amount / TotalDays;
        }
    }
}