// FormPrincipal.cs - Adicione estes usings no topo
using System;
using System.Collections.Generic;  // ← Adicionar para List<>
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;  // ← Adicionar para FirstOrDefault
using System.Windows.Forms;
using GeradorXML.Models;  // ← Adicionar
using GeradorXML.Services;
using System.Text;  // ← Adicionar para StringBuilder

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
        private TextBox? txtCaminhoArquivo;
        private Label? lblInfoArquivo;
        private ProgressBar? progressBar;
        private Label? lblProgresso;
        private Label? lblStatus;
        private Panel? panelProgresso;
        private ToolTip? toolTip;
        private PictureBox? pictureBoxIcone;

        public FormPrincipal()
        {
            try
            {
                InicializarComponentes();
                ConfigurarEventos();
                ConfigurarToolTips();
                ConfigurarBackgroundWorker();
                CarregarIcones();
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
                // Ícone da janela
                string iconePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "icone.ico");
                if (File.Exists(iconePath))
                {
                    this.Icon = new Icon(iconePath);
                }
                
                // Ícone no PictureBox
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
            // Configuração da Janela Principal
            this.Text = "Gerador de XML para Edificações - Netwin";
            this.Size = new Size(900, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(236, 240, 241);

            // Panel Superior (Header)
            var panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.FromArgb(52, 73, 94)
            };

            // PictureBox para o ícone
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

            // ==================== BOTÃO CONFIGURAÇÃO ====================
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
            
            // Efeito hover no botão configuração
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

            // ==================== BOTÃO SUPORTE WHATSAPP ====================
            btnSuporte = new Button
            {
                Text = "💬 Suporte Técnico",
                Location = new Point(770, 65),
                Size = new Size(105, 40),
                BackColor = Color.FromArgb(37, 211, 102),  // Cor do WhatsApp
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
            
            // Efeito hover no botão suporte
            btnSuporte.MouseEnter += (s, e) => 
            {
                btnSuporte.BackColor = Color.FromArgb(47, 221, 112);
                btnSuporte.FlatAppearance.BorderSize = 1;
                btnSuporte.FlatAppearance.BorderColor = Color.FromArgb(57, 231, 122);
            };
            
            btnSuporte.MouseLeave += (s, e) => 
            {
                btnSuporte.BackColor = Color.FromArgb(37, 211, 102);
                btnSuporte.FlatAppearance.BorderSize = 0;
            };

            panelHeader.Controls.AddRange(new Control[] { 
                pictureBoxIcone,
                lblTitulo, 
                lblSubtitulo,
                btnConfiguracao,
                btnSuporte
            });

            // GroupBox Upload
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
                Font = new Font("Segoe UI", 10)
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

            // Panel de Progresso
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

            // Panel Footer
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
                btnConversor,
                btnSobre,
                lblFooter
            });

            // Adicionar todos os controles ao formulário
            this.Controls.AddRange(new Control[] {
                panelHeader,
                groupBoxUpload,
                panelProgresso,
                panelFooter
            });
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
                    
                    if (btnProcessar != null)
                        btnProcessar.Enabled = true;
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
                
                if (btnProcessar != null)
                    btnProcessar.Enabled = false;
                return;
            }

            // Preparar UI para processamento
            if (btnSelecionarArquivo != null)
                btnSelecionarArquivo.Enabled = false;
            
            if (btnProcessar != null)
                btnProcessar.Enabled = false;
            
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
            
            // Iniciar processamento em background
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
                
                // Validar colunas
                var colunasValidas = csvReader.ValidarColunas(arquivoPath!, out var colunasFaltantes);
                
                if (!colunasValidas)
                {
                    var msg = $"Colunas faltantes: {string.Join(", ", colunasFaltantes)}";
                    throw new Exception(msg);
                }
                
                worker?.ReportProgress(20, "📖 Lendo arquivo CSV...");
                
                // Ler CSV
                var registros = csvReader.LerCsv(arquivoPath!);
                var totalRegistros = registros.Count;
                
                worker?.ReportProgress(30, $"✅ {totalRegistros} registros encontrados");
                
                // Criar diretório temporário
                var estacao = registros.FirstOrDefault()?.EstacaoAbastecedora ?? "DESCONHECIDA";
                var diretorioPrincipal = Path.Combine(Path.GetTempPath(), 
                    $"moradias_xml_{estacao}_{DateTime.Now:yyyyMMddHHmmss}");
                Directory.CreateDirectory(diretorioPrincipal);
                
                var pastasCriadas = new List<string>();
                
                // Contadores
                int contadorSemComplementos = 0;
                int contador1Complemento = 0;
                int contador2Complementos = 0;
                int contador3Complementos = 0;
                
                // Processar cada registro
                for (int i = 0; i < totalRegistros; i++)
                {
                    var registro = registros[i];
                    var percentual = 30 + (i * 60 / totalRegistros);
                    
                    // Determinar tipo de complemento
                    var tipo = xmlGenerator.DeterminarTipoComplemento(
                        registro.Complemento, 
                        registro.Complemento2, 
                        registro.Complemento3);
                    
                    worker?.ReportProgress(percentual, 
                        $"📝 Registro {i + 1}/{totalRegistros} - Survey: {registro.CodSurvey} - Tipo: {tipo} complemento(s)");
                    
                    // Gerar XML
                    var xmlBytes = xmlGenerator.GerarXml(registro, tipo);
                    
                    // Criar pasta para esta moradia
                    var nomePasta = $"moradia{i + 1}";
                    var caminhoPasta = Path.Combine(diretorioPrincipal, nomePasta);
                    Directory.CreateDirectory(caminhoPasta);
                    pastasCriadas.Add(caminhoPasta);
                    
                    // Salvar XML
                    var xmlPath = Path.Combine(caminhoPasta, $"{nomePasta}.xml");
                    File.WriteAllBytes(xmlPath, xmlBytes);
                    
                    // Atualizar contadores
                    switch (tipo)
                    {
                        case 0: contadorSemComplementos++; break;
                        case 1: contador1Complemento++; break;
                        case 2: contador2Complementos++; break;
                        case 3: contador3Complementos++; break;
                    }
                }
                
                worker?.ReportProgress(95, "📦 Criando arquivo ZIP...");
                
                // Criar ZIP
                var culturaBrasil = new System.Globalization.CultureInfo("pt-BR");
                var dataArquivo = DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss", culturaBrasil);
                var zipFilename = $"moradias_xml_{estacao}_{dataArquivo}.zip";
                var zipPath = Path.Combine(Path.GetTempPath(), zipFilename);
                
                System.IO.Compression.ZipFile.CreateFromDirectory(diretorioPrincipal, zipPath);
                
                // Limpar diretório temporário
                Directory.Delete(diretorioPrincipal, true);
                
                // Mover para pasta de Downloads do usuário
                var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                var destinoZip = Path.Combine(downloadsPath, zipFilename);
                File.Copy(zipPath, destinoZip, true);
                File.Delete(zipPath);
                
                // Formatar data em português do Brasil para o log
                var dataFormatada = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", culturaBrasil);
                
                // USANDO O MÉTODO GERAR TABELA
                var logEstatisticas = GerarTabelaEstatisticas(
                    contador3Complementos,
                    contador2Complementos,
                    contador1Complemento,
                    contadorSemComplementos,
                    totalRegistros
                );
                
                // Adicionar informações extras
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

        // MÉTODO GERAR TABELA (coloque dentro da mesma classe) 
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
                    
                    // ⭐⭐⭐ LIMPAR TUDO APÓS O PROCESSAMENTO ⭐⭐⭐
                    
                    // 1. Limpar o TextBox do caminho do arquivo
                    if (txtCaminhoArquivo != null)
                    {
                        txtCaminhoArquivo.Text = string.Empty;
                    }
                    
                    // 2. Limpar o Label de informação do arquivo
                    if (lblInfoArquivo != null)
                    {
                        lblInfoArquivo.Text = "Nenhum arquivo selecionado";
                        lblInfoArquivo.ForeColor = SystemColors.ControlText; // Cor padrão
                    }
                    
                    // 3. Limpar a variável global do caminho
                    arquivoSelecionadoPath = null;
                    
                    // 4. Desabilitar o botão processar até selecionar novo arquivo
                    if (btnProcessar != null)
                        btnProcessar.Enabled = false;
                    
                    // Mostrar resultado com opção de abrir pasta
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
                    
                    MessageBox.Show(
                        $"Erro ao processar o arquivo:\n\n{resultado.Erro}",
                        "Erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            
            // Reabilitar controles
            if (btnSelecionarArquivo != null)
                btnSelecionarArquivo.Enabled = true;
            
            if (btnConversor != null)
                btnConversor.Enabled = true;
            
            // Esconder progresso após 2 segundos
            var timer = new System.Windows.Forms.Timer { Interval = 2000 };
            timer.Tick += (s, args) => 
            {
                if (panelProgresso != null)
                    panelProgresso.Visible = false;
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        } 
      
    
        private void AbrirPastaComArquivoSelecionado(string caminhoArquivo)
        {
            try
            {
                // Verificar se o arquivo existe
                if (File.Exists(caminhoArquivo))
                {
                    // Comando para abrir o Explorer e selecionar o arquivo
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
                // Número formatado para WhatsApp (sem espaços, sem +)
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
    }

    public class ProcessamentoResultadoSimulado
    {
        public bool Sucesso { get; set; }
        public string? Erro { get; set; }
        public string? ArquivoGerado { get; set; }
        public string? ArquivoGeradoCompleto { get; set; }  // ← Adicione esta propriedade
        public int TotalRegistros { get; set; }
        public string? Log { get; set; }
    }
}