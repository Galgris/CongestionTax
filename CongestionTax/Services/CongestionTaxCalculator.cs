using System;
using System.Collections.Generic;
using System.Linq;

namespace CongestionTax
{

    public class CongestionTaxCalculator
    {
        private readonly ICalculationRules _rules;

        public CongestionTaxCalculator(ICalculationRules rules)
        {
            _rules = rules;
        }

        public int GetTax(string vehicleType, List<DateTime> dateTimes)
        {
            if (dateTimes == null || !dateTimes.Any())
                return 0;

            if (IsTollFreeVehicle(vehicleType))
                return 0;

            DateTime currentDay = dateTimes.First().Date;
            DateTime? intervalStart = null;
            int intervalFee = 0;
            int dailyFee = 0;
            int totalFee = 0;

            int maxDailyFee = _rules.MaxDailyFee;

            foreach (var dateTime in dateTimes)
            {
                if (currentDay != dateTime.Date)
                {
                    // New day -> add to total and reset
                    dailyFee += intervalFee;
                    dailyFee = Math.Min(dailyFee, maxDailyFee);
                    totalFee += dailyFee;

                    // Reset day and interval
                    dailyFee = 0;
                    intervalFee = 0;
                    intervalStart = null;
                    currentDay = dateTime.Date;
                }

                if (IsTollFreeDay(dateTime) || IsTollFreeDate(dateTime))
                    continue;

                int fee = GetTollFee(dateTime);

                if (fee == 0)
                    continue;

                if (intervalStart == null)
                {
                    // No interval -> create new
                    intervalStart = dateTime;
                    intervalFee = fee;
                }
                else
                {
                    // Check if still in valid interval
                    var duration = dateTime - intervalStart.Value;
                    if (duration.TotalMinutes >= _rules.IntervalMinutes)
                    {
                        // Interval expired -> add to daily fee
                        dailyFee += intervalFee;
                        dailyFee = Math.Min(dailyFee, maxDailyFee);

                        // Reset interval
                        intervalFee = fee;
                        intervalStart = dateTime;
                    }
                    else
                    {
                        // Update interval fee
                        intervalFee = Math.Max(intervalFee, fee);
                    }
                }
            }

            // Add last interval and day fees
            dailyFee += intervalFee;
            dailyFee = Math.Min(dailyFee, maxDailyFee);
            totalFee += dailyFee;

            return totalFee;
        }

        // --- Helper methods ---
        private int GetTollFee(DateTime dateTime)
        {
            // Find a matching tax period or return 0
            var timeOfDay = dateTime.TimeOfDay;
            var taxPeriod = _rules.TaxPeriods.FirstOrDefault(period =>
                timeOfDay >= period.StartTime && timeOfDay < period.EndTime);

            return taxPeriod?.TaxAmount ?? 0;
        }

        private bool IsTollFreeVehicle(string vehicleType)
        {
            return _rules.ExcludedVehicleTypes.Contains(vehicleType, StringComparer.InvariantCultureIgnoreCase);
        }

        private bool IsTollFreeDay(DateTime dateTime)
        {
            return _rules.ExcludedDaysOfWeek.Contains(dateTime.DayOfWeek);
        }

        private bool IsTollFreeDate(DateTime dateTime)
        {
            return _rules.ExcludedDateTimePeriods.Any(period =>
                dateTime >= period.StartDateTime && dateTime < period.EndDateTime);
        }
    }
}
