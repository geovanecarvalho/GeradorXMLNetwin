// Models/Endereco.cs
using System.Xml.Linq;

namespace GeradorXML.Models
{
    public class Endereco
    {
        public string Id { get; set; } = "93128133";
        public string Logradouro { get; set; } = "";
        public string NumeroFachada { get; set; } = "SN";
        public string? Complemento1 { get; set; }
        public string? Argumento1 { get; set; }
        public string? Complemento2 { get; set; }
        public string? Argumento2 { get; set; }
        public string? Complemento3 { get; set; }
        public string? Argumento3 { get; set; }
        public string Cep { get; set; } = "71065071";
        public string Bairro { get; set; } = "";
        public string IdRoteiro { get; set; } = "57149008";
        public string IdLocalidade { get; set; } = "1894644";
        public string CodLograd { get; set; } = "2700035341";

        public bool TemComplemento1 => !string.IsNullOrWhiteSpace(Complemento1);
        public bool TemComplemento2 => !string.IsNullOrWhiteSpace(Complemento2);
        public bool TemComplemento3 => !string.IsNullOrWhiteSpace(Complemento3);

        public XElement GerarXml()
        {
            var endereco = new XElement("enderecoEdificio",
                new XElement("id", Id),
                new XElement("logradouro", Logradouro),
                new XElement("numero_fachada", NumeroFachada),
                new XElement("cep", Cep),
                new XElement("bairro", Bairro),
                new XElement("id_roteiro", IdRoteiro),
                new XElement("id_localidade", IdLocalidade),
                new XElement("cod_lograd", CodLograd)
            );

            if (TemComplemento1)
            {
                endereco.Add(new XElement("id_complemento1", Complemento1));
                endereco.Add(new XElement("argumento1", Argumento1 ?? "1"));
            }

            if (TemComplemento2)
            {
                endereco.Add(new XElement("id_complemento2", Complemento2));
                endereco.Add(new XElement("argumento2", Argumento2 ?? "1"));
            }

            if (TemComplemento3)
            {
                endereco.Add(new XElement("id_complemento3", Complemento3));
                endereco.Add(new XElement("argumento3", Argumento3 ?? "1"));
            }

            return endereco;
        }
    }
}