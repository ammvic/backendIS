using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AnimalShelterAPI.Models;
using AnimalShelterAPI.Services;
using AnimalShelterAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AnimalShelterAPI.Cron
{
    public class ReminderCron : CronJobService
    {
        private readonly ILogger<ReminderCron> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMailer _mailer;
        private readonly IConfiguration _configuration;


        public ReminderCron(IScheduleConfig<ReminderCron> config, ILogger<ReminderCron> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IMailer mailer)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _mailer = mailer;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ReminderCron starts.");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} ReminderCron: Checking vaccination dates...");

            using var scope = _serviceProvider.CreateScope();
            var animalAggegatorRepo = scope.ServiceProvider.GetRequiredService<IAnimalAggregatorRepository>();

            DateTime minDate = DateTime.Now.AddDays(-30);
            var animals = animalAggegatorRepo.GetBeforeVaccinationDate(minDate);

            if (animals.Count != 0)
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                string url = _configuration.GetSection("WebUrl").Value + "/animal/";
                var sb = new StringBuilder();
                sb.Append("Gyvūnai skiepiti ne ankščiau kaip prieš 30 dienų:<br>");
                foreach (Animal animal in animals)
                {
                    sb.Append(animal.VaccinationDate).Append(" ").Append(url + animal.Id).Append("<br>");
                }

                var emails = userManager.Users.Select(user => user.Email).ToList();
                _mailer.SendMassEmailAsync(emails, "Skiepų priminimas", sb.ToString());

                animalAggegatorRepo.UpdateReminders(animals);
            }

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ReminderCron is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
