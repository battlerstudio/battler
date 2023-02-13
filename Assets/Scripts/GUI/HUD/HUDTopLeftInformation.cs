using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDTopLeftInformation : MonoBehaviour
{
    public Text txtFPS;
    private HUDTopLeftInformation hudTopLeftInformation;

    private void Start()
    {
        hudTopLeftInformation = GetComponent<HUDTopLeftInformation>();
        StartCoroutine(FramesPerSecond());
    }


    private void Update()
    {

    }

    private IEnumerator FramesPerSecond()
    {
        while (true)
        {
            int fps = (int)(1f / Time.deltaTime);
            DisplayFPS(fps);

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void DisplayFPS(float fps)
    {
        txtFPS.text = $"{fps}";
    }
}
