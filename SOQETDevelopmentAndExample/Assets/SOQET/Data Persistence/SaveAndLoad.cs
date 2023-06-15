using System.Text;
using UnityEngine;
using System.IO;
using SOQET.Others;

namespace SOQET.DataPersistence
{
    public static class SaveAndLoad
    {
        private const string separator = "\n";
        private const string space = " ";
        private const string hyphen = "-";
        private const string column = ":";
        private const string fileName = "StoryData.txt";
        private static string savePath = "";
        

        private static void Initialize()
        {
            savePath = Application.persistentDataPath + "/" +  fileName;
        }

        //Save
        public static void SaveJson(Story story)
        {
            if(!story) return;
            Initialize();
            
            //Story
            string storyname = story.name + space + "Story" + space + hyphen;
            string storyJsonObject = storyname + JsonUtility.ToJson(story);
            File.WriteAllText(savePath, storyJsonObject, Encoding.UTF8);

            //Objectives
            foreach(Objective objective in story.GetObjectives())
            {
                //Append to file
                string objectiveName = separator + "Objective" + objective.Order + column 
                    + space + objective.Text + space + hyphen;
                
                string objectiveJsonObject = objectiveName + JsonUtility.ToJson(objective);
                File.AppendAllText(savePath, objectiveJsonObject, Encoding.UTF8);

                //Quests
                foreach(Quest quest in objective.GetQuests())
                {
                    //Append to file
                    string questName = separator + "Quest" + quest.Order + column 
                        + space + quest.Text + space + hyphen;
                
                    string questJsonObject = questName + JsonUtility.ToJson(quest);
                    File.AppendAllText(savePath, questJsonObject, Encoding.UTF8);
                }
            }            
        }

        //Load
        public static Story Load()
        {
            Initialize();
            Story storyData = null;
            
            if(File.Exists(savePath))
            {
                string jsonObject = File.ReadAllText(savePath);
                storyData = JsonUtility.FromJson<Story>(jsonObject);
            }
            
            return storyData;
        }
    }
}