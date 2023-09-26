using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�������ӵ���ϸ����
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

//�洢����������
[System.Serializable]
public class InventoryDice
{
    public int diceID;
    public string diceName;
}



//��������Ч������ϸ����
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

//�洢������Ч��
[System.Serializable]
public class InventoryDiceEffect
{
    public int effectID;
    public string effectName;
}


