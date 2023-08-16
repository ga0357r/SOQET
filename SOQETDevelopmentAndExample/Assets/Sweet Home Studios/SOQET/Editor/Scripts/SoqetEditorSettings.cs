using System;
using UnityEngine;

namespace SOQET.Editor
{
    /// <summary>
    /// SOQET Editor Settings 
    /// </summary>
    [Serializable]
    public sealed class SoqetEditorSettings
    {
#if UNITY_EDITOR
        public const float canvasSize = 1000000;
        public const float backgroundSize = 50;
#endif

        /// <summary>
        /// Toggle Saving On/Off
        /// </summary>
        [SerializeField] private bool saveState = false;

        /// <summary>
        /// Toggle Debug On/Off
        /// </summary>
        [SerializeField] private bool enableDebug = SOQET.Debugging.Debug.EnableDebug;

        /// <summary>
        /// Toggle SOQET On/Off
        /// </summary>
        [SerializeField] private bool enableStory = true;

        /// <summary>
        /// Toggle Save Encyption On/Off
        /// </summary>
        [SerializeField] private bool encryptSaveFile = false;
        
        public bool EncryptSaveFile => encryptSaveFile;
        public bool SaveState => saveState;
        public bool EnableDebug
        {
            get => enableDebug;

            set => enableDebug = value;
        }

        public static bool EnableStory { get; set; } = true;

        public void SetEnableStory(bool value)
        {
            enableStory = value;
        }

        public bool GetEnableStory()
        {
            return enableStory;
        }
    }
}