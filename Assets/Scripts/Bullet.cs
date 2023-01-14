using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public float force;
    public float range;

    // impact settings
    public GameObject defaultDecal;
    public GameObject defaultImpact;
    public AudioClip defaultImpactSound;
    public AudioSource audioSource;
    
}
