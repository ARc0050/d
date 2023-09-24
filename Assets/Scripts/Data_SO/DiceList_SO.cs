using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiceList_SO", menuName = "Dice List")]

public class DiceList_SO : ScriptableObject
{
    public List<DiceDetails> diceDetailsList;
}

