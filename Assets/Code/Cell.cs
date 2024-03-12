using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Cell : MonoBehaviour
{
    public bool hasMine;
    TextMeshPro text;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshPro>();
    }

    public void SetText(string newText)
    {
        text.text = newText;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
