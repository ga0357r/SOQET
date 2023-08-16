namespace SOQET.Debugging
{
    /// <summary>
    /// For Inspector Logging 
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// Toggle Debug on/off
        /// </summary>
        public static bool EnableDebug { get; set; } = true;

        /// <summary>
        /// log to console
        /// </summary>
        /// <param name="message"> information to display in inspector</param>
        public static void Log(string message)
        {
            if (!EnableDebug)
            {
                return;
            }

            UnityEngine.Debug.Log(message);
        }

        /// <summary>
        /// Log a warning to console
        /// </summary>
        /// <param name="message">warning to display in inspector</param>
        public static void LogWarning(string message)
        {
            if (!EnableDebug)
            {
                return;
            }

            UnityEngine.Debug.LogWarning(message);
        }

        /// <summary>
        /// Log an error to console
        /// </summary>
        /// <param name="message">error to display in inspector</param>
        public static void LogError(string message)
        {
            if (!EnableDebug)
            {
                return;
            }

            UnityEngine.Debug.LogError(message);
        }
    }
}