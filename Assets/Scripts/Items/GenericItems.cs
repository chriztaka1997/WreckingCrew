using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericItems : MonoBehaviour
{
    public bool hasEffect;
    public GameObject VisualEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    protected virtual void ItemTriggerEvent(Collider2D collision)
    {
        TutorialMB.SignalTutorial("item");
    }
    
    protected void ShowEffect()
    {
        GameObject se = Instantiate(VisualEffect, GameObject.Find("EffectHolder").transform);
        se.transform.position = GameObject.Find("Player").transform.position;
        Destroy(se, 2f);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            ItemTriggerEvent(collision);
            if(hasEffect)
                ShowEffect();
            gameObject.SetActive(false);
        }
    }
}
