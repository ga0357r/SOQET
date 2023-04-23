using System;
using UnityEngine;

namespace SOQET.Editor
{
    [Serializable]
    public class SoqetEditorSettings
    {
        public const string symbol = "ENABLE_STORY";

#if UNITY_EDITOR
        public const float canvasSize = 1000000;
        public const float backgroundSize = 50;
#endif

        [SerializeField] private bool saveState = false;
        [SerializeField] private bool enableDebug = SOQET.Debugging.Debug.EnableDebug;

        public bool SaveState => saveState;
        public bool EnableDebug => enableDebug;
    }
}
