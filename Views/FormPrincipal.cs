// FormPrincipal.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeradorXML.Models;
using GeradorXML.Services;

namespace GeradorXML.Views
{
    public partial class FormPrincipal : Form
    {
        private OpenFileDialog? openFileDialog;
        private BackgroundWorker? backgroundWorker;
        private string? arquivoSelecionadoPath;
        
        // Controles
        private Button? btnSelecionarArquivo;
        private Button? btnProcessar;
        private Button? btnConversor;
        private Button? btnSobre;
        private Button? btnConfiguracao;
        private Button? btnSuporte;
        private Button? btnNotificacao;
        private TextBox? txtCaminhoArquivo;
        private Label? lblInfoArquivo;
        private ProgressBar? progressBar;
        private Label? lblProgresso;
        private Label? lblStatus;
        private Panel? panelProgresso;
        private ToolTip? toolTip;
        private PictureBox? pictureBoxIcone;
        
        // Campos para atualização
        private UpdateService? _updateService;
        private NotifyIcon? _notifyIcon;
        private int _notificacoesPendentes = 0;
        private List<UpdateInfo> _atualizacoesPendentes = new();
        private System.Windows.Forms.Timer? _blinkTimer;
        private bool _blinkState = false;
        private bool _temNovaVersao = false;

