using Microsoft.Extensions.Logging;

namespace WpfApp4.Services
{
    public class GreetingService : IGreetingService
    {
        private readonly ILogger<GreetingService> _logger;

        public GreetingService(ILogger<GreetingService> logger)
        {
            _logger = logger;
        }

        public string Greet(string name)
        {
            _logger.LogInformation("Greeting {Name}", name);
            return $"Hello, {name}!";
        }
    }
}
