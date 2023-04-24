using System;
using UnityEngine;

namespace SOQET.Editor
{
    [Serializable]
    public class SoqetEditorSettings
    {
#if UNITY_EDITOR
        public const float canvasSize = 1000000;
        public const float backgroundSize = 50;
#endif

        [SerializeField] private bool saveState = false;
        [SerializeField] private bool enableDebug = SOQET.Debugging.Debug.EnableDebug;
        [SerializeField] private bool enableStory = true;

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