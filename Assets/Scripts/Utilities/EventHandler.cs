using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    public static event Action<InventoryDiceLocation, List<InventoryDice>> UpdateInventoryDiceUI;

    public static void CallUpdateInventoryDiceUI(InventoryDiceLocation location, List<InventoryDice> list)
    {
        UpdateInventoryDiceUI?.Invoke(location, list);//用list刷新location的内容,比如用A的列表数据赋予A的UI显示,这是骰子的

    }

    public static event Action<InventoryEffectLocation, List<InventoryDiceEffect>> UpdateInventoryDiceEffectUI;

    public static void CallUpdateInventoryDiceEffectUI(InventoryEffectLocation location, List<InventoryDiceEffect> list)
    {
        UpdateInventoryDiceEffectUI?.Invoke(location, list);//用list刷新location的内容,比如用A的列表数据赋予A的UI显示，这是效果的

    }


    //public static event Action<int, Vector3> InstantiateItemInScene;

    //public static void CallInstantiateItemInScene(int ID, Vector3 pos)
    //{
    //    InstantiateItemInScene?.Invoke(ID, pos);//在pos生成ID物品（可拾取）

    //}

}
