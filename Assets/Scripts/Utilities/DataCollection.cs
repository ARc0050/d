using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//描述骰子的详细属性
[System.Serializable]
public class DiceDetails
{
    public int diceID;
    public string diceName;
    public DiceType diceType;
    public Sprite diceIcon;
    public string diceDescription;
    public int effectID_A;
    public int effectID_B;
    public int effectID_C;
    public int effectID_D;
    public int effectID_E;
    public int effectID_F;
    public int dicePrice;
    [Range(0, 1)]
    public float sellPercentage;
}

//存储起来的骰子
[System.Serializable]
public class InventoryDice
{
    public int diceID;
    public string diceName;
}



//描述骰子效果的详细属性
[System.Serializable]
public class DiceEffectDetails
{
    public int effectID;
    public string effectName;
    public Sprite effectIcon;
    public string effectDescription;
    public EffectType effectType;
    
    public EffectRangeType effectRangeType;
    public int effectRange;

    public TargetType targetType;
    public int effectTargetNum;

    public int effectValue;

}

//存储起来的效果
[System.Serializable]
public class InventoryDiceEffect
{
    public int effectID;
    public string effectName;
}


