using AnimalShelterAPI.Configuration;
using AnimalShelterAPI.Cron;
using AnimalShelterAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AnimalShelterAPI.Configurations
{
    public static class StartupExtensions
    {
        public static void SetUpAutoMapper(this IServiceCollection services)
        {
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperConfiguration());
            });
            var mapper = config.CreateMapper();

            services.AddSingleton(mapper);
        }

        public static void SetUpCronJobs(this IServiceCollection services)
        {

            services.AddCronJob<ReminderCron>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"*/1 * * * *";
            });
        }
    }
}
