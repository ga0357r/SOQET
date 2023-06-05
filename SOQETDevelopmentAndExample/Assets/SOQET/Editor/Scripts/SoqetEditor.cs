#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Linq;
using System;
using SOQET.Others;

namespace SOQET.Editor
{
    public sealed class SoqetEditor : EditorWindow
    {
        private static Story selectedStory;
        private static bool shouldDrawRewards = false;

        [NonSerialized] private GUIStyle guiStyle;
        [NonSerialized] private Quest selectedQuest = null;
        [NonSerialized] private Objective selectedObjective = null;
        [NonSerialized] private Vector2 dragNodeOffset = Vector2.zero;
        [NonSerialized] private Quest deletedQuest = null;
        [NonSerialized] private Reward deletedReward = null;
        [NonSerialized] private Objective deletedObjective = null;
        [SerializeField] private Vector2 scrollPosition;
        [NonSerialized] private bool isDraggingCanvas = false;
        [NonSerialized] private Vector2 draggingCanvasOffset = Vector2.zero;

        [MenuItem("SOQET/Editor Window")]        
        public static void DisplayStoryEditorWindow()
        {
            GetWindow(typeof(SoqetEditor), false, "Soqet Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenStoryAssetCallback(int instanceID, int line)
        {
            Story story = null;
            shouldDrawRewards = false;

            try
            {
                story = (Story)EditorUtility.InstanceIDToObject(instanceID);
            }
            catch (Exception)
            {
                return false;
            }

            if (story)
            {
                selectedStory = story;
                DisplayStoryEditorWindow();
                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            guiStyle = new GUIStyle();
            guiStyle.normal.background = (Texture2D)EditorGUIUtility.Load("node0");
            guiStyle.padding = new RectOffset(20, 20, 20, 20);
            guiStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnSelectionChanged()
        {
            Story story = null;
            
            try
            {
                story = (Story)Selection.activeObject;
            }
            catch (Exception)
            {
                return;
            }
            
            
            if(story)
            {
                selectedStory = story;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if (!selectedStory)
            {
                EditorGUILayout.LabelField("No Story Selected");
            }

            else
            {
                if (!shouldDrawRewards)
                {
                    ProcessEvents();
                    DrawDefaultGraphics();
                    DeleteObjectivesAndQuests();
                }

                else
                {
                    //Draw Reward Graphics
                    DrawRewardGraphics();
                    //Delete Rewards
                }
            }
        }

        private void DeleteObjectivesAndQuests()
        {
            if (deletedQuest != null && selectedObjective)
            {
                selectedObjective.DeleteQuest(deletedQuest);
                deletedQuest = null;

            }

            if (deletedObjective != null)
            {
                selectedStory.DeleteObjective(deletedObjective);
                deletedObjective = null;
            }
        }

        private void DrawDefaultGraphics()
        {
            AddObjectiveButton();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            DrawCanvas();
            DrawObjectiveConnections();
            DrawObjectiveNodes();
            DrawQuestConnections();
            DrawQuestNodes();
            EditorGUILayout.EndScrollView();
        }

        private void DrawRewardGraphics()
        {
            //Add Exit Button
            AddExitButton();
            //Add Reward Button
            AddRewardButton();
            //Add Quest Name
            AddSelectedObjectName();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            DrawCanvas();
            DrawRewardNodes();
            EditorGUILayout.EndScrollView();   
        }

        private void DrawCanvas()
        {
            float canvasSize = SoqetEditorSettings.canvasSize;
            float backgroundSize = SoqetEditorSettings.backgroundSize;

            Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize);
            Texture2D backgroundTexture = (Texture2D)Resources.Load("background");
            Rect textureCoordinates = new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize);
            GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, textureCoordinates);
        }

        private void DeleteQuestButton(Quest questToDelete, Objective selectedObjective)
        {
            if (GUILayout.Button("Delete Quest"))
            {
                deletedQuest = questToDelete;
                this.selectedObjective = selectedObjective;
            }
        }

        private void DeleteRewardButton(Reward rewardToDelete)
        {
            if (GUILayout.Button("Delete Reward"))
            {
                deletedReward = rewardToDelete;
            }
        }

        private void DeleteObjectiveButton(Objective objectiveToDelete)
        {
            if (GUILayout.Button("Delete Objective"))
            {
                deletedObjective = objectiveToDelete;
            }
        }

        private void AddObjectiveButton()
        {
            if (GUILayout.Button("Add Objective"))
            {
                selectedStory.CreateObjective();
            }
        }
        
        private void AddExitButton()
        {
            if (GUILayout.Button("Exit"))
            {
                shouldDrawRewards = false;
            }
        }

        private void AddRewardButton()
        {
            if (GUILayout.Button("Add Reward"))
            {
                //if quest is selected
                if(IsQuestSelected())
                {
                    //add reward to quest
                    selectedQuest.CreateReward();
                }

            }
        }

        private void AddSelectedObjectName()
        {
            if (Selection.activeObject)
            {
                if (IsQuestSelected())
                {
                    GUILayout.Label("Quest : " + Selection.activeObject.name);
                }
            }
        }

        private void AddQuestButton(Objective objective)
        {
            if (GUILayout.Button("Add Quest"))
            {
                objective.CreateQuest();
            }
        }

        private void DrawQuestConnections()
        {
            Vector3 startPosition = Vector3.zero;
            Vector3 endPosition = Vector3.zero;
            Quest nextQuest = null;

            foreach (Objective currentObjective in selectedStory.GetObjectives())
            {
                foreach (Quest currentQuest in currentObjective.GetQuests())
                {
                    if (int.TryParse(currentQuest.NextQuest, out int nextQuestString))
                    {
                        nextQuest = currentObjective.GetQuest(nextQuestString);

                        if (nextQuest != null)
                        {
                            startPosition = new Vector2(currentQuest.Rect.center.x + currentObjective.Rect.x, currentQuest.Rect.center.y + currentObjective.Rect.y);
                            endPosition = new Vector2(nextQuest.Rect.center.x + currentObjective.Rect.x, nextQuest.Rect.center.y + currentObjective.Rect.y);
                            Handles.DrawBezier(
                                startPosition, endPosition,
                                startPosition,
                                endPosition,
                                Color.white, null, 4f);
                        }
                    }
                }
            }
        }

        private void DrawObjectiveConnections()
        {
            Vector3 startPosition = Vector3.zero;
            Vector3 endPosition = Vector3.zero;
            Vector3 controlPointOffset = Vector3.zero;
            Objective nextObjective = null;

            foreach (Objective currentObjective in selectedStory.GetObjectives())
            {
                if (int.TryParse(currentObjective.NextObjective, out int nextObjectiveString))
                {
                    nextObjective = selectedStory.GetObjective(nextObjectiveString);

                    if (nextObjective != null)
                    {
                        startPosition = new Vector2(currentObjective.Rect.xMax, currentObjective.Rect.center.y);
                        endPosition = nextObjective.Rect.center;
                        controlPointOffset = endPosition - startPosition;
                        controlPointOffset.y = 0;
                        controlPointOffset.x *= 0.8f;
                        Handles.DrawBezier(
                            startPosition, endPosition,
                            startPosition + controlPointOffset,
                            endPosition - controlPointOffset,
                            Color.white, null, 4f);
                    }
                }
            }
        }

        private void ProcessEvents()
        {
            HandleDragEvents();
            HandleClickEvents();
        }

        private void HandleDragEvents()
        {
            DragObjectiveNodes();  
        }

        private void HandleClickEvents()
        {
            OpenRewardsScreen();
            
        }

        private void OpenRewardsScreen()
        {
            
            if (Event.current.type == EventType.MouseDown && Event.current.clickCount == 2)
            {
                if (Selection.activeObject == null)
                {
                    return;
                }

                if (IsQuestSelected())
                {
                    SOQET.Debugging.Debug.Log("Edit the Rewards here");
                    SOQET.Debugging.Debug.Log($"Selected quest is {selectedQuest == Selection.activeObject}");
                    SOQET.Debugging.Debug.Log($"Selected objective is {selectedObjective == Selection.activeObject}");
                    //if the player double clicks a quest
                    //clear the drawn ui
                    shouldDrawRewards = true;
                    Repaint();

                    //draw a new ui to edit the rewards
                }

            }
        }

        private bool IsQuestSelected()
        {
            return Selection.activeObject == selectedQuest;
        }

        private void DragObjectiveNodes()
        {
            if (Event.current.type == EventType.MouseDown && selectedObjective == null)
            {
                Vector2 selectedObjectivePos = Event.current.mousePosition + scrollPosition;
                Vector2 selectedQuestPos = selectedObjectivePos;
                selectedObjective = GetObjectiveNodeAtPoint(selectedObjectivePos);
                
                if (selectedObjective != null)
                {
                    selectedQuestPos -= selectedObjective.Rect.position;
                    selectedQuest = GetQuestNodeAtPoint(selectedQuestPos);

                    if (selectedQuest == null)
                    {
                        dragNodeOffset = selectedObjective.Rect.position - Event.current.mousePosition;
                        Selection.activeObject = selectedObjective;
                    }

                    else
                    {
                        Selection.activeObject = selectedQuest;
                    }
                }
                else
                {
                    isDraggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                    Selection.activeObject = selectedStory;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && selectedObjective != null)
            {
                selectedObjective.SetRectPosition(Event.current.mousePosition + dragNodeOffset);
                GUI.changed = true;
            }

            else if (Event.current.type == EventType.MouseDrag && isDraggingCanvas)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }

            else if (Event.current.type == EventType.MouseUp && selectedObjective != null)
            {
                selectedObjective = null;
            }

            else if (Event.current.type == EventType.MouseUp && isDraggingCanvas)
            {
                isDraggingCanvas = false;
            }
        }

        private Quest GetQuestNodeAtPoint(Vector2 point)
        {
            if (selectedObjective == null)
            {
                return null;
            }


            var selectedQuest = from quest in selectedObjective.GetQuests()
                                where quest.Rect.Contains(point)
                                select quest;

            return selectedQuest.LastOrDefault();
        }

        private Objective GetObjectiveNodeAtPoint(Vector2 point)
        {
            var selectedObjective = from objective in selectedStory.GetObjectives()
                                where objective.Rect.Contains(point)
                                select objective;

            return selectedObjective.LastOrDefault();
        }

        private void DrawQuestNodes()
        {
            foreach (Objective objective in selectedStory.GetObjectives())
            {
                foreach (Quest quest in objective.GetQuests())
                {
                    if (!quest.IsCompleted)
                    {
                        guiStyle.normal.background = (Texture2D)EditorGUIUtility.Load("node0");
                    }
                    else
                    {
                        guiStyle.normal.background = (Texture2D)EditorGUIUtility.Load("node2");
                    }

                    Rect newRect = new Rect(quest.Rect.x + objective.Rect.x, quest.Rect.y + objective.Rect.y, quest.Rect.width, quest.Rect.height);
                    GUILayout.BeginArea(newRect, guiStyle);
                    if (string.IsNullOrEmpty(quest.Text)) EditorGUILayout.LabelField($"Quest {quest.Order}:", EditorStyles.boldLabel);
                    else EditorGUILayout.LabelField($"Quest {quest.Order}: " + quest.Text, EditorStyles.boldLabel);
                    quest.Text = EditorGUILayout.TextField(quest.Text);
                    DeleteQuestButton(quest, objective);
                    GUILayout.EndArea();
                }
            }
        }

        private void DrawRewardNodes()
        {
            foreach (Reward reward in selectedQuest.GetRewards())
            {
                guiStyle.normal.background = (Texture2D)EditorGUIUtility.Load("node2");

                GUILayout.BeginArea(reward.Rect, guiStyle);
                if (string.IsNullOrEmpty(reward.Text)) EditorGUILayout.LabelField($"Reward {reward.Order}:", EditorStyles.boldLabel);
                else EditorGUILayout.LabelField($"Reward {reward.Order}: " + reward.Text, EditorStyles.boldLabel);
                reward.Text = EditorGUILayout.TextField(reward.Text);
                DeleteRewardButton(reward);
                GUILayout.EndArea();
            }
        }

        private void DrawObjectiveNodes()
        {
            foreach (Objective objective in selectedStory.GetObjectives())
            {
                if (!objective.IsCompleted)
                {
                    guiStyle.normal.background = (Texture2D)EditorGUIUtility.Load("node0");
                }
                else
                {
                    guiStyle.normal.background = (Texture2D)EditorGUIUtility.Load("node2");
                }

                GUILayout.BeginArea(objective.Rect, guiStyle);
                if (string.IsNullOrEmpty(objective.Text)) EditorGUILayout.LabelField($"Objective {objective.Order}:", EditorStyles.boldLabel);
                else EditorGUILayout.LabelField($"Objective {objective.Order}: " + objective.Text, EditorStyles.boldLabel);
                objective.Text = EditorGUILayout.TextField(objective.Text);
                DeleteObjectiveButton(objective);
                AddQuestButton(objective);
                GUILayout.EndArea();
            }
        }
    }
}
#endif
