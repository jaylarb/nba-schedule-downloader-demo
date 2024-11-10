using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nba.Data.Models;

namespace Nba.Data
{
    public static class DataServiceCollectionExtensions
    {
        /// <summary>
        /// Used by consuming programs to add the data services to the service collection.
        /// </summary>
        public static IServiceCollection AddDataServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<NbaSqlContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }
    }

}
