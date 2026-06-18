// Services/IUpdateService.cs
using System;
using System.Threading.Tasks;
using GeradorXML.Models;

namespace GeradorXML.Services
{
    public interface IUpdateService
    {
        event EventHandler<UpdateProgressEventArgs> OnDownloadProgress;
        event EventHandler<string> OnError;
        
        Task<UpdateResponse> CheckForUpdatesAsync();
        Task<bool> BaixarAtualizacaoAsync(string downloadUrl);
        string FormatFileSize(long bytes);
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