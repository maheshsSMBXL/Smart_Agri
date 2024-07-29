using Agri_Smart.data;

namespace Agri_Smart.Models
{
    public class ExpensesInput
    {
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryDate { get; set; }
        public DateTime? EstimatedHarvestDate { get; set; }
        public TimeSpan? IrrigationDuration { get; set; }
        public string? Observations { get; set; }
        public string? Attachments { get; set; }
        public double? FuelCost { get; set; }        
        public List<CategorySubExpensesInput>? CategorySubExpenses { get; set; }
        public List<WorkersExpenses>? Workers { get; set; }
        public List<MachineryExpenses>? Machinery { get; set; }
        public List<OtherExpensesInput>? OtherExpenses { get; set; }
        public string? TotalCost { get; set; }

    }
    public class WorkersExpenses 
    {
        public int? NoOfWorkers { get; set; }
        public double? CostPerWorker { get; set; }
        public double? TotalCost { get; set; }
    }
    public class MachineryExpenses
    {
        public int? NoOfWorkers { get; set; }
        public double? CostPerWorker { get; set; }
        public double? TotalCost { get; set; }
    }
    public class OtherExpensesInput
    {
        public int? NoOfWorkers { get; set; }
        public double? CostPerWorker { get; set; }
        public double? TotalCost { get; set; }
    }
    public class CategorySubExpensesInput
    {
        public string? Name { get; set; }
        public int? Quantity { get; set; }
        public string? Units { get; set; }
        public double? Cost { get; set; }
    }
}
