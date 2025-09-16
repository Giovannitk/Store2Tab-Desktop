// Store2Tab/App.xaml.cs
using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Store2Tab.Data;

namespace Store2Tab
{
    public partial class App : Application
    {
        public static string? TipoParametro { get; private set; }
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        private void OnStartup(object sender, StartupEventArgs e)
        {
            // Configurazione Dependency Injection
            ServiceProvider = ServiceConfiguration.ConfigureServices();

            // Gestione parametri da linea di comando
            if (e.Args.Length > 0)
            {
                TipoParametro = e.Args[0].Replace("#/#", "").Trim();
            }
            else
            {
                TipoParametro = null;
            }

            var mainWindow = new MainWindow();
            mainWindow.ShowWithParam(TipoParametro);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Cleanup del ServiceProvider
            if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }

            base.OnExit(e);
        }

    }
}