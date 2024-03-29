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

    static bool hasClicked;

    [Tooltip("How much red is adding to sprite's color after click.")]
    [SerializeField] private float redColorAmount = 0.5f;

    private TextMeshPro text;
    private bool hasChangedColor;
    private bool hasBeenScanned;
    private int closeMines;

    delegate void d();
    d f;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshPro>();
        GameManager.setNumbers += SetNumbers;
        content = ' ';
    }

    /*IEnumerator Start()
    {
        yield return new WaitForSeconds(1);

        f += () => SetText("aa");
        f += () => { GetComponentInChildren<SpriteRenderer>().color = Color.blue; };
        f();
    }*/

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
            StartCoroutine(GameManager.CountUnrevealed());
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
            StartCoroutine(GameManager.CountUnrevealed());

            if (content == ' ')
                Scan();

        });

        // update mineButton on click
        GameManager.mineButton.onClick.RemoveAllListeners();
        GameManager.mineButton.onClick.AddListener(() => {
            GameObject flagObj = transform.GetChild(2).gameObject;
            flagObj.SetActive(!flagObj.activeSelf);
            isFlagged = !isFlagged;
            GameManager.SetGUIActive(false);
            StartCoroutine(GameManager.CountUnrevealed());
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

        GameManager.unrevealesCellsAmount--;
        cell.GetComponentInChildren<SpriteRenderer>().color += new Color(redColorAmount, 0, 0, 0);
        cell.hasChangedColor = true;
    }
}
