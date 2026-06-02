// FormSobre.cs
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GeradorXML.Views
{
    public partial class FormSobre : Form
    {
        public FormSobre()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Sobre o Sistema";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Ícone (placeholders - você pode adicionar imagens depois)
            var pictureBoxIcon = new PictureBox
            {
                Location = new Point(200, 20),
                Size = new Size(100, 100),
                BackColor = Color.FromArgb(52, 152, 219),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            var lblTitulo = new Label
            {
                Text = "Gerador de XML para Edificações",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(50, 130),
                Size = new Size(400, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblVersao = new Label
            {
                Text = "Versão 1.0.0",
                Font = new Font("Segoe UI", 9),
                Location = new Point(50, 165),
                Size = new Size(400, 25),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray
            };

            var groupBoxInfo = new GroupBox
            {
                Text = "Informações do Sistema",
                Location = new Point(30, 205),
                Size = new Size(440, 120),
                Font = new Font("Segoe UI", 9)
            };

            var lblInfo = new Label
            {
                Text = "Desenvolvido para: Telemont\n\n" +
                       "Funcionalidades:\n" +
                       "• Geração de XML a partir de CSV\n" +
                       "• Suporte a complementos (1, 2 ou 3)\n" +
                       "• Validação de dados\n" +
                       "• Conversão para Power Query",
                Location = new Point(15, 20),
                Size = new Size(410, 90),
                Font = new Font("Consolas", 8)
            };
            groupBoxInfo.Controls.Add(lblInfo);

            var btnOK = new Button
            {
                Text = "OK",
                Location = new Point(200, 340),
                Size = new Size(100, 30),
                DialogResult = DialogResult.OK,
                FlatStyle = FlatStyle.Flat
            };

            Controls.AddRange(new Control[] { 
                pictureBoxIcon, lblTitulo, lblVersao, groupBoxInfo, btnOK 
            });
        }
    }
}