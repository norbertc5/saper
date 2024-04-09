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

    public static bool hasFirstClicked;

    [Tooltip("How much red is adding to sprite's color after click.")]
    [SerializeField] private float redColorAmount = 0.5f;

    private TextMeshPro text;
    private bool hasChangedColor;
    private bool hasBeenScanned;
    private int closeMines;

    private void Start()
    {
        text = GetComponentInChildren<TextMeshPro>();
        GameManager.setNumbers += SetNumbers;
        content = ' ';
        hasFirstClicked = false;
    }

    private void OnDisable()
    {
        GameManager.setNumbers -= SetNumbers;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && GameManager.touchedObject.collider.GetComponent<Cell>() == this)
            {
                // when cursor is over ui or player clicked on already revealed cell, do nothing
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId) || hasRevealed)
                    return;

                // the first click reveals part of the board without using gui
                if (!hasFirstClicked)
                {
                    GameManager.setMines?.Invoke(xId, yId);

                    if (content == ' ')
                        Scan();

                    hasFirstClicked = true;
                    return;
                }

                GameManager.SetCellFrame(transform.position);

                // update digButton on click
                GameManager.digButton.onClick.RemoveAllListeners();
                GameManager.digButton.onClick.AddListener(() =>
                {

                    if (transform.GetChild(2).gameObject.activeSelf)
                        return;

                    if (hasMine)
                    {
                        GameManager.EndGame("Game over.");
                        GameManager.showMines?.Invoke();
                        return;
                    }

                    SetText(content.ToString());
                    ChangeColor(this);
                    hasRevealed = true;
                    GameManager.SetGUIActive(false);

                    if (content == ' ')
                        Scan();
                });

                // update mineButton on click
                GameManager.mineButton.onClick.RemoveAllListeners();
                GameManager.mineButton.onClick.AddListener(() =>
                {
                    isFlagged = !isFlagged;
                    GameManager.placedFlags = (isFlagged) ? ++GameManager.placedFlags : --GameManager.placedFlags;

                    if (GameManager.placedFlags > GameManager.minesAmount)
                    {
                        //Debug.Log("Nie mo¿esz postawiæ wiêcej min");
                        GameManager.placedFlags = GameManager.minesAmount;
                        isFlagged = false;
                        return;
                    }
                    GameManager.minesAmountDisplay.text = (GameManager.minesAmount - GameManager.placedFlags).ToString();

                    GameObject flagObj = transform.GetChild(2).gameObject;
                    flagObj.SetActive(!flagObj.activeSelf);
                    GameManager.SetGUIActive(false);

                    if (GameManager.placedFlags >= GameManager.minesAmount)
                    {
                        // check if all mines are flagged
                        int correctFlags = 0;

                        GameManager.cellsWithMine.ForEach((Cell c) =>
                        {
                            if (c.isFlagged)
                            {
                                correctFlags++;
                            }
                        });

                        if (correctFlags >= GameManager.minesAmount)
                        {
                            //Debug.Log("Wszystkie miny zaznaczone");
                            GameManager.EndGame("You won.");
                        }
                    }
                });
            }
        }
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
                catch
                {
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
                catch
                {
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

        cell.GetComponentInChildren<SpriteRenderer>().color += new Color(redColorAmount, 0, 0, 0);
        cell.hasChangedColor = true;
    }
}
