using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : GenericItems
{

    public float moveSpeed = 7f;
    public float attack = 5f;
    public float deathTime = 0.5f;
    public float timeToDie;
    

    // Start is called before the first frame update
    void Start()
    {
        /*Destroy(gameObject, deathTime);*/
        timeToDie = Time.time + deathTime;
    }

    private void OnEnable()
    {
        timeToDie = Time.time + deathTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= timeToDie)
            gameObject.SetActive(false);
    }

    protected override void ItemTriggerEvent(Collider2D collision)
    {
        base.ItemTriggerEvent(collision);
        collision.gameObject.GetComponent<PlayerMB>().AlterHP(attack*-1.0f);
    }

    private new void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if ((collision.gameObject.tag =="Wall") || (collision.gameObject.tag == "Ball") || Time.time>=timeToDie)
        {
            gameObject.SetActive(false);
        }
    }
}
