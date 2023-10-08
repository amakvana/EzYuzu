namespace EzYuzu.Classes.Settings
{
    public sealed class EzYuzuSettings
    {
        public string YuzuLocation { get; set; } = "";

        public bool LaunchYuzuAfterUpdate { get; set; } = false;

        public bool ExitEzYuzuAfterUpdate { get; set; } = false;

        public bool AutoUpdateYuzuOnEzYuzuLaunch { get; set; } = false;

        public bool EnableHdrSupport { get; set; } = false;
    }
}
