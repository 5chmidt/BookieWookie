namespace BookieWookie.API
{
    /// <summary>
    /// Static class to get the setting configuration.
    /// </summary>
    public static class ConfigurationManager
    {
        /// <summary>
        /// Gets the application settings from appsettings.json.
        /// </summary>
        public static IConfiguration AppSetting { get; }
        static ConfigurationManager()
        {
            AppSetting = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
#if DEBUG
                    .AddJsonFile($"appsettings.Development.json")   //TODO: Add Staging settings.
#else
                    .AddJsonFile($"appsettings.Production.json")
#endif
                    .Build();
        }
    }
}
