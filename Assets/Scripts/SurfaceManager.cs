using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceManager : MonoBehaviour
{
    [System.Serializable]
    public struct SurfaceProperties
    {
        public SurfaceType surfaceType;
        public GameObject decalprefab;
        public GameObject bulletImpact;
        public AudioClip impactSound;
    }
    public SurfaceProperties[] surfaceProperties;

    void Start()
    {
            
    }

    void Update()
    {
        
    }
}
