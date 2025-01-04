using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName){
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load(){
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        GameData loadedData = null;
        if(File.Exists(fullPath)){
            try
            {
                string dataToLoad = "";
                using(FileStream stream = new FileStream(fullPath, FileMode.Open)){
                    using(StreamReader reader = new StreamReader(stream)){
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
               Debug.LogError("Error occured when trying to load: " + e);
            }
        }

        return loadedData;
    }

    public GameData LoadStarterMap(){
        GameData loadedData = null;
        
        try
        {
            string dataToLoad = "";
            TextAsset dataText = Resources.Load<TextAsset>("Saves/starter");
            dataToLoad = dataText.text;
            loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
        }
        catch (Exception e)
        {
           Debug.LogError("Error occured when trying to load: " + e);
        }
        

        return loadedData;
    }

    public void Save(GameData data){
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            using(FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(stream)){
                    writer.Write(dataToStore);
                }               
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Error occured when trying to save: " + e);        
        }
    }
}
