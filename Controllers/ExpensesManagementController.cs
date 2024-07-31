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
                        categorySubExpenses.Cost = categorySubExpense.Cost;
                        categorySubExpenses.Observations = request.Observations;
                        categorySubExpenses.Attachments = request.Attachments;
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
        [HttpPost]
        [Route("SaveExpensesNew")]
        public async Task<IActionResult> SaveExpensesNew([FromBody] ExpensesInput request)
        {
            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;
            var UserInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);
            var activityId = Guid.NewGuid();
            var activityCretedDate = new DateTime();

            if (request != null)
            {
                var expenses = await _dbcontext.Expenses
                    .FirstOrDefaultAsync(e => e.CategoryId == request.CategoryId && e.ActivityId == request.ActivityId);

                if (expenses == null)
                {
                    expenses = new Expenses();
                    expenses.ActivityId = request.ActivityId != null ? request.ActivityId : activityId;
                    expenses.CreatedDate = activityCretedDate;
                    expenses.UserID = UserInfo?.Id;
                    expenses.CreatedBy = UserInfo?.Id;
                    await _dbcontext.Expenses.AddAsync(expenses);
                }

                expenses.CategoryId = request.CategoryId;
                expenses.CategoryName = request?.CategoryName;
                expenses.CategoryDate = request?.CategoryDate;
                expenses.EstimatedHarvestDate = request?.EstimatedHarvestDate;
                expenses.FuelCost = request?.FuelCost;
                expenses.TotalCost = request?.TotalCost;

                if (request?.CategorySubExpenses != null)
                {
                    var existingSubExpenses = await _dbcontext.CategorySubExpenses
                        .Where(cse => cse.CategoryId == request.CategoryId && cse.ActivityId == request.ActivityId)
                        .ToListAsync();

                    _dbcontext.CategorySubExpenses.RemoveRange(existingSubExpenses);

                    foreach (var categorySubExpense in request.CategorySubExpenses)
                    {
                        var categorySubExpenses = new CategorySubExpenses();
                        categorySubExpenses.CategoryId = request.CategoryId;
                        categorySubExpenses.ActivityId = request.ActivityId != null ? request.ActivityId : activityId;
                        categorySubExpenses.IrrigationDuration = request.IrrigationDuration != null
                            ? new TimeSpan((int)(request?.IrrigationDuration?.Hours), (int)(request?.IrrigationDuration?.Minutes), (int)(request?.IrrigationDuration?.Seconds))
                            : null;
                        categorySubExpenses.Name = categorySubExpense.Name;
                        categorySubExpenses.Quantity = categorySubExpense.Quantity;
                        categorySubExpenses.Units = categorySubExpense.Units;
                        categorySubExpenses.Cost = categorySubExpense.Cost;
                        categorySubExpenses.Observations = request.Observations;
                        categorySubExpenses.Attachments = request.Attachments;
                        categorySubExpenses.CreatedDate = activityCretedDate;
                        await _dbcontext.CategorySubExpenses.AddAsync(categorySubExpenses);
                    }
                }

                if (request?.Workers != null)
                {
                    var existingWorkers = await _dbcontext.Workers
                        .Where(w => w.CategoryId == request.CategoryId && w.ActivityId == request.ActivityId)
                        .ToListAsync();

                    _dbcontext.Workers.RemoveRange(existingWorkers);

                    foreach (var workerExpense in request.Workers)
                    {
                        var worker = new Workers();
                        worker.CategoryId = request.CategoryId;
                        worker.ActivityId = request.ActivityId != null ? request.ActivityId : activityId;
                        worker.NoOfWorkers = workerExpense.NoOfWorkers;
                        worker.CostPerWorker = workerExpense.CostPerWorker;
                        worker.TotalCost = workerExpense.TotalCost;
                        worker.CreatedDate = activityCretedDate;
                        await _dbcontext.Workers.AddAsync(worker);
                    }
                }

                if (request?.Machinery != null)
                {
                    var existingMachinery = await _dbcontext.Machineries
                        .Where(m => m.CategoryId == request.CategoryId && m.ActivityId == request.ActivityId)
                        .ToListAsync();

                    _dbcontext.Machineries.RemoveRange(existingMachinery);

                    foreach (var machineryExpense in request.Machinery)
                    {
                        var machinery = new Machinery();
                        machinery.CategoryId = request.CategoryId;
                        machinery.ActivityId = request.ActivityId != null ? request.ActivityId : activityId;
                        machinery.NoOfMachines = machineryExpense.NoOfMachines;
                        machinery.CostPerMachine = machineryExpense.CostPerMachine;
                        machinery.TotalCost = machineryExpense.TotalCost;
                        machinery.CreatedDate = activityCretedDate;
                        await _dbcontext.Machineries.AddAsync(machinery);
                    }
                }

                if (request?.OtherExpenses != null)
                {
                    var existingOtherExpenses = await _dbcontext.OtherExpenses
                        .Where(oe => oe.CategoryId == request.CategoryId && oe.ActivityId == request.ActivityId)
                        .ToListAsync();

                    _dbcontext.OtherExpenses.RemoveRange(existingOtherExpenses);

                    foreach (var otherExpense in request.OtherExpenses)
                    {
                        var otherExpenses = new OtherExpenses();
                        otherExpenses.CategoryId = request.CategoryId;
                        otherExpenses.ActivityId = request.ActivityId != null ? request.ActivityId : activityId;
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

        [HttpGet]
        [Route("GetExpenses/{categoryId}")]
        public async Task<IActionResult> GetExpenses(Guid categoryId)
        {
            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;
            var userInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);
            
            if (userInfo == null)
            {
                return NotFound("User not found");
            }

            var expenses = await _dbcontext.Expenses
                .Where(e => e.UserID == userInfo.Id && e.CategoryId == categoryId)
                .ToListAsync();

            var categorySubExpenses = await _dbcontext.CategorySubExpenses
                .Where(cse => cse.CategoryId == categoryId && cse.ActivityId == expenses.FirstOrDefault().ActivityId)
                .ToListAsync();

            var workers = await _dbcontext.Workers
                .Where(w => w.CategoryId == categoryId && w.ActivityId == expenses.FirstOrDefault().ActivityId)
                .ToListAsync();

            var machineries = await _dbcontext.Machineries
                .Where(m => m.CategoryId == categoryId && m.ActivityId == expenses.FirstOrDefault().ActivityId)
                .ToListAsync();

            var otherExpenses = await _dbcontext.OtherExpenses
                .Where(oe => oe.CategoryId == categoryId && oe.ActivityId == expenses.FirstOrDefault().ActivityId)
                .ToListAsync();

            var result = new
            {
                Expenses = expenses.Select(e => new
                {
                    e.CategoryId,
                    e.ActivityId,
                    e.CategoryName,
                    e.CategoryDate,
                    e.EstimatedHarvestDate,
                    e.FuelCost,
                    e.TotalCost,
                    e.CreatedDate,
                    e.UserID,
                    e.CreatedBy
                }),
                CategorySubExpenses = categorySubExpenses.Select(cse => new
                {
                    cse.CategoryId,
                    cse.ActivityId,
                    cse.IrrigationDuration,
                    cse.Name,
                    cse.Quantity,
                    cse.Units,
                    cse.Cost,
                    cse.Observations,
                    cse.Attachments,
                    cse.CreatedDate
                }),
                Workers = workers.Select(w => new
                {
                    w.CategoryId,
                    w.ActivityId,
                    w.NoOfWorkers,
                    w.CostPerWorker,
                    w.TotalCost,
                    w.CreatedDate
                }),
                Machineries = machineries.Select(m => new
                {
                    m.CategoryId,
                    m.ActivityId,
                    m.NoOfMachines,
                    m.CostPerMachine,
                    m.TotalCost,
                    m.CreatedDate
                }),
                OtherExpenses = otherExpenses.Select(oe => new
                {
                    oe.CategoryId,
                    oe.ActivityId,
                    oe.Expense,
                    oe.Cost,
                    oe.TotalCost,
                    oe.CreatedDate
                })
            };

            return Ok(result);
        }

    }
}
