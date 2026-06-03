// FormConversorCsv.cs - Versão Completa com validações e nome no formato BR
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GeradorXML.Views
{
    public partial class FormConversorCsv : Form
    {
        private OpenFileDialog? openFileDialog;
        private string? arquivoSelecionadoPath;
        private TextBox? txtCaminho;
        private Label? lblInfo;
        private Button? btnConverter;
        private ProgressBar? progressBar;
        private Label? lblProgressoPercentual;
        private Label? lblStatus;
        private DataGridView? dgvPreview;
        private string? ultimoArquivoGerado;

        public FormConversorCsv()
        {
            InitializeComponent();
            // Estado inicial do botão
            AtualizarEstadoBotao("aguardando");
        }

        private void InitializeComponent()
        {
            this.Text = "Conversor CSV para Power Query";
            this.Size = new Size(950, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(236, 240, 241);

            // Título
            var lblTitulo = new Label
            {
                Text = "🔄 Conversor CSV para Power Query",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(50, 20),
                Size = new Size(850, 35),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Descrição
            var lblDescricao = new Label
            {
                Text = "Converte CSV com separador '|' para formato compatível com Power Query (separador ';' UTF-8)",
                Font = new Font("Segoe UI", 9),
                Location = new Point(50, 55),
                Size = new Size(850, 25),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray
            };

            // GroupBox de upload
            var groupBoxUpload = new GroupBox
            {
                Text = "Configurações",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 90),
                Size = new Size(910, 130),
                BackColor = Color.White
            };

            var btnSelecionar = new Button
            {
                Text = "📁 Selecionar CSV",
                Location = new Point(15, 30),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSelecionar.Click += BtnSelecionar_Click!;

            txtCaminho = new TextBox
            {
                Location = new Point(180, 37),
                Size = new Size(710, 23),
                ReadOnly = true,
                BackColor = Color.FromArgb(245, 245, 245)
            };

            lblInfo = new Label
            {
                Text = "Nenhum arquivo selecionado",
                Location = new Point(180, 70),
                Size = new Size(710, 20),
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 8)
            };

            btnConverter = new Button
            {
                Text = "⏳ Aguardando CSV...",
                Location = new Point(300, 95),
                Size = new Size(300, 35),
                BackColor = Color.FromArgb(211, 211, 211), // Cinza claro opaco
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnConverter.Click += BtnConverter_Click!;

            groupBoxUpload.Controls.AddRange(new Control[] { 
                btnSelecionar, txtCaminho, lblInfo, btnConverter
            });

            // GroupBox Preview
            var groupBoxPreview = new GroupBox
            {
                Text = "Pré-visualização dos dados processados",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 230),
                Size = new Size(910, 420),
                BackColor = Color.White
            };

            dgvPreview = new DataGridView
            {
                Location = new Point(10, 25),
                Size = new Size(890, 310),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            // Barra de Progresso
            progressBar = new ProgressBar
            {
                Location = new Point(10, 350),
                Size = new Size(800, 25),
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                Visible = false
            };

            // Label de porcentagem
            lblProgressoPercentual = new Label
            {
                Text = "0%",
                Location = new Point(820, 350),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.Blue,
                Visible = false
            };

            // Status
            lblStatus = new Label
            {
                Text = "Aguardando seleção do arquivo...",
                Location = new Point(10, 390),
                Size = new Size(890, 25),
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.Gray
            };

            groupBoxPreview.Controls.AddRange(new Control[] { 
                dgvPreview, progressBar, lblProgressoPercentual, lblStatus 
            });

            // Botão Abrir Pasta
            var btnAbrirPasta = new Button
            {
                Text = "📂 Abrir Pasta do Último Arquivo",
                Location = new Point(20, 660),
                Size = new Size(180, 30),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9)
            };
            btnAbrirPasta.Click += BtnAbrirPasta_Click!;

            // Botão Abrir Arquivo
            var btnAbrirArquivo = new Button
            {
                Text = "📄 Abrir Último Arquivo",
                Location = new Point(210, 660),
                Size = new Size(180, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9)
            };
            btnAbrirArquivo.Click += BtnAbrirArquivo_Click!;

            // Botão fechar
            var btnFechar = new Button
            {
                Text = "Fechar",
                Location = new Point(850, 660),
                Size = new Size(80, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnFechar.Click += (s, e) => this.Close();

            Controls.AddRange(new Control[] { 
                lblTitulo, lblDescricao, groupBoxUpload, groupBoxPreview, 
                btnAbrirPasta, btnAbrirArquivo, btnFechar 
            });
        }

        private void AtualizarEstadoBotao(string estado)
        {
            if (btnConverter == null) return;
            
            switch (estado)
            {
                case "aguardando":
                    btnConverter.Text = "⏳ Aguardando CSV...";
                    btnConverter.BackColor = Color.FromArgb(211, 211, 211); // Cinza claro
                    btnConverter.Enabled = false;
                    break;
                    
                case "pronto":
                    btnConverter.Text = "🔄 Converter CSV";
                    btnConverter.BackColor = Color.FromArgb(46, 204, 113); // Verde
                    btnConverter.Enabled = true;
                    break;
                    
                case "processando":
                    btnConverter.Text = "⏳ Processando...";
                    btnConverter.BackColor = Color.FromArgb(211, 211, 211); // Cinza claro
                    btnConverter.Enabled = false;
                    break;
            }
        }

        private void BtnAbrirPasta_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ultimoArquivoGerado) && File.Exists(ultimoArquivoGerado))
            {
                string pasta = Path.GetDirectoryName(ultimoArquivoGerado)!;
                AbrirPastaSelecionandoArquivo(ultimoArquivoGerado);
            }
            else
            {
                MessageBox.Show("Nenhum arquivo foi gerado ainda ou o arquivo não existe mais.", 
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnAbrirArquivo_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ultimoArquivoGerado) && File.Exists(ultimoArquivoGerado))
            {
                AbrirArquivo(ultimoArquivoGerado);
            }
            else
            {
                MessageBox.Show("Nenhum arquivo foi gerado ainda ou o arquivo não existe mais.", 
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AbrirPastaSelecionandoArquivo(string caminhoArquivo)
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
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir o Explorer: {ex.Message}", 
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AbrirArquivo(string caminhoArquivo)
        {
            try
            {
                if (File.Exists(caminhoArquivo))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = caminhoArquivo,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show($"Arquivo não encontrado: {caminhoArquivo}", 
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir o arquivo: {ex.Message}", 
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSelecionar_Click(object? sender, EventArgs e)
        {
            using (openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Arquivos CSV (*.csv)|*.csv|Arquivos TXT (*.txt)|*.txt|Todos os arquivos (*.*)|*.*";
                openFileDialog.Title = "Selecione o arquivo CSV";
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    arquivoSelecionadoPath = openFileDialog.FileName;
                    
                    if (txtCaminho != null)
                        txtCaminho.Text = arquivoSelecionadoPath;
                    
                    if (lblInfo != null)
                    {
                        lblInfo.Text = $"Arquivo selecionado: {Path.GetFileName(arquivoSelecionadoPath)}";
                        lblInfo.ForeColor = Color.Green;
                    }
                    
                    // Mudar estado do botão para "pronto"
                    AtualizarEstadoBotao("pronto");
                    
                    CarregarPreview();
                }
            }
        }

        private void CarregarPreview()
        {
            try
            {
                if (string.IsNullOrEmpty(arquivoSelecionadoPath)) return;
                
                AtualizarStatus("Carregando pré-visualização...", Color.Blue);
                
                var dtPreview = ProcessarArquivo(arquivoSelecionadoPath, true);
                
                dgvPreview!.DataSource = dtPreview;
                dgvPreview.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                
                AtualizarStatus($"Pré-visualização: {dtPreview.Rows.Count} registros", Color.Green);
            }
            catch (Exception ex)
            {
                AtualizarStatus($"Erro no preview: {ex.Message}", Color.Red);
            }
        }

        private void BtnConverter_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(arquivoSelecionadoPath))
            {
                MessageBox.Show("Por favor, selecione um arquivo CSV primeiro.", 
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Mudar estado do botão para "processando"
                AtualizarEstadoBotao("processando");
                
                // Mostrar barra de progresso
                progressBar!.Visible = true;
                progressBar.Value = 0;
                lblProgressoPercentual!.Visible = true;
                lblProgressoPercentual.Text = "0%";
                
                AtualizarStatus("Iniciando conversão...", Color.Blue);
                Application.DoEvents();
                
                // Processar arquivo completo com progresso
                var dtCompleto = ProcessarArquivoComProgresso(arquivoSelecionadoPath);
                
                AtualizarProgresso(50, "Salvando arquivo processado...");
                Application.DoEvents();
                
                // Salvar arquivo
                ultimoArquivoGerado = SalvarArquivoProcessado(dtCompleto);
                
                AtualizarProgresso(100, "Conversão concluída!");
                Application.DoEvents();
                
                // Esconder barra de progresso
                progressBar.Visible = false;
                lblProgressoPercentual.Visible = false;
                
                // Voltar estado do botão para "aguardando" (limpa após conversão)
                AtualizarEstadoBotao("aguardando");
                
                // Limpar campos após conversão bem-sucedida
                LimparCampos();
                
                // Habilitar botões de acesso aos arquivos
                HabilitarBotoesAcesso(true);
                
                // Perguntar se quer abrir a pasta com o arquivo selecionado
                var resultado = MessageBox.Show(
                    $"Arquivo convertido com sucesso!\n\n" +
                    $"Arquivo gerado: {Path.GetFileName(ultimoArquivoGerado)}\n" +
                    $"Total de registros: {dtCompleto.Rows.Count:N0}\n" +
                    $"Local: {Path.GetDirectoryName(ultimoArquivoGerado)}\n\n" +
                    $"Deseja abrir a pasta com o arquivo selecionado?",
                    "Conversão Concluída",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);
                
                if (resultado == DialogResult.Yes)
                {
                    AbrirPastaSelecionandoArquivo(ultimoArquivoGerado);
                }
                
                AtualizarStatus("Conversão concluída com sucesso!", Color.Green);
            }
            catch (Exception ex)
            {
                progressBar!.Visible = false;
                lblProgressoPercentual!.Visible = false;
                AtualizarEstadoBotao("aguardando");
                AtualizarStatus($"Erro: {ex.Message}", Color.Red);
                
                MessageBox.Show($"Erro ao converter arquivo:\n\n{ex.Message}", 
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimparCampos()
        {
            // Limpar o TextBox do caminho do arquivo
            if (txtCaminho != null)
                txtCaminho.Text = string.Empty;
            
            // Limpar o Label de informação do arquivo
            if (lblInfo != null)
            {
                lblInfo.Text = "Nenhum arquivo selecionado";
                lblInfo.ForeColor = Color.Gray;
            }
            
            // Limpar a variável global do caminho
            arquivoSelecionadoPath = null;
            
            // Limpar o DataGridView
            if (dgvPreview != null)
                dgvPreview.DataSource = null;
        }

        private void HabilitarBotoesAcesso(bool habilitar)
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button btn && (btn.Text.Contains("Abrir Pasta") || btn.Text.Contains("Abrir Último")))
                {
                    btn.Enabled = habilitar;
                }
            }
        }

        private DataTable ProcessarArquivoComProgresso(string caminho)
        {
            var linhas = File.ReadAllLines(caminho, Encoding.GetEncoding(1252));
            
            if (linhas.Length == 0)
                throw new Exception("Arquivo vazio");
            
            var colunasFinais = new string[]
            {
                "CHAVE LOG", "CELULA", "ESTACAO_ABASTECEDORA", "UF", "MUNICIPIO",
                "LOCALIDADE", "COD_LOCALIDADE", "LOCALIDADE_ABREV", "LOGRADOURO",
                "COD_LOGRADOURO", "NUM_FACHADA", "COMPLEMENTO", "COMPLEMENTO2",
                "COMPLEMENTO3", "CEP", "BAIRRO", "COD_SURVEY", "QUANTIDADE_UMS",
                "COD_VIABILIDADE", "TIPO_VIABILIDADE", "TIPO_REDE", "UCS_RESIDENCIAIS",
                "UCS_COMERCIAIS", "NOME_CDO", "ID_ENDERECO", "LATITUDE", "LONGITUDE",
                "TIPO_SURVEY", "REDE_INTERNA", "UMS_CERTIFICADAS", "REDE_EDIF_CERT",
                "DISP_COMERCIAL", "ESTADO_CONTROLE", "DATA_ESTADO_CONTROLE", "ID_CELULA",
                "QUANTIDADE_HCS", "ID_ROTEIRO", "ID_LOCALIDADE", "COD_ZONA", "ORDEM",
                "RESULTADO", "COMPARATIVO", "Nº ARGUMENTO3 COMPLEMENTO3", "VALIDAÇÃO"
            };
            
            var dt = new DataTable();
            foreach (var coluna in colunasFinais)
            {
                dt.Columns.Add(coluna, typeof(string));
            }
            
            int totalLinhas = linhas.Length - 1;
            int linhasProcessadas = 0;
            
            for (int i = 1; i < linhas.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(linhas[i]))
                    continue;
                
                var campos = linhas[i].Split('|');
                var row = dt.NewRow();
                
                if (campos.Length > 0) row["CELULA"] = GetCampo(campos, 0);
                if (campos.Length > 1) row["ESTACAO_ABASTECEDORA"] = GetCampo(campos, 1);
                if (campos.Length > 2) row["UF"] = GetCampo(campos, 2);
                if (campos.Length > 3) row["MUNICIPIO"] = GetCampo(campos, 3);
                if (campos.Length > 4) row["LOCALIDADE"] = GetCampo(campos, 4);
                if (campos.Length > 5) row["COD_LOCALIDADE"] = GetCampo(campos, 5);
                if (campos.Length > 6) row["LOCALIDADE_ABREV"] = GetCampo(campos, 6);
                if (campos.Length > 7) row["LOGRADOURO"] = GetCampo(campos, 7);
                if (campos.Length > 8) row["COD_LOGRADOURO"] = GetCampo(campos, 8);
                if (campos.Length > 9) row["NUM_FACHADA"] = GetCampo(campos, 9);
                if (campos.Length > 10) row["COMPLEMENTO"] = GetCampo(campos, 10);
                if (campos.Length > 11) row["COMPLEMENTO2"] = GetCampo(campos, 11);
                if (campos.Length > 12) row["COMPLEMENTO3"] = GetCampo(campos, 12);
                if (campos.Length > 13) row["CEP"] = GetCampo(campos, 13);
                if (campos.Length > 14) row["BAIRRO"] = GetCampo(campos, 14);
                if (campos.Length > 15) row["COD_SURVEY"] = GetCampo(campos, 15);
                
                ProcessarRow(row);
                dt.Rows.Add(row);
                
                linhasProcessadas++;
                
                if (linhasProcessadas % 100 == 0 || linhasProcessadas * 100 / totalLinhas > (linhasProcessadas - 100) * 100 / totalLinhas)
                {
                    int percentual = (linhasProcessadas * 100) / totalLinhas;
                    AtualizarProgresso(percentual, $"Processando registros... {linhasProcessadas:N0} de {totalLinhas:N0}");
                    Application.DoEvents();
                }
            }
            
            return dt;
        }

        private DataTable ProcessarArquivo(string caminho, bool apenasPreview)
        {
            int limiteLinhas = apenasPreview ? 50 : int.MaxValue;
            var linhas = File.ReadAllLines(caminho, Encoding.GetEncoding(1252));
            
            if (linhas.Length == 0)
                throw new Exception("Arquivo vazio");
            
            var colunasFinais = new string[]
            {
                "CHAVE LOG", "CELULA", "ESTACAO_ABASTECEDORA", "UF", "MUNICIPIO",
                "LOCALIDADE", "COD_LOCALIDADE", "LOCALIDADE_ABREV", "LOGRADOURO",
                "COD_LOGRADOURO", "NUM_FACHADA", "COMPLEMENTO", "COMPLEMENTO2",
                "COMPLEMENTO3", "CEP", "BAIRRO", "COD_SURVEY", "QUANTIDADE_UMS",
                "COD_VIABILIDADE", "TIPO_VIABILIDADE", "TIPO_REDE", "UCS_RESIDENCIAIS",
                "UCS_COMERCIAIS", "NOME_CDO", "ID_ENDERECO", "LATITUDE", "LONGITUDE",
                "TIPO_SURVEY", "REDE_INTERNA", "UMS_CERTIFICADAS", "REDE_EDIF_CERT",
                "DISP_COMERCIAL", "ESTADO_CONTROLE", "DATA_ESTADO_CONTROLE", "ID_CELULA",
                "QUANTIDADE_HCS", "ID_ROTEIRO", "ID_LOCALIDADE", "COD_ZONA", "ORDEM",
                "RESULTADO", "COMPARATIVO", "Nº ARGUMENTO3 COMPLEMENTO3", "VALIDAÇÃO"
            };
            
            var dt = new DataTable();
            foreach (var coluna in colunasFinais)
            {
                dt.Columns.Add(coluna, typeof(string));
            }
            
            int linhasProcessadas = 0;
            for (int i = 1; i < linhas.Length && linhasProcessadas < limiteLinhas; i++)
            {
                if (string.IsNullOrWhiteSpace(linhas[i]))
                    continue;
                
                var campos = linhas[i].Split('|');
                var row = dt.NewRow();
                
                if (campos.Length > 0) row["CELULA"] = GetCampo(campos, 0);
                if (campos.Length > 1) row["ESTACAO_ABASTECEDORA"] = GetCampo(campos, 1);
                if (campos.Length > 2) row["UF"] = GetCampo(campos, 2);
                if (campos.Length > 3) row["MUNICIPIO"] = GetCampo(campos, 3);
                if (campos.Length > 4) row["LOCALIDADE"] = GetCampo(campos, 4);
                if (campos.Length > 5) row["COD_LOCALIDADE"] = GetCampo(campos, 5);
                if (campos.Length > 6) row["LOCALIDADE_ABREV"] = GetCampo(campos, 6);
                if (campos.Length > 7) row["LOGRADOURO"] = GetCampo(campos, 7);
                if (campos.Length > 8) row["COD_LOGRADOURO"] = GetCampo(campos, 8);
                if (campos.Length > 9) row["NUM_FACHADA"] = GetCampo(campos, 9);
                if (campos.Length > 10) row["COMPLEMENTO"] = GetCampo(campos, 10);
                if (campos.Length > 11) row["COMPLEMENTO2"] = GetCampo(campos, 11);
                if (campos.Length > 12) row["COMPLEMENTO3"] = GetCampo(campos, 12);
                if (campos.Length > 13) row["CEP"] = GetCampo(campos, 13);
                if (campos.Length > 14) row["BAIRRO"] = GetCampo(campos, 14);
                if (campos.Length > 15) row["COD_SURVEY"] = GetCampo(campos, 15);
                
                ProcessarRow(row);
                dt.Rows.Add(row);
                linhasProcessadas++;
            }
            
            return dt;
        }
        
        private void AtualizarProgresso(int percentual, string mensagem)
        {
            if (progressBar != null && progressBar.InvokeRequired)
            {
                progressBar.Invoke(new Action(() => AtualizarProgresso(percentual, mensagem)));
                return;
            }
            
            if (progressBar != null)
            {
                progressBar.Value = Math.Min(percentual, 100);
            }
            
            if (lblProgressoPercentual != null)
            {
                lblProgressoPercentual.Text = $"{percentual}%";
            }
            
            if (lblStatus != null)
            {
                lblStatus.Text = mensagem;
                lblStatus.ForeColor = Color.Blue;
            }
        }
        
        private void AtualizarStatus(string mensagem, Color cor)
        {
            if (lblStatus != null && lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new Action(() => AtualizarStatus(mensagem, cor)));
                return;
            }
            
            if (lblStatus != null)
            {
                lblStatus.Text = mensagem;
                lblStatus.ForeColor = cor;
            }
        }
        
        private string GetCampo(string[] campos, int indice)
        {
            return indice < campos.Length ? campos[indice].Trim() : "";
        }
        
        private void ProcessarRow(DataRow row)
        {
            string cep = row["CEP"]?.ToString() ?? "";
            cep = Regex.Replace(cep, @"\D", "");
            if (cep.Length > 8) cep = cep.Substring(0, 8);
            if (!string.IsNullOrEmpty(cep)) cep = cep.PadLeft(8, '0');
            row["CEP"] = cep;
            
            string codLogradouro = row["COD_LOGRADOURO"]?.ToString() ?? "";
            codLogradouro = Regex.Replace(codLogradouro, @"\D", "");
            if (codLogradouro.Length > 10) codLogradouro = codLogradouro.Substring(0, 10);
            row["COD_LOGRADOURO"] = codLogradouro;
            
            string estacao = row["ESTACAO_ABASTECEDORA"]?.ToString() ?? "";
            string localidade = row["LOCALIDADE"]?.ToString() ?? "";
            string logradouro = row["LOGRADOURO"]?.ToString() ?? "";
            string complemento = row["COMPLEMENTO"]?.ToString() ?? "";
            string complemento2 = row["COMPLEMENTO2"]?.ToString() ?? "";
            
            string chaveLog = $"{estacao}-{localidade}-{logradouro}-{complemento}-{complemento2}";
            chaveLog = Regex.Replace(chaveLog, @"-+", "-").Trim('-');
            row["CHAVE LOG"] = chaveLog;
            
            string complemento3 = row["COMPLEMENTO3"]?.ToString() ?? "";
            string complemento3Tratado = complemento3.Trim().ToUpper();
            string prefixo = complemento3Tratado.Length >= 2 ? complemento3Tratado.Substring(0, 2) : "";
            
            string uf = row["UF"]?.ToString() ?? "";
            string localidadeAbrev = row["LOCALIDADE_ABREV"]?.ToString() ?? "";
            string celula = row["CELULA"]?.ToString() ?? "";
            string numCelula = celula.Split(' ').FirstOrDefault() ?? "";
            
            string codZona = $"{uf}-{localidadeAbrev}-{estacao}-CEOS-{numCelula}";
            row["COD_ZONA"] = codZona;
            
            if (!string.IsNullOrEmpty(prefixo))
            {
                row["ORDEM"] = "1";
                row["RESULTADO"] = prefixo.Replace(" ", "");
                row["COMPARATIVO"] = row["RESULTADO"]?.ToString() == complemento3Tratado ? "VERDADEIRO" : "FALSO";
            }
            else
            {
                row["ORDEM"] = "0";
                row["RESULTADO"] = "";
                row["COMPARATIVO"] = "FALSO";
            }
            
            var match = Regex.Match(complemento3Tratado, @"(\d+)");
            int numero = match.Success ? int.Parse(match.Groups[1].Value) : 0;
            row["Nº ARGUMENTO3 COMPLEMENTO3"] = numero.ToString();
            
            int ordem = int.Parse(row["ORDEM"]?.ToString() ?? "0");
            string validacao;
            if (ordem == 0)
                validacao = "SEM PREFIXO VÁLIDO";
            else if (numero == 0)
                validacao = "VERIFICAR COMPLEMENTO3-VAZIO";
            else if (numero > 10)
                validacao = "VERIFICAR COMPLEMENTO3 >10";
            else if (ordem > 10)
                validacao = "VERIFICAR RESULTADO >10";
            else
                validacao = "OK";
            
            row["VALIDAÇÃO"] = validacao;
        }
        
        private string SalvarArquivoProcessado(DataTable dt)
        {
            // Nome do arquivo no formato: Enderecos_Totais_Co_Convertido_03.06.2026_14.56.10.csv
            string nomeOriginal = "Enderecos_Totais_Co_Convertido";
            
            // Formatar data no padrão brasileiro: dd.MM.yyyy_HH.mm.ss
            string dataHora = DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss");
            string nomeSaida = $"{nomeOriginal}_{dataHora}.csv";
            
            string diretorio = Path.GetDirectoryName(arquivoSelecionadoPath!)!;
            string caminhoSaida = Path.Combine(diretorio, nomeSaida);
            
            using (var writer = new StreamWriter(caminhoSaida, false, new UTF8Encoding(true)))
            {
                var cabecalho = string.Join(";", dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                writer.WriteLine(cabecalho);
                
                int total = dt.Rows.Count;
                int linhaAtual = 0;
                
                foreach (DataRow row in dt.Rows)
                {
                    var valores = new List<string>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        string valor = row[col]?.ToString() ?? "";
                        if (valor.Contains(";") || valor.Contains("\""))
                        {
                            valor = "\"" + valor.Replace("\"", "\"\"") + "\"";
                        }
                        valores.Add(valor);
                    }
                    writer.WriteLine(string.Join(";", valores));
                    
                    linhaAtual++;
                    if (linhaAtual % 500 == 0)
                    {
                        int percentual = (linhaAtual * 100) / total;
                        AtualizarProgresso(50 + (percentual / 2), $"Salvando... {linhaAtual:N0} de {total:N0} registros");
                        Application.DoEvents();
                    }
                }
            }
            
            
            return caminhoSaida;
        }
        
       
    }
}