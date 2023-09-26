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

        public void UpdateSlot(DiceEffectDetails newDiceEffectDetails)//���¸�����Ʒ��Ϣ
        {
            diceEffectDetails = newDiceEffectDetails;

            slotImage.sprite = diceEffectDetails.effectIcon;
            slotImage.enabled = true;

            button.interactable = true;
            

        }

        public void UpdateEmptySlot()//��������Ʒ��Ϣ�ÿ�
        {
            if (isSelected)
            {
                isSelected = false;
            }

            slotImage.enabled = false;

            button.interactable = false;
        }

        public void OnPointerClick(PointerEventData eventData)//�����Ʒ
        {
            if ( diceEffectDetails.effectID <= 0) return;
            isSelected = !isSelected;

            inventoryDiceEffectUI.UpdateEfeectSlotHighlight(effectSlotType, effectSlotIndex);

        }

        public void OnBeginDrag(PointerEventData eventData)//��ק
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
        public void OnEndDrag(PointerEventData eventData)//��ק�󽻻�Ŀ��λ����Ʒ
        {
            inventoryDiceEffectUI.dragEffect.enabled = false;
            //Debug.Log(eventData.pointerCurrentRaycast.gameObject);
            if (eventData.pointerCurrentRaycast.gameObject != null)//������ǿյ�
            {
                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<EffectSlotUI>() == null)
                    return;
                var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<EffectSlotUI>();
                int targetIndex = targetSlot.effectSlotIndex;

                InventoryDiceEffectManager.Instance.SwapEffect(effectSlotIndex, targetIndex, effectSlotType, targetSlot.effectSlotType);//�������ݵĴ���


                inventoryDiceEffectUI.UpdateEfeectSlotHighlight(effectSlotType ,- 1);
                inventoryDiceEffectUI.UpdateEfeectSlotHighlight(targetSlot.effectSlotType, -1);
            }
            //�������ڵ���
            //else
            //{
            //    var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,-Camera.main.transform.position.z));

            //    EventHandler.CallInstantiateItemInScene(diceEffectDetails.effectID, pos);
            //}
            


        }


    }

}

