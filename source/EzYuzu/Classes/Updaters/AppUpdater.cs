using Microsoft.Extensions.DependencyInjection;

namespace EzYuzu.Classes.Updaters
{
    public sealed class AppUpdater
    {
        private readonly IHttpClientFactory? clientFactory;

        public AppUpdater(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public AppUpdater(IServiceProvider serviceProvider)
        {
            this.clientFactory = serviceProvider.GetService<IHttpClientFactory>();
        }

        public enum CurrentVersion
        {
            LatestVersion,
            UpdateAvailable,
            NotSupported
        }

        public async Task<CurrentVersion> CheckVersionAsync()
        {
            // latest version is always on top line 
            // so we check and see how many times the loop has iterated and compare it against 1 
            const int LatestVersionLineLocation = 1;
            string currentAppVersion = Application.ProductVersion.Trim();

            try
            {
                var client = clientFactory!.CreateClient("GitHub-EzYuzu");
                using var stream = await client.GetStreamAsync("version");
                using var reader = new StreamReader(stream);
                int i = 1;
                string? onlineVersion;
                while ((onlineVersion = await reader.ReadLineAsync()) is not null)
                {
                    if (currentAppVersion == onlineVersion.Trim() && LatestVersionLineLocation == i)
                    {
                        return CurrentVersion.LatestVersion;
                    }
                    else if (currentAppVersion == onlineVersion.Trim() && LatestVersionLineLocation != i)
                    {
                        return CurrentVersion.UpdateAvailable;
                    }
                    i++;
                }
                return CurrentVersion.NotSupported;
            }
            catch
            {
                return CurrentVersion.NotSupported;
            }
        }
    }
}
