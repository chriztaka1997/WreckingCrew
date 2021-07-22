using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : GenericItems
{

    public float moveSpeed = 7f;
    public float baseAttack = 5f;
    private float attInc;
    public float attack => baseAttack * attInc;
    public float deathTime = 0.5f;
    public float timeToDie;
    

    // Start is called before the first frame update
    void Start()
    {
        /*Destroy(gameObject, deathTime);*/
        timeToDie = Time.time + deathTime;

        ScaleAttack(0);
    }

    private void OnEnable()
    {
        timeToDie = Time.time + deathTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= timeToDie)
        {
            gameObject.SetActive(false);
        }
    }

    public void ScaleAttack(float scale) => attInc = scale;

    protected override void ItemTriggerEvent(Collider2D collision)
    {
        base.ItemTriggerEvent(collision);
        collision.gameObject.GetComponent<PlayerMB>().OnBulletHit(this);
        gameObject.SetActive(false);
    }

    private new void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if ((collision.gameObject.tag =="Wall") || (collision.gameObject.tag == "Ball") )
        {
            gameObject.SetActive(false);
        }
    }
}
