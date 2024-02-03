using CodePlusBlog.IService;

namespace CodePlusBlog.CRONService
{
    public class DailyNewsArticleCRONService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer;

        public DailyNewsArticleCRONService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            // Set up the timer to trigger at the desired time
            var now = DateTime.Now;
            var nextRunTime = new DateTime(now.Year, now.Month, now.Day,  // Specify the time of day you want to execute
                hour: 8, minute: 0, second: 0);  // Adjust the time here
            if (now > nextRunTime)
            {
                nextRunTime = nextRunTime.AddDays(1); // If it's already past the desired time, schedule for the next day
            }
            var delay = nextRunTime - now;
            _timer = new Timer(ExecuteMethod, null, delay, TimeSpan.FromDays(1)); // Repeat daily

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Execute once immediately on startup
            ExecuteMethod(null);

            await Task.CompletedTask;
        }

        private void ExecuteMethod(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<IDailyNewsArticleService>();
                service.GetTodayNewsService(); // Call your method from the service layer
            }
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}
