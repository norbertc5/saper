using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int minesAmount = 5;

    public static Cell[,] cells;
    public static int width;
    public static int height;

    public delegate void Action();
    public static event Action action;

    // Start is called before the first frame update
    void Start()
    {
        // let grid generate before drawing mines
        Invoke("SetMines", .1f);
    }

    /// <summary>
    /// Draw random cells and set it as mines.
    /// </summary>
    void SetMines()
    {
        for (int i = 0; i < minesAmount; i++)
        {
            Cell cell = cells[Random.Range(0, width), Random.Range(0, height)];

            // if cell is already occuped
            if (cell.hasMine)
                i--;

            cell.hasMine = true;
            cell.SetText("M");
        }
        action?.Invoke();
    }
}
