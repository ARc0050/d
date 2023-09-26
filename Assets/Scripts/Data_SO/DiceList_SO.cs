using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiceList_SO", menuName = "DiceList_SO")]

public class DiceList_SO : ScriptableObject
{
    public List<DiceDetails> diceDetailsList;
}

