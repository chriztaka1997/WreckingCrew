using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPotion : GenericItems
{
    public static float heal = 100.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void ItemTriggerEvent(Collider2D collision)
    {
        base.ItemTriggerEvent(collision);
        if (collision.gameObject.name == "Player")
        {
            collision.gameObject.GetComponent<PlayerMB>().AlterHP(heal);
        }
    }

}
