using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightBehaviour : MonoBehaviour
{
    Light m_light;
    public float damper;
    void Awake()
    {
        m_light = GetComponent<Light>();
    }
    // Update is called once per frame
    void Update()
    {
        m_light.intensity -= damper * Time.deltaTime; 
    }
}
