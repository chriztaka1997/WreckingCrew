using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : GenericItems
{

    public float moveSpeed = 7f;
    public float attack = 5f;
    public float deathTime = 2f;
    

    // Start is called before the first frame update
    void Start()
    {
        /*Destroy(gameObject, deathTime);*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void ItemTriggerEvent(Collider2D collision)
    {
        base.ItemTriggerEvent(collision);
        collision.gameObject.GetComponent<PlayerMB>().AlterHP(attack*-1.0f);
    }

    private new void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.gameObject.tag != "Enemy" && collision.gameObject.tag != "Player" && collision.gameObject.tag != "Ball")
        {
            gameObject.SetActive(false);
        }
    }
}
