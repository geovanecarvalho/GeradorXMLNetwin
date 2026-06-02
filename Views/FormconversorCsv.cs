// FormConversorCsv.cs - Versão Corrigida
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GeradorXML.Views
{
    public partial class FormConversorCsv : Form
    {
        private OpenFileDialog? openFileDialog;
        private string? arquivoSelecionadoPath;

        public FormConversorCsv()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Conversor CSV para Power Query";
            this.Size = new Size(700, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Título
            var lblTitulo = new Label
            {
                Text = "🔄 Conversor CSV para Power Query",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(50, 20),
                Size = new Size(600, 35),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Descrição
            var lblDescricao = new Label
            {
                Text = "Converta arquivos 'Enderecos_Totais_CO.csv' para o formato compatível com Power Query",
                Font = new Font("Segoe UI", 9),
                Location = new Point(50, 60),
                Size = new Size(600, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray
            };

            // GroupBox de upload
            var groupBoxUpload = new GroupBox
            {
                Text = "Selecionar Arquivo",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(50, 110),
                Size = new Size(600, 150)
            };

            var btnSelecionar = new Button
            {
                Text = "📁 Selecionar CSV",
                Location = new Point(20, 35),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSelecionar.Click += BtnSelecionar_Click!;

            var txtCaminho = new TextBox
            {
                Location = new Point(185, 42),
                Size = new Size(390, 23),
                ReadOnly = true
            };

            var lblInfo = new Label
            {
                Text = "Nenhum arquivo selecionado",
                Location = new Point(185, 75),
                Size = new Size(390, 20),
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 8)
            };

            var btnConverter = new Button
            {
                Text = "🔄 Converter Arquivo",
                Location = new Point(200, 105),
                Size = new Size(200, 35),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Name = "btnConverter"
            };
            btnConverter.Click += BtnConverter_Click!;

            groupBoxUpload.Controls.AddRange(new Control[] { 
                btnSelecionar, txtCaminho, lblInfo, btnConverter 
            });

            // Informações
            var groupBoxInfo = new GroupBox
            {
                Text = "Informações",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(50, 280),
                Size = new Size(600, 120)
            };

            var lblInfoTexto = new Label
            {
                Text = "• Formato de entrada: CSV com separador '|'\n" +
                       "• Formato de saída: CSV com separador ';' (UTF-8 com BOM)\n" +
                       "• Adiciona colunas: CHAVE LOG, RESULTADO, VALIDAÇÃO, etc.\n" +
                       "• Compatível com Power Query",
                Location = new Point(15, 30),
                Size = new Size(570, 80),
                Font = new Font("Consolas", 9)
            };
            groupBoxInfo.Controls.Add(lblInfoTexto);

            // Botão fechar
            var btnFechar = new Button
            {
                Text = "Fechar",
                Location = new Point(300, 415),
                Size = new Size(100, 30),
                FlatStyle = FlatStyle.Flat
            };
            btnFechar.Click += (s, e) => this.Close();

            Controls.AddRange(new Control[] { 
                lblTitulo, lblDescricao, groupBoxUpload, groupBoxInfo, btnFechar 
            });
        }

        private void BtnSelecionar_Click(object? sender, EventArgs e)
        {
            using (openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Arquivos CSV (*.csv)|*.csv";
                openFileDialog.Title = "Selecione o arquivo Enderecos_Totais_CO.csv";
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    arquivoSelecionadoPath = openFileDialog.FileName;
                    
                    // Encontrar os controles no GroupBox
                    foreach (Control ctrl in this.Controls)
                    {
                        if (ctrl is GroupBox gb && gb.Text == "Selecionar Arquivo")
                        {
                            foreach (Control subCtrl in gb.Controls)
                            {
                                if (subCtrl is TextBox txt)
                                {
                                    txt.Text = arquivoSelecionadoPath;
                                }
                                else if (subCtrl is Label lbl && lbl.Text == "Nenhum arquivo selecionado")
                                {
                                    lbl.Text = $"Arquivo selecionado: {Path.GetFileName(arquivoSelecionadoPath)}";
                                    lbl.ForeColor = Color.Green;
                                }
                                else if (subCtrl is Button btn && btn.Name == "btnConverter")
                                {
                                    btn.Enabled = true;
                                }
                            }
                            break;
                        }
                    }
                }
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

            MessageBox.Show(
                "Funcionalidade em desenvolvimento.\n\n" +
                "Em breve será implementada a conversão completa do CSV.\n\n" +
                $"Arquivo selecionado: {Path.GetFileName(arquivoSelecionadoPath)}",
                "Em Desenvolvimento",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}