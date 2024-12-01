using Microsoft.AspNetCore.Mvc;

namespace CongestionTax
{
    [ApiController]
    [Route("[controller]")]
    public class TaxController : ControllerBase
    {
        private readonly CongestionTaxCalculator _calculator;

        public TaxController(CongestionTaxCalculator calculator)
        {
            _calculator = calculator;
        }

        [HttpPost("calculate")]
        public IActionResult CalculateTax([FromBody] TaxRequest request)
        {
            var tax = _calculator.GetTax(request.VehicleType, request.DateTimes);
            return Ok(new { Tax = tax });
        }
    }

    public class TaxRequest
    {
        public string VehicleType { get; set; }
        public List<DateTime> DateTimes { get; set; }
    }
}
