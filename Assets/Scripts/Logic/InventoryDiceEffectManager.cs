using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewDice.Inventory
{
    public class InventoryDiceEffectManager : Singleton<InventoryDiceEffectManager>
    {
        public DiceList_SO diceList_SO;//ȫ��������
        public EffectList_SO effectList_SO;//ȫ����Ч������

        public DiceBag_SO diceBag_SO;//���������������


        public List<InventoryDiceEffect> curEffectList;//��ǰЧ������
        public List<InventoryDiceEffect> inventoryEffectList;//�ݴ�Ч������

        //�������û�Ч��ʱʹ�õı���
        public InventoryDiceEffect curEffect;//��ǰЧ��
        public InventoryDiceEffect targetEffect;//Ŀ��Ч��


        private void Start()
        {
            curEffectList = new List<InventoryDiceEffect> ();
            inventoryEffectList = new List<InventoryDiceEffect>();

            EventHandler.CallUpdateInventoryDiceEffectUI(InventoryEffectLocation.Cur, curEffectList);//ˢ�µ�ǰЧ������
            EventHandler.CallUpdateInventoryDiceEffectUI(InventoryEffectLocation.Inventory, inventoryEffectList);//ˢ���ݴ�Ч������

        }







        public DiceEffectDetails GetEffectDetails(int ID)//����Ч��ID��ȡЧ������
        {
            return effectList_SO.effectList.Find(i => i.effectID == ID);
        }


        //public void Additem(Item item,bool toDestroy,int amount)//�ӵ�ͼ���򱳰������ָ����������Ʒ
        //{
        //    AddItemAtIndex(item.itemID, GetItemIndexInBag(item.itemID), amount, item.itemDetails.canStack);

        //    if (item.gameObject != null)//�����Ҫ���ٵĻ������ٸ���Ʒ
        //    {
        //        if (toDestroy) { Destroy(item.gameObject); }
        //    }

        //    EventHandler.CallUpdateInventoryUI(InventoryLocation.Bag, playerBag.itemBagList);

        //}


        private int GetEffectIndexInLocation(int ID , InventoryEffectLocation location)//�ҵ�ָ��λ����ָ��Ч��ID��λ��
        {

            if(location == InventoryEffectLocation.Cur)//��ǰ
            {
                for (int i = 0; i < curEffectList.Count; i++)
                {
                    if (curEffectList[i].effectID == ID)
                        return i;
                }
            }
            else if (location == InventoryEffectLocation.Cur)//�ݴ�
            {
                for (int i = 0; i < inventoryEffectList.Count; i++)
                {
                    if (inventoryEffectList[i].effectID == ID)
                        return i;
                }
                
            }

            return -1;
        }

        //private int CheckBagCapacity()//�ҵ������ر����п�λ��λ��
        //{
        //    for (int i = 0; i < playerBag.itemBagList.Count; i++)
        //    {
        //        if (playerBag.itemBagList[i].itemID == 0)
        //        {
        //            return i;
        //        }
        //    }
        //    return -1;
        //}

        //private void AddItemAtIndex(int ID, int index, int amount, bool stack)//�ڱ���������ָ����ƷID�ĸ�������ƷID����Ʒ���ܴ��ڵ�λ�ã���Ʒ�������Ƿ�ɵ���
        //{
        //    if (amount < 0) amount = 0;
        //    if (index != -1 && stack)//�������и���Ʒ�ҿ��Ե���
        //    {
        //        int currentAmount = playerBag.itemBagList[index].itemAmount + amount;
        //        var item = new InventoryItem { itemID = ID, itemAmount = currentAmount };

        //        playerBag.itemBagList[index] = item;
        //    }
        //    else if (CheckBagCapacity() != -1)//�������п�λ
        //    {
        //        var item = new InventoryItem { itemID = ID, itemAmount = amount };
        //        playerBag.itemBagList[CheckBagCapacity()] = item;
        //    }


        //}

        public void SwapEffect(int fromIndex, int targetIndex, EffectSlotType fromSlottype, EffectSlotType targetSlotType)//����������Ч������Ҫ��ʼIndex����ʼ�������࣬�յ�Index���յ��������
        {
            

            switch (fromSlottype)//��ȡ��ʼ���Ч������
            {
                case EffectSlotType.Cur:
                    curEffect = curEffectList[fromIndex];
                    break;
                case EffectSlotType.Inventory:
                    curEffect = inventoryEffectList[fromIndex];
                    break;
            }

            var curEffectDetails = GetEffectDetails(curEffect.effectID);//��õ�ǰҪ��������ʼЧ������ϸ��Ϣ

            switch (targetSlotType)//��ȡĿ����Ч������
            {
                case EffectSlotType.Cur:
                    targetEffect = curEffectList[targetIndex];
                    break;
                case EffectSlotType.Inventory:
                    targetEffect = inventoryEffectList[targetIndex];
                    break;
                case EffectSlotType.Black:
                    break;
            }

            switch (fromSlottype)//�޸���ʼ���Ч������
            {
                case EffectSlotType.Cur:
                    if (targetEffect.effectID > 0)
                    {
                        curEffectList[fromIndex] = targetEffect;
                    }
                    else
                    {
                        curEffectList[fromIndex] = new InventoryDiceEffect();
                    }
                    break;
                case EffectSlotType.Inventory:
                    if (targetEffect.effectID > 0)
                    {
                        inventoryEffectList[fromIndex] = targetEffect;
                    }
                    else
                    {
                        inventoryEffectList[fromIndex] = new InventoryDiceEffect();
                    }
                    break;

            }

            switch (targetSlotType)//�޸�Ŀ����Ч������
            {
                case EffectSlotType.Cur:
                    curEffectList[targetIndex] = curEffect;
                    break;
                case EffectSlotType.Inventory:
                    inventoryEffectList[targetIndex] = curEffect;
                    break;
                case EffectSlotType.Black:
                    DestoryEffect(fromIndex, fromSlottype);
                    break;
            }

            EventHandler.CallUpdateInventoryDiceEffectUI(InventoryEffectLocation.Cur, curEffectList);
            EventHandler.CallUpdateInventoryDiceEffectUI(InventoryEffectLocation.Inventory, inventoryEffectList);

            //���ˣ��û���������������Ҫ�ѵ�ǰ���ӵ���Ϣ���µ�ѡ�е�������ȥ

        }

        public void DestoryEffect(int Index, EffectSlotType  slottype)//��Ŀ��Ч��λ�õ�Ŀ�����������գ���ҪIndex��λ��
        {

            switch (slottype)
            {
                case EffectSlotType.Cur:
                    curEffectList[Index] = new InventoryDiceEffect();
                    EventHandler.CallUpdateInventoryDiceEffectUI(InventoryEffectLocation.Cur, curEffectList);//ˢ�µ�ǰЧ������
                    break;
                case EffectSlotType.Inventory:
                    inventoryEffectList[Index] = new InventoryDiceEffect();
                    EventHandler.CallUpdateInventoryDiceEffectUI(InventoryEffectLocation.Inventory, inventoryEffectList);//ˢ���ݴ�Ч������
                    break;
            }
        }


    }

}
