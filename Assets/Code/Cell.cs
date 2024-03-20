using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Cell : MonoBehaviour
{
    public bool hasMine;
    public int xId;
    public int yId;
    public int closeMines;
    public char content = ' ';
    public bool hasBeenScanned;

    static bool hasClicked;

    TextMeshPro text;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshPro>();
        GameManager.setNumbers += SetNumbers;
        content = ' ';
    }

    private void OnMouseDown()
    {
        if(!hasClicked)
        {
            GameManager.setMines?.Invoke(xId, yId);
            hasClicked = true;
        }

        if (content == ' ')
            Scan();

        SetText(content.ToString());
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
                        cell.content = cell.closeMines.ToString()[0];
                        //cell.SetText(cell.closeMines.ToString()); 
                    }
                }
                catch(Exception e)
                {
                    Debug.Log(e);
                }
            }
        }
    }

    /// <summary>
    /// Invoked when player has clicked on empty cell (i.e. with no mine and number)
    /// Next determines empty cells in neighbourhood and reveals numbers around empty area
    /// </summary>
    public void Scan()
    {
        if (hasBeenScanned)
            return;

        Debug.Log("scan");

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                try
                {
                    Cell cell = GameManager.cells[xId + j, yId + i];
                    hasBeenScanned = true;

                    if (cell.content == ' ')
                    {
                        //cell.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.blue;
                        cell.Scan();
                    }
                    else
                        cell.SetText(cell.content.ToString());
                }
                catch(Exception e)
                {
                    Debug.Log(e);
                }
            }
        }
    }
}
