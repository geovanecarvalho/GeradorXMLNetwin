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
                // Registrar encoding para suporte a Latin-1 (necessário para CSV)
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                
                // Executa o formulário principal
                Application.Run(new FormPrincipal());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro fatal: {ex.Message}\n\n{ex.StackTrace}", 
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}