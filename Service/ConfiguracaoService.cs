// Services/ConfiguracaoService.cs
using System;
using System.IO;
using System.Text.Json;
using GeradorXML.Models;

namespace GeradorXML.Services
{
    public class ConfiguracaoService
    {
        private readonly string _configPath;
        private readonly string _roteirosPath;
        
        public ConfiguracaoService()
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var appDataPath = Path.Combine(userProfile, ".gerador_de_xml_netwin");
            
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);
            
            _roteirosPath = Path.Combine(appDataPath, "Roteiros");
            if (!Directory.Exists(_roteirosPath))
                Directory.CreateDirectory(_roteirosPath);
            
            _configPath = Path.Combine(appDataPath, "config.json");
        }
        
        public Configuracao CarregarConfiguracao()
        {
            if (!File.Exists(_configPath))
                return new Configuracao();
                
            try
            {
                var json = File.ReadAllText(_configPath);
                return JsonSerializer.Deserialize<Configuracao>(json) ?? new Configuracao();
            }
            catch
            {
                return new Configuracao();
            }
        }
        
        public void SalvarConfiguracao(Configuracao config)
        {
            config.UltimaAtualizacao = DateTime.Now;
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configPath, json);
        }
        
        public string GetRoteiroPath()
        {
            var config = CarregarConfiguracao();
            if (!string.IsNullOrEmpty(config.RoteiroPath) && File.Exists(config.RoteiroPath))
                return config.RoteiroPath;
            
            return Path.Combine(_roteirosPath, "roteiro.xml");
        }
        
        public string GetRoteirosPath()
        {
            return _roteirosPath;
        }
        
        public string SalvarArquivoRoteiro(string arquivoOrigem, string nomeDestino)
        {
            var destinoPath = Path.Combine(_roteirosPath, nomeDestino);
            File.Copy(arquivoOrigem, destinoPath, true);
            return destinoPath;
        }
        
        public bool RoteiroExiste()
        {
            var config = CarregarConfiguracao();
            return !string.IsNullOrEmpty(config.RoteiroPath) && File.Exists(config.RoteiroPath);
        }
    }
}