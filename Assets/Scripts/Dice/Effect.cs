using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewDice.Inventory
{
    public class Effect : MonoBehaviour
    {
        public int effectID;
        private SpriteRenderer spriteRenderer;
        public DiceEffectDetails diceEffectDetails;
        private BoxCollider2D boxCollider2D;


        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            boxCollider2D = GetComponent<BoxCollider2D>();

        }

        private void Start()
        {
            if (effectID > 0) { Init(effectID); }
        }

        private void Init(int ID)
        {
            effectID = ID;

            diceEffectDetails = InventoryDiceEffectManager.Instance.GetEffectDetails(effectID);

            if (diceEffectDetails != null)
            {
                spriteRenderer.sprite = diceEffectDetails.effectIcon;
                Vector2 newSize = new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
                boxCollider2D.size = newSize;
                boxCollider2D.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y);
            }
        }
    }

}
