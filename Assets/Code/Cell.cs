using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Drawing;

public class Cell : MonoBehaviour
{
    public bool hasMine;
    public int xId;
    public int yId;
    public int closeMines;

    static bool hasClicked;

    TextMeshPro text;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshPro>();
        GameManager.setNumbers += SetNumbers;
    }

    private void OnMouseDown()
    {
        if(!hasClicked)
        {
            GameManager.setMines?.Invoke(xId, yId);
            hasClicked = true;
        }

        transform.GetChild(2).gameObject.SetActive(true);
    }

    /// <summary>
    /// Set the text inside cell.
    /// </summary>
    /// <param name="newText"></param>
    public void SetText(string newText)
    {
        text.text = newText;
    }

    /// <summary>
    /// Only for cells with mines. Set numbers for cells in neighbourhood.
    /// </summary>
    void SetNumbers()
    {
        if (!hasMine)
            return;

        //transform.GetChild(3).gameObject.SetActive(true);

        // get cells around mine and set the texts
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                // when the mine is on the edge, there is no cell next to is and exception is thrown
                try
                {
                    Cell cell = GameManager.cells[xId + j, yId + i];
                    if (!cell.hasMine)
                    {
                        cell.closeMines++;
                        cell.SetText(cell.closeMines.ToString()); 
                    }
                }
                catch(Exception e)
                {
                    Debug.Log(e);
                }
            }
        }
    }
}
