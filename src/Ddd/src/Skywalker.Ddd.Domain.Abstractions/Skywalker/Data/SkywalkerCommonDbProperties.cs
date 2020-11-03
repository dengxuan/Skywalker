namespace Skywalker.Data
{
    public static class SkywalkerCommonDbProperties
    {
        /// <summary>
        /// This table prefix is shared by most of the Skywalker modules.
        /// You can change it to set table prefix for all modules using this.
        /// 
        /// Default value: "Sky".
        /// </summary>
        public static string DbTablePrefix { get; set; } = "Sky";

        /// <summary>
        /// Default value: null.
        /// </summary>
        public static string DbSchema { get; set; } = null;
    }
}
