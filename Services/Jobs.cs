namespace Agri_Smart.Services
{
    public class Jobs : IHostedService, IDisposable
    {
        private readonly PushNotificationService _pushNotificationService;
        private Timer _timer;

        public Jobs(PushNotificationService pushNotificationService)
        {
            _pushNotificationService = pushNotificationService;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            //_pushNotificationService.SendNotificationAsync().GetAwaiter().GetResult();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
