//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace NewDice.Inventory
//{
//    public class Dice : MonoBehaviour
//    {
//        public int diceID;
//        private SpriteRenderer spriteRenderer;
//        public DiceDetails diceDetails;
//        private BoxCollider2D boxCollider2D;


//        private void Awake()
//        {
//            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
//            boxCollider2D = GetComponent<BoxCollider2D>();

//        }

//        private void Start()
//        {
//            if (diceID > 0) { Init(diceID); }
//        }

//        private void Init(int ID)
//        {
//            diceID = ID;

//            diceDetails = InventoryDiceManager.Instance.GetDiceDetails(diceID);

//            if (diceDetails != null)
//            {
//                spriteRenderer.sprite = diceDetails.diceIcon;
//                Vector2 newSize = new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
//                boxCollider2D.size = newSize;
//                boxCollider2D.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y);
//            }
//        }
//    }

//}
