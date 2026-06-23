// Models/Edificio.cs
using System;
using System.Globalization;
using System.Xml.Linq;

namespace GeradorXML.Models
{
    public class Edificio
    {
        // Todos os campos começam com valores vazios ou null
        public string CodSurvey { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string CodigoZona { get; set; } = string.Empty;
        public string Localidade { get; set; } = string.Empty;
        public Endereco Endereco { get; set; } = new Endereco();
        public string TecnicoId { get; set; } = string.Empty;
        public string TecnicoNome { get; set; } = string.Empty;
        public string EmpresaId { get; set; } = string.Empty;
        public string EmpresaNome { get; set; } = string.Empty;
        public int TotalUCs { get; set; }
        public string Ocupacao { get; set; } = string.Empty;
        public int NumPisos { get; set; }
        public string Destinacao { get; set; } = string.Empty;

        public XDocument GerarXml()
        {
            var edificio = new XElement("edificio",
                new XAttribute("tipo", "M"),
                new XAttribute("versao", "7.9.2"),
                new XElement("gravado", "false"),
                new XElement("nEdificio", CodSurvey ?? ""),
                new XElement("coordX", Longitude?.ToString(CultureInfo.InvariantCulture) ?? "0"),
                new XElement("coordY", Latitude?.ToString(CultureInfo.InvariantCulture) ?? "0"),
                new XElement("codigoZona", CodigoZona ?? ""),
                new XElement("nomeZona", CodigoZona ?? ""),
                new XElement("localidade", Localidade ?? ""),
                Endereco?.GerarXml() ?? new XElement("endereco"),
                new XElement("tecnico",
                    new XElement("id", TecnicoId ?? ""),
                    new XElement("nome", TecnicoNome ?? "")),
                new XElement("empresa",
                    new XElement("id", EmpresaId ?? ""),
                    new XElement("nome", EmpresaNome ?? "")),
                new XElement("data", DateTime.Now.ToString("yyyyMMddHHmmss")),
                new XElement("totalUCs", TotalUCs),
                new XElement("ocupacao", Ocupacao ?? ""),
                new XElement("numPisos", NumPisos),
                new XElement("destinacao", Destinacao ?? "")
            );

            return new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                edificio
            );
        }
    }
}