using AnimalShelterAPI.Models;
using AnimalShelterAPI.Infrastructure.Repositories;
using AnimalShelterAPI.Services;
using AnimalShelterAPI.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AnimalShelterAPI.Configurations
{
    public static class DepedencyInjectionExtensions
    {
        public static IServiceCollection AddAllDependencies(this IServiceCollection service)
        {
            return service
                .AddInfrastructureDependencies()
                .AddApplicationDependencies();
        }

        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection service)
        {
            return service
                .AddScoped<IRepository<Animal>, AnimalRepository>()
                .AddScoped<IReportRepository, ReportRepository>()
                .AddScoped<IStatusRepository, StatusRepository>()
                .AddScoped<IFilterRepository, FilterRepository>()
                .AddScoped<IAnimalAggregatorRepository, AnimalAggregatorRepository>()
                .AddScoped<IUserRepository, UserRepository>();  
        }

        public static IServiceCollection AddApplicationDependencies(this IServiceCollection service)
        {
            return service
                .AddScoped<IAnimalService, AnimalService>()
                .AddScoped<IReportService, ReportService>()
                .AddScoped<IFilterService, FilterService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IAccommodationService, AccommodationService>();

        }
    }
}
