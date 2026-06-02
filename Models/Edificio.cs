// Models/Edificio.cs
using System;
using System.Globalization;
using System.Xml.Linq;

namespace GeradorXML.Models
{
    public class Edificio
    {
        public string CodSurvey { get; set; } = "";
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string CodigoZona { get; set; } = "DF-GURX-ETGR-CEOS-68";
        public string Localidade { get; set; } = "GUARA";
        public Endereco Endereco { get; set; } = new Endereco();
        public string TecnicoId { get; set; } = "1828772688";
        public string TecnicoNome { get; set; } = "NADIA CAROLINE";
        public string EmpresaId { get; set; } = "42541126";
        public string EmpresaNome { get; set; } = "TELEMONT";
        public int TotalUCs { get; set; } = 1;
        public string Ocupacao { get; set; } = "EDIFICACAOCOMPLETA";
        public int NumPisos { get; set; } = 1;
        public string Destinacao { get; set; } = "COMERCIO";

        public XDocument GerarXml()
        {
            var edificio = new XElement("edificio",
                new XAttribute("tipo", "M"),
                new XAttribute("versao", "7.9.2"),
                new XElement("gravado", "false"),
                new XElement("nEdificio", CodSurvey),
                new XElement("coordX", Longitude?.ToString(CultureInfo.InvariantCulture) ?? "0"),
                new XElement("coordY", Latitude?.ToString(CultureInfo.InvariantCulture) ?? "0"),
                new XElement("codigoZona", CodigoZona),
                new XElement("nomeZona", CodigoZona),
                new XElement("localidade", Localidade),
                Endereco.GerarXml(),
                new XElement("tecnico",
                    new XElement("id", TecnicoId),
                    new XElement("nome", TecnicoNome)),
                new XElement("empresa",
                    new XElement("id", EmpresaId),
                    new XElement("nome", EmpresaNome)),
                new XElement("data", DateTime.Now.ToString("yyyyMMddHHmmss")),
                new XElement("totalUCs", TotalUCs),
                new XElement("ocupacao", Ocupacao),
                new XElement("numPisos", NumPisos),
                new XElement("destinacao", Destinacao)
            );

            return new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                edificio
            );
        }
    }
}