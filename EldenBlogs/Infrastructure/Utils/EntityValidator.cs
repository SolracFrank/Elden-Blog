using Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Utils
{
    public static class EntityValidator
    {
        public static void EnsureDoesNotExist<T>(this T entity, ILogger logger) where T : class
        {
            if (entity != null)
            {
                var entityName = typeof(T).Name;
                logger.LogInformation($"{entityName} already exists");
                throw new BadRequestException($"{entityName} already exists");
            }
        }
        public static void EnsureExists<T>(this T entity, ILogger logger, string? logInformation = null, string? exceptionInformation = null) where T : class
        {
            if (entity == null)
            {
                var entityName = typeof(T).Name;
                logger.LogInformation(logInformation?? $"{entityName} does not exist");
                throw new BadRequestException(exceptionInformation ?? $"{entityName} does not exists");
            }
        }

    }
}
