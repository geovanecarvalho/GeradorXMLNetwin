// Services/CsvReaderService.cs - Atualizado
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
        private readonly Encoding[] _encodings = new[]
        {
            Encoding.GetEncoding("ISO-8859-1"),
            Encoding.UTF8,
            Encoding.GetEncoding("Windows-1252"),
            Encoding.Default
        };

        public List<CsvRegistro> LerCsv(string caminhoArquivo)
        {
            var registros = new List<CsvRegistro>();
            var encodingEncontrado = DetectarEncoding(caminhoArquivo);
            
            var config = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null,
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
                    reader.Peek();
                    return encoding;
                }
                catch
                {
                    // Tenta o próximo encoding
                }
            }
            
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
                "ESTADO_CONTROLE", "DATA_ESTADO_CONTROLE", "ID_CELULA", "QUANTIDADE_HCS",
                "COD_ZONA" 
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
                
                // Mostra todas as colunas do arquivo para debug
                Console.WriteLine($"Colunas encontradas no CSV: {string.Join(", ", colunasArquivo)}");
                
                colunasFaltantes = colunasObrigatorias
                    .Where(col => !colunasArquivo.Contains(col.ToUpperInvariant()))
                    .ToList();
                
                if (colunasFaltantes.Any())
                {
                    Console.WriteLine($"Colunas faltantes: {string.Join(", ", colunasFaltantes)}");
                }
                
                return colunasFaltantes.Count == 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao validar colunas: {ex.Message}");
            }
        }
    }
}