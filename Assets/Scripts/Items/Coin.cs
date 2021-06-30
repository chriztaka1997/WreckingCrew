using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : GenericItems
{
    public int value;
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
        ObjectPooler.currentCoin += value;
    }
}
