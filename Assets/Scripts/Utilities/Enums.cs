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

//格子（骰子的）类型
public enum SlotType
{
    Curr,Inventory
}

//储存位置（骰子的）类型
public enum InventoryLocation
{
    Curr, Inventory
}

//单位的状态类型
public enum movementStates
{
    Unselected,
    Selected,
    Moved,
    Wait
}