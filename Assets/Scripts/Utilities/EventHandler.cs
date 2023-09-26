using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    public static event Action<InventoryDiceLocation, List<InventoryDice>> UpdateInventoryDiceUI;

    public static void CallUpdateInventoryDiceUI(InventoryDiceLocation location, List<InventoryDice> list)
    {
        UpdateInventoryDiceUI?.Invoke(location, list);//��listˢ��location������,������A���б����ݸ���A��UI��ʾ,�������ӵ�

    }

    public static event Action<InventoryEffectLocation, List<InventoryDiceEffect>> UpdateInventoryDiceEffectUI;

    public static void CallUpdateInventoryDiceEffectUI(InventoryEffectLocation location, List<InventoryDiceEffect> list)
    {
        UpdateInventoryDiceEffectUI?.Invoke(location, list);//��listˢ��location������,������A���б����ݸ���A��UI��ʾ������Ч����

    }


    //public static event Action<int, Vector3> InstantiateItemInScene;

    //public static void CallInstantiateItemInScene(int ID, Vector3 pos)
    //{
    //    InstantiateItemInScene?.Invoke(ID, pos);//��pos����ID��Ʒ����ʰȡ��

    //}

}
