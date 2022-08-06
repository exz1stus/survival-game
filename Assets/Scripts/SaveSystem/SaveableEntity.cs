using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class SaveableEntity : MonoBehaviour
{
    [SerializeField] private string id;
    public string Id => id;
    
    [ContextMenu("Generate Id")]
    private void GenerateId()
    {
        id = Guid.NewGuid().ToString();
    }
    private void Start()
    {
        /*foreach(var saveableObj in GetComponents<IDataPersistance>())
        {
            DataPersistenceManager.instance.saveableEntities.Add(saveableObj);
        }*/
        DataPersistenceManager.instance.saveableEntities.Add(this);
    }
    public object SaveState()
    {
        var state = new Dictionary<string, object>();
        foreach(var saveable in GetComponents<IDataPersistance>())
        {
            object dataToSave = saveable.SaveData();
            state[saveable.GetType().ToString()] = new SaveData(dataToSave.GetType().Name, dataToSave);
        }
        return state;
    }
    public void LoadState(Dictionary<string, object> state)
    {
        var stateDictionary = /*(Dictionary<string, object>)*/state;
        foreach (var saveable in GetComponents<IDataPersistance>())
        {
            string typeName = saveable.GetType().ToString();
            if(stateDictionary.TryGetValue(typeName, out object savedState))
            {
                JObject jobj = JObject.FromObject(savedState);
                SaveData saveData = jobj.ToObject<SaveData>();
                JObject jobj2 = JObject.FromObject(saveData.data);
                object data = jobj2.ToObject(Type.GetType(saveData.dataType));
                saveable.LoadData(data);
            }
        }
    }
    private void OnDestroy()
    {
        DataPersistenceManager.instance.saveableEntities.Remove(this);
    }
    [System.Serializable]
    public struct SaveData
    {
        public string dataType;
        public object data;

        public SaveData(string dataType, object data)
        {
            this.dataType = dataType;
            this.data = data;
        }
    }
}

