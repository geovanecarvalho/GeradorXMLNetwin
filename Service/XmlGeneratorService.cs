// Services/XmlGeneratorService.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using GeradorXML.Data;
using GeradorXML.Models;

namespace GeradorXML.Services
{
    public class XmlGeneratorService
    {
        private Dictionary<string, string>? _mapeamentoRoteiro;

        public XmlGeneratorService()
        {
            CarregarRoteiro();
        }

        public void CarregarRoteiro()
        {
            try
            {
                var configService = new ConfiguracaoService();
                var roteiroPath = configService.GetRoteiroPath();

                if (File.Exists(roteiroPath))
                {
                    _mapeamentoRoteiro = new Dictionary<string, string>();
                    var doc = XDocument.Load(roteiroPath);

                    // Tenta diferentes estruturas de XML comuns
                    foreach (var elemento in doc.Descendants())
                    {
                        if (elemento.Name.LocalName == "item" || elemento.Name.LocalName == "Item" ||
                            elemento.Name.LocalName == "registro" || elemento.Name.LocalName == "Registro")
                        {
                            var chave = elemento.Attribute("codigo")?.Value ??
                                        elemento.Attribute("id")?.Value ??
                                        elemento.Attribute("code")?.Value ??
                                        elemento.Element("codigo")?.Value ??
                                        elemento.Element("id")?.Value ??
                                        elemento.Element("code")?.Value;

                            var valor = elemento.Attribute("valor")?.Value ??
                                        elemento.Attribute("value")?.Value ??
                                        elemento.Attribute("nome")?.Value ??
                                        elemento.Element("valor")?.Value ??
                                        elemento.Element("value")?.Value ??
                                        elemento.Element("nome")?.Value;

                            if (!string.IsNullOrEmpty(chave) && !string.IsNullOrEmpty(valor))
                            {
                                _mapeamentoRoteiro[chave] = valor;
                            }
                        }
                    }

                    Console.WriteLine($"✅ Roteiro carregado: {_mapeamentoRoteiro.Count} itens");
                }
                else
                {
                    Console.WriteLine("⚠️ Nenhum arquivo de roteiro configurado");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar roteiro: {ex.Message}");
                _mapeamentoRoteiro = null;
            }
        }

        public string? ObterValorRoteiro(string codLogradouro)
        {
            if (string.IsNullOrEmpty(codLogradouro))
                return null;

            if (_mapeamentoRoteiro != null && _mapeamentoRoteiro.TryGetValue(codLogradouro, out var valor))
                return valor;

            return null;
        }

        public bool RoteiroEstaCarregado()
        {
            return _mapeamentoRoteiro != null && _mapeamentoRoteiro.Count > 0;
        }

        public int ObterQuantidadeItensRoteiro()
        {
            return _mapeamentoRoteiro?.Count ?? 0;
        }

        public byte[] GerarXml(CsvRegistro registro, int tipoComplemento)
        {
            var edificio = ConstruirEdificio(registro, tipoComplemento);
            var xmlDoc = edificio.GerarXml();

            using var memoryStream = new MemoryStream();
            xmlDoc.Save(memoryStream);
            return memoryStream.ToArray();
        }

        // Services/XmlGeneratorService.cs - Método ConstruirEdificio CORRIGIDO
        private Edificio ConstruirEdificio(CsvRegistro registro, int tipoComplemento)
        {
            // Obter valores do roteiro se disponível
            var valorRoteiro = ObterValorRoteiro(registro.CodLogradouro);
            
            var edificio = new Edificio
            {
                CodSurvey = string.IsNullOrWhiteSpace(registro.CodSurvey) ? "SEM_CODIGO" : registro.CodSurvey,
                Latitude = ConverterCoordenada(registro.Latitude),
                Longitude = ConverterCoordenada(registro.Longitude),
                
                // ⭐ CORREÇÃO AQUI - Usar o valor do CSV ou valor padrão
                CodigoZona = !string.IsNullOrWhiteSpace(registro.CodZona) 
                    ? registro.CodZona.Trim() 
                    : "DF-GURX-ETGR-CEOS-68", // Valor padrão se não tiver no CSV
                
                Localidade = string.IsNullOrWhiteSpace(registro.Localidade) ? "GUARA" : registro.Localidade,
                
                // ⭐ ADICIONAR ESTES CAMPOS QUE ESTAVAM FALTANDO
                TecnicoId = "1828772688",
                TecnicoNome = "NADIA CAROLINE",
                EmpresaId = "42541126",
                EmpresaNome = "TELEMONT",
                Ocupacao = "EDIFICACAOCOMPLETA",
                NumPisos = 1,
                
                TotalUCs = registro.QuantidadeUms > 0 ? registro.QuantidadeUms : 1,
                Destinacao = DeterminarDestinacao(registro.UcsResidenciais, registro.UcsComerciais)
            };

            // Construir endereço completo
            var codLograd = string.IsNullOrWhiteSpace(registro.CodLogradouro) ? "2700035341" : registro.CodLogradouro;
            var logradouroCompleto = $"{registro.Logradouro}, {registro.Bairro}, {registro.Municipio}, {registro.Localidade} - {registro.Uf} ({codLograd})";

            edificio.Endereco = new Endereco
            {
                Id = string.IsNullOrWhiteSpace(registro.IdEndereco) ? "93128133" : registro.IdEndereco,
                Logradouro = logradouroCompleto,
                NumeroFachada = string.IsNullOrWhiteSpace(registro.NumFachada) ? "SN" : registro.NumFachada,
                Cep = string.IsNullOrWhiteSpace(registro.Cep) ? "71065071" : registro.Cep,
                Bairro = string.IsNullOrWhiteSpace(registro.Bairro) ? edificio.Localidade : registro.Bairro,
                IdRoteiro = !string.IsNullOrEmpty(valorRoteiro) ? valorRoteiro : 
                            (string.IsNullOrWhiteSpace(registro.IdRoteiro) ? "57149008" : registro.IdRoteiro),
                IdLocalidade = string.IsNullOrWhiteSpace(registro.IdLocalidade) ? "1894644" : registro.IdLocalidade,
                CodLograd = codLograd
            };

            // Adicionar complementos baseado no tipo
            if (tipoComplemento >= 1)
            {
                edificio.Endereco.Complemento1 = CodigosComplemento.ObterCodigo(registro.Complemento);
                edificio.Endereco.Argumento1 = CodigosComplemento.ExtrairNumeroArgumento(registro.Complemento);
            }

            if (tipoComplemento >= 2)
            {
                edificio.Endereco.Complemento2 = CodigosComplemento.ObterCodigo(registro.Complemento2);
                edificio.Endereco.Argumento2 = CodigosComplemento.ExtrairNumeroArgumento(registro.Complemento2);
            }

            if (tipoComplemento >= 3)
            {
                edificio.Endereco.Complemento3 = CodigosComplemento.ObterCodigo(registro.Complemento3);
                edificio.Endereco.Argumento3 = CodigosComplemento.ExtrairNumeroArgumento(registro.Complemento3);
            }

            // ⭐ DEBUG - Mostrar o que está sendo construído
            Console.WriteLine($"Edifício construído:");
            Console.WriteLine($"  COD_SURVEY: {edificio.CodSurvey}");
            Console.WriteLine($"  CODIGO_ZONA: {edificio.CodigoZona}");
            Console.WriteLine($"  TECNICO: {edificio.TecnicoNome}");
            Console.WriteLine($"  OCUPACAO: {edificio.Ocupacao}");
            Console.WriteLine($"  NUM_PISOS: {edificio.NumPisos}");

            return edificio;
        }

        private double? ConverterCoordenada(string coordenada)
        {
            if (string.IsNullOrWhiteSpace(coordenada)) 
                return null;

            try
            {
                // Substituir vírgula por ponto
                coordenada = coordenada.Trim().Replace(",", ".");
                
                if (double.TryParse(coordenada, NumberStyles.Any, CultureInfo.InvariantCulture, out var valor))
                    return valor;
            }
            catch
            {
                // Ignora erro de conversão
            }

            return null;
        }

        private string DeterminarDestinacao(int ucsResidenciais, int ucsComerciais)
        {
            if (ucsResidenciais > 0 && ucsComerciais == 0) 
                return "RESIDENCIA";
            
            if (ucsComerciais > 0 && ucsResidenciais == 0) 
                return "COMERCIO";
            
            return "MISTA";
        }

        public int DeterminarTipoComplemento(string complemento1, string complemento2, string complemento3)
        {
            var comp1Vazio = string.IsNullOrWhiteSpace(complemento1);

            if (comp1Vazio) 
                return 0;  // Sem complementos

            var comp2Vazio = string.IsNullOrWhiteSpace(complemento2);
            var comp3Vazio = string.IsNullOrWhiteSpace(complemento3);

            if (!comp2Vazio && !comp3Vazio) 
                return 3;  // Três complementos
            
            if (!comp2Vazio && comp3Vazio) 
                return 2;  // Dois complementos
            
            return 1;  // Apenas um complemento
        }

        public string ObterDescricaoTipoComplemento(int tipo)
        {
            return tipo switch
            {
                0 => "Sem complementos",
                1 => "1 complemento",
                2 => "2 complementos",
                3 => "3 complementos",
                _ => "Tipo desconhecido"
            };
        }
    }
}