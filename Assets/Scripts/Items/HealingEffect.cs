using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingEffect : MonoBehaviour
{
     public ParticleSystem[] m_particleSystem;

        private void FixedUpdate()
        {
            transform.position = GameObject.Find("Player").transform.position;
        foreach(ParticleSystem i in m_particleSystem)
        {
            ParticleSystem.ShapeModule _editableShape = i.shape;
            _editableShape.position = transform.position; 
        }
        }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
