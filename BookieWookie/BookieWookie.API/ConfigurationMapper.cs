namespace BookieWookie.API
{
    static class ConfigurationManager
    {
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
