using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiceInHand_SO", menuName = "DiceInHand_SO")]

public class DiceInHand_SO : ScriptableObject
{
    public List<InventoryDice> diceInHand;
}
