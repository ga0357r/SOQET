using System;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.Events;
using SOQET.Editor;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Debug = UnityEngine.Debug;

namespace SOQET.Others
{
    public class Quest : ScriptableObject
    {
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
                Undo.RecordObject(this, "Change quest node name");
                text = value;
                SetName(value);
                EditorUtility.SetDirty(this);
            #else
                text = value;
                SetName(value);
            #endif
            }
        }

        [SerializeField] private string id;
        public string ID { get => id; }

        [SerializeField] private string order;
        public string Order { get => order; set => order = value; }

        [SerializeField] private bool isCompleted;
        public bool IsCompleted { get => isCompleted; set => isCompleted = value; }

        [SerializeField] private string nextQuest;
        public string NextQuest { get => nextQuest; set => nextQuest = value; }

        public UnityEvent OnQuestCompleted = new UnityEvent();

        #if UNITY_EDITOR
        [SerializeField] private Rect rect = new Rect(0f, 0f, 200f, 100f);

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

        private string GenerateID()
        {
            return Guid.NewGuid().ToString();
        }

        [Conditional(SoqetEditorSettings.symbol)]
        public void CompleteQuest()
        {
            if(isCompleted)
            {
                Debug.Log($"{name} quest already complete");
                return;
            }

            isCompleted = true;
            Debug.Log($"{name} quest completed");
            OnQuestCompleted?.Invoke();
        }

        [Conditional(SoqetEditorSettings.symbol)]
        public void MarkAsIncomplete()
        {
            isCompleted = false;
        }
    }
}
