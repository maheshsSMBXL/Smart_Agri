using Agri_Smart.data;
using Agri_Smart.Models;
using InfluxDB.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agri_Smart.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesManagementController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _context;
        private readonly InfluxDBClient _influxDBClient;
        private readonly string _bucket;
        private readonly string _org;
        private readonly IApplicationDbContext _dbcontext;

        public ExpensesManagementController(UserManager<IdentityUser> context, InfluxDBClient influxDBClient,
            IConfiguration configuration, IApplicationDbContext dbcontext)
        {
            _context = context;
            _influxDBClient = influxDBClient;
            _bucket = configuration["ConnectionStrings:InfluxDBBucket"];
            _org = configuration["ConnectionStrings:InfluxDBOrg"];
            _dbcontext = dbcontext;
        }
        [HttpGet]
        [Route("GetExpensesCategories")]
        public async Task<IActionResult> GetExpensesCategories()
        {
            var categories = await _dbcontext.Category.ToListAsync();
            return Ok(categories);
        }
        [HttpPost]
        [Route("SaveExpenses")]
        public async Task<IActionResult> SaveExpenses([FromBody] ExpensesInput request)
        {
            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;
            var UserInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);
            var activityId = Guid.NewGuid();
            var activityCretedDate = new DateTime();
           
            if (request != null)
            {
                var expenses = new Expenses();
                expenses.CategoryId = request.CategoryId;
                expenses.ActivityId = activityId;
                expenses.CategoryName = request?.CategoryName;
                expenses.CategoryDate = request?.CategoryDate;
                expenses.EstimatedHarvestDate = request?.EstimatedHarvestDate;
                expenses.FuelCost = request?.FuelCost;
                expenses.TotalCost = request?.TotalCost;
                expenses.CreatedDate = activityCretedDate;
                expenses.UserID = UserInfo?.Id;
                expenses.CreatedBy = UserInfo?.Id;
                await _dbcontext.Expenses.AddAsync(expenses);

                if (request?.CategorySubExpenses != null) 
                {
                    foreach (var categorySubExpense in request.CategorySubExpenses) 
                    {
                        var categorySubExpenses = new CategorySubExpenses();
                        categorySubExpenses.CategoryId = request.CategoryId;
                        categorySubExpenses.ActivityId = activityId;
                        categorySubExpenses.IrrigationDuration = request.IrrigationDuration != null ? new TimeSpan((int)(request?.IrrigationDuration?.Hours), (int)(request?.IrrigationDuration?.Minutes), (int)(request?.IrrigationDuration?.Seconds)) : null;
                        categorySubExpenses.Name = categorySubExpense.Name;
                        categorySubExpenses.Quantity = categorySubExpense.Quantity;
                        categorySubExpenses.Units = categorySubExpense.Units;
                        categorySubExpenses.Cost = categorySubExpenses.Cost;
                        categorySubExpenses.Observations = categorySubExpenses.Observations;
                        categorySubExpenses.Attachments = categorySubExpenses.Attachments;
                        categorySubExpenses.CreatedDate = activityCretedDate;
                        await _dbcontext.CategorySubExpenses.AddAsync(categorySubExpenses);
                    }
                }
                if (request?.Workers != null)
                {
                    foreach (var workerExpense in request.Workers)
                    {
                        var worker = new Workers();
                        worker.CategoryId = request.CategoryId;
                        worker.ActivityId = activityId;
                        worker.NoOfWorkers = workerExpense.NoOfWorkers;
                        worker.CostPerWorker = workerExpense.CostPerWorker;
                        worker.TotalCost = workerExpense.TotalCost;
                        worker.CreatedDate = activityCretedDate;
                        await _dbcontext.Workers.AddAsync(worker);
                    }
                }
                if (request?.Machinery != null)
                {
                    foreach (var machineryExpense in request.Machinery)
                    {
                        var machinery = new Machinery();
                        machinery.CategoryId = request.CategoryId;
                        machinery.ActivityId = activityId;
                        machinery.NoOfMachines = machineryExpense.NoOfMachines;
                        machinery.CostPerMachine = machineryExpense.CostPerMachine;
                        machinery.TotalCost = machineryExpense.TotalCost;
                        machinery.CreatedDate = activityCretedDate;
                        await _dbcontext.Machineries.AddAsync(machinery);
                    }
                }
                if (request?.OtherExpenses != null)
                {
                    foreach (var otherExpense in request.OtherExpenses)
                    {
                        var otherExpenses = new OtherExpenses();
                        otherExpenses.CategoryId = request.CategoryId;
                        otherExpenses.ActivityId = activityId;
                        otherExpenses.Expense = otherExpense.Expense;
                        otherExpenses.Cost = otherExpense.Cost;
                        otherExpenses.TotalCost = otherExpense.TotalCost;
                        otherExpenses.CreatedDate = activityCretedDate;
                        await _dbcontext.OtherExpenses.AddAsync(otherExpenses);
                    }
                }
            }
            _dbcontext.SaveChanges();


            return Ok(null);
        }

    }
}
