using System;
using System.Windows.Forms;

namespace AlocadorDeProdutos
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // ApplicationConfiguration.Initialize() (gerado pelo SDK em net*-windows)
            // configura: EnableVisualStyles, CompatibleTextRenderingDefault=false,
            // HighDpiMode=PerMonitorV2 e fonte default. Substitui a sequencia
            // legada do .NET Framework.
            ApplicationConfiguration.Initialize();
            Application.Run(new FormMain());
        }
    }
}
