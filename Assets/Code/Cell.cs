using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour
{
    [HideInInspector] public bool hasMine;
    [HideInInspector] public int xId;
    [HideInInspector] public int yId;
    [HideInInspector] public char content = ' ';
    [HideInInspector] public bool hasRevealed;
    [HideInInspector] public bool isFlagged;
    //static int unrevealesCellsAmount;

    static bool hasClicked;

    [Tooltip("How much red is adding to sprite's color after click.")]
    [SerializeField] private float redColorAmount = 0.5f;

    private TextMeshPro text;
    private bool hasChangedColor;
    private bool hasBeenScanned;
    private int closeMines;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshPro>();
        GameManager.setNumbers += SetNumbers;
        content = ' ';
    }

    private void OnMouseDown()
    {
        // when cursor is over ui or player clicked on already revealed cell, do nothing
        if (EventSystem.current.IsPointerOverGameObject() || hasRevealed)
            return;

        // the first click reveals part of the board without using gui
        if (!hasClicked)
        {
            GameManager.setMines?.Invoke(xId, yId);

            if (content == ' ')
                Scan();

            hasClicked = true;
            return;
        }

        GameManager.SetCellFrame(transform.position);

        // update digButton on click
        GameManager.digButton.onClick.RemoveAllListeners();
        GameManager.digButton.onClick.AddListener(() => {

            if (transform.GetChild(2).gameObject.activeSelf)
                return;

            if (hasMine)
            {
                GameManager.GameOver();
                return;
            }

            SetText(content.ToString());
            ChangeColor(this);
            hasRevealed = true;
            GameManager.SetGUIActive(false);

            if (content == ' ')
                Scan();

            int unrevealedCellsAmount = CountUnrevealed();
            Debug.Log(unrevealedCellsAmount);
            if (unrevealedCellsAmount <= GameManager.minesAmount)
            {
                Debug.Log("Zosta³y tylko kafelki z minami");
            }
        });

        // update mineButton on click
        GameManager.mineButton.onClick.RemoveAllListeners();
        GameManager.mineButton.onClick.AddListener(() => 
        {
            isFlagged = !isFlagged;
            GameManager.placedFlags = (isFlagged) ? ++GameManager.placedFlags : --GameManager.placedFlags;
            if (GameManager.placedFlags > GameManager.minesAmount)
            {
                Debug.Log("Nie mo¿esz postawiæ wiêcej min");
                return;
            }

            GameObject flagObj = transform.GetChild(2).gameObject;
            flagObj.SetActive(!flagObj.activeSelf);
            GameManager.SetGUIActive(false);
         
            if (GameManager.placedFlags >= GameManager.minesAmount)
            {
                // check if all mines are flagged
                int correctFlags = 0;

                GameManager.cellsWithMine.ForEach((Cell c) => { 
                    if(c.isFlagged)
                    {
                        correctFlags++;
                    }
                });

                if(correctFlags >= GameManager.minesAmount)
                {
                    Debug.Log("Wszystkie miny zaznaczone");
                }
            }
        });
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
                    }
                }
                catch(Exception e)
                {
                    //Debug.Log(e);
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

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                try
                {
                    Cell cell = GameManager.cells[xId + j, yId + i];
                    hasBeenScanned = true;
                    cell.hasRevealed = true;
                    ChangeColor(cell);

                    if (cell.content == ' ')
                        cell.Scan();
                    else
                    {
                        cell.SetText(cell.content.ToString());
                    }
                }
                catch(Exception e)
                {
                    //Debug.Log(e);
                }
            }
        }
    }

    /// <summary>
    /// Change color to yellow.
    /// </summary>
    /// <param name="cell"></param>
    void ChangeColor(Cell cell)
    {
        if (cell.hasChangedColor)
            return;

        //unrevealesCellsAmount--;
        cell.GetComponentInChildren<SpriteRenderer>().color += new Color(redColorAmount, 0, 0, 0);
        cell.hasChangedColor = true;
    }

    /// <summary>
    /// Determines how many cells are still unrevealed.
    /// </summary>
    /// <returns></returns>
    int CountUnrevealed()
    {
        // I've tried to make it better performance by just decrement unrevealesCellsAmount but it was making issues
        int revealedCellsAmount = 0;
        foreach (Cell cell in GameManager.cells)
        {
            if (cell.hasRevealed || cell.isFlagged)
            {
                revealedCellsAmount++;
            }
        }
        return GameManager.width * GameManager.height - revealedCellsAmount;
    }
}
