using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portrait : MonoBehaviour
{
    private static Portrait instance;
    public Camera PortraitCamera;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void Render()
    {
        PortraitCamera.Render();
    }

    public static void TakePortraitNextFrame()
    {
        instance.StartCoroutine(instance.RendeBeforeNextFrame());
    }

    private IEnumerator RendeBeforeNextFrame()
    {
        yield return new WaitForEndOfFrame();
        Render();
    }
}
