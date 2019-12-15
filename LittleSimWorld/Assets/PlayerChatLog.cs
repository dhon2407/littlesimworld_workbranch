using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using UnityEngine.UI;
public class PlayerChatLog : MonoBehaviour
{
    public static PlayerChatLog Instance;
    public Scrollbar Scroll;
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(this);
        }
    }
    void Start()
    {
        
    }
    public void Reset()
    {
        Scroll.value = 1;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
