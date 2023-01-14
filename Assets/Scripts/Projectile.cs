using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody rocketRigidbody;
    public ParticleSystem explosion;
    public float maxRange;
    private float passedDistance = 0;
    void Awake()
    {
        rocketRigidbody = GetComponent<Rigidbody>();
        rocketRigidbody.AddForce(transform.forward * 300);

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        rocketRigidbody.AddForce(transform.forward * 100);
        passedDistance += rocketRigidbody.velocity.magnitude * Time.fixedDeltaTime;
        if (passedDistance>maxRange)
        {
            Instantiate(explosion, transform.position, Quaternion.LookRotation(-transform.forward));
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        rocketRigidbody.detectCollisions = false;
        ContactPoint contact = collision.GetContact(0);
        Instantiate(explosion, contact.point, Quaternion.LookRotation(contact.normal));
        var damageScript= collision.gameObject.GetComponent<DamageHandler>();
        if (damageScript!=null)
        {
            damageScript.DamageReceiver(1000);
        }
        Destroy(gameObject);
    }
}
