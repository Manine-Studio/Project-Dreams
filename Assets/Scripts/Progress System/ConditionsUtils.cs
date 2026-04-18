using System.Collections.Generic;
using System.IO;
using System.Numerics;
using UnityEngine;

namespace Progress_System
{
    [System.Serializable()]
    public class Condition : SerializableDictionaryBase<Conditions, int> { }

    public static class ConditionsUtils
    {
        private static string _sFileName = "ActualDialogConditions.txt";

        /// <summary>
        /// Checker for the given conditions
        /// </summary>
        /// <param name="xDialogue"></param>
        /// <returns></returns>
        public static bool CheckConditions(Condition condition)
        {
            //obtain the scriptable object named "ActualDialogueConditions" in Resources folder that contain the player knowing
            //ActualDialogueCondition[] actualConditions = Resources.LoadAll<ActualDialogueCondition>("DialogueSystemInternalUse");
            Condition mapActualCondition = GetConditions();
            bool[] preconditionsCheck = new bool[condition.Count];

            foreach (KeyValuePair<Conditions, int> pair in condition)
            {
                if (!mapActualCondition.TryGetValue(pair.Key, out int value))
                    return false;

                if (value != pair.Value)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Method for application of the given conditions in the actual player's condition list
        /// </summary>
        public static void ApplyCondition(Condition condition)
        {
            //obtain the scriptable object named "ActualDialogueConditions" in Resources folder that contain the player knowing
            //ActualDialogueCondition[] actualConditions = Resources.LoadAll<ActualDialogueCondition>("DialogueSystemInternalUse");
            Condition mapActualCondition = GetConditions();

            foreach (KeyValuePair<Conditions, int> pair in condition)
            {
                if (mapActualCondition.ContainsKey(pair.Key))
                {
                    mapActualCondition[pair.Key] = pair.Value;
                }
                else
                {
                    mapActualCondition.Add(pair.Key, pair.Value);
                }
            }
            SaveConditionToFile(mapActualCondition, -1);
        }

        public static Condition GetConditions()
        {
            return LoadSavedFile().xActualConditionMap;
            //return Resources.LoadAll<ActualDialogueCondition>("DialogueSystemInternalUse")[0].MConditions;
        }

        private static SavedFile LoadSavedFile()
        {
            string path = Path.Combine(Application.persistentDataPath, _sFileName);
            SavedFile savedFile;

            if (File.Exists(path))
            {
                string data = File.ReadAllText(path);
                savedFile = JsonUtility.FromJson<SavedFile>(data);
            }
            else
            {
                savedFile = new SavedFile();
                savedFile.xActualConditionMap = new Condition();
            }
            return savedFile;
        }


        public static void SaveConditionToFile(Condition condition, int nrScene)
        {
            SavedFile savedFile = LoadSavedFile();
            if(nrScene >= 0)
            {
                savedFile.iCurrentScene = nrScene;
            }
            if(condition != null)
            {
                savedFile.xActualConditionMap = condition;
            }

            string sSerializedObject = JsonUtility.ToJson(savedFile);
            string path = Path.Combine(Application.persistentDataPath, _sFileName);

            File.WriteAllText(path, sSerializedObject);
        }

        public static void ResetConditionFile()
        {
            SavedFile savedFile = new SavedFile();
            savedFile.iCurrentScene = 0;
            savedFile.xActualConditionMap = new Condition();
            
            string sSerializedObject = JsonUtility.ToJson(savedFile);
            string path = Path.Combine(Application.persistentDataPath, _sFileName);

            File.WriteAllText(path, sSerializedObject);
        }
    }
}