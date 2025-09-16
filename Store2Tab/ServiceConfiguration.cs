// Store2Tab/ServiceConfiguration.cs - VERSIONE IBRIDA
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Store2Tab.Core.Services;
using Store2Tab.Core.Services.Interfaces;
using Store2Tab.Data;
using Store2Tab.Data.Repositories;
using Store2Tab.Data.Repositories.Interfaces;
using System.IO;

namespace Store2Tab
{
    public static class ServiceConfiguration
    {
        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Prima prova a leggere da configurazioni salvate dall'utente
            var connectionString = GetConnectionStringFromUserSettings() ?? GetDefaultConnectionString();

            // Entity Framework
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString);

#if DEBUG
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                options.LogTo(message => System.Diagnostics.Debug.WriteLine(message));
#endif
            });

            // Repository Pattern
            services.AddScoped<IBancaRepository, BancaRepository>();

            // Business Services
            services.AddScoped<IBancaService, BancaService>();

            // ViewModels
            services.AddTransient<ViewModels.BancheViewModel>();

            return services.BuildServiceProvider();
        }

        private static string? GetConnectionStringFromUserSettings()
        {
            try
            {
                // Legge da un file di configurazione utente in AppData
                var userConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                 "Store2Tab", "user-config.json");

                if (File.Exists(userConfigPath))
                {
                    var configJson = File.ReadAllText(userConfigPath);
                    var config = System.Text.Json.JsonSerializer.Deserialize<UserConfig>(configJson);
                    if (!string.IsNullOrWhiteSpace(config?.ConnectionString))
                    {
                        System.Diagnostics.Debug.WriteLine("Using user-configured connection string from JSON");
                        return config.ConnectionString;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading user settings: {ex.Message}");
            }

            return null;
        }

        public static string GetEffectiveConnectionString()
        {
            return GetConnectionStringFromUserSettings() ?? GetDefaultConnectionString();
        }


        private static string GetDefaultConnectionString()
        {
            // Fallback di default
            return "Server=SERVER2019\\PSERVICE;Database=pss_b_marcello;User Id=sa;Password=barcatfilcat;TrustServerCertificate=true;Connection Timeout=30;";
        }

        public static string GetEffectiveDefaultConnectionString()
        {
            return GetDefaultConnectionString() ?? GetDefaultConnectionString();
        }

        // Metodo per salvare nuove configurazioni utente
        public static void SaveUserConnectionString(string connectionString)
        {
            try
            {
                // Salva in file JSON utente in AppData
                var userConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                 "Store2Tab", "user-config.json");

                Directory.CreateDirectory(Path.GetDirectoryName(userConfigPath)!);

                var config = new UserConfig { ConnectionString = connectionString };
                var configJson = System.Text.Json.JsonSerializer.Serialize(config, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(userConfigPath, configJson);

                System.Diagnostics.Debug.WriteLine($"User connection string saved to {userConfigPath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving user settings: {ex.Message}");
                throw;
            }
        }

        // Metodo per testare una connection string prima di salvarla
        public static async Task<bool> TestConnectionStringAsync(string connectionString)
        {
            try
            {
                using var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
                await connection.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    // Classe per la configurazione utente
    public class UserConfig
    {
        public string? ConnectionString { get; set; }
        public DateTime LastModified { get; set; } = DateTime.Now;
    }
}