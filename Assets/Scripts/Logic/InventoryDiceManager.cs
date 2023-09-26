//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace NewDice.Inventory
//{
//    public class InventoryDiceManager : Singleton<InventoryDiceManager>
//    {
//        public DiceList_SO diceList_SO;//全骰子数据
//        public EffectList_SO effectList_SO;//全骰子效果数据

//        public DiceBag_SO diceBag_SO;//背包里的骰子数据


        

//        //以下是置换效果时使用的变量
        


//        private void Start()
//        {
//            curEffectList = new List<InventoryDiceEffect> ();
//            inventoryEffectList = new List<InventoryDiceEffect>();

//            EventHandler.CallUpdateInventoryDiceEffectUI(InventoryEffectLocation.Cur, curEffectList);//刷新当前效果数据
//            EventHandler.CallUpdateInventoryDiceEffectUI(InventoryEffectLocation.Inventory, inventoryEffectList);//刷新暂存效果数据

//        }







//        public DiceEffectDetails GetEffectDetails(int ID)//根据效果ID获取效果详情
//        {
//            return effectList_SO.effectList.Find(i => i.effectID == ID);
//        }


//        //public void Additem(Item item,bool toDestroy,int amount)//从地图从向背包中添加指定数量的物品
//        //{
//        //    AddItemAtIndex(item.itemID, GetItemIndexInBag(item.itemID), amount, item.itemDetails.canStack);

//        //    if (item.gameObject != null)//如果需要销毁的话，销毁该物品
//        //    {
//        //        if (toDestroy) { Destroy(item.gameObject); }
//        //    }

//        //    EventHandler.CallUpdateInventoryUI(InventoryLocation.Bag, playerBag.itemBagList);

//        //}


//        private int GetEffectIndexInLocation(int ID , InventoryEffectLocation location)//找到指定位置中指定效果ID的位置
//        {

//            if(location == InventoryEffectLocation.Cur)//当前
//            {
//                for (int i = 0; i < curEffectList.Count; i++)
//                {
//                    if (curEffectList[i].effectID == ID)
//                        return i;
//                }
//            }
//            else if (location == InventoryEffectLocation.Cur)//暂存
//            {
//                for (int i = 0; i < inventoryEffectList.Count; i++)
//                {
//                    if (inventoryEffectList[i].effectID == ID)
//                        return i;
//                }
                
//            }

//            return -1;
//        }

//        //private int CheckBagCapacity()//找到并返回背包中空位的位置
//        //{
//        //    for (int i = 0; i < playerBag.itemBagList.Count; i++)
//        //    {
//        //        if (playerBag.itemBagList[i].itemID == 0)
//        //        {
//        //            return i;
//        //        }
//        //    }
//        //    return -1;
//        //}

//        //private void AddItemAtIndex(int ID, int index, int amount, bool stack)//在背包中增加指定物品ID的个数。物品ID，物品可能存在的位置，物品数量，是否可叠加
//        //{
//        //    if (amount < 0) amount = 0;
//        //    if (index != -1 && stack)//背包里有该物品且可以叠加
//        //    {
//        //        int currentAmount = playerBag.itemBagList[index].itemAmount + amount;
//        //        var item = new InventoryItem { itemID = ID, itemAmount = currentAmount };

//        //        playerBag.itemBagList[index] = item;
//        //    }
//        //    else if (CheckBagCapacity() != -1)//背包里有空位
//        //    {
//        //        var item = new InventoryItem { itemID = ID, itemAmount = amount };
//        //        playerBag.itemBagList[CheckBagCapacity()] = item;
//        //    }


//        //}

        

//        public void DestoryEffect(int Index, EffectSlotType  slottype)//将目标效果位置的目标格子内容清空，需要Index和位置
//        {

//            switch (slottype)
//            {
//                case EffectSlotType.Cur:
//                    curEffectList[Index] = new InventoryDiceEffect();
//                    EventHandler.CallUpdateInventoryDiceEffectUI(InventoryEffectLocation.Cur, curEffectList);//刷新当前效果数据
//                    break;
//                case EffectSlotType.Inventory:
//                    inventoryEffectList[Index] = new InventoryDiceEffect();
//                    EventHandler.CallUpdateInventoryDiceEffectUI(InventoryEffectLocation.Inventory, inventoryEffectList);//刷新暂存效果数据
//                    break;
//            }
//        }


//    }

//}
