// Views/FormUpdate.cs
using System;
using System.Drawing;
using System.Windows.Forms;
using GeradorXML.Models;
using GeradorXML.Services;

namespace GeradorXML.Views
{
    public partial class FormUpdate : Form
    {
        private Label lblTitle = null!;
        private Label lblCurrentVersion = null!;
        private Label lblNewVersion = null!;
        private Label lblDescription = null!;
        private Label lblSize = null!;
        private RichTextBox txtChangelog = null!;
        private Button btnUpdate = null!;
        private Button btnLater = null!;
        private Button btnIgnore = null!;
        private ProgressBar progressBar = null!;
        private Label lblProgress = null!;
        
        private readonly UpdateInfo _updateInfo;
        private readonly IUpdateService _updateService;
        private bool _isDownloading = false;
        
        public FormUpdate(UpdateInfo updateInfo, IUpdateService updateService)
        {
            _updateInfo = updateInfo;
            _updateService = updateService;
            InitializeComponent();
            LoadUpdateInfo();
            
            _updateService.OnDownloadProgress += OnDownloadProgress;
            _updateService.OnError += OnError;
        }
        
        private void InitializeComponent()
        {
            this.Text = "🔄 Atualização Disponível";
            this.Size = new Size(600, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(236, 240, 241);
            
            // Título
            lblTitle = new Label
            {
                Text = "🔄 Nova Versão Disponível!",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(20, 20),
                Size = new Size(560, 45),
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            // Separador
            var separator = new Label
            {
                Location = new Point(20, 70),
                Size = new Size(560, 2),
                BackColor = Color.FromArgb(189, 195, 199)
            };
            
            // Versão Atual
            lblCurrentVersion = new Label
            {
                Location = new Point(20, 85),
                Size = new Size(560, 30),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            
            // Nova Versão
            lblNewVersion = new Label
            {
                Location = new Point(20, 115),
                Size = new Size(560, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113)
            };
            
            // Data de Publicação
            var lblDate = new Label
            {
                Location = new Point(20, 145),
                Size = new Size(560, 25),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray
            };
            
            // Tamanho
            lblSize = new Label
            {
                Location = new Point(20, 170),
                Size = new Size(560, 25),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray
            };
            
            // Descrição
            lblDescription = new Label
            {
                Text = "📝 Novidades desta versão:",
                Location = new Point(20, 205),
                Size = new Size(560, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            
            // Changelog
            txtChangelog = new RichTextBox
            {
                Location = new Point(20, 235),
                Size = new Size(560, 180),
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 9.5f),
                BackColor = Color.White,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };
            
            // Progresso
            progressBar = new ProgressBar
            {
                Location = new Point(20, 235),
                Size = new Size(560, 30),
                Visible = false,
                Style = ProgressBarStyle.Continuous,
                ForeColor = Color.FromArgb(46, 204, 113)
            };
            
            lblProgress = new Label
            {
                Location = new Point(20, 270),
                Size = new Size(560, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(52, 73, 94),
                Visible = false
            };
            
            // Botões
            btnUpdate = new Button
            {
                Text = "📥 Atualizar Agora",
                Size = new Size(150, 40),
                Location = new Point(20, 460),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnUpdate.FlatAppearance.BorderSize = 0;
            btnUpdate.Click += BtnUpdate_Click!;
            
            btnLater = new Button
            {
                Text = "⏰ Lembrar Depois",
                Size = new Size(150, 40),
                Location = new Point(180, 460),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10)
            };
            btnLater.FlatAppearance.BorderSize = 0;
            btnLater.Click += (s, e) => this.Close();
            
            btnIgnore = new Button
            {
                Text = "❌ Ignorar Esta Versão",
                Size = new Size(150, 40),
                Location = new Point(340, 460),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10)
            };
            btnIgnore.FlatAppearance.BorderSize = 0;
            btnIgnore.Click += BtnIgnore_Click!;
            
            // Botão Fechar
            var btnClose = new Button
            {
                Text = "✕ Fechar",
                Size = new Size(80, 40),
                Location = new Point(500, 460),
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(52, 73, 94),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10)
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();
            
            // Adicionar controles
            this.Controls.AddRange(new Control[] {
                lblTitle, separator,
                lblCurrentVersion, lblNewVersion, lblDate, lblSize,
                lblDescription, txtChangelog, progressBar, lblProgress,
                btnUpdate, btnLater, btnIgnore, btnClose
            });
        }
        
        private void LoadUpdateInfo()
        {
            lblCurrentVersion.Text = $"📌 Versão atual: {_updateInfo.VersaoAtual}";
            lblNewVersion.Text = $"✨ Nova versão: {_updateInfo.VersaoNova}";
            lblSize.Text = $"📦 Tamanho: {_updateService.FormatFileSize(_updateInfo.Tamanho)}";
            
            txtChangelog.Text = string.IsNullOrEmpty(_updateInfo.Descricao) 
                ? "• Melhorias de desempenho\n• Correções de bugs\n• Novas funcionalidades" 
                : _updateInfo.Descricao;
        }
        
        private void OnDownloadProgress(object? sender, UpdateProgressEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(() => OnDownloadProgress(sender, e));
                return;
            }
            
            progressBar.Value = Math.Min(e.Percent, 100);
            lblProgress.Text = e.Message;
            btnUpdate.Text = e.Percent < 100 ? "📥 Baixando..." : "✅ Download concluído!";
            
            if (e.Percent >= 100)
            {
                progressBar.ForeColor = Color.FromArgb(46, 204, 113);
            }
        }
        
        private void OnError(object? sender, string error)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(() => OnError(sender, error));
                return;
            }
            
            progressBar.Visible = false;
            lblProgress.Visible = false;
            txtChangelog.Visible = true;
            
            MessageBox.Show($"Erro na atualização:\n{error}", "Erro",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            btnUpdate.Enabled = true;
            btnLater.Enabled = true;
            btnIgnore.Enabled = true;
            btnUpdate.Text = "📥 Tentar Novamente";
            _isDownloading = false;
        }
        
        private async void BtnUpdate_Click(object? sender, EventArgs e)
        {
            if (_isDownloading) return;
            
            _isDownloading = true;
            
            txtChangelog.Visible = false;
            progressBar.Visible = true;
            lblProgress.Visible = true;
            progressBar.Value = 0;
            lblProgress.Text = "Preparando download...";
            
            btnUpdate.Enabled = false;
            btnLater.Enabled = false;
            btnIgnore.Enabled = false;
            
            try
            {
                var success = await _updateService.BaixarAtualizacaoAsync(_updateInfo.DownloadUrl);
                
                if (success)
                {
                    // A aplicação será fechada pelo UpdateService
                }
            }
            catch (Exception ex)
            {
                OnError(this, ex.Message);
            }
        }
        
        private void BtnIgnore_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                $"Tem certeza que deseja ignorar a versão {_updateInfo.VersaoNova}?\n\n" +
                "Você não será notificado sobre esta versão novamente.",
                "Ignorar Atualização",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                _updateService.IgnoreVersion(_updateInfo.VersaoNova);
                this.Close();
            }
        }
        
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            _updateService.OnDownloadProgress -= OnDownloadProgress;
            _updateService.OnError -= OnError;
        }
    }
}