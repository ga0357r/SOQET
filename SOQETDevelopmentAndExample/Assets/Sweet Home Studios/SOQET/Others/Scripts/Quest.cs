using System;
using UnityEngine;
using UnityEngine.Events;
using SOQET.Editor;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SOQET.Others
{
    public sealed class Quest : ScriptableObject
    {
        /// <summary>
        /// Quest name
        /// </summary>
        [SerializeField] private string text;
        public string Text 
        {
            get
            {
                return text;
            }

            set
            {
            #if UNITY_EDITOR
                text = value;
                SetName(value);
                EditorUtility.SetDirty(this);
            #else
                text = value;
                SetName(value);
            #endif
            }
        }

        /// <summary>
        /// Unique Identifier
        /// </summary>
        [HideInInspector] [SerializeField] private string id;
        public string ID { get => id; }

        [HideInInspector] [SerializeField] private string order;
        public string Order { get => order; set => order = value; }

        /// <summary>
        /// Is quest started?
        /// </summary>
        [SerializeField] private bool isStarted;
        public bool IsStarted { get => isStarted; set => isStarted = value; }

        // <summary>
        /// Is quest completed?
        /// </summary>
        [SerializeField] private bool isCompleted;
        public bool IsCompleted { get => isCompleted; set => isCompleted = value; }

        [HideInInspector] [SerializeField] private string nextQuest;
        public string NextQuest { get => nextQuest; set => nextQuest = value; }

        public UnityEvent OnStartQuest = new UnityEvent();
        public UnityEvent OnQuestCompleted = new UnityEvent();

        #if UNITY_EDITOR
        [HideInInspector] [SerializeField] private Rect rect = new Rect(0f, 0f, 200f, 100f);

        public Rect Rect { get => rect; }
        #endif

        
        public void SetRectPosition(Vector2 newPosition)
        {
            #if UNITY_EDITOR
                Undo.RecordObject(this, "Change quest node position");
                rect.position = newPosition;
                EditorUtility.SetDirty(this);
            #endif
        }

        public void Initialize(string order, string nextQuest)
        {
            id = GenerateID();
            this.order = order;
            this.nextQuest = nextQuest;
        }

        public void SetName(string newName)
        {
            name = newName;
        }

        /// <summary>
        /// Generate Unique Identifier
        /// </summary>
        /// <returns></returns>
        private string GenerateID()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Start Quest. Invokes OnStartQuestEvent here 
        /// </summary>
        public void StartQuest()
        {
            if(!SoqetEditorSettings.EnableStory)
            {
                return;
            }

            if(isStarted)
            {
                SOQET.Debugging.Debug.Log($"{name} quest already started");
                return;
            }

            isStarted = true;
            SOQET.Debugging.Debug.Log($"{name} quest started");
            OnStartQuest?.Invoke();
        }

        /// <summary>
        /// Complete Quest. Invokes OnCompleteQuestEvent here 
        /// </summary>
        public void CompleteQuest()
        {
            if(!SoqetEditorSettings.EnableStory)
            {
                return;
            }

            if(isCompleted)
            {
                SOQET.Debugging.Debug.Log($"{name} quest already complete");
                return;
            }

            isCompleted = true;
            SOQET.Debugging.Debug.Log($"{name} quest completed");
            OnQuestCompleted?.Invoke();
        }

        public void MarkAsIncomplete()
        {
            if(!SoqetEditorSettings.EnableStory)
            {
                return;
            }
            
            isStarted = false;
            isCompleted = false;
            SOQET.Debugging.Debug.Log($"{name} quest marked incomplete");
        }
    }
}
