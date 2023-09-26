using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace NewDice.Inventory
{
    public class EffectSlotUI : MonoBehaviour,IPointerClickHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        [SerializeField] private Image slotImage;
        public Image effectSlotHighlight;
        [SerializeField] private Button button;

        public EffectSlotType effectSlotType;

        public bool isSelected;

        public int effectSlotIndex;

        public DiceEffectDetails diceEffectDetails;


        private InventoryDiceEffectUI inventoryDiceEffectUI => GetComponentInParent<InventoryDiceEffectUI>();


        public void Start()
        {
            isSelected = false;
            if (diceEffectDetails.effectID <= 0)
            {
                UpdateEmptySlot();
            }
        }

        public void UpdateSlot(DiceEffectDetails newDiceEffectDetails)//更新格子物品信息
        {
            diceEffectDetails = newDiceEffectDetails;

            slotImage.sprite = diceEffectDetails.effectIcon;
            slotImage.enabled = true;

            button.interactable = true;
            

        }

        public void UpdateEmptySlot()//将格子物品信息置空
        {
            if (isSelected)
            {
                isSelected = false;
            }

            slotImage.enabled = false;

            button.interactable = false;
        }

        public void OnPointerClick(PointerEventData eventData)//点击物品
        {
            if ( diceEffectDetails.effectID <= 0) return;
            isSelected = !isSelected;

            inventoryDiceEffectUI.UpdateEfeectSlotHighlight(effectSlotType, effectSlotIndex);

        }

        public void OnBeginDrag(PointerEventData eventData)//拖拽
        {
            if (diceEffectDetails.effectID > 0)
            {
                inventoryDiceEffectUI.dragEffect.enabled = true;
                inventoryDiceEffectUI.dragEffect.sprite=slotImage.sprite;
                inventoryDiceEffectUI.dragEffect.SetNativeSize();
                isSelected = true;
                //inventoryDiceEffectUI.UpdateSlotHighlight(effectSlotIndex);
            }
        }
        public void OnDrag(PointerEventData eventData)
        {
            inventoryDiceEffectUI.dragEffect.transform.position = Input.mousePosition;
        }
        public void OnEndDrag(PointerEventData eventData)//拖拽后交换目标位置物品
        {
            inventoryDiceEffectUI.dragEffect.enabled = false;
            //Debug.Log(eventData.pointerCurrentRaycast.gameObject);
            if (eventData.pointerCurrentRaycast.gameObject != null)//如果不是空的
            {
                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<EffectSlotUI>() == null)
                    return;
                var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<EffectSlotUI>();
                int targetIndex = targetSlot.effectSlotIndex;

                InventoryDiceEffectManager.Instance.SwapEffect(effectSlotIndex, targetIndex, effectSlotType, targetSlot.effectSlotType);//交换内容的代码


                inventoryDiceEffectUI.UpdateEfeectSlotHighlight(effectSlotType ,- 1);
                inventoryDiceEffectUI.UpdateEfeectSlotHighlight(targetSlot.effectSlotType, -1);
            }
            //测试扔在地上
            //else
            //{
            //    var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,-Camera.main.transform.position.z));

            //    EventHandler.CallInstantiateItemInScene(diceEffectDetails.effectID, pos);
            //}
            


        }


    }

}

