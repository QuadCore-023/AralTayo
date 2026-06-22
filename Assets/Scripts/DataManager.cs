using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    private string dataFolder;
    public List<string> loadedFileNames = new List<string>();
    public Dictionary<string, QuizDataFile> loadedFiles = new Dictionary<string, QuizDataFile>();

    void Awake()
    {
        Instance = this;
        dataFolder = Path.Combine(Application.persistentDataPath, "QuizData");
        if (!Directory.Exists(dataFolder))
            Directory.CreateDirectory(dataFolder);

        CopySampleFilesFromStreamingAssets();
        LoadAllFiles();
    }

    // On first run, copy any .json files from Assets/Data into the save folder
    void CopySampleFilesFromStreamingAssets()
    {
        string streamingPath = Path.Combine(Application.streamingAssetsPath);
        if (!Directory.Exists(streamingPath)) return;

        foreach (string file in Directory.GetFiles(streamingPath, "*.json"))
        {
            string dest = Path.Combine(dataFolder, Path.GetFileName(file));
            if (!File.Exists(dest))
                File.Copy(file, dest);
        }
    }

    public void LoadAllFiles()
    {
        loadedFileNames.Clear();
        loadedFiles.Clear();

        foreach (string file in Directory.GetFiles(dataFolder, "*.json"))
        {
            try
            {
                string json = File.ReadAllText(file);
                QuizDataFile data = JsonUtility.FromJson<QuizDataFile>(json);
                if (data != null && data.questions != null && data.questions.Count > 0)
                {
                    string name = Path.GetFileNameWithoutExtension(file);
                    loadedFileNames.Add(name);
                    loadedFiles[name] = data;
                }
            }
            catch { Debug.LogWarning($"Failed to load {file}"); }
        }
    }

    public string GetDataFolder() => dataFolder;
}