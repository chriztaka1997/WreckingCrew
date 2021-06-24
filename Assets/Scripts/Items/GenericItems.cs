using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericItems : MonoBehaviour
{
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

    }
    
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            ItemTriggerEvent(collision);
            gameObject.SetActive(false);
        }
    }
}
