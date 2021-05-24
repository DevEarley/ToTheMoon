using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowText : MonoBehaviour
{
    public string Text;
    public Text UIText;
    public Text UIText2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        UIText.text = Text;
        UIText2.text = Text;
        UIText.enabled = true;
        UIText2.enabled = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        UIText.text = "";
        UIText2.text = "";
        UIText.enabled = false;
        UIText2.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
