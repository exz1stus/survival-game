using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistance
{
    void LoadData(object data);
    object SaveData();
}
