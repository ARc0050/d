using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectList_SO", menuName = "EffectList_SO")]

public class EffectList_SO : ScriptableObject
{
    public List<DiceEffectDetails> effectList;
}