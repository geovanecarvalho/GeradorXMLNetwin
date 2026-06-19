// Services/IUpdateService.cs
using System;
using System.Threading.Tasks;
using GeradorXML.Models;

namespace GeradorXML.Services
{
    public interface IUpdateService
    {
        event EventHandler<UpdateProgressEventArgs>? OnDownloadProgress;
        event EventHandler<string>? OnError;
        event Action<UpdateInfo>? OnUpdateAvailable;
        
        Task<UpdateResponse> CheckForUpdatesAsync();
        Task<bool> BaixarAtualizacaoAsync(string downloadUrl);
        Task<bool> VerificarAtualizacaoAsync(bool notificarSeAtualizado = false);
        string FormatFileSize(long bytes);
        string GetCurrentVersion();
        UpdateConfig GetConfig();
        void SaveConfig(UpdateConfig config);
        void IgnoreVersion(string version);
        bool IsVersionIgnored(string version);
    }

    public class UpdateProgressEventArgs : EventArgs
    {
        public string Message { get; set; } = "";
        public int Percent { get; set; }
    }
}