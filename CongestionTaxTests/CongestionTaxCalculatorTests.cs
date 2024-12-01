namespace CongestionTaxTests
{
    using NUnit.Framework;
    using Moq;
    using System;
    using System.Collections.Generic;
    using CongestionTax;

    [TestFixture]
    public class CongestionTaxCalculatorTests
    {
        private Mock<ICalculationRules> _mockRules;
        private CongestionTaxCalculator _calculator;

        [SetUp]
        public void Setup()
        {
            // Mock CalculationRules
            _mockRules = new Mock<ICalculationRules>();
            _mockRules.Setup(r => r.MaxDailyFee).Returns(60);
            _mockRules.Setup(r => r.IntervalMinutes).Returns(60);
            _mockRules.Setup(r => r.ExcludedVehicleTypes).Returns(new List<string> { "Motorcycle", "Bus", "Emergency", "Diplomat", "Foreign", "Military" });
            _mockRules.Setup(r => r.ExcludedDaysOfWeek).Returns(new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday });
            _mockRules.Setup(r => r.TaxPeriods).Returns(new List<TaxPeriod>
            {
                new TaxPeriod { StartTime = TimeSpan.Parse("06:00"), EndTime = TimeSpan.Parse("06:30"), TaxAmount = 8 },
                new TaxPeriod { StartTime = TimeSpan.Parse("06:30"), EndTime = TimeSpan.Parse("07:00"), TaxAmount = 13 },
                new TaxPeriod { StartTime = TimeSpan.Parse("07:00"), EndTime = TimeSpan.Parse("08:00"), TaxAmount = 18 },
                new TaxPeriod { StartTime = TimeSpan.Parse("08:00"), EndTime = TimeSpan.Parse("08:30"), TaxAmount = 13 },
                new TaxPeriod { StartTime = TimeSpan.Parse("08:30"), EndTime = TimeSpan.Parse("15:00"), TaxAmount = 8 },
                new TaxPeriod { StartTime = TimeSpan.Parse("15:00"), EndTime = TimeSpan.Parse("15:30"), TaxAmount = 13 },
                new TaxPeriod { StartTime = TimeSpan.Parse("15:30"), EndTime = TimeSpan.Parse("17:00"), TaxAmount = 18 },
                new TaxPeriod { StartTime = TimeSpan.Parse("17:00"), EndTime = TimeSpan.Parse("18:00"), TaxAmount = 13 },
                new TaxPeriod { StartTime = TimeSpan.Parse("18:00"), EndTime = TimeSpan.Parse("18:30"), TaxAmount = 8 }
            });
            _mockRules.Setup(r => r.ExcludedDateTimePeriods).Returns(new List<DateTimePeriod>
            {
                new DateTimePeriod
                {
                    StartDateTime = DateTime.Parse("2013-01-01T00:00:00"),
                    EndDateTime = DateTime.Parse("2013-01-02T00:00:00")
                },
                new DateTimePeriod
                {
                    StartDateTime = DateTime.Parse("2013-03-28T00:00:00"),
                    EndDateTime = DateTime.Parse("2013-03-30T00:00:00")
                },
                new DateTimePeriod
                {
                    StartDateTime = DateTime.Parse("2013-04-01T00:00:00"),
                    EndDateTime = DateTime.Parse("2013-04-02T00:00:00")
                },
                new DateTimePeriod
                {
                    StartDateTime = DateTime.Parse("2013-04-30T00:00:00"),
                    EndDateTime = DateTime.Parse("2013-05-02T00:00:00")
                },
                new DateTimePeriod
                {
                    StartDateTime = DateTime.Parse("2013-05-08T00:00:00"),
                    EndDateTime = DateTime.Parse("2013-05-10T00:00:00")
                },
                new DateTimePeriod
                {
                    StartDateTime = DateTime.Parse("2013-06-05T00:00:00"),
                    EndDateTime = DateTime.Parse("2013-06-07T00:00:00")
                },
                new DateTimePeriod
                {
                    StartDateTime = DateTime.Parse("2013-06-21T00:00:00"),
                    EndDateTime = DateTime.Parse("2013-06-22T00:00:00")
                },
                new DateTimePeriod
                {
                    StartDateTime = DateTime.Parse("2013-07-01T00:00:00"),
                    EndDateTime = DateTime.Parse("2013-08-01T00:00:00")
                },
                new DateTimePeriod
                {
                    StartDateTime = DateTime.Parse("2013-11-01T00:00:00"),
                    EndDateTime = DateTime.Parse("2013-11-02T00:00:00")
                },
                new DateTimePeriod
                {
                    StartDateTime = DateTime.Parse("2013-12-24T00:00:00"),
                    EndDateTime = DateTime.Parse("2013-12-27T00:00:00")
                },
                new DateTimePeriod
                {
                    StartDateTime = DateTime.Parse("2013-12-31T00:00:00"),
                    EndDateTime = DateTime.Parse("2014-01-02T00:00:00")
                }
            });

            // Create instance of calculator with mock rules
            _calculator = new CongestionTaxCalculator(_mockRules.Object);
        }

        [Test]
        public void CalculateTax_ShouldReturnZero_ForExcludedVehicleType()
        {
            // Arrange
            var vehicleType = "Motorcycle";
            var dateTimes = new List<DateTime> { DateTime.Parse("2013-11-29T07:00:00") };

            // Act
            var tax = _calculator.GetTax(vehicleType, dateTimes);

            // Assert
            Assert.That(tax, Is.EqualTo(0));
        }

        [Test]
        public void CalculateTax_ShouldReturnZero_ForTollFreeDate()
        {
            // Arrange
            var vehicleType = "Car";
            var dateTimes = new List<DateTime> { DateTime.Parse("2013-07-01T07:00:00"), DateTime.Parse("2013-07-04T07:00:00")}; // July excluded

            // Act
            var tax = _calculator.GetTax(vehicleType, dateTimes);

            // Assert
            Assert.That(tax, Is.EqualTo(0));
        }

        [Test]
        public void CalculateTax_ShouldReturnZero_ForExcludedDay()
        {
            // Arrange
            var vehicleType = "Car";
            var dateTimes = new List<DateTime> { DateTime.Parse("2013-01-12T06:15:00") }; // was a Saturday

            // Act
            var tax = _calculator.GetTax(vehicleType, dateTimes);

            // Assert
            Assert.That(tax, Is.EqualTo(0));
        }

        [Test]
        public void CalculateTax_ShouldRespectMaxDailyFee()
        {
            // Arrange
            var vehicleType = "Car";
            var dateTimes = new List<DateTime>
            {
                DateTime.Parse("2013-11-29T06:15:00"),
                DateTime.Parse("2013-11-29T07:15:00"),
                DateTime.Parse("2013-11-29T08:15:00"),
                DateTime.Parse("2013-11-29T09:15:00"),
                DateTime.Parse("2013-11-29T10:15:00"),
                DateTime.Parse("2013-11-29T11:15:00"),
                DateTime.Parse("2013-11-29T12:15:00")
            };

            // Act
            var tax = _calculator.GetTax(vehicleType, dateTimes);

            // Assert
            Assert.That(tax, Is.EqualTo(60));
        }

        [Test]
        public void CalculateTax_ShouldHandleIntervalsCorrectly()
        {
            // Arrange
            var vehicleType = "Car";
            var dateTimes = new List<DateTime>
            {
                DateTime.Parse("2013-11-29T06:15:00"), // 8
                DateTime.Parse("2013-11-29T06:45:00"), // 13 within interval, ignore 8
                DateTime.Parse("2013-11-29T07:45:00")  // 18 next interval
            };

            // Act
            var tax = _calculator.GetTax(vehicleType, dateTimes);

            // Assert
            Assert.That(tax, Is.EqualTo(13+18));
        }

        [Test]
        public void CalculateTax_ShouldHandleMultipleDaysCorrectly()
        {
            // Arrange
            var vehicleType = "Car";
            var dateTimes = new List<DateTime>
            {
                DateTime.Parse("2013-11-28T07:44:00"), // 18
                DateTime.Parse("2013-11-29T07:45:00")  // 18 next day
            };

            // Act
            var tax = _calculator.GetTax(vehicleType, dateTimes);

            // Assert
            Assert.That(tax, Is.EqualTo(18+18)); 
        }
    }

}