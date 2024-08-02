using Agri_Smart.data;
using Agri_Smart.Models;
using InfluxDB.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agri_Smart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BackOfficeController : ControllerBase
    {
        private readonly IApplicationDbContext _dbcontext;

        public BackOfficeController(IApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [HttpPost]
        [Route("RegisterUspDevice")]
        public async Task<IActionResult> RegisterUspDevice([FromBody] RegisterDeviceInput request)
        {
            if (request.OldMacId == null)
            {
                var tenantId = Guid.NewGuid().ToString();
                var device = new Devices();
                device.PhoneNumber = request.PhoneNumber;
                device.MacId = request.NewMacId;
                device.TenantId = tenantId;
                await _dbcontext.Devices.AddAsync(device);
                _dbcontext.SaveChanges();

                return Ok(new { Status = "Success", Message = "Device mapped with user successfully." });
            }
            else
            {
                var device = await _dbcontext.Devices.FirstOrDefaultAsync(a => a.MacId == request.OldMacId && a.PhoneNumber == request.PhoneNumber);
                if (device != null) 
                {
                    device.MacId = request.NewMacId;
                    _dbcontext.SaveChanges();

                    return Ok(new { Status = "Success", Message = "Old device replaced with new device successfully." });
                }
            }
            return Ok(new { Status = "Failed"});

        }
        [HttpPost]
        [Route("SaveCategory")]
        public async Task<IActionResult> SaveCategory([FromBody] List<Category> request)
        {
            foreach (var req in request)
            {
                var category = new Category();

                category.CategoryId = Guid.NewGuid();
                category.CategoryName = req.CategoryName;
                category.Icon = req.Icon;
                
                await _dbcontext.Category.AddAsync(category);
                _dbcontext.SaveChanges();
            }

            return Ok(new { message = "Data inserted successfully." });            
        }
        [HttpPost]
        [Route("SaveSubCategory")]
        public async Task<IActionResult> SaveSubCategory([FromBody] List<SubCategory> request)
        {
            foreach (var req in request)
            {
                var subCategory = new SubCategory();

                subCategory.SubCategoryId = Guid.NewGuid();
                subCategory.SubCategoryName = req.SubCategoryName;

                await _dbcontext.SubCategory.AddAsync(subCategory);
                _dbcontext.SaveChanges();
            }

            return Ok(new { message = "Data inserted successfully." });
        }
        [HttpPost]
        [Route("MapCategorySubCategory")]
        public async Task<IActionResult> MapCategorySubCategory([FromBody] MapCatSubCat request)
        {
            foreach (var req in request.SubCategoryId)
            {
                var mapCategorySubCategory = new MapCategorySubCategory();

                mapCategorySubCategory.Id = Guid.NewGuid();
                mapCategorySubCategory.CategoryId = request.CategoryId;
                mapCategorySubCategory.SubCategoryId = req;

                await _dbcontext.MapCategorySubCategory.AddAsync(mapCategorySubCategory);
                _dbcontext.SaveChanges();
            }

            return Ok(new { message = "Data inserted successfully." });
        }
        [HttpPost]
        [Route("SaveRevenueCategory")]
        public async Task<IActionResult> SaveRevenueCategory([FromBody] List<Revenue> request)
        {
            foreach (var req in request)
            {
                var revenue = new Revenue();

                revenue.RevenueId = Guid.NewGuid();
                revenue.RevenueName = req.RevenueName;

                await _dbcontext.Revenue.AddAsync(revenue);
                _dbcontext.SaveChanges();
            }

            return Ok(new { message = "Data inserted successfully." });
        }

    }
}
