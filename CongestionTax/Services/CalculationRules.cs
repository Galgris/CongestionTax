using Microsoft.Extensions.Options;

namespace CongestionTax
{
    public interface ICalculationRules
    {
        List<DateTimePeriod> ExcludedDateTimePeriods { get; }
        List<DayOfWeek> ExcludedDaysOfWeek { get; }
        List<string> ExcludedVehicleTypes { get; }
        int IntervalMinutes { get; }
        int MaxDailyFee { get; }
        List<TaxPeriod> TaxPeriods { get; }
    }

    public class CalculationRules : ICalculationRules
    {
        private readonly CalculationRulesConfig _config;

        public CalculationRules(IOptions<CalculationRulesConfig> config)
        {
            _config = config.Value;
        }

        public int MaxDailyFee => _config.MaxDailyFee;
        public int IntervalMinutes => _config.IntervalMinutes;
        public List<string> ExcludedVehicleTypes => _config.ExcludedVehicleTypes;
        public List<DayOfWeek> ExcludedDaysOfWeek => _config.ExcludedDaysOfWeek;
        public List<TaxPeriod> TaxPeriods => _config.TaxPeriods;
        public List<DateTimePeriod> ExcludedDateTimePeriods => _config.ExcludedDateTimePeriods;
    }
}
