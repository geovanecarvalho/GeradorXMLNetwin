// TestForm.cs
using System;
using System.Windows.Forms;

namespace GeradorXML.Views
{
    public class TestForm : Form
    {
        public TestForm()
        {
            this.Text = "Teste - Formulário Mínimo";
            this.Size = new System.Drawing.Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            var label = new Label
            {
                Text = "Se você está vendo esta mensagem, o Windows Forms está funcionando!",
                AutoSize = true,
                Location = new System.Drawing.Point(50, 50)
            };
            
            var button = new Button
            {
                Text = "Fechar",
                Location = new System.Drawing.Point(150, 150),
                Size = new System.Drawing.Size(100, 40)
            };
            
            button.Click += (s, e) => this.Close();
            
            Controls.Add(label);
            Controls.Add(button);
        }
    }
}