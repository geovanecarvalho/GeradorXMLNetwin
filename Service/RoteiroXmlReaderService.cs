// Services/RoteiroXmlReaderService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace GeradorXML.Services
{
    public class RoteiroXmlReaderService
    {
        public Dictionary<string, string> CarregarMapeamento(string caminhoXml)
        {
            var mapeamento = new Dictionary<string, string>();
            
            if (string.IsNullOrEmpty(caminhoXml) || !File.Exists(caminhoXml))
                return mapeamento;
            
            try
            {
                var doc = XDocument.Load(caminhoXml);
                
                // Exemplo de leitura - adapte conforme a estrutura do seu XML
                // Supondo que o XML tenha estrutura tipo:
                // <roteiro>
                //   <item codigo="123" valor="ABC" />
                // </roteiro>
                
                foreach (var elemento in doc.Descendants("item"))
                {
                    var chave = elemento.Attribute("codigo")?.Value;
                    var valor = elemento.Attribute("valor")?.Value;
                    
                    if (!string.IsNullOrEmpty(chave) && !string.IsNullOrEmpty(valor))
                    {
                        mapeamento[chave] = valor;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler XML: {ex.Message}");
            }
            
            return mapeamento;
        }
        
        public bool ValidarXml(string caminhoXml)
        {
            try
            {
                XDocument.Load(caminhoXml);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}