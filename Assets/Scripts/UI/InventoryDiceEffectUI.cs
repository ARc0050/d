using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace NewDice.Inventory
{
    public class InventoryDiceEffectUI : MonoBehaviour
    {

        public Image dragEffect;

        [SerializeField] private GameObject curEffectUI;
        [SerializeField] private GameObject inventoryEffectUI;

        private bool bagOpend;

        [SerializeField] private EffectSlotUI[] curEffectSlots;
        [SerializeField] private EffectSlotUI[] inventoryEffectSlots;



        private void OnEnable()
        {
            EventHandler.UpdateInventoryDiceEffectUI += OnUpdateInventoryDiceEffectUI;
        }

        private void OnDisable()
        {
            EventHandler.UpdateInventoryDiceEffectUI -= OnUpdateInventoryDiceEffectUI;
        }




        private void Start ()
        {
            for (int i = 0; i < curEffectSlots.Length; i++)//����ǰЧ���ĸ���������
            {
                curEffectSlots[i].effectSlotIndex = i;
            }

            for (int i = 0; i < inventoryEffectSlots.Length; i++)//���ݴ�Ч���ĸ���������
            {
                inventoryEffectSlots[i].effectSlotIndex = i;
            }

            //bagOpend = bagUI.activeInHierarchy;//ȷ�ϱ�����ǰ�Ĵ�״̬

        }

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.B))
        //    {
        //        SwitchBagUI();
        //    }
        //}



        private void OnUpdateInventoryDiceEffectUI(InventoryEffectLocation location, List<InventoryDiceEffect> list)//��ĳ���ط��������Ч������
        {
            switch (location)
            {
                case InventoryEffectLocation.Cur:
                    for (int i = 0; i < curEffectSlots.Length; i++)
                    {
                        if (list[i].effectID > 0)
                        {
                            var effect = InventoryDiceEffectManager.Instance.GetEffectDetails(list[i].effectID);
                            curEffectSlots[i].UpdateSlot(effect);
                        }
                        else
                        {
                            curEffectSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
                case InventoryEffectLocation.Inventory:
                    for (int i = 0; i < inventoryEffectSlots.Length; i++)
                    {
                        if (list[i].effectID > 0)
                        {
                            var effect = InventoryDiceEffectManager.Instance.GetEffectDetails(list[i].effectID);
                            inventoryEffectSlots[i].UpdateSlot(effect);
                        }
                        else
                        {
                            inventoryEffectSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
            }

        }

        //public void SwitchBagUI()//�л�UI�򿪵�״̬
        //{
        //    bagOpend = !bagOpend;

        //    bagUI.SetActive(bagOpend);

        //}


        public void UpdateEfeectSlotHighlight(EffectSlotType slotType, int index)//����ָ�����͵�ָ������Ϊѡ�и���״̬
        {   
            switch (slotType)
            {
                case EffectSlotType.Cur:
                    foreach (var slot in curEffectSlots)
                    {
                        if (slot.isSelected && slot.effectSlotIndex == index)
                        {
                            slot.effectSlotHighlight.gameObject.SetActive(true);
                        }
                        else
                        {
                            slot.effectSlotHighlight.gameObject.SetActive(false);
                            slot.isSelected = false;
                        }
                    }
                    break;
                case EffectSlotType.Inventory:
                    foreach (var slot in inventoryEffectSlots)
                    {
                        if (slot.isSelected && slot.effectSlotIndex == index)
                        {
                            slot.effectSlotHighlight.gameObject.SetActive(true);
                        }
                        else
                        {
                            slot.effectSlotHighlight.gameObject.SetActive(false);
                            slot.isSelected = false;
                        }
                    }
                    break;
            }
            
        }
        

            

    }
}

