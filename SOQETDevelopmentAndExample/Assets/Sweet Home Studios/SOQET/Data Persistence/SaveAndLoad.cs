using System.Text;
using UnityEngine;
using System.IO;
using SOQET.Others;
using System.Collections.Generic;

namespace SOQET.DataPersistence
{
    /// <summary>
    /// Save And Load Story Data Using JSON
    /// </summary>
    public static class SaveAndLoad
    {
        /// <summary>
        /// Default Save File Name
        /// </summary>
        private const string fileName = "StoryData.txt";

        /// <summary>
        /// Save Path
        /// </summary>
        private static string savePath = "";
        
        private static void Initialize()
        {
            savePath = Application.persistentDataPath + "/" +  fileName;
        }

        /// <summary>
        /// Get Platform Persistent Path
        /// </summary>
        /// <returns>Platform Persistent Path</returns>
        public static string GetSavePath()
        {
            return Application.persistentDataPath + "/" +  fileName;
        }

        /// <summary>
        /// Save Story Data using JSON
        /// </summary>
        /// <param name="story"> Story Data</param>
        public static void SaveDefaultJson(in Story story)
        {
            if (!story) return;
            Initialize();
            char newLine = '\n';
            char separator = '*';
            char space = ' ';
            char column = ':';

            //Story
            string storyname = story.name + space + "Story" + space + separator;
            string storyJsonObject = storyname + JsonUtility.ToJson(story);
            File.WriteAllText(savePath, storyJsonObject, Encoding.UTF8);

            //Objectives
            foreach (Objective objective in story.GetObjectives())
            {
                //Append to file
                string objectiveName = newLine + "Objective" + space + objective.Order + column
                    + space + objective.Text + space + separator;

                string objectiveJsonObject = objectiveName + JsonUtility.ToJson(objective);
                File.AppendAllText(savePath, objectiveJsonObject, Encoding.UTF8);

                //Quests
                foreach (Quest quest in objective.GetQuests())
                {
                    //Append to file
                    string questName = newLine + "Quest" + space + quest.Order + column
                        + space + quest.Text + space + separator;

                    string questJsonObject = questName + JsonUtility.ToJson(quest);
                    File.AppendAllText(savePath, questJsonObject, Encoding.UTF8);
                }
            }
        }

        /// <summary>
        /// Load Story Data
        /// </summary>
        /// <param name="story"> Story Data</param>
        public static void LoadDefaultJson(Story story)
        {
            Initialize();
            char separator = '*';
            Story storyData = ScriptableObject.CreateInstance<Story>();
            Objective objectiveData = ScriptableObject.CreateInstance<Objective>();
            Quest questData = ScriptableObject.CreateInstance<Quest>();
            string saveFile = "";
            string[] saveLines;
            List<string> jsonObjects = new List<string>();
            string[] splitString;
            string jsonObject = "";
            int jsonObjectIndex = 0;

            if (File.Exists(savePath))
            {
                saveFile = File.ReadAllText(savePath);
                saveLines = saveFile.Split('\n');

                //for each saveLine append the necessary jsonObjects
                foreach (string saveLine in saveLines)
                {
                    splitString = saveLine.Split(separator);
                    jsonObjects.Add(splitString[1]);
                }

                //Story
                jsonObject = jsonObjects[jsonObjectIndex];
                JsonUtility.FromJsonOverwrite(jsonObject, storyData);
                story.SetIsStarted(storyData.GetIsStarted());
                story.SetIsCompleted(storyData.GetIsCompleted());
                story.SetCurrentObjective(storyData.GetCurrentObjective());
                jsonObjectIndex++;

                //Objectives
                foreach (Objective objective in story.GetObjectives())
                {
                    jsonObject = jsonObjects[jsonObjectIndex];
                    JsonUtility.FromJsonOverwrite(jsonObject,objectiveData);
                    objective.IsStarted = objectiveData.IsStarted;
                    objective.IsCompleted = objectiveData.IsCompleted;
                    objective.SetCurrentQuest(objectiveData.GetCurrentQuest());
                    jsonObjectIndex++;

                    //Quests
                    foreach (Quest quest in objective.GetQuests())
                    {
                        jsonObject = jsonObjects[jsonObjectIndex];
                        JsonUtility.FromJsonOverwrite(jsonObject,questData);
                        quest.IsStarted = questData.IsStarted;
                        quest.IsCompleted = questData.IsCompleted;
                        jsonObjectIndex++;
                    }
                }
            }
        }
    }
}