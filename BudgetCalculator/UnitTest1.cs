using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BudgetCalculator
{
    [TestClass]
    public class UnitTest1
    {
        private IRepository<Budget> _repository = Substitute.For<IRepository<Budget>>();
        private Accounting _accounting;


        [TestMethod]
        public void no_budgets()
        {
            givenBudgets();
            totalAmountShouldBe(0,new DateTime(2018, 3, 1),new DateTime(2018, 3, 1));
        }

        [TestInitialize]
        public void Testinit()
        {
            _accounting = new Accounting(_repository);
        }

        [TestMethod]
        public void invalid_period()
        {
            givenBudgets();

            Action actual = () => _accounting.TotalAmount(new DateTime(2018, 3, 1), new DateTime(2018, 2, 1));

            actual.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void fifteen_effective_days_period_between_budget_month()
        {

            givenBudgets(new Budget() { YearMonth = "201801", Amount = 62 });
            totalAmountShouldBe(30,new DateTime(2018, 1, 1),new DateTime(2018, 1, 15));
        }

        [TestMethod]
        public void period_equals_to_budget_month()
        {
            givenBudgets(new Budget() { YearMonth = "201801", Amount = 62 });

            totalAmountShouldBe(62,new DateTime(2018, 1, 1),new DateTime(2018, 1, 31));
        }

        [TestMethod]
        public void no_effective_days_period_after_budget_month()
        {
            givenBudgets(new Budget() { YearMonth = "201801", Amount = 62 });
            var start = new DateTime(2018, 2, 1);

            var end = new DateTime(2018, 2, 15);

            totalAmountShouldBe(0,start,end);
        }

        [TestMethod]
        public void not_continus_mutiple_budgets()
        {
            givenBudgets(new Budget() { YearMonth = "201801", Amount = 62 }, new Budget() { YearMonth = "201803", Amount = 62 });

            totalAmountShouldBe(82,new DateTime(2018, 1, 1),new DateTime(2018, 3, 10));
        }

        [TestMethod]
        public void period_equals_to_two_budget_month()
        {
            givenBudgets(new Budget() { YearMonth = "201801", Amount = 62 },
                new Budget() { YearMonth = "201802", Amount = 280 });

            totalAmountShouldBe(342,new DateTime(2018, 1, 1),new DateTime(2018, 2, 28));
        }

        [TestMethod]
        public void period_cross_three_budget_month()
        {
            givenBudgets(new Budget() { YearMonth = "201801", Amount = 62 },
                new Budget() { YearMonth = "201802", Amount = 280 },
                new Budget() { YearMonth = "201803", Amount = 62 });
            totalAmountShouldBe(362,new DateTime(2018, 1, 1),new DateTime(2018, 3, 10));
        }

        [TestMethod]
        public void period_cross_year_when_multiple_budgets()
        {
            givenBudgets(
                new Budget() { YearMonth = "201712", Amount = 310 },
                new Budget() { YearMonth = "201801", Amount = 310 },
                new Budget() { YearMonth = "201802", Amount = 280 },
                new Budget() { YearMonth = "201803", Amount = 310 }
            );

            totalAmountShouldBe(1000,new DateTime(2017, 12, 1),new DateTime(2018, 3, 10));
        }

        private void givenBudgets(params Budget[] budgets)
        {
            _repository.GetAll().Returns(budgets.ToList());
        }

        private void totalAmountShouldBe(int expected,DateTime startDate,DateTime endDate)
        {
            _accounting.TotalAmount(startDate, endDate).Should().Be(expected);
        }
    }
}