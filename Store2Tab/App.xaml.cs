using System;
using System.Windows;

namespace Store2Tab
{
    public partial class App : Application
    {
        public static string? TipoParametro { get; private set; }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            // esempio: pssTab.exe TIPOPAGAMENTO
            if (e.Args.Length > 0)
            {
                // Pulisce il parametro rimuovendo eventuali caratteri extra
                TipoParametro = e.Args[0].Replace("#/#", "").Trim();
            }
            else
            {
                TipoParametro = null;
            }

            MessageBox.Show($"Parametro passato: {TipoParametro}");

            var mainWindow = new MainWindow();
            mainWindow.ShowWithParam(TipoParametro);
        }
    }
}