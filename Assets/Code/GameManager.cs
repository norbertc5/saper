using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int distanceToMine = 3;
    private bool canReloadScene;
    private static bool isGameOver;

    public static int minesAmount = 5;
    public static int width;
    public static int height;
    public static int placedFlags;

    // references from scene
    public static Cell[,] cells;
    public static List<Cell> cellsWithMine = new List<Cell>();
    private static LineRenderer cellFrame;
    private static GameObject gameOverPanel;
    public static Button digButton;
    public static Button mineButton;

    public delegate void MinesAction(int x, int y);
    public delegate void NumbersAction();
    public static MinesAction setMines;
    public static NumbersAction setNumbers;
    public static TextMeshProUGUI minesAmountDisplay;

    /*
     * First click sets mines postions
     * Next, each click reveals one cell
    */

    // Start is called before the first frame update
    void Start()
    {
        cellFrame = FindObjectOfType<LineRenderer>();
        digButton = FindObjectsOfType<Button>()[1];
        mineButton = FindObjectsOfType<Button>()[0];
        minesAmountDisplay = GameObject.Find("MinesAmountDisplay").GetComponent<TextMeshProUGUI>();
        gameOverPanel = GameObject.Find("GameOverPanel");
        SetGUIActive(false);
        gameOverPanel.SetActive(false);
        isGameOver = false;
        placedFlags = 0;

        // firstly generate mines
        setMines += SetMines;
    }

    private void OnDisable()
    {
        setMines -= SetMines;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            // it determines if after click gui shows
            if (t.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(t.fingerId))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(t.position), Vector2.zero);

                if (hit)
                    SetGUIActive(!hit.collider.GetComponent<Cell>().hasRevealed);
                else
                    SetGUIActive(false);
            }
        }
        else if(gameOverPanel.activeSelf)
            canReloadScene = true;

        // reload scene when game over and tap on screen
        if (isGameOver && Input.touchCount > 0 && canReloadScene)
        {
            SceneManager.LoadScene(0);
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
            cellsWithMine.Add(cell);
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

    public static void GameOver()
    {
        Debug.Log("Game Over");
        gameOverPanel.SetActive(true);
        isGameOver = true;
    }
}
