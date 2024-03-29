using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private static int minesAmount = 5;
    [SerializeField] private int distanceToMine = 3;
    public static int unrevealesCellsAmount;

    public static Cell[,] cells;
    public static int width;
    public static int height;
    static LineRenderer cellFrame;

    public delegate void MinesAction(int x, int y);
    public delegate void NumbersAction();
    public static MinesAction setMines;
    public static NumbersAction setNumbers;
    public static Button digButton;
    public static Button mineButton;

    /*
     * First click sets mines postions
     * Next, each click reveals one cell
    */

    private void Awake()
    {
        cellFrame = FindObjectOfType<LineRenderer>();
        digButton = FindObjectsOfType<Button>()[1];
        mineButton = FindObjectsOfType<Button>()[0];
    }

    // Start is called before the first frame update
    void Start()
    {
        // firstly generate mines
        setMines += SetMines;
        unrevealesCellsAmount = width * height;
    }

    private void Update()
    {
        // it determines if after click gui shows
        if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if(hit)
                SetGUIActive(!hit.collider.GetComponent<Cell>().hasRevealed);
            else
                SetGUIActive(false);
        }
    }

    /// <summary>
    /// Draw random cells and set it as mines.
    /// </summary>
    void SetMines(int x, int y)
    {
        for (int i = 0; i < minesAmount; i++)
        {
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
        }
        // after drawing mines set numbers
        setNumbers?.Invoke();
    }

    /// <summary>
    /// Moves cell frame with buttons to pointed position.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="cellSize"></param>
    public static void SetCellFrame(Vector2 pos)
    {
        cellFrame.transform.parent.position = pos;
    }

    /// <summary>
    /// Shows or hides cellFrame and button above it. 
    /// </summary>
    /// <param name="value"></param>
    public static void SetGUIActive(bool value)
    {
        cellFrame.transform.parent.gameObject.SetActive(value);
    }

    /// <summary>
    /// Determines how many cells are still unrevealed.
    /// </summary>
    /// <returns></returns>
    public static IEnumerator CountUnrevealed()
    {
        yield return new WaitForSeconds(.1f);  // delay to let all cells to reveal

        // I've tried to make it better performance by just decrement unrevealesCellsAmount but it was making issues
        int revealedCellsAmount = 0;
        foreach (Cell cell in cells)
        {
            if (cell.hasRevealed || cell.isFlagged)
            {
                revealedCellsAmount++;
            }
        }
        unrevealesCellsAmount = 81 - revealedCellsAmount;

        if (unrevealesCellsAmount <= 0)
            Debug.Log("Wszystkie pola ods³oniête lub oflagowane");
    }

    public static void GameOver()
    {
        Debug.Log("Game Over");
    }
}
