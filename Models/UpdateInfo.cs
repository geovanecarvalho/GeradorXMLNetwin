// Models/UpdateInfo.cs
using System;

namespace GeradorXML.Models
{
    public class UpdateInfo
    {
        public string VersaoAtual { get; set; } = "";
        public string VersaoNova { get; set; } = "";
        public string DownloadUrl { get; set; } = "";
        public long Tamanho { get; set; }
        public string Descricao { get; set; } = "";
        public DateTime DataPublicacao { get; set; }
        public bool IsPrerelease { get; set; }
        public string HashMd5 { get; set; } = "";
        public string NomeArquivo { get; set; } = "";
        public bool IsObrigatoria { get; set; }
        
        public bool IsStable => !IsPrerelease;
    }

    public class UpdateResponse
    {
        public bool HasUpdate { get; set; }
        public UpdateInfo? UpdateInfo { get; set; }
        public string? Error { get; set; }
    }
}