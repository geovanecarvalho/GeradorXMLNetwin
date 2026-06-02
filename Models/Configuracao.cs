// Models/Configuracao.cs
using System;

namespace GeradorXML.Models
{
    public class Configuracao
    {
        public string RoteiroPath { get; set; } = "";
        public DateTime UltimaAtualizacao { get; set; }
        public string Versao { get; set; } = "1.0.0";
        public string UltimoArquivoProcessado { get; set; } = "";
    }
}