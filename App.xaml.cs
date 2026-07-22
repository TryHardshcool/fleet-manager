using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace GestionVehicule
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ConfigurerSerilog();

            // Capture les exceptions non gerees pour qu'elles finissent dans les logs
            // plutot que de simplement crasher sans laisser de trace exploitable.
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Log.Information("=== Démarrage de l'application GestionVehicule ===");
        }

        private void ConfigurerSerilog()
        {
            // Dossier "Logs" a cote de l'executable (bin/Debug/... ou bin/Release/...)
            // AppContext.BaseDirectory pointe toujours vers le dossier de l'exe, contrairement
            // a Environment.CurrentDirectory qui peut changer selon comment l'app est lancee.
            var cheminLogs = Path.Combine(AppContext.BaseDirectory, "Logs");

            // Cree le dossier manuellement en plus (le sink File le fait aussi tout seul,
            // mais l'avoir ici rend l'intention explicite et evite toute ambiguite).
            Directory.CreateDirectory(cheminLogs);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()

                // Un fichier par jour, garde les 14 derniers jours, purge automatiquement le reste
                .WriteTo.File(
                    path: Path.Combine(cheminLogs, "log-.txt"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 14,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")

                // Visible dans la fenetre Output/Sortie de Visual Studio pendant le debug
                .WriteTo.Debug(outputTemplate: "[{Level:u3}] {Message:lj}{NewLine}{Exception}")

                .CreateLogger();
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Fatal(e.Exception, "Exception non geree sur le thread UI");
            MessageBox.Show(
                "Une erreur inattendue est survenue. Consultez le dossier Logs pour plus de details.",
                "Erreur critique", MessageBoxButton.OK, MessageBoxImage.Error);

            // e.Handled = true; // A decommenter seulement si on veux empecher le crash complet de l'appli
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Fatal(e.ExceptionObject as Exception, "Exception non geree en dehors du thread UI");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("=== Fermeture de l'application ===");

            // Force l'ecriture de tous les logs en attente et libere le fichier proprement
            Log.CloseAndFlush();

            base.OnExit(e);
        }
    }
}