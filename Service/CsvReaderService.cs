// Services/CsvReaderService.cs - Corrigido
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using GeradorXML.Models;

namespace GeradorXML.Services
{
    public class CsvReaderService
    {
        // Encodings corrigidos - nomes corretos do Windows/.NET
        private readonly Encoding[] _encodings = new[]
        {
            Encoding.GetEncoding("ISO-8859-1"),  // Nome correto para Latin-1
            Encoding.UTF8,
            Encoding.GetEncoding("Windows-1252"), // CP1252
            Encoding.Default
        };

        public List<CsvRegistro> LerCsv(string caminhoArquivo)
        {
            var registros = new List<CsvRegistro>();
            var encodingEncontrado = DetectarEncoding(caminhoArquivo);
            
            var config = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
            {
                Delimiter = ";",           // Separador ponto e vírgula
                HasHeaderRecord = true,    // Primeira linha é cabeçalho
                MissingFieldFound = null,  // Ignorar campos faltantes
                HeaderValidated = null,    // Ignorar validação de cabeçalho
                Encoding = encodingEncontrado
            };

            using var reader = new StreamReader(caminhoArquivo, encodingEncontrado);
            using var csv = new CsvReader(reader, config);
            
            var records = csv.GetRecords<CsvRegistro>().ToList();
            registros.AddRange(records);
            
            return registros;
        }

        private Encoding DetectarEncoding(string caminhoArquivo)
        {
            foreach (var encoding in _encodings)
            {
                try
                {
                    using var reader = new StreamReader(caminhoArquivo, encoding, detectEncodingFromByteOrderMarks: true);
                    reader.Peek(); // Tenta ler um caractere
                    return encoding; // Se chegou aqui, o encoding funcionou
                }
                catch
                {
                    // Tenta o próximo encoding
                }
            }
            
            // Fallback para UTF-8
            return Encoding.UTF8;
        }

        public bool ValidarColunas(string caminhoArquivo, out List<string> colunasFaltantes)
        {
            colunasFaltantes = new List<string>();
            
            var colunasObrigatorias = new[]
            {
                "CELULA", "ESTACAO_ABASTECEDORA", "UF", "MUNICIPIO", "LOCALIDADE",
                "COD_LOCALIDADE", "LOCALIDADE_ABREV", "LOGRADOURO", "COD_LOGRADOURO",
                "NUM_FACHADA", "COMPLEMENTO", "COMPLEMENTO2", "COMPLEMENTO3", "CEP",
                "BAIRRO", "COD_SURVEY", "QUANTIDADE_UMS", "COD_VIABILIDADE",
                "TIPO_VIABILIDADE", "TIPO_REDE", "UCS_RESIDENCIAIS", "UCS_COMERCIAIS",
                "NOME_CDO", "ID_ENDERECO", "LATITUDE", "LONGITUDE", "TIPO_SURVEY",
                "REDE_INTERNA", "UMS_CERTIFICADAS", "REDE_EDIF_CERT", "DISP_COMERCIAL",
                "ESTADO_CONTROLE", "DATA_ESTADO_CONTROLE", "ID_CELULA", "QUANTIDADE_HCS"
            };
            
            try
            {
                var encoding = DetectarEncoding(caminhoArquivo);
                using var reader = new StreamReader(caminhoArquivo, encoding);
                using var csv = new CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    HasHeaderRecord = true
                });
                
                csv.Read();
                csv.ReadHeader();
                var colunasArquivo = csv.HeaderRecord?.Select(h => h.ToUpperInvariant()).ToList() ?? new List<string>();
                
                colunasFaltantes = colunasObrigatorias
                    .Where(col => !colunasArquivo.Contains(col.ToUpperInvariant()))
                    .ToList();
                
                return colunasFaltantes.Count == 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao validar colunas: {ex.Message}");
            }
        }
    }
}