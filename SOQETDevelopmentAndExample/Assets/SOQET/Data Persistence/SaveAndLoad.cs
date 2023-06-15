using System.Text;
using UnityEngine;
using System.IO;
using SOQET.Others;
using System.Collections.Generic;

namespace SOQET.DataPersistence
{
    public static class SaveAndLoad
    {
        private const char newLine = '\n';
        private const char separator = '*';
        private const char space = ' ';
        private const char hyphen = '-';
        private const char column = ':';
        private const string fileName = "StoryData.txt";
        private static string savePath = "";
        

        private static void Initialize()
        {
            savePath = Application.persistentDataPath + "/" +  fileName;
        }

        //Save
        public static void SaveDefaultJson(in Story story)
        {
            if(!story) return;
            Initialize();
            
            //Story
            string storyname = story.name + space + "Story" + space + separator;
            string storyJsonObject = storyname + JsonUtility.ToJson(story);
            File.WriteAllText(savePath, storyJsonObject, Encoding.UTF8);

            //Objectives
            foreach(Objective objective in story.GetObjectives())
            {
                //Append to file
                string objectiveName = newLine + "Objective" + space + objective.Order + column 
                    + space + objective.Text + space + separator;
                
                string objectiveJsonObject = objectiveName + JsonUtility.ToJson(objective);
                File.AppendAllText(savePath, objectiveJsonObject, Encoding.UTF8);

                //Quests
                foreach(Quest quest in objective.GetQuests())
                {
                    //Append to file
                    string questName = newLine + "Quest" + space + quest.Order + column 
                        + space + quest.Text + space + separator;
                
                    string questJsonObject = questName + JsonUtility.ToJson(quest);
                    File.AppendAllText(savePath, questJsonObject, Encoding.UTF8);
                }
            }            
        }

        private static Story CreateStoryData(string jsonObject)
        {
            return JsonUtility.FromJson<Story>(jsonObject);
        }

        //Load
        public static void LoadDefaultJson(Story story)
        {
            Initialize();
            Story storyData = null;
            Objective objectiveData = null;
            Quest questData = null;
            string saveFile = "";
            string[] saveLines;
            List<string> jsonObjects = new List<string>();
            string[] splitString;
            string jsonObject = "";

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

                //Objective
                //Quest
                //Story
                jsonObject = jsonObjects[0];
                storyData = CreateStoryData(jsonObject);
                JsonUtility.FromJsonOverwrite(jsonObject, storyData);
                story.SetIsStarted(storyData.GetIsStarted());
                story.SetIsCompleted(storyData.GetIsCompleted());
                story.SetCurrentObjective(storyData.GetCurrentObjective());

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
        }
    }
}