using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
   [HideInInspector] public float ejectForce;
    public float angularEjectForce { set; get; }
    public AudioClip collisionSound;
    private Rigidbody shellRigidbody;
    private Vector3 defaultScale;
    private AudioSource audioSource;

    void Awake()
    {
        defaultScale = transform.localScale;
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = collisionSound;
        angularEjectForce = ejectForce;
        shellRigidbody = GetComponent<Rigidbody>();
        shellRigidbody.maxAngularVelocity = 100;
        shellRigidbody.AddForce(transform.right * ejectForce);
        shellRigidbody.AddTorque(transform.up * angularEjectForce);
        Destroy(gameObject, 5);
    }

    // Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
        shellRigidbody.angularDrag = 5;
        transform.localScale = defaultScale;
        audioSource.Play();
    }
}
