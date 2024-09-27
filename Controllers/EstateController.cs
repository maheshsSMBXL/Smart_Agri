using Agri_Smart.data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agri_Smart.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EstateController : ControllerBase
    {
        private readonly IApplicationDbContext _dbcontext;
        public EstateController(IApplicationDbContext dbcontext) 
        {
            _dbcontext = dbcontext;
        }
        [HttpPost]
        [Route("AddFarm")]
        public async Task<IActionResult> AddFarm([FromBody] Farms farm)
        {
            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;
            var userInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);

            var estate = await _dbcontext.Estates.FirstOrDefaultAsync(a => a.EstateManagerId == userInfo.Id);

            farm.FarmId = Guid.NewGuid();
            farm.EstateId = estate?.EstateId;
            await _dbcontext.Farms.AddAsync(farm);
            _dbcontext.SaveChanges();

            return Ok(new { Status = "Success", Message = "Farm added successfully." });            
        }

    }
}
