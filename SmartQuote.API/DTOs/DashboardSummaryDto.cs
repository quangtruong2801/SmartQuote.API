using SmartQuote.API.Controllers;

public class DashboardSummaryDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalQuotations { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalProducts { get; set; }
    public List<ChartDataDto> ChartData { get; set; } = new List<ChartDataDto>();
}