using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agri_Smart.Services
{
    public class PushNotificationService
    {
        private readonly ILogger<PushNotificationService> _logger;

        public PushNotificationService(ILogger<PushNotificationService> logger, IWebHostEnvironment env)
        {
            _logger = logger;

            // Initialize the Firebase Admin SDK
            var filePath = Path.Combine(env.ContentRootPath, "Files", "service-account-MC.json");
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(filePath) // Replace with the path to your service account key file
            });
        }

        public async Task SendNotificationAsync()
        {
            var registrationTokens = new List<string>
        {            
            //"fsPF7zC-TD6y4LLYFl0pov:APA91bEHa5FrhvZSEe5IpNMldfLyqhJuG0Riu9p0122jfkVekFFY5TOS_szo2TMJ-YIVgCljppMDXeDNZ8qN8ngQzNtb_zhgRPy1ieS2X1K1yjZu7w3MQrUXk715gwcQrQi9z46eazwe",
            // "capmArQ6QVq3YIsLW77u6v:APA91bFhgfnObXxc7VQ9uFnlnLwtPCg-w8iW1IvsB2KMuX6BLGleVFol_SYKGTYsyEo2bVZ_1I6atd5KcOTaO1u-tDtGVEFm0orCZdLBGOcpWbYnlggdrdo2qRZZCA2QAZ7uYzUAKURI",
        };

            var message = new MulticastMessage
            {
                Tokens = registrationTokens,
                Notification = new Notification
                {
                    Title = "Naruto Fans, Rejoice! 🍥",
                    Body = "The wait is over! Naruto: Shippuden is now streaming in HD!!"
                    // ImageUrl = "https://internalportal.smbxl.com/assets/img.jpg" // Uncomment if needed
                }
            };

            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
                _logger.LogInformation("Successfully sent message: {0}", response.SuccessCount);
            }
            catch (FirebaseMessagingException ex)
            {
                _logger.LogError("Error sending message: {0}", ex.Message);
            }
        }
    }
}
