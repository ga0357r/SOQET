namespace SOQET.Debugging
{
    public static class Debug
    {
        public static bool EnableDebug { get; set; } = true;

        public static void Log(string message)
        {
            if (!EnableDebug)
            {
                return;
            }

            UnityEngine.Debug.Log(message);
        }

        public static void LogWarning(string message)
        {
            if (!EnableDebug)
            {
                return;
            }

            UnityEngine.Debug.LogWarning(message);
        }

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