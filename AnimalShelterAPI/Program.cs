using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnimalShelterAPI.Database;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AnimalShelterAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelterAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Izvlači port iz environment varijable
            var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";

            var host = CreateWebHostBuilder(args, port).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<ApiContext>();

                    // Primeni migracije
                    context.Database.Migrate();

                    // Seed podaci
                    DatabaseSeeder.Initialize(context);

                    Console.WriteLine("Baza je inicijalizovana i podaci ubačeni.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Greška pri inicijalizaciji baze: " + ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, string port) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls($"http://*:{port}");  // PORT handling za Render
    }

}
