using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
public class FileDataHandler
{
    private string dataDirPath = "";

    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public void SaveFile(object state)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            Debug.Log(state);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonConvert.SerializeObject(state,Formatting.None);

            using (FileStream stream = new FileStream(fullPath, FileMode.OpenOrCreate))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                    writer.Close();
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Save data error " + fullPath + "\n" + e);
        }
    }
    public Dictionary<string, object> LoadFile()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        if (File.Exists(fullPath))
        {           
            try
            {
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                         string dataToLoad = reader.ReadToEnd();    //close
                         reader.Close();
                         return JsonConvert.DeserializeObject<Dictionary<string, object>>(dataToLoad);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Load data error " + fullPath + "\n" + e);
                return new Dictionary<string, object>();
            }
        }
        else
        {
            Debug.Log("No file found");
            return new Dictionary<string, object>();
        }
    }
}
