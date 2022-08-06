using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Stroge Config")]
    [SerializeField] private string fileName;

    public static DataPersistenceManager instance { get; private set; }

    public List<SaveableEntity> saveableEntities = new List<SaveableEntity>();

    private FileDataHandler fileDataHandler;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("An instance has already been created");
            return;
        }
        instance = this;
    }
    private void Start()
    {
        fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
    }

    public void NewGame()
    {
        
    }
    [ContextMenu("Load")]
    public void LoadGame()
    {
        Dictionary<string, object> state = fileDataHandler.LoadFile();
        LoadStateInEntities(state);
    }

    [ContextMenu("Save")]
    public void SaveGame()
    {
        Dictionary<string, object> state = fileDataHandler.LoadFile();
        SaveStateInEntities(state);
        fileDataHandler.SaveFile(state);
    }

    private void SaveStateInEntities(Dictionary<string, object> state)
    {
        foreach (var saveable in saveableEntities)
        {
            state[saveable.Id] = saveable.SaveState();
        }
    }
    private void LoadStateInEntities(Dictionary<string, object> state)
    {
        foreach (var saveable in saveableEntities)
        {
            if (state.TryGetValue(saveable.Id, out object savedState))
            {
                JObject jobj = JObject.FromObject(savedState);
                Dictionary<string, object> dict = jobj.ToObject<Dictionary<string, object>>();
                saveable.LoadState(dict);
            }
        }
    }
}

