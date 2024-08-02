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
        [AllowAnonymous]
        public async Task<IActionResult> GetExpensesCategories()
        {
            var categories = await _dbcontext.Category.ToListAsync();

            var categorySubCategories = new List<CategorySubCategory>();
            foreach (var cat in categories)
            {
                var categorySubCategory = new CategorySubCategory();
                categorySubCategory.Id = cat.CategoryId;
                categorySubCategory.Name = cat.CategoryName;
                categorySubCategory.Icon = cat.Icon;

                var subCategoriesList = await _dbcontext.MapCategorySubCategory.Where(a => a.CategoryId == cat.CategoryId).
                    Select(a => a.SubCategoryId).ToListAsync();

                var finalSubCategories = new List<SubCategory>();
                foreach (var subCategoryId in subCategoriesList) 
                {
                    var subCategory = new SubCategory();
                    var subCategories = await _dbcontext.SubCategory.Where(a => a.SubCategoryId == subCategoryId).ToListAsync();
                    foreach (var val in subCategories) 
                    {
                        subCategory.SubCategoryId = val.SubCategoryId;
                        subCategory.SubCategoryName = val.SubCategoryName;
                    }
                    finalSubCategories.Add(subCategory);
                    categorySubCategory.SubCategory = finalSubCategories;
                }
                categorySubCategories.Add(categorySubCategory);
            }
            return Ok(categorySubCategories);            
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


            return Ok(new { Status = "Success", Message = "Data Saved Successfully." });
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
                        categorySubExpenses.UserId = UserInfo?.Id;
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
                        worker.UserId = UserInfo?.Id;
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
                        machinery.UserId = UserInfo?.Id;
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
                        otherExpenses.UserId = UserInfo?.Id;
                        await _dbcontext.OtherExpenses.AddAsync(otherExpenses);
                    }
                }
            }

            _dbcontext.SaveChanges();

            return Ok(new { Status = "Success", Message = "Data Saved Successfully." });
        }

        [HttpGet]
        [Route("GetExpenses")]
        public async Task<IActionResult> GetExpenses()
        {
            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;
            var userInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);
                      
            var expenses = await _dbcontext.Expenses
                .Where(e => e.UserID == userInfo.Id)
                .ToListAsync();
            var CustomerExpenses = new List<CustomerExpenses>();
            foreach (var expense in expenses) 
            {
                var CustomerExpense = new CustomerExpenses();

                var categorySubExpenses = await _dbcontext.CategorySubExpenses
                .Where(cse => cse.CategoryId == expense.CategoryId && cse.ActivityId == expense.ActivityId)
                .ToListAsync();

                var workers = await _dbcontext.Workers
                    .Where(w => w.CategoryId == expense.CategoryId && w.ActivityId == expense.ActivityId)
                    .ToListAsync();

                var machineries = await _dbcontext.Machineries
                    .Where(m => m.CategoryId == expense.CategoryId && m.ActivityId == expense.ActivityId)
                    .ToListAsync();

                var otherExpenses = await _dbcontext.OtherExpenses
                    .Where(oe => oe.CategoryId == expense.CategoryId && oe.ActivityId == expense.ActivityId)
                    .ToListAsync();

                CustomerExpense.Expenses = expense;
                CustomerExpense.Workers = workers;
                CustomerExpense.Machinery = machineries;
                CustomerExpense.OtherExpenses = otherExpenses;

                CustomerExpenses.Add(CustomerExpense);
            }
            return Ok(CustomerExpenses);
        }
        [HttpGet]
        [Route("GetRevenueCategories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRevenueCategories()
        {
            var RevenueCategories = await _dbcontext.Revenue.ToListAsync();
            return Ok(RevenueCategories);
        }
        [HttpPost]
        [Route("SaveCustomerRevenue")]
        public async Task<IActionResult> SaveCustomerRevenue([FromBody] List<CustomerRevenue> request)
        {
            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;
            var UserInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);
            var activityId = Guid.NewGuid();
            var activityCretedDate = new DateTime();

            var customerRevenue = new CustomerRevenue();
            foreach (var req in request) 
            {
                customerRevenue.Id = Guid.NewGuid();
                customerRevenue.RevenueCategoryId = req.RevenueCategoryId;
                customerRevenue.RevenueCategoryName = req.RevenueCategoryName;
                customerRevenue.ActivityId = activityId;
                customerRevenue.Name = req.Name;
                customerRevenue.Price = req.Price;
                customerRevenue.PriceUnits = req.PriceUnits;
                customerRevenue.Quantity = req.Quantity;
                customerRevenue.QuantityUnits = req.QuantityUnits;
                customerRevenue.Date = req.Date;
                customerRevenue.Total = req.Total;
                customerRevenue.ActivityTotal = req.ActivityTotal;
                customerRevenue.CreatedDate = activityCretedDate;
                customerRevenue.UserID = UserInfo.Id;
                customerRevenue.CreatedBy = UserInfo.Id;
                await _dbcontext.CustomerRevenue.AddAsync(customerRevenue);
                _dbcontext.SaveChanges();
            }

            return Ok(new { Status = "Success", Message = "Data Saved Successfully." });
        }
        [HttpGet]
        [Route("GetCustomerRevenues")]
        public async Task<IActionResult> GetCustomerRevenues()
        {
            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;
            var UserInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);

            var CustomerRevenues = await _dbcontext.CustomerRevenue.Where(a => a.UserID == UserInfo.Id).ToListAsync(); 

            var groupedByActivity = CustomerRevenues
                .GroupBy(cr => cr.ActivityId)
                .Select(group => new
                {
                    ActivityId = group.Key,
                    Revenues = group.ToList()
                }).ToList();

            return Ok(groupedByActivity);
        }
        [HttpGet]
        [Route("GetToatlRevenueAndExpenses")]
        public async Task<IActionResult> GetToatlRevenueAndExpenses()
        {
            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;
            var UserInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);

            var totalRevenueAndExpenses = new TotalRevenueAndExpenses();

            var Workers = await _dbcontext.Workers.Where(w => w.UserId == UserInfo.Id).SumAsync(w => w.TotalCost) ?? 0;
            var Machinery = await _dbcontext.Machineries.Where(w => w.UserId == UserInfo.Id).SumAsync(w => w.TotalCost) ?? 0;
            var OtherExpenses = await _dbcontext.OtherExpenses.Where(w => w.UserId == UserInfo.Id).SumAsync(w => w.TotalCost) ?? 0;

            var RevenueDetails = await _dbcontext.CustomerRevenue
                .Where(w => w.UserID == UserInfo.Id && w.RevenueCategoryName == "Revenue details")
                .SumAsync(w => w.ActivityTotal) ?? 0;

            var HarvestedAndSold = await _dbcontext.CustomerRevenue
                .Where(w => w.UserID == UserInfo.Id && w.RevenueCategoryName == "Harvested and sold")
                .SumAsync(w => w.ActivityTotal) ?? 0;

            totalRevenueAndExpenses.TotalRevenue = (RevenueDetails + HarvestedAndSold);
            totalRevenueAndExpenses.CategorisedRevenues.RevenueDetails = RevenueDetails;
            totalRevenueAndExpenses.CategorisedRevenues.HarvestedAndSold = HarvestedAndSold;

            totalRevenueAndExpenses.TotalExpenses = (decimal)(Workers + Machinery + Machinery);
            totalRevenueAndExpenses.CategorisedExpenses.Workers = (decimal)Workers;
            totalRevenueAndExpenses.CategorisedExpenses.Machinery = (decimal)Machinery;
            totalRevenueAndExpenses.CategorisedExpenses.OtherExpenses = (decimal)Machinery;

            return Ok(totalRevenueAndExpenses);
        }
    }
}
