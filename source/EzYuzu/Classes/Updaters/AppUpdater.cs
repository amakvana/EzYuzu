using Microsoft.Extensions.DependencyInjection;

namespace EzYuzu.Classes.Updaters
{
    public sealed class AppUpdater
    {
        private const int LatestVersionLineLocation = 1;
        private readonly IHttpClientFactory? clientFactory;
        private readonly string currentAppVersion;

        public AppUpdater(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
            currentAppVersion = Application.ProductVersion.Trim();
        }

        public AppUpdater(IServiceProvider serviceProvider) : this(serviceProvider.GetService<IHttpClientFactory>()!) { }

        public enum CurrentVersion
        {
            LatestVersion,
            UpdateAvailable,
            NotSupported,
            Undetectable    // used when any errors are thrown, such as no connection, etc.
        }

        public async Task<CurrentVersion> CheckVersionAsync()
        {
            // latest version is always on top line 
            // so we check and see how many times the loop has iterated and compare it against 1 
            try
            {
                var client = clientFactory!.CreateClient("GitHub-EzYuzu");
                using var response = await client.GetAsync("version");

                // if response isn't okay, return undetectable
                if (!response.IsSuccessStatusCode)
                    return CurrentVersion.Undetectable;

                // otherwise get the version and parse it 
                using var stream = await response.Content.ReadAsStreamAsync();
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
                // connection issues
                return CurrentVersion.Undetectable;
            }
        }
    }
}
