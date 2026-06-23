// Models/CsvRegistro.cs - Versão completa
using CsvHelper.Configuration.Attributes;

namespace GeradorXML.Models
{
    public class CsvRegistro
    {
        [Name("CELULA")]
        public string Celula { get; set; } = "";
        
        [Name("ESTACAO_ABASTECEDORA")]
        public string EstacaoAbastecedora { get; set; } = "";
        
        [Name("UF")]
        public string Uf { get; set; } = "";
        
        [Name("MUNICIPIO")]
        public string Municipio { get; set; } = "";
        
        [Name("LOCALIDADE")]
        public string Localidade { get; set; } = "";

        [Name("COD_ZONA")]
        public string CodZona { get; set; } = "";
        
        [Name("COD_LOCALIDADE")]
        public string CodLocalidade { get; set; } = "";
        
        [Name("LOCALIDADE_ABREV")]
        public string LocalidadeAbrev { get; set; } = "";
        
        [Name("LOGRADOURO")]
        public string Logradouro { get; set; } = "";
        
        [Name("COD_LOGRADOURO")]
        public string CodLogradouro { get; set; } = "";
        
        [Name("NUM_FACHADA")]
        public string NumFachada { get; set; } = "";
        
        [Name("COMPLEMENTO")]
        public string Complemento { get; set; } = "";
        
        [Name("COMPLEMENTO2")]
        public string Complemento2 { get; set; } = "";
        
        [Name("COMPLEMENTO3")]
        public string Complemento3 { get; set; } = "";
        
        [Name("CEP")]
        public string Cep { get; set; } = "";
        
        [Name("BAIRRO")]
        public string Bairro { get; set; } = "";
        
        [Name("COD_SURVEY")]
        public string CodSurvey { get; set; } = "";
        
        [Name("QUANTIDADE_UMS")]
        public int QuantidadeUms { get; set; } = 1;
        
        [Name("COD_VIABILIDADE")]
        public string CodViabilidade { get; set; } = "";
        
        [Name("TIPO_VIABILIDADE")]
        public string TipoViabilidade { get; set; } = "";
        
        [Name("TIPO_REDE")]
        public string TipoRede { get; set; } = "";
        
        [Name("UCS_RESIDENCIAIS")]
        public int UcsResidenciais { get; set; } = 0;
        
        [Name("UCS_COMERCIAIS")]
        public int UcsComerciais { get; set; } = 0;
        
        [Name("NOME_CDO")]
        public string NomeCdo { get; set; } = "";
        
        [Name("ID_ENDERECO")]
        public string IdEndereco { get; set; } = "";
        
        [Name("LATITUDE")]
        public string Latitude { get; set; } = "";
        
        [Name("LONGITUDE")]
        public string Longitude { get; set; } = "";
        
        [Name("TIPO_SURVEY")]
        public string TipoSurvey { get; set; } = "";
        
        [Name("REDE_INTERNA")]
        public string RedeInterna { get; set; } = "";
        
        [Name("UMS_CERTIFICADAS")]
        public string UmsCertificadas { get; set; } = "";
        
        [Name("REDE_EDIF_CERT")]
        public string RedeEdifCert { get; set; } = "";
        
        [Name("DISP_COMERCIAL")]
        public string DispComercial { get; set; } = "";
        
        [Name("ESTADO_CONTROLE")]
        public string EstadoControle { get; set; } = "";
        
        [Name("DATA_ESTADO_CONTROLE")]
        public string DataEstadoControle { get; set; } = "";
        
        [Name("ID_CELULA")]
        public string IdCelula { get; set; } = "";
        
        [Name("QUANTIDADE_HCS")]
        public string QuantidadeHcs { get; set; } = "";
        
        // ⭐ NOVAS PROPRIEDADES ADICIONADAS ⭐
        [Name("ID_ROTEIRO")]
        public string IdRoteiro { get; set; } = "";
        
        [Name("ID_LOCALIDADE")]
        public string IdLocalidade { get; set; } = "";
    }
}