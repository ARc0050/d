using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryEffect_SO", menuName = "Inventory Effect List")]

public class InventoryEffect_SO : ScriptableObject
{
    public List<DiceDetails> diceDetailsList;
}

