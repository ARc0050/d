using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiceBag_SO", menuName = "DiceBag_SO")]

public class DiceBag_SO : ScriptableObject
{
    public List<InventoryDice> diceBag;
}