        public FormPrincipal()
        {
            try
            {
                this.Cursor = Cursors.Default;
                this.UseWaitCursor = false;
                
                InicializarComponentes();
                ConfigurarEventos();
                ConfigurarToolTips();
                ConfigurarBackgroundWorker();
                CarregarIcones();
                
                _updateService = new UpdateService();
                _updateService.OnUpdateAvailable += OnUpdateAvailable;
                
                this.Load += FormPrincipal_Load!;
                
                // ⭐ TIMER PARA PISCAR (APENAS QUANDO HOUVER NOVA VERSÃO)
                _blinkTimer = new System.Windows.Forms.Timer { Interval = 500 };
                _blinkTimer.Tick += (s, e) =>
                {
                    if (btnNotificacao != null && _temNovaVersao && _notificacoesPendentes > 0)
                    {
                        _blinkState = !_blinkState;
                        
                        if (_blinkState)
                        {
                            btnNotificacao.ForeColor = Color.FromArgb(211, 56, 7); // Vermelho
                        }
                        else
                        {
                            btnNotificacao.ForeColor = Color.White; // Branco
                        }
                    }
                };
                // ⭐ NÃO INICIAR O TIMER AINDA - só quando houver nova versão
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao inicializar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarIcones()
        {
            try
            {
                string iconePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "icone.ico");
                if (File.Exists(iconePath))
                {
                    this.Icon = new Icon(iconePath);
                }
                
                if (pictureBoxIcone != null)
                {
                    string pngPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "icone.png");
                    if (File.Exists(pngPath))
                    {
                        using (var originalImage = Image.FromFile(pngPath))
                        {
                            var highQualityImage = new Bitmap(originalImage.Width, originalImage.Height);
                            using (var graphics = Graphics.FromImage(highQualityImage))
                            {
                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                graphics.SmoothingMode = SmoothingMode.HighQuality;
                                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                graphics.DrawImage(originalImage, 0, 0, originalImage.Width, originalImage.Height);
                            }
                            pictureBoxIcone.Image = highQualityImage;
                        }
                        pictureBoxIcone.SizeMode = PictureBoxSizeMode.Zoom;
                        pictureBoxIcone.BackColor = Color.Transparent;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar ícones: {ex.Message}");
            }
        }

        private void InicializarComponentes()
        {
            this.Text = "Gerador de XML para Edificações - Netwin";
            this.Size = new Size(900, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(236, 240, 241);

            var panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.FromArgb(52, 73, 94)
            };

            pictureBoxIcone = new PictureBox
            {
                Location = new Point(20, 20),
                Size = new Size(80, 80),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            var lblTitulo = new Label
            {
                Text = "Gerador de XML Netwin",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(120, 30),
                Size = new Size(500, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblSubtitulo = new Label
            {
                Text = "Converta arquivos CSV em estrutura de XML organizada",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(200, 200, 200),
                Location = new Point(122, 75),
                Size = new Size(500, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            btnConfiguracao = new Button
            {
                Text = "⚙️",
                Location = new Point(840, 5),
                Size = new Size(45, 45),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
            
            btnConfiguracao.MouseEnter += (s, e) => 
            {
                btnConfiguracao.BackColor = Color.FromArgb(70, 90, 110);
                btnConfiguracao.FlatAppearance.BorderSize = 1;
                btnConfiguracao.FlatAppearance.BorderColor = Color.FromArgb(100, 120, 140);
            };
            
            btnConfiguracao.MouseLeave += (s, e) => 
            {
                btnConfiguracao.BackColor = Color.Transparent;
                btnConfiguracao.FlatAppearance.BorderSize = 0;
            };

            btnSuporte = new Button
            {
                Text = "💬 Suporte Técnico",
                Location = new Point(770, 65),
                Size = new Size(105, 40),
                BackColor = Color.FromArgb(255, 99, 71),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
            
            btnSuporte.MouseEnter += (s, e) => 
            {
                btnSuporte.BackColor = Color.FromArgb(255, 99, 71);
                btnSuporte.FlatAppearance.BorderSize = 1;
                btnSuporte.FlatAppearance.BorderColor = Color.FromArgb(255, 123, 81);
            };
            
            btnSuporte.MouseLeave += (s, e) => 
            {
                btnSuporte.BackColor = Color.FromArgb(255, 99, 71);
                btnSuporte.FlatAppearance.BorderSize = 0;
            };

            // ⭐ BOTÃO DE NOTIFICAÇÃO
            btnNotificacao = new Button
            {
                Text = "🔔",
                Location = new Point(790, 5),
                Size = new Size(45, 45),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 },
                Tag = 0
            };

            btnNotificacao.MouseEnter += (s, e) => 
            {
                btnNotificacao.BackColor = Color.FromArgb(70, 90, 110);
                btnNotificacao.FlatAppearance.BorderSize = 1;
                btnNotificacao.FlatAppearance.BorderColor = Color.FromArgb(100, 120, 140);
            };

            btnNotificacao.MouseLeave += (s, e) => 
            {
                btnNotificacao.BackColor = Color.Transparent;
                btnNotificacao.FlatAppearance.BorderSize = 0;
            };

            btnNotificacao.Click += BtnNotificacao_Click!;

            panelHeader.Controls.AddRange(new Control[] { 
                pictureBoxIcone,
                lblTitulo, 
                lblSubtitulo,
                btnConfiguracao,
                btnSuporte,
                btnNotificacao
            });

            var groupBoxUpload = new GroupBox
            {
                Text = "Upload do Arquivo CSV",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(50, 140),
                Size = new Size(800, 160),
                BackColor = Color.White
            };

            btnSelecionarArquivo = new Button
            {
                Text = "📁 Selecionar CSV",
                Location = new Point(20, 35),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };

            txtCaminhoArquivo = new TextBox
            {
                Location = new Point(185, 45),
                Size = new Size(590, 25),
                ReadOnly = true,
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(245, 245, 245)
            };

            lblInfoArquivo = new Label
            {
                Text = "Nenhum arquivo selecionado",
                Location = new Point(185, 80),
                Size = new Size(590, 25),
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9)
            };

            btnProcessar = new Button
            {
                Text = "🚀 Gerar XML",
                Location = new Point(200, 110),
                Size = new Size(500, 40),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            groupBoxUpload.Controls.AddRange(new Control[] {
                btnSelecionarArquivo,
                txtCaminhoArquivo,
                lblInfoArquivo,
                btnProcessar
            });

            panelProgresso = new Panel
            {
                Location = new Point(50, 320),
                Size = new Size(800, 100),
                Visible = false
            };

            progressBar = new ProgressBar
            {
                Location = new Point(20, 20),
                Size = new Size(700, 30),
                Style = ProgressBarStyle.Continuous
            };

            lblProgresso = new Label
            {
                Text = "0%",
                Location = new Point(730, 25),
                Size = new Size(50, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.Transparent
            };

            lblStatus = new Label
            {
                Text = "Aguardando início...",
                Location = new Point(20, 65),
                Size = new Size(760, 25),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Blue
            };

            panelProgresso.Controls.AddRange(new Control[] {
                progressBar,
                lblProgresso,
                lblStatus
            });

            var panelFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.FromArgb(44, 62, 80)
            };

            btnConversor = new Button
            {
                Text = "🔄 Conversor CSV",
                Location = new Point(700, 10),
                Size = new Size(150, 30),
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };

            btnSobre = new Button
            {
                Text = "ℹ️ Sobre",
                Location = new Point(20, 10),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };

            var lblFooter = new Label
            {
                Text = "© 2026 Geovane Carvalho - Gerador de XML Netwin v1.0.0",
                Location = new Point(230, 18),
                Size = new Size(400, 20),
                ForeColor = Color.FromArgb(180, 180, 180),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 8)
            };

            panelFooter.Controls.AddRange(new Control[] {
                btnSobre,
                lblFooter,
                btnConversor
            });

            this.Controls.AddRange(new Control[] {
                panelHeader,
                groupBoxUpload,
                panelProgresso,
                panelFooter
            });

            AtualizarEstadoBotaoProcessar("aguardando", false);
        }

        private void ConfigurarEventos()
        {
            if (btnSelecionarArquivo != null)
                btnSelecionarArquivo.Click += BtnSelecionarArquivo_Click!;
            
            if (btnProcessar != null)
                btnProcessar.Click += BtnProcessar_Click!;
            
            if (btnConversor != null)
                btnConversor.Click += BtnConversor_Click!;
            
            if (btnSobre != null)
                btnSobre.Click += BtnSobre_Click!;
            
            if (btnConfiguracao != null)
                btnConfiguracao.Click += BtnConfiguracao_Click!;
            
            if (btnSuporte != null)
                btnSuporte.Click += BtnSuporte_Click!;
        }

        private void ConfigurarToolTips()
        {
            toolTip = new ToolTip();
            
            if (btnSelecionarArquivo != null)
                toolTip.SetToolTip(btnSelecionarArquivo, "Clique para selecionar um arquivo CSV");
            
            if (btnProcessar != null)
                toolTip.SetToolTip(btnProcessar, "Processa o arquivo CSV e gera os XMLs");
            
            if (btnConversor != null)
                toolTip.SetToolTip(btnConversor, "Acessa o conversor de CSV para Power Query");
            
            if (btnSobre != null)
                toolTip.SetToolTip(btnSobre, "Informações sobre o sistema");
            
            if (btnConfiguracao != null)
                toolTip.SetToolTip(btnConfiguracao, "Configuração dos Roteiros XML");
            
            if (btnSuporte != null)
                toolTip.SetToolTip(btnSuporte, "Fale conosco no WhatsApp para suporte");
            
            if (btnNotificacao != null)
                toolTip.SetToolTip(btnNotificacao, "Notificações de atualização");
        }

        private void ConfigurarBackgroundWorker()
        {
            backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = false
            };
            
            backgroundWorker.DoWork += BackgroundWorker_DoWork!;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged!;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted!;
        }

        #region Métodos de Atualização

        private async void FormPrincipal_Load(object? sender, EventArgs e)
        {
            try
            {
                await Task.Delay(1000);
                
                if (_updateService != null)
                {
                    await _updateService.VerificarAtualizacaoAsync();
                }
                
                Cursor.Current = Cursors.Default;
                this.UseWaitCursor = false;
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na verificação: {ex.Message}");
                Cursor.Current = Cursors.Default;
                this.UseWaitCursor = false;
            }
        }

        private void OnUpdateAvailable(UpdateInfo updateInfo)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(() => OnUpdateAvailable(updateInfo));
                return;
            }
            
            try
            {
                _notificacoesPendentes++;
                _atualizacoesPendentes.Add(updateInfo);
                
                // ⭐ MARCAR QUE HÁ NOVA VERSÃO E INICIAR PISCAR
                _temNovaVersao = true;
                _blinkState = true;
                _blinkTimer?.Start();
                
                AtualizarIconeSino();
                MostrarNotificacaoSistema(updateInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar atualização: {ex.Message}");
            }
        }

        private void AtualizarIconeSino()
        {
            if (btnNotificacao == null) return;
            
            try
            {
                if (_notificacoesPendentes > 0 && _temNovaVersao)
                {
                    btnNotificacao.Text = $"🔔 {_notificacoesPendentes}";
                    btnNotificacao.Tag = _notificacoesPendentes;
                    btnNotificacao.ForeColor = Color.FromArgb(211, 56, 7);
                }
                else
                {
                    _blinkTimer?.Stop();
                    _temNovaVersao = false;
                    _blinkState = false;
                    btnNotificacao.Text = "🔔";
                    btnNotificacao.ForeColor = Color.White;
                    btnNotificacao.Tag = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar sino: {ex.Message}");
            }
        }

        private void MostrarNotificacaoSistema(UpdateInfo updateInfo)
        {
            try
            {
                Icon appIcon;
                try
                {
                    appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) ?? SystemIcons.Information;
                }
                catch
                {
                    appIcon = SystemIcons.Information;
                }
                
                _notifyIcon = new NotifyIcon
                {
                    Icon = appIcon,
                    BalloonTipTitle = "🔄 Nova Atualização Disponível",
                    BalloonTipText = $"Versão {updateInfo.VersaoNova} está disponível!\nClique aqui para instalar.",
                    Visible = true,
                    Tag = updateInfo
                };

                _notifyIcon.BalloonTipClicked += (s, e) =>
                {
                    var icon = s as NotifyIcon;
                    if (icon?.Tag is UpdateInfo info)
                    {
                        icon.Visible = false;
                        icon.Dispose();
                        _notifyIcon = null;
                        AbrirFormularioAtualizacao(info);
                    }
                };

                _notifyIcon.Click += (s, e) =>
                {
                    var icon = s as NotifyIcon;
                    if (icon?.Tag is UpdateInfo info)
                    {
                        icon.Visible = false;
                        icon.Dispose();
                        _notifyIcon = null;
                        AbrirFormularioAtualizacao(info);
                    }
                };

                _notifyIcon.ShowBalloonTip(30000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao mostrar notificação: {ex.Message}");
            }
        }

        private void BtnNotificacao_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_notificacoesPendentes > 0 && _atualizacoesPendentes.Count > 0)
                {
                    // ⭐ PARAR O PISCAR QUANDO O USUÁRIO CLICAR
                    _blinkTimer?.Stop();
                    _temNovaVersao = false;
                    _blinkState = false;
                    
                    // Manter o sino vermelho (sem piscar)
                    if (btnNotificacao != null)
                    {
                        btnNotificacao.ForeColor = Color.FromArgb(211, 56, 7);
                    }
                    
                    var updateInfo = _atualizacoesPendentes.Last();
                    AbrirFormularioAtualizacao(updateInfo);
                }
                else
                {
                    if (_updateService != null)
                    {
                        Task.Run(async () =>
                        {
                            await _updateService.VerificarAtualizacaoAsync(true);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AbrirFormularioAtualizacao(UpdateInfo updateInfo)
        {
            try
            {
                var formUpdate = new FormUpdate(updateInfo, _updateService!);
                formUpdate.FormClosed += (s, e) =>
                {
                    _atualizacoesPendentes.Remove(updateInfo);
                    _notificacoesPendentes = _atualizacoesPendentes.Count;
                    AtualizarIconeSino();
                };
                formUpdate.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir atualização: {ex.Message}", 
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Métodos do BackgroundWorker

        private void BtnSelecionarArquivo_Click(object? sender, EventArgs e)
        {
            using (openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Arquivos CSV (*.csv)|*.csv|Todos os arquivos (*.*)|*.*";
                openFileDialog.Title = "Selecione o arquivo CSV";
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    arquivoSelecionadoPath = openFileDialog.FileName;
                    
                    if (txtCaminhoArquivo != null)
                        txtCaminhoArquivo.Text = arquivoSelecionadoPath;
                    
                    if (lblInfoArquivo != null)
                    {
                        lblInfoArquivo.Text = $"Arquivo selecionado: {Path.GetFileName(arquivoSelecionadoPath)}";
                        lblInfoArquivo.ForeColor = Color.Green;
                    }
                    
                    AtualizarEstadoBotaoProcessar("pronto", true);
                }
            }
        }

        private void BtnProcessar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(arquivoSelecionadoPath))
            {
                MessageBox.Show("Por favor, selecione um arquivo CSV primeiro.", 
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(arquivoSelecionadoPath))
            {
                MessageBox.Show("O arquivo selecionado não existe mais.", 
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                LimparCampos();
                AtualizarEstadoBotaoProcessar("aguardando", false);
                return;
            }

            AtualizarEstadoBotaoProcessar("processando", false);
            
            if (btnSelecionarArquivo != null)
                btnSelecionarArquivo.Enabled = false;
            
            if (btnConversor != null)
                btnConversor.Enabled = false;
            
            if (progressBar != null)
                progressBar.Value = 0;
            
            if (lblProgresso != null)
                lblProgresso.Text = "0%";
            
            if (lblStatus != null)
            {
                lblStatus.Text = "Iniciando processamento...";
                lblStatus.ForeColor = Color.Blue;
            }
            
            if (panelProgresso != null)
                panelProgresso.Visible = true;
            
            backgroundWorker?.RunWorkerAsync(arquivoSelecionadoPath);
        }

        private void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            var arquivoPath = e.Argument as string;
            var worker = sender as BackgroundWorker;
            
            try
            {
                worker?.ReportProgress(10, "📂 Validando arquivo...");
                
                var csvReader = new CsvReaderService();
                var xmlGenerator = new XmlGeneratorService();
                
                var colunasValidas = csvReader.ValidarColunas(arquivoPath!, out var colunasFaltantes);
                
                if (!colunasValidas)
                {
                    var msg = $"Colunas faltantes: {string.Join(", ", colunasFaltantes)}";
                    throw new Exception(msg);
                }
                
                worker?.ReportProgress(20, "📖 Lendo arquivo CSV...");
                
                var registros = csvReader.LerCsv(arquivoPath!);
                var totalRegistros = registros.Count;
                
                worker?.ReportProgress(30, $"✅ {totalRegistros} registros encontrados");
                
                var estacao = registros.FirstOrDefault()?.EstacaoAbastecedora ?? "DESCONHECIDA";
                var diretorioPrincipal = Path.Combine(Path.GetTempPath(), 
                    $"moradias_xml_{estacao}_{DateTime.Now:yyyyMMddHHmmss}");
                Directory.CreateDirectory(diretorioPrincipal);
                
                var pastasCriadas = new List<string>();
                
                int contadorSemComplementos = 0;
                int contador1Complemento = 0;
                int contador2Complementos = 0;
                int contador3Complementos = 0;
                
                for (int i = 0; i < totalRegistros; i++)
                {
                    var registro = registros[i];
                    var percentual = 30 + (i * 60 / totalRegistros);
                    
                    var tipo = xmlGenerator.DeterminarTipoComplemento(
                        registro.Complemento, 
                        registro.Complemento2, 
                        registro.Complemento3);
                    
                    worker?.ReportProgress(percentual, 
                        $"📝 Registro {i + 1}/{totalRegistros} - Survey: {registro.CodSurvey} - Tipo: {tipo} complemento(s)");
                    
                    var xmlBytes = xmlGenerator.GerarXml(registro, tipo);
                    
                    var nomePasta = $"moradia{i + 1}";
                    var caminhoPasta = Path.Combine(diretorioPrincipal, nomePasta);
                    Directory.CreateDirectory(caminhoPasta);
                    pastasCriadas.Add(caminhoPasta);
                    
                    var xmlPath = Path.Combine(caminhoPasta, $"{nomePasta}.xml");
                    File.WriteAllBytes(xmlPath, xmlBytes);
                    
                    switch (tipo)
                    {
                        case 0: contadorSemComplementos++; break;
                        case 1: contador1Complemento++; break;
                        case 2: contador2Complementos++; break;
                        case 3: contador3Complementos++; break;
                    }
                }
                
                worker?.ReportProgress(95, "📦 Criando arquivo ZIP...");
                
                var culturaBrasil = new System.Globalization.CultureInfo("pt-BR");
                var dataArquivo = DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss", culturaBrasil);
                var zipFilename = $"moradias_xml_{estacao}_{dataArquivo}.zip";
                var zipPath = Path.Combine(Path.GetTempPath(), zipFilename);
                
                System.IO.Compression.ZipFile.CreateFromDirectory(diretorioPrincipal, zipPath);
                
                Directory.Delete(diretorioPrincipal, true);
                
                var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                var destinoZip = Path.Combine(downloadsPath, zipFilename);
                File.Copy(zipPath, destinoZip, true);
                File.Delete(zipPath);
                
                var dataFormatada = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", culturaBrasil);
                
                var logEstatisticas = GerarTabelaEstatisticas(
                    contador3Complementos,
                    contador2Complementos,
                    contador1Complemento,
                    contadorSemComplementos,
                    totalRegistros
                );
                
                logEstatisticas += $@"

📁 Arquivo salvo em: {destinoZip}

📅 Data/Hora: {dataFormatada}";
                
                e.Result = new ProcessamentoResultadoSimulado
                {
                    Sucesso = true,
                    ArquivoGerado = zipFilename,
                    ArquivoGeradoCompleto = destinoZip,
                    TotalRegistros = totalRegistros,
                    Log = logEstatisticas
                };
            }
            catch (Exception ex)
            {
                e.Result = new ProcessamentoResultadoSimulado
                {
                    Sucesso = false,
                    Erro = ex.Message
                };
            }
        }

        private string GerarTabelaEstatisticas(int c3, int c2, int c1, int c0, int total)
        {
            return $@"
✅ PROCESSAMENTO CONCLUÍDO

📊 ESTATÍSTICAS FINAIS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
• XML com 3 complementos: {c3,4} registros
• XML com 2 complementos: {c2,4} registros
• XML com 1 complemento:  {c1,4} registros
• XML sem complementos:   {c0,4} registros
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
• TOTAL DE REGISTROS: {total,4} registros
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━";
        }
       
        private void BackgroundWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (progressBar != null)
                progressBar.Value = e.ProgressPercentage;
            
            if (lblProgresso != null)
                lblProgresso.Text = $"{e.ProgressPercentage}%";
            
            if (lblStatus != null && e.UserState is string mensagem)
            {
                lblStatus.Text = mensagem;
            }
        }
        
        private void BackgroundWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is ProcessamentoResultadoSimulado resultado)
            {
                if (resultado.Sucesso)
                {
                    if (lblStatus != null)
                    {
                        lblStatus.Text = "✅ Processamento concluído com sucesso!";
                        lblStatus.ForeColor = Color.Green;
                    }
                    
                    if (progressBar != null)
                        progressBar.Value = 100;
                    
                    if (lblProgresso != null)
                        lblProgresso.Text = "100%";
                    
                    LimparCampos();
                    AtualizarEstadoBotaoProcessar("aguardando", false);
                    
                    var resultDialog = MessageBox.Show(
                        resultado.Log + "\n\nDeseja abrir a pasta onde o arquivo foi salvo?",
                        "Sucesso",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);
                    
                    if (resultDialog == DialogResult.Yes)
                    {
                        if (!string.IsNullOrEmpty(resultado.ArquivoGeradoCompleto))
                        {
                            var pasta = Path.GetDirectoryName(resultado.ArquivoGeradoCompleto);
                            if (!string.IsNullOrEmpty(pasta) && Directory.Exists(pasta))
                            {
                                AbrirPastaComArquivoSelecionado(resultado.ArquivoGeradoCompleto);
                            }
                        }
                    }
                }
                else
                {
                    if (lblStatus != null)
                    {
                        lblStatus.Text = "❌ Erro no processamento!";
                        lblStatus.ForeColor = Color.Red;
                    }
                    
                    LimparCampos();
                    AtualizarEstadoBotaoProcessar("aguardando", false);
                    
                    MessageBox.Show(
                        $"Erro ao processar o arquivo:\n\n{resultado.Erro}",
                        "Erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            
            if (btnSelecionarArquivo != null)
                btnSelecionarArquivo.Enabled = true;
            
            if (btnConversor != null)
                btnConversor.Enabled = true;
            
            var timer = new System.Windows.Forms.Timer { Interval = 2000 };
            timer.Tick += (s, args) => 
            {
                var t = s as System.Windows.Forms.Timer;
                if (t != null)
                {
                    t.Stop();
                    t.Dispose();
                }
                
                if (panelProgresso != null)
                    panelProgresso.Visible = false;
            };
            timer.Start();
        }

        private void AbrirPastaComArquivoSelecionado(string caminhoArquivo)
        {
            try
            {
                if (File.Exists(caminhoArquivo))
                {
                    System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{caminhoArquivo}\"");
                }
                else
                {
                    MessageBox.Show($"Arquivo não encontrado: {caminhoArquivo}", 
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir o Explorer: {ex.Message}", 
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void BtnConversor_Click(object? sender, EventArgs e)
        {
            var formConversor = new FormConversorCsv();
            formConversor.ShowDialog();
        }
        
        private void BtnSobre_Click(object? sender, EventArgs e)
        {
            MessageBox.Show(
                "Gerador de XML para Edificações\n\n" +
                "Versão: 1.0.0\n\n" +
                "Desenvolvido para Telemont\n\n" +
                "Funcionalidades:\n" +
                "• Geração de XML a partir de CSV\n" +
                "• Suporte a complementos (1, 2 ou 3)\n" +
                "• Validação de dados\n" +
                "• Conversão para Power Query",
                "Sobre o Sistema",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        
        private void BtnConfiguracao_Click(object? sender, EventArgs e)
        {
            var formConfig = new FormConfiguracao();
            formConfig.ShowDialog();
        }
        
        private void BtnSuporte_Click(object? sender, EventArgs e)
        {
            var resultado = MessageBox.Show(
                "Precisa de ajuda com o sistema?\n\n" +
                "Clique em OK para abrir o WhatsApp e falar com nosso suporte.\n\n" +
                "Telefone: +55 61 99548-8068",
                "Suporte Técnico",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);
            
            if (resultado == DialogResult.OK)
            {
                var telefone = "5561995488068";
                var mensagem = "Olá! Preciso de ajuda com o Gerador de XML para Edificações.";
                var mensagemCodificada = Uri.EscapeDataString(mensagem);
                var urlWhatsApp = $"https://wa.me/{telefone}?text={mensagemCodificada}";
                
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = urlWhatsApp,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao abrir o WhatsApp: {ex.Message}", 
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AtualizarEstadoBotaoProcessar(string estado, bool habilitado = false)
        {
            if (btnProcessar == null) return;
            
            btnProcessar.Enabled = habilitado;
            
            switch (estado)
            {
                case "aguardando":
                    btnProcessar.Text = "⏳ Aguardando CSV...";
                    btnProcessar.BackColor = Color.FromArgb(211, 211, 211);
                    btnProcessar.ForeColor = Color.White;
                    btnProcessar.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
                    break;
                    
                case "pronto":
                    btnProcessar.Text = "🚀 Gerar XML";
                    btnProcessar.BackColor = Color.FromArgb(46, 204, 113);
                    btnProcessar.ForeColor = Color.White;
                    btnProcessar.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
                    break;
                    
                case "processando":
                    btnProcessar.Text = "⏳ Processando...";
                    btnProcessar.BackColor = Color.FromArgb(211, 211, 211);
                    btnProcessar.ForeColor = Color.White;
                    btnProcessar.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
                    break;
            }
        }

        private void LimparCampos()
        {
            if (txtCaminhoArquivo != null)
                txtCaminhoArquivo.Text = string.Empty;
            
            if (lblInfoArquivo != null)
            {
                lblInfoArquivo.Text = "Nenhum arquivo selecionado";
                lblInfoArquivo.ForeColor = SystemColors.ControlText;
            }
            
            arquivoSelecionadoPath = null;
        }

        #endregion

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // ⭐ Parar timer de piscar
            if (_blinkTimer != null)
            {
                _blinkTimer.Stop();
                _blinkTimer.Dispose();
                _blinkTimer = null;
            }
            
            _notifyIcon?.Dispose();
            if (_updateService != null)
                _updateService.OnUpdateAvailable -= OnUpdateAvailable;
            base.OnFormClosed(e);
        }
    }

    public class ProcessamentoResultadoSimulado
    {
        public bool Sucesso { get; set; }
        public string? Erro { get; set; }
        public string? ArquivoGerado { get; set; }
        public string? ArquivoGeradoCompleto { get; set; }
        public int TotalRegistros { get; set; }
        public string? Log { get; set; }
    }
}