using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    public static event Action<InventoryLocation, List<InventoryDiceEffect>> UpdateInventoryUI;

    public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryDiceEffect> list)
    {
        UpdateInventoryUI?.Invoke(location, list);//��listˢ��location������,������A���б����ݸ���A��UI��ʾ

    }

    //public static event Action<int, Vector3> InstantiateItemInScene;

    //public static void CallInstantiateItemInScene(int ID, Vector3 pos)
    //{
    //    InstantiateItemInScene?.Invoke(ID, pos);//��pos����ID��Ʒ����ʰȡ��

    //}

}
