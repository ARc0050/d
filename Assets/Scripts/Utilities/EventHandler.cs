using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    public static event Action<InventoryLocation, List<InventoryDiceEffect>> UpdateInventoryUI;

    public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryDiceEffect> list)
    {
        UpdateInventoryUI?.Invoke(location, list);//用list刷新location的内容,比如用A的列表数据赋予A的UI显示

    }

    //public static event Action<int, Vector3> InstantiateItemInScene;

    //public static void CallInstantiateItemInScene(int ID, Vector3 pos)
    //{
    //    InstantiateItemInScene?.Invoke(ID, pos);//在pos生成ID物品（可拾取）

    //}

}
