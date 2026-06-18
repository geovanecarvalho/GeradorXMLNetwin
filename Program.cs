// Program.cs
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeradorXML.Models;  // ← IMPORTANTE: Para UpdateInfo
using GeradorXML.Services;
using GeradorXML.Views;
using System.Drawing;

namespace GeradorXML
{
    internal static class Program
    {
        private static UpdateService? _updateService;
        private static bool _isCheckingUpdates = false;
        private static System.Windows.Forms.Timer? _updateTimer;

        [STAThread]
        static void Main()
        {
            try
            {
                // Registrar encoding para suporte a Latin-1
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                
                // Iniciar verificação automática de atualizações
                Task.Run(async () => await IniciarVerificacaoAutomaticaAsync());
                
                // Executar o formulário principal
                Application.Run(new FormPrincipal());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro fatal: {ex.Message}\n\n{ex.StackTrace}", 
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static async Task IniciarVerificacaoAutomaticaAsync()
        {
            try
            {
                _updateService = new UpdateService();
                var config = _updateService.GetConfig();
                
                // Se for a primeira execução ou já passou o intervalo
                if (config.IsFirstRun || 
                    (DateTime.Now - config.LastCheck).TotalHours >= config.CheckIntervalHours)
                {
                    await VerificarEAtualizarAutomaticamenteAsync();
                }
                
                // Configurar timer para verificar periodicamente
                if (config.AutoCheck)
                {
                    ConfigurarTimerVerificacao(config.CheckIntervalHours);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao iniciar verificação automática: {ex.Message}");
            }
        }

        private static void ConfigurarTimerVerificacao(int intervalHours)
        {
            if (_updateTimer == null)
            {
                _updateTimer = new System.Windows.Forms.Timer();
                _updateTimer.Interval = intervalHours * 60 * 60 * 1000; // Converter horas para milissegundos
                _updateTimer.Tick += async (s, e) => 
                {
                    await VerificarEAtualizarAutomaticamenteAsync();
                };
                _updateTimer.Start();
            }
        }

        private static async Task VerificarEAtualizarAutomaticamenteAsync()
        {
            if (_isCheckingUpdates || _updateService == null) return;
            
            _isCheckingUpdates = true;
            
            try
            {
                var response = await _updateService.CheckForUpdatesAsync();
                
                if (response.HasUpdate && response.UpdateInfo != null)
                {
                    // Atualizar configuração
                    var config = _updateService.GetConfig();
                    config.IsFirstRun = false;
                    config.LastCheck = DateTime.Now;
                    _updateService.SaveConfig(config);
                    
                    // Verificar se a versão é obrigatória ou se deve mostrar notificação
                    if (response.UpdateInfo.IsObrigatoria)
                    {
                        // Atualização obrigatória - baixar e instalar automaticamente
                        await BaixarEInstalarAutomaticamenteAsync(response.UpdateInfo);
                    }
                    else
                    {
                        // Atualização opcional - mostrar notificação
                        MostrarNotificacaoAtualizacao(response.UpdateInfo);
                    }
                }
                else
                {
                    // Atualizar configuração mesmo sem atualização
                    var config = _updateService.GetConfig();
                    config.LastCheck = DateTime.Now;
                    _updateService.SaveConfig(config);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na verificação automática: {ex.Message}");
            }
            finally
            {
                _isCheckingUpdates = false;
            }
        }

        private static async Task BaixarEInstalarAutomaticamenteAsync(UpdateInfo updateInfo)
        {
            try
            {
                // Mostrar notificação de atualização obrigatória
                var result = MessageBox.Show(
                    $"📢 ATUALIZAÇÃO OBRIGATÓRIA!\n\n" +
                    $"Nova versão {updateInfo.VersaoNova} disponível.\n" +
                    $"O programa será atualizado automaticamente.\n\n" +
                    $"Deseja continuar?",
                    "Atualização Obrigatória",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Configurar eventos de progresso
                    _updateService!.OnDownloadProgress += (s, e) =>
                    {
                        // Atualizar status na barra de tarefas ou notificação
                        Console.WriteLine($"Download: {e.Percent}% - {e.Message}");
                    };

                    _updateService.OnError += (s, error) =>
                    {
                        MessageBox.Show($"Erro na atualização automática:\n{error}", 
                            "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    };

                    await _updateService.BaixarAtualizacaoAsync(updateInfo.DownloadUrl);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao baixar atualização:\n{ex.Message}", 
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void MostrarNotificacaoAtualizacao(UpdateInfo updateInfo)
        {
            try
            {
                // Mostrar notificação no canto inferior direito
                var notifyIcon = new NotifyIcon
                {
                    Icon = SystemIcons.Information,
                    BalloonTipTitle = "🔄 Nova Atualização Disponível",
                    BalloonTipText = $"Versão {updateInfo.VersaoNova} está disponível!\nClique aqui para instalar.",
                    Visible = true
                };

                // Evento de clique na notificação
                notifyIcon.BalloonTipClicked += (s, e) =>
                {
                    // Abrir o formulário de atualização
                    var formUpdate = new FormUpdate(updateInfo, _updateService!);
                    formUpdate.ShowDialog();
                    notifyIcon.Dispose();
                };

                // Evento de clique no ícone
                notifyIcon.Click += (s, e) =>
                {
                    var formUpdate = new FormUpdate(updateInfo, _updateService!);
                    formUpdate.ShowDialog();
                    notifyIcon.Dispose();
                };

                // Mostrar balão por 30 segundos
                notifyIcon.ShowBalloonTip(30000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao mostrar notificação: {ex.Message}");
            }
        }
    }
}