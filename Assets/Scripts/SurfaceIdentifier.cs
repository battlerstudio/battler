using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SurfaceType { CONCRETE, METAL, DIRT, WOOD }

[RequireComponent(typeof(AudioSource))]
public class SurfaceIdentifier : MonoBehaviour
{
    public SurfaceType surfaceType;
    public GameObject decalprefab;
    public GameObject bulletImpact;
    public AudioClip impactSound;

    public SurfaceManager surfaceManager;

    public AudioSource audioSource;
    void Start()
    {
        surfaceManager = FindObjectOfType<SurfaceManager>();
        SurfaceInitializer();
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1;
        audioSource.maxDistance = 500;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    public void SurfaceInitializer()
    {
        foreach (var item in surfaceManager.surfaceProperties)
        {
            if (item.surfaceType==surfaceType)
            {
                decalprefab = item.decalprefab;
                bulletImpact = item.bulletImpact;
                impactSound = item.impactSound;
            }
        }
    }
}
