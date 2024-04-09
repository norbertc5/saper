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
    private static bool isEnd;

    public static int minesAmount = 18;
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
    public static TextMeshProUGUI minesAmountDisplay;
    public static RaycastHit2D touchedObject;

    public delegate void MinesAction(int x, int y);
    public delegate void NumbersAction();
    public static MinesAction setMines;
    public static NumbersAction setNumbers;
    public static NumbersAction showMines;

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
        isEnd = false;
        placedFlags = 0;
        minesAmountDisplay.text = minesAmount.ToString();

        // firstly generate mines
        setMines += SetMines;
    }

    private void OnDisable()
    {
        setMines -= SetMines;
        showMines = null;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            // it determines if after click gui shows
            if (t.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(t.fingerId))
            {
                touchedObject = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(t.position), Vector2.zero);

                if (touchedObject && Cell.hasFirstClicked)
                    SetGUIActive(!touchedObject.collider.GetComponent<Cell>().hasRevealed, touchedObject.collider.GetComponent<Cell>().xId);
                else
                    SetGUIActive(false);
            }
        }
        else if(gameOverPanel.activeSelf)
            canReloadScene = true;

        // reload scene when game over and tap on screen
        if (isEnd && Input.touchCount > 0 && canReloadScene)
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
            showMines += () => { cell.transform.GetChild(3).gameObject.SetActive(true); };
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
    public static void SetGUIActive(bool value, int column = -1)
    {
        GameObject guiObj = cellFrame.transform.parent.gameObject;
        guiObj.SetActive(value);

        // flip buttons if clicked on right side of the board and vice versa
        if (column == -1)
            return;
        
        int xScale = (column > width / 2) ? -1 : 1;
        guiObj.transform.Find("DigButton").localScale = new Vector2(.45f * xScale, .45f);
        guiObj.transform.Find("MineButton").localScale = new Vector2(.45f * xScale, .45f);
        guiObj.transform.localScale = new Vector2(4*xScale, 4);
    }

    public static void EndGame(string text)
    {
        //Debug.Log("End");
        gameOverPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        gameOverPanel.SetActive(true);
        isEnd = true;
        SetGUIActive(false);
    }
}
