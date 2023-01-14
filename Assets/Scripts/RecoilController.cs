using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilController : MonoBehaviour
{
    [HideInInspector] public float damper;
    public float fireDamper;
    public float returnSpeed;
    private Quaternion initialRotation;
    void Start()
    {
        initialRotation = transform.localRotation;
        damper = fireDamper;
    }

    public void FireRecoil(Vector3 recoil)
    {
        damper = fireDamper;
        transform.Rotate(recoil, Space.Self);
    }
    void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0))
        {
            damper = returnSpeed;
        }
        transform.localRotation = Quaternion.Lerp(transform.localRotation, initialRotation, Time.deltaTime * damper);
    }
}
