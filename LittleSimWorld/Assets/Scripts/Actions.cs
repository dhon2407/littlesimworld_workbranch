using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Timers;
using UnityEngine.Events;

public class Actions : MonoBehaviour
{
    public float progressSpeed;
    public GameObject floatingText;
    public static Actions Instance;

    public IEnumerator FloatingMessage(string text)
    {
        while (true)
        {
            
            GameObject FloatingText = Instantiate(floatingText, GameLibOfMethods.player.transform.position, GameLibOfMethods.player.transform.rotation);
            FloatingText.GetComponent<TextMeshPro>().text = text;
            FloatingText.transform.position = new Vector3(FloatingText.transform.position.x, FloatingText.transform.position.y + Time.deltaTime);
            Destroy(FloatingText, 2);
            yield return new WaitForFixedUpdate();
        }
       
    }

}
  
