using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int minesAmount = 5;
    [SerializeField] private int distanceToMine = 3;

    public static Cell[,] cells;
    public static int width;
    public static int height;

    public delegate void MinesAction(int x, int y);
    public delegate void NumbersAction();
    public static MinesAction setMines;
    public static NumbersAction setNumbers;

    /*
     * First click sets mines postions
     * Next, each click reveals one cell
    */

    // Start is called before the first frame update
    void Start()
    {
        // firstly generate mines
        setMines += SetMines;
    }

    /// <summary>
    /// Draw random cells and set it as mines.
    /// </summary>
    void SetMines(int x, int y)
    {
        for (int i = 0; i < minesAmount; i++)
        {
            Debug.Log("wyk");
            int randX = Random.Range(0, width), randY = Random.Range(0, height);

            // mines won't appear near the first revealed cell
            if (Mathf.Abs(randX - x) < distanceToMine && Mathf.Abs(randY - y) < distanceToMine)
            {
                i--;
                continue;
            }

            Cell cell = cells[randX, randY];

            // if cell is already occuped
            if (cell.hasMine)
            {
                i--;
                continue;
            }

            cell.hasMine = true;
            cell.content = 'M';
            //cell.SetText("M");
        }
        // after drawing mines set numbers
        setNumbers?.Invoke();
    }
}
