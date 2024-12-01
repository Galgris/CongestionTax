namespace CongestionTax
{
    public class CalculationRulesConfig
    {
        public int MaxDailyFee { get; set; }
        public int IntervalMinutes { get; set; }
        public List<string> ExcludedVehicleTypes { get; set; }
        public List<DayOfWeek> ExcludedDaysOfWeek { get; set; }
        public List<TaxPeriod> TaxPeriods { get; set; }
        public List<DateTimePeriod> ExcludedDateTimePeriods { get; set; }
    }

    public class DateTimePeriod
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }

    public class TaxPeriod
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int TaxAmount { get; set; }
    }
}
