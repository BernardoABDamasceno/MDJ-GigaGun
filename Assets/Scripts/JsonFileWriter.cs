using System;
using System.IO;
using UnityEngine;

public class JsonFileWriter : MonoBehaviour
{
    private string fileName;
    
    [HideInInspector]
    public static PlayerData sampleData;

    void Start()
    {
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "logs")))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "logs"));
            //Debug.Log("Logs directory created.");
        }

        sampleData = new PlayerData();
        WriteToJson();
        //Debug.Log("Sample data initialized and written to JSON.");
    }

    void OnDestroy()
    {
        WriteToJson();
        //Debug.Log("Data written to JSON on destroy.");
    }

    [ContextMenu("Write to JSON")]
    public void WriteToJson()
    {
        fileName = "logs\\" + sampleData.playerId + ".json";
        WriteClassToJson(sampleData, fileName);
    }
    
    public void WriteClassToJson<T>(T dataClass, string filename)
    {
        try
        {
            // Convert the class to JSON
            string jsonString = JsonUtility.ToJson(dataClass, true);
            
            // Get the path for persistent data
            string filePath = Path.Combine(Application.persistentDataPath, filename);
            
            // Write to file
            File.WriteAllText(filePath, jsonString);
            
            //Debug.Log($"Successfully wrote data to: {filePath}");
            //Debug.Log($"JSON Content:\n{jsonString}");
        }
        catch (Exception e)
        {
            //Debug.LogError($"Failed to write JSON file: {e.Message}");
        }
    }
    
    [ContextMenu("Read from JSON")]
    public void ReadFromJson()
    {
        PlayerData loadedData = ReadClassFromJson<PlayerData>(fileName);
        if (loadedData != null)
        {
            sampleData = loadedData;
            //Debug.Log("Data loaded successfully!");
        }
    }
    
    public T ReadClassFromJson<T>(string filename) where T : class
    {
        try
        {
            string filePath = Path.Combine(Application.persistentDataPath, filename);
            
            if (!File.Exists(filePath))
            {
                //Debug.LogWarning($"File not found: {filePath}");
                return null;
            }
            
            string jsonString = File.ReadAllText(filePath);
            T loadedData = JsonUtility.FromJson<T>(jsonString);
            
            //Debug.Log($"Successfully loaded data from: {filePath}");
            return loadedData;
        }
        catch (Exception e)
        {
            //Debug.LogError($"Failed to read JSON file: {e.Message}");
            return null;
        }
    }
    
    [ContextMenu("Show File Location")]
    public void ShowFileLocation()
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        //Debug.Log($"File location: {filePath}");
        
        // Open the folder in file explorer (Windows/Mac)
        #if UNITY_EDITOR_WIN
        System.Diagnostics.Process.Start("explorer.exe", "/select," + filePath.Replace('/', '\\'));
        #elif UNITY_EDITOR_OSX
        System.Diagnostics.Process.Start("open", "-R " + filePath);
        #endif
    }
    
    // Method to write any serializable class to JSON
    public static void SaveToJson<T>(T obj, string filename)
    {
        try
        {
            string json = JsonUtility.ToJson(obj, true);
            string path = Path.Combine(Application.persistentDataPath, filename);
            File.WriteAllText(path, json);
            //Debug.Log($"Saved {typeof(T).Name} to {path}");
        }
        catch (Exception e)
        {
            //Debug.LogError($"Save failed: {e.Message}");
        }
    }
    
    // Method to load any serializable class from JSON
    public static T LoadFromJson<T>(string filename) where T : class
    {
        try
        {
            string path = Path.Combine(Application.persistentDataPath, filename);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonUtility.FromJson<T>(json);
            }
            //Debug.LogWarning($"File not found: {path}");
            return null;
        }
        catch (Exception e)
        {
            //Debug.LogError($"Load failed: {e.Message}");
            return null;
        }
    }
}