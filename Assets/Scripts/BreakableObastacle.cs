using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObastacle : MonoBehaviour
{
    public GameObject m;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Instantiate(m, Vector3.up, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Ball")
            Destroy(this.gameObject);
    }
}
