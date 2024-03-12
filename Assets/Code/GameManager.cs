using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int minesAmount = 5;

    public static Cell[,] cells;
    public static int width;
    public static int height;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("SetMines", .1f);
    }

    void SetMines()
    {
        for (int i = 0; i < minesAmount; i++)
        {
            Cell cell = cells[Random.Range(0, width), Random.Range(0, height)];
            cell.hasMine = true;
            cell.SetText("M");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
