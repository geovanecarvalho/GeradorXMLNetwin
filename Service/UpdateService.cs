// Services/UpdateService.cs
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeradorXML.Models;

namespace GeradorXML.Services
{
    public class UpdateService : IUpdateService, IDisposable
    {
        private readonly string _configPath;
        private readonly HttpClient _httpClient;
        private readonly string _versaoAtual;
        private readonly string _tempPath;
        private readonly string _repoOwner;
        private readonly string _repoName;
        
        public event EventHandler<UpdateProgressEventArgs>? OnDownloadProgress;
        public event EventHandler<string>? OnError;

        public UpdateService(string repoOwner = "geovanecarvalho", string repoName = "GeradorXMLNetwin")
        {
            _repoOwner = repoOwner;
            _repoName = repoName;
            
            _versaoAtual = GetCurrentVersion();
            
            _tempPath = Path.Combine(Path.GetTempPath(), "GeradorXMLNetwin_Update");
            
            if (!Directory.Exists(_tempPath))
                Directory.CreateDirectory(_tempPath);
            
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".gerador_de_xml_netwin"
            );
            
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);
                
            _configPath = Path.Combine(appDataPath, "update_config.json");
            
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(30);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", $"{_repoName}");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
        }

        private string GetCurrentVersion()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;
                if (version != null)
                    return $"{version.Major}.{version.Minor}.{version.Build}";
                
                return "1.0.0";
            }
            catch
            {
                return "1.0.0";
            }
        }

        public async Task<UpdateResponse> CheckForUpdatesAsync()
        {
            try
            {
                var config = GetConfig();
                
                if (IsVersionIgnored(config.IgnoredVersion))
                {
                    return new UpdateResponse { HasUpdate = false };
                }
                
                var url = string.IsNullOrEmpty(config.UpdateUrl) 
                    ? $"https://api.github.com/repos/{_repoOwner}/{_repoName}/releases/latest"
                    : config.UpdateUrl;
                
                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return await CheckAllReleasesAsync();
                    }
                    
                    return new UpdateResponse 
                    { 
                        HasUpdate = false, 
                        Error = $"Erro ao verificar atualizações: {response.StatusCode}" 
                    };
                }
                
                var json = await response.Content.ReadAsStringAsync();
                var updateInfo = ParseGitHubRelease(json);
                
                if (updateInfo == null)
                    return new UpdateResponse { HasUpdate = false };
                
                config.LastCheck = DateTime.Now;
                SaveConfig(config);
                
                var hasUpdate = CompareVersions(updateInfo.VersaoNova, _versaoAtual) > 0;
                
                return new UpdateResponse
                {
                    HasUpdate = hasUpdate,
                    UpdateInfo = hasUpdate ? updateInfo : null
                };
            }
            catch (Exception ex)
            {
                return new UpdateResponse 
                { 
                    HasUpdate = false, 
                    Error = $"Erro ao verificar atualizações: {ex.Message}" 
                };
            }
        }

        private async Task<UpdateResponse> CheckAllReleasesAsync()
        {
            try
            {
                var url = $"https://api.github.com/repos/{_repoOwner}/{_repoName}/releases";
                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                    return new UpdateResponse { HasUpdate = false };
                
                var json = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(json);
                var releases = document.RootElement.EnumerateArray();
                
                UpdateInfo? latestStable = null;
                UpdateInfo? latestPrerelease = null;
                
                foreach (var release in releases)
                {
                    var isPrerelease = release.GetProperty("prerelease").GetBoolean();
                    var tagName = release.GetProperty("tag_name").GetString() ?? "";
                    var version = tagName.TrimStart('v');
                    
                    if (release.GetProperty("draft").GetBoolean())
                        continue;
                    
                    var info = ParseGitHubRelease(release);
                    if (info == null) continue;
                    
                    if (isPrerelease)
                    {
                        if (latestPrerelease == null || 
                            CompareVersions(info.VersaoNova, latestPrerelease.VersaoNova) > 0)
                        {
                            latestPrerelease = info;
                        }
                    }
                    else
                    {
                        if (latestStable == null || 
                            CompareVersions(info.VersaoNova, latestStable.VersaoNova) > 0)
                        {
                            latestStable = info;
                        }
                    }
                }
                
                var updateInfo = latestStable ?? latestPrerelease;
                
                if (updateInfo == null)
                    return new UpdateResponse { HasUpdate = false };
                
                var hasUpdate = CompareVersions(updateInfo.VersaoNova, _versaoAtual) > 0;
                
                return new UpdateResponse
                {
                    HasUpdate = hasUpdate,
                    UpdateInfo = hasUpdate ? updateInfo : null
                };
            }
            catch
            {
                return new UpdateResponse { HasUpdate = false };
            }
        }

        private UpdateInfo? ParseGitHubRelease(string json)
        {
            try
            {
                using var document = JsonDocument.Parse(json);
                return ParseGitHubRelease(document.RootElement);
            }
            catch
            {
                return null;
            }
        }

        private UpdateInfo? ParseGitHubRelease(JsonElement release)
        {
            try
            {
                var tagName = release.GetProperty("tag_name").GetString() ?? "";
                var version = tagName.TrimStart('v');
                var body = release.GetProperty("body").GetString() ?? "";
                var publishedAt = release.GetProperty("published_at").GetString() ?? "";
                var isPrerelease = release.GetProperty("prerelease").GetBoolean();
                var draft = release.GetProperty("draft").GetBoolean();
                
                if (draft) return null;
                
                var assets = release.GetProperty("assets");
                string? downloadUrl = null;
                long size = 0;
                string? fileName = null;
                
                foreach (var asset in assets.EnumerateArray())
                {
                    var name = asset.GetProperty("name").GetString() ?? "";
                    if (name.EndsWith(".exe") || name.EndsWith(".zip"))
                    {
                        downloadUrl = asset.GetProperty("browser_download_url").GetString() ?? string.Empty;
                        size = asset.GetProperty("size").GetInt64();
                        fileName = name;
                        break;
                    }
                }
                
                if (string.IsNullOrEmpty(downloadUrl))
                    return null;
                
                return new UpdateInfo
                {
                    VersaoAtual = _versaoAtual,
                    VersaoNova = version,
                    DownloadUrl = downloadUrl,
                    Tamanho = size,
                    Descricao = body,
                    DataPublicacao = DateTime.Parse(publishedAt),
                    IsPrerelease = isPrerelease,
                    NomeArquivo = fileName
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> BaixarAtualizacaoAsync(string downloadUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(downloadUrl))
                {
                    OnError?.Invoke(this, "URL de download não encontrada");
                    return false;
                }

                var extension = Path.GetExtension(downloadUrl);
                var nomeArquivo = $"GeradorXMLNetwin_Update{extension}";
                var caminhoTemp = Path.Combine(_tempPath, nomeArquivo);
                
                if (File.Exists(caminhoTemp))
                    File.Delete(caminhoTemp);
                
                OnDownloadProgress?.Invoke(this, new UpdateProgressEventArgs 
                { 
                    Message = "Iniciando download...", 
                    Percent = 0 
                });
                
                using var response = await _httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                
                var totalBytes = response.Content.Headers.ContentLength ?? -1;
                var canReportProgress = totalBytes != -1;
                
                using var contentStream = await response.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(caminhoTemp, FileMode.Create, FileAccess.Write, FileShare.None);
                
                var buffer = new byte[81920];
                var totalRead = 0L;
                var lastReport = 0;
                
                while (true)
                {
                    var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;
                    
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    totalRead += bytesRead;
                    
                    if (canReportProgress)
                    {
                        var percent = (int)((totalRead * 100) / totalBytes);
                        if (percent != lastReport && percent % 5 == 0)
                        {
                            lastReport = percent;
                            OnDownloadProgress?.Invoke(this, new UpdateProgressEventArgs
                            {
                                Message = $"Baixando... {percent}% ({FormatFileSize(totalRead)} / {FormatFileSize(totalBytes)})",
                                Percent = percent
                            });
                        }
                    }
                }
                
                OnDownloadProgress?.Invoke(this, new UpdateProgressEventArgs
                {
                    Message = "✅ Download concluído! Instalando...",
                    Percent = 100
                });
                
                await Task.Delay(500);
                
                var startInfo = new ProcessStartInfo
                {
                    FileName = caminhoTemp,
                    UseShellExecute = true,
                    Verb = "runas"
                };
                
                if (extension.ToLower() == ".exe")
                {
                    startInfo.Arguments = "/SILENT /CLOSEAPPLICATIONS /SUPPRESSMSGBOXES";
                }
                
                var process = Process.Start(startInfo);
                
                if (process != null)
                {
                    await Task.Delay(1000);
                }
                
                Application.Exit();
                
                return true;
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, $"Erro ao baixar atualização: {ex.Message}");
                return false;
            }
        }

        public string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            
            return $"{len:0.##} {sizes[order]}";
        }

        public UpdateConfig GetConfig()
        {
            if (!File.Exists(_configPath))
                return new UpdateConfig { IsFirstRun = true };
                
            try
            {
                var json = File.ReadAllText(_configPath);
                return JsonSerializer.Deserialize<UpdateConfig>(json) ?? new UpdateConfig { IsFirstRun = true };
            }
            catch
            {
                return new UpdateConfig { IsFirstRun = true };
            }
        }

        public void SaveConfig(UpdateConfig config)
        {
            try
            {
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_configPath, json);
            }
            catch
            {
                // Ignorar erro ao salvar configuração
            }
        }

        public void IgnoreVersion(string version)
        {
            var config = GetConfig();
            config.IgnoredVersion = version;
            SaveConfig(config);
        }

        public bool IsVersionIgnored(string version)
        {
            var config = GetConfig();
            return !string.IsNullOrEmpty(config.IgnoredVersion) && 
                   config.IgnoredVersion == version;
        }

        private int CompareVersions(string v1, string v2)
        {
            try
            {
                var parts1 = v1.Split('.');
                var parts2 = v2.Split('.');
                
                for (int i = 0; i < Math.Max(parts1.Length, parts2.Length); i++)
                {
                    var num1 = i < parts1.Length && int.TryParse(parts1[i], out var n1) ? n1 : 0;
                    var num2 = i < parts2.Length && int.TryParse(parts2[i], out var n2) ? n2 : 0;
                    
                    if (num1 != num2)
                        return num1.CompareTo(num2);
                }
                
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}