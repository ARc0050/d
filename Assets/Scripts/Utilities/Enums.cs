//���ӵ�����
public enum DiceType
{
    Damage
}


//Ч��������
public enum EffectType
{
    Damage
}


//Ŀ�������
public enum TargetType
{
    Enemy_Random, Enemy_Far,Enemy_Near
}

//Ч����Χ������
public enum EffectRangeType
{
    One
}

//���ӣ�Ч��������
public enum EffectSlotType
{
    Cur,Inventory, Black
}

//����λ�ã�Ч��������
public enum InventoryEffectLocation
{
    Cur, Inventory, Black
}

//���ӣ����ӣ�����
public enum DiceSlotType
{
    Cur, Inventory
}

//����λ�ã����ӣ�����
public enum InventoryDiceLocation
{
    Cur, Inventory
}

//��λ��״̬����
public enum movementStates
{
    Unselected,
    Selected,
    Moved,
    Wait
}