// Program.cs
using System;
using System.Text;
using System.Windows.Forms;
using GeradorXML.Views;

namespace GeradorXML
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormPrincipal());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro fatal: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}