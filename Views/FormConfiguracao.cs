// Views/FormConfiguracao.cs
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GeradorXML.Models;
using GeradorXML.Services;

namespace GeradorXML.Views
{
    public partial class FormConfiguracao : Form
    {
        private readonly ConfiguracaoService _configService;
        private Configuracao _configuracao;
        
        private TextBox? txtRoteiro;
        private Label? lblStatus;
        private Label? lblCaminhoConfig;
        
        public FormConfiguracao()
        {
            _configService = new ConfiguracaoService();
            _configuracao = _configService.CarregarConfiguracao();
            InitializeComponent();
            CarregarDados();
        }
        
        private void InitializeComponent()
        {
            this.Text = "⚙️ Configuração do Roteiro XML";
            this.Size = new Size(650, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(236, 240, 241);
            
            // Título
            var lblTitulo = new Label
            {
                Text = "Configuração do Roteiro XML",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(600, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            
            // Descrição
            var lblDescricao = new Label
            {
                Text = "Configure o arquivo XML de roteiro para o processamento dos endereços",
                Font = new Font("Segoe UI", 9),
                Location = new Point(20, 55),
                Size = new Size(600, 25),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray
            };
            
            // Informação do local de salvamento
            var configService = new ConfiguracaoService();
            var caminhoConfig = configService.GetRoteirosPath();
            
            lblCaminhoConfig = new Label
            {
                Text = $"📁 Arquivos salvos em: {caminhoConfig}",
                Font = new Font("Segoe UI", 8),
                Location = new Point(20, 85),
                Size = new Size(600, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(52, 152, 219)
            };
            
            // Panel Roteiro
            var groupRoteiro = new GroupBox
            {
                Text = "📄 Arquivo de Roteiro (XML)",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 120),
                Size = new Size(600, 100),
                BackColor = Color.White
            };
            
            txtRoteiro = new TextBox
            {
                Location = new Point(15, 35),
                Size = new Size(420, 25),
                ReadOnly = true,
                BackColor = Color.FromArgb(245, 245, 245)
            };
            
            var btnSelecionar = new Button
            {
                Text = "📁 Selecionar XML",
                Location = new Point(445, 33),
                Size = new Size(140, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSelecionar.Click += BtnSelecionar_Click!;
            
            lblStatus = new Label
            {
                Text = "Nenhum arquivo XML selecionado",
                Location = new Point(15, 70),
                Size = new Size(570, 20),
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 8)
            };
            
            groupRoteiro.Controls.AddRange(new Control[] {
                txtRoteiro, btnSelecionar, lblStatus
            });
            
            // Botão Salvar
            var btnSalvar = new Button
            {
                Text = "💾 Salvar Configuração",
                Location = new Point(150, 250),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSalvar.Click += BtnSalvar_Click!;
            
            // Botão Limpar
            var btnLimpar = new Button
            {
                Text = "🗑️ Limpar",
                Location = new Point(310, 250),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLimpar.Click += BtnLimpar_Click!;
            
            // Botão Abrir Pasta
            var btnAbrirPasta = new Button
            {
                Text = "📂 Abrir Pasta",
                Location = new Point(420, 250),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAbrirPasta.Click += BtnAbrirPasta_Click!;
            
            // Botão Fechar
            var btnFechar = new Button
            {
                Text = "Fechar",
                Location = new Point(530, 250),
                Size = new Size(80, 40),
                FlatStyle = FlatStyle.Flat
            };
            btnFechar.Click += (s, e) => this.Close();
            
            // Informações adicionais
            var lblInfo = new Label
            {
                Text = "ℹ️ O arquivo XML é salvo na pasta .gerador_de_xml_netwin no seu perfil do Windows",
                Location = new Point(20, 310),
                Size = new Size(600, 25),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter
            };
            
            Controls.AddRange(new Control[] {
                lblTitulo, lblDescricao, lblCaminhoConfig,
                groupRoteiro, 
                btnSalvar, btnLimpar, btnAbrirPasta, btnFechar, lblInfo
            });
        }
        
        private void CarregarDados()
        {
            if (!string.IsNullOrEmpty(_configuracao.RoteiroPath) && File.Exists(_configuracao.RoteiroPath))
            {
                txtRoteiro!.Text = _configuracao.RoteiroPath;
                lblStatus!.Text = $"✅ Arquivo: {Path.GetFileName(_configuracao.RoteiroPath)}";
                lblStatus.ForeColor = Color.Green;
            }
        }
        
        private void BtnSelecionar_Click(object? sender, EventArgs e)
        {
            using var openDialog = new OpenFileDialog();
            openDialog.Filter = "Arquivos XML (*.xml)|*.xml|Todos os arquivos (*.*)|*.*";
            openDialog.Title = "Selecione o arquivo XML de roteiro";
            
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                var arquivoPath = openDialog.FileName;
                
                // Validar se é XML
                if (Path.GetExtension(arquivoPath).ToLower() != ".xml")
                {
                    MessageBox.Show("Por favor, selecione um arquivo XML válido!", 
                        "Formato inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Salvar na pasta de configuração
                var nomeArquivo = "roteiro.xml";
                var destinoPath = _configService.SalvarArquivoRoteiro(arquivoPath, nomeArquivo);
                
                txtRoteiro!.Text = destinoPath;
                lblStatus!.Text = $"✅ Arquivo: {nomeArquivo}";
                lblStatus.ForeColor = Color.Green;
                
                // Atualizar configuração
                _configuracao.RoteiroPath = destinoPath;
            }
        }
        
        private void BtnSalvar_Click(object? sender, EventArgs e)
        {
            _configService.SalvarConfiguracao(_configuracao);
            MessageBox.Show("Configuração salva com sucesso!", "Sucesso", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
        
        private void BtnLimpar_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show("Deseja limpar a configuração do roteiro?", 
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                _configuracao = new Configuracao();
                txtRoteiro!.Text = "";
                lblStatus!.Text = "Nenhum arquivo XML selecionado";
                lblStatus.ForeColor = Color.Gray;
                
                _configService.SalvarConfiguracao(_configuracao);
                MessageBox.Show("Configuração limpa!", "Sucesso", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        private void BtnAbrirPasta_Click(object? sender, EventArgs e)
        {
            var pasta = _configService.GetRoteirosPath();
            if (Directory.Exists(pasta))
            {
                System.Diagnostics.Process.Start("explorer.exe", pasta);
            }
        }
    }
}