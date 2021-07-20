using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedElixir : GenericItems
{
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
        collision.gameObject.GetComponent<PlayerMB>().SetSpdBuff();
    }

}
