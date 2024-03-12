using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private Color color1;
    [SerializeField] private Color color2;

    // Start is called before the first frame update
    void Start()
    {
        // generate grid

        // set position of board in middle
        Vector2 position = -(new Vector3(width * cellPrefab.transform.localScale.x, height * cellPrefab.transform.localScale.y) 
            - cellPrefab.transform.localScale) / 2; 

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                var newCell = Instantiate(cellPrefab, position, Quaternion.identity, transform);
                newCell.name = $"Cell ({j}, {i})";
                position.x += cellPrefab.transform.localScale.x;

                // set colors like in chessboard
                if ((i + j) % 2 == 0)
                    newCell.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color1;
                else
                    newCell.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color2;
            }
            position.y += cellPrefab.transform.localScale.y;
            position.x = -(width * cellPrefab.transform.localScale.x - cellPrefab.transform.localScale.x) / 2;
        }
    }
}
