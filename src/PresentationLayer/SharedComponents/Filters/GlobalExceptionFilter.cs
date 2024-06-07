using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace SharedComponents.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger) 
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogInformation($"This error ocurred in execution: {context.Exception}");
            context.Result = new ObjectResult("Internal error ocurred")
            {
                StatusCode = 500,
            };
        }
    }
}
