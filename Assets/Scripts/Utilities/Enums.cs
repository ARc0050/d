//骰子的类型
public enum DiceType
{
    Damage
}


//效果的类型
public enum EffectType
{
    Damage
}


//目标的类型
public enum TargetType
{
    Enemy_Random, Enemy_Far,Enemy_Near
}

//效果范围的类型
public enum EffectRangeType
{
    One
}

//格子（效果）类型
public enum EffectSlotType
{
    Cur,Inventory, Black
}

//储存位置（效果）类型
public enum InventoryEffectLocation
{
    Cur, Inventory, Black
}

//格子（骰子）类型
public enum DiceSlotType
{
    Cur, Inventory
}

//储存位置（骰子）类型
public enum InventoryDiceLocation
{
    Cur, Inventory
}

//单位的状态类型
public enum movementStates
{
    Unselected,
    Selected,
    Moved,
    Wait
}