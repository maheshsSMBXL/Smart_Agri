using Agri_Smart.data;
using Agri_Smart.Models;
using Agri_Smart.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Threading;

namespace Agri_Smart.Controllers
{
    [Route("api/controller")]
    [ApiController]
    public class IntractionsController : ControllerBase
    {
        private readonly SmsService _smsService;
        private readonly IApplicationDbContext _dbcontext;
        public IntractionsController(SmsService smsService, IApplicationDbContext dbcontext)
        {
            _smsService = smsService;
            _dbcontext = dbcontext;
        }

        [HttpPost("SendSms")]
        public async Task<IActionResult> SendSms([FromBody] SmsRequest smsRequest)
        {
            var otp = GenerateOtp();

            var randotp = otp;
            var notBefore = DateTime.UtcNow;
            var notAfter = DateTime.UtcNow.AddMinutes(10);
            var interaction = await _dbcontext.Interactions.FirstOrDefaultAsync(x => x.PhoneNumber == smsRequest.PhoneNumber);

            if (interaction == null)
            {
                await _dbcontext.Interactions.AddAsync(new Intractions()
                {
                    Id = Guid.NewGuid(),
                    SmsOtp = randotp,
                    NotBefore = notBefore,
                    NotAfter = notAfter,
                    PhoneNumber = smsRequest.PhoneNumber
                });
            }
            else
            {
                interaction.SmsOtp = randotp;
                interaction.NotBefore = notBefore;
                interaction.NotAfter = notAfter;
                interaction.PhoneNumber = smsRequest.PhoneNumber;
            }

            _dbcontext.SaveChanges();

            string templateMessage = "Your 6 digit registration code to complete your registration with MarketCentral is #var#. Need Help? Contact +914046623456 - Team MarketCentral";
            string formattedMessage = templateMessage.Replace("#var#", otp.ToString());

            bool result = await _smsService.SendSmsAsync(smsRequest.PhoneNumber, formattedMessage);

            if (result)
            {
                return Ok(new { Status = "Success", Message = "Six digit otp sent to your mobile number!" });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Failed to send SMS" });
            }
        }
        [HttpPost("ValidateOtp")]
        public async Task<IActionResult> ValidateOtp([FromBody] OtpValidateRequest request)
        {
            var Interactions = await _dbcontext.Interactions.FirstOrDefaultAsync(a => a.PhoneNumber == request.PhoneNumber);
            var currentDate = DateTime.UtcNow;

            if (Interactions == null)
                return Ok(new Response { Status = "Error", Message = "Not Found" });
            if(Interactions.SmsOtp != request.Otp)
                return Ok(new Response { Status = "Error", Message = "Please enter valid otp" });
            if(currentDate < Interactions.NotBefore || currentDate > Interactions.NotAfter)
                return Ok(new Response { Status = "Error", Message = "OTP Expired" });

            Interactions.PhoneVerified = true;
            _dbcontext.SaveChanges();

            return Ok(new Response { Status = "Success", Message = "OTP Verified"});
        }
        private int GenerateOtp()
        {
            int _min = 100000;
            int _max = 999999;

            var randomGenerator = RandomNumberGenerator.GetInt32(_min, _max);

            return randomGenerator;
        }
    }
}
