using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    public float health;
    void Start()
    {
        
    }

    public void DamageReceiver(float damageAmount)
    {
        health -= damageAmount;
        if (health<=0)
        {
            OnDeath();
        }
    }

    private void OnDeath()
    {
        Destroy(gameObject);
    }
}
