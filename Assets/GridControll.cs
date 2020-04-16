using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridControll : MonoBehaviour
{
    Table grid;
    public GameObject MrP;
    public GameObject MsP;
    int MrPx = 0;
    int MrPy = 17;

    int MsPx = 28;
    int MsPy = 0;
    // Start is called before the first frame update
    void Start()
    {
        grid = new Table();
        MrP.gameObject.transform.position = new Vector3(MrPx, MrPy, 0);
        MrP.gameObject.transform.localScale = new Vector3(3.5f, 3.5f, 1);
        MrP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
        MsP.gameObject.transform.position = new Vector3(MsPx, MsPy, 0);
        MsP.gameObject.transform.localScale = new Vector3(3.5f, 3.5f, 1);
        MsP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(MrPy < 17 && !grid.GetCell( MrPy + 1, MrPx).Closed)
            {
                grid.GetCell(MrPy, MrPx).Mr = false;
                MrPy++;
                grid.GetCell(MrPy, MrPx).Mr = true;
                MrP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 270);
                MrP.gameObject.transform.position = new Vector3(MrPx, MrPy, 0);
                Debug.Log("Up: " + MrPy + " - " + MrP.transform.position);
                PrintTable();
            }
            
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (MrPy > 0 && !grid.GetCell(MrPy - 1, MrPx).Closed)
            {
                grid.GetCell(MrPy, MrPx).Mr = false;
                MrPy--;
                grid.GetCell(MrPy, MrPx).Mr = true;
                MrP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
                MrP.gameObject.transform.position = new Vector3(MrPx, MrPy, 0);
                Debug.Log("Down " + MrPy + " - " + MrP.transform.position);
                PrintTable();
            }
                
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (MrPx > 0 && !grid.GetCell(MrPy , MrPx - 1).Closed)
            {
                grid.GetCell(MrPy, MrPx).Mr = false;
                MrPx--;
                grid.GetCell(MrPy, MrPx).Mr = true;
                MrP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                MrP.gameObject.transform.position = new Vector3(MrPx, MrPy, 0);
                Debug.Log("Down " + MrPy + " - " + MrP.transform.position);
                PrintTable();
            }
                
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (MrPx < 28 && !grid.GetCell( MrPy, MrPx + 1).Closed)
            {
                grid.GetCell(MrPy, MrPx).Mr = false;
                MrPx++;
                grid.GetCell(MrPy, MrPx).Mr = true;
                MrP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
                MrP.gameObject.transform.position = new Vector3(MrPx, MrPy, 0);
                Debug.Log("Down " + MrPy + " - " + MrP.transform.position);
                PrintTable();
            }
                
        }


        /*
         * Moving MS PAC MAN
         * 
         */

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (MsPy < 17 && !grid.GetCell(MsPy + 1, MsPx).Closed)
            {
                grid.GetCell(MsPy, MsPx).Ms = false;
                MsPy++;
                grid.GetCell(MsPy, MsPx).Ms = true;
                MsP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 270);
                MsP.gameObject.transform.position = new Vector3(MsPx, MsPy, 0);
                Debug.Log("Up: " + MsPy + " - " + MsP.transform.position);
                PrintTable();
            }

        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (MsPy > 0 && !grid.GetCell(MsPy - 1, MsPx).Closed)
            {
                grid.GetCell(MsPy, MsPx).Ms = false;
                MsPy--;
                grid.GetCell(MsPy, MsPx).Ms = true;
                MsP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
                MsP.gameObject.transform.position = new Vector3(MsPx, MsPy, 0);
                Debug.Log("Down " + MsPy + " - " + MsP.transform.position);
                PrintTable();
            }

        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (MsPx > 0 && !grid.GetCell(MsPy, MsPx - 1).Closed)
            {
                grid.GetCell(MsPy, MsPx).Ms = false;
                MsPx--;
                grid.GetCell(MsPy, MsPx).Ms = true;
                MsP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                MsP.gameObject.transform.position = new Vector3(MsPx, MsPy, 0);
                Debug.Log("Down " + MsPy + " - " + MsP.transform.position);
                PrintTable();
            }

        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (MsPx < 28 && !grid.GetCell(MsPy, MsPx + 1).Closed)
            {
                grid.GetCell(MsPy, MsPx).Ms = false;
                MsPx++;
                grid.GetCell(MsPy, MsPx).Ms = true;
                MsP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
                MsP.gameObject.transform.position = new Vector3(MsPx, MsPy, 0);
                Debug.Log("Down " + MsPy + " - " + MsP.transform.position);
                PrintTable();
            }

        }
    }

    public void PrintTable()
    {
        string st = "|";
        for (int i = 17; i >= 0; i--)
        {
            for (int j = 0; j < 29; j++)
            {
               if( grid.GetCell(i, j).Closed)
                {
                    st = st + "x | ";
                }
               else if (grid.GetCell(i, j).Mr)
                {
                    st = st + "r | ";
                }
                else if (grid.GetCell(i, j).Ms)
                {
                    st = st + "s | ";
                }
                else
                    st = st + "   | ";
            }
            st = st + "\n|";
        }
        Debug.Log(st);
    }
}

/*
 *Creates the object Cell, it describe Every state, in this case each cell of the grid
 * Each cell has:
 * @row: row number this cell is at
 * @col: col number this cell is at
 * @reward: reward of each state
 * @utility: the utility for that state
 * @closed: in this example there are cells of the grid that are closed, behaves like walls
 * @mr: is mr pac man in this cell?
 * @ms: is ms pac man in this cell?
 */
public class Cell
{
    int row;
    int col;
    float reward;
    float utility;
    bool closed;
    bool mr;
    bool ms;

    public Cell(int row, int col, float reward, float utility, bool closed, bool mr, bool ms)
    {
        Row = row;
        Col = col;
        Reward = reward;
        Utility = utility;
        Closed = closed;
        Mr = mr;
        Ms = ms;
    }

    public Cell(int row, int col) //Overwrite contructor to create walls
    {
        Row = row;
        Col = col;
        Reward = 0;
        Utility = 0;
        Closed = true;
        Mr = false;
        Ms = false;
    }

    public int Row { get => row; set => row = value; }
    public int Col { get => col; set => col = value; }
    public float Reward { get => reward; set => reward = value; }
    public float Utility { get => utility; set => utility = value; }
    public bool Closed { get => closed; set => closed = value; }
    public bool Mr { get => mr; set => mr = value; }
    public bool Ms { get => ms; set => ms = value; }

    public override string ToString()
    {
        return "( " + Row + ", " + Col + ")";
    }
}


/*
 * This class create the object grid table in which the UAV exists
*/
public class Table
{
    int row = 18;
    int col = 29;

    Cell[,] gd = new Cell[18, 29];

    public Table() //Constructor of the game maze
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (i == 17 && j == 0) //Places Mr PacMan on (17,0)
                {
                    gd[i, j] = new Cell(i, j, 0, 0.5f, false, true, false);
                }
                else if (i == 0 && j == 28)//Places Ms PacMan on (0,28)
                {
                    gd[i, j] = new Cell(i, j, 0, 0.5f, false, false, true);
                }
                else if ((i == 0 && j == 3) ||  //If it is any of these, than its a wall
                    (i == 1 && (j == 1 || j == 3 || j == 5 || j == 6 || j == 7 || j == 8 || j == 9 || j == 10 || j == 11 || j == 12 || j == 13 || j == 14 || j == 16 || j == 17 || j == 18 || j == 20 || j == 21 || j == 22 || j == 23 || j == 24 || j == 25 || j == 26 || j == 27)) ||
                    (i == 2 && (j == 1 || j == 3 || j == 5 || j == 18 || j == 20)) ||
                    (i == 3 && (j == 1 || j == 3 || j == 5 || j == 7 || j == 8 || j == 9 || j == 11 || j == 12 || j == 13 || j == 14 || j == 15 || j == 16 || j == 18 || j == 20 || j == 21 || j == 22 || j == 23 || j == 24 || j == 25 || j == 27)) ||
                    (i == 4 && (j == 1 || j == 5 || j == 27)) ||
                    (i == 5 && (j == 1 || j == 3 || j == 4 || j == 5 || j == 7 || j == 8 || j == 10 || j == 11 || j == 12 || j == 13 || j == 14 || j == 16 || j == 17 || j == 18 || j == 19 || j == 21 || j == 23 || j == 25)) ||
                    (i == 6 && (j == 1 || j == 8 || j == 10 || j == 16 || j == 21 || j == 23 || j == 25 || j == 26 || j == 27)) ||
                    (i == 7 && (j == 3 || j == 4 || j == 5 || j == 6 || j == 8 || j == 10 || j == 11 || j == 13 || j == 14 || j == 15 || j == 16 || j == 18 || j == 19 || j == 20 || j == 21 || j == 23)) ||
                    (i == 8 && (j == 1 || j == 2 || j == 11 || j == 23 || j == 25 || j == 26 || j == 27)) ||
                    (i == 9 && (j == 2 || j == 4 || j == 5 || j == 6 || j == 7 || j == 8 || j == 9 || j == 10 || j == 11 || j == 13 || j == 14 || j == 15 || j == 16 || j == 17 || j == 18 || j == 19 || j == 20 || j == 21 || j == 27)) ||
                    (i == 10 && (j == 1 || j == 2 || j == 4 || j == 21 || j == 23 || j == 24 || j == 25 || j == 26 || j == 27)) ||
                    (i == 11 && (j == 4 || j == 6 || j == 7 || j == 8 || j == 9 || j == 10 || j == 11 || j == 12 || j == 13 || j == 15 || j == 16 || j == 17 || j == 18 || j == 19 || j == 21 || j == 23)) ||
                    (i == 12 && (j == 1 || j == 2 || j == 4 || j == 6 || j == 21 || j == 23 || j == 24 || j == 26 || j == 27)) ||
                    (i == 13 && (j == 4 || j == 6 || j == 8 || j == 10 || j == 11 || j == 12 || j == 13 || j == 14 || j == 15 || j == 16 || j == 17 || j == 18 || j == 19 || j == 21 || j == 27)) ||
                    (i == 14 && (j == 1 || j == 2 || j == 4 || j == 6 || j == 8 || j == 10 || j == 24 || j == 25 || j == 26 || j == 27)) ||
                    (i == 15 && (j == 6 || j == 8 || j == 10 || j == 12 || j == 13 || j == 14 || j == 16 || j == 18 || j == 19 || j == 20 || j == 21 || j == 22)) ||
                    (i == 16 && (j == 1 || j == 2 || j == 3 || j == 4 || j == 5 || j == 6 || j == 8 || j == 10 || j == 14 || j == 16 || j == 18 || j == 24 || j == 25 || j == 26 || j == 27)) ||
                    (i == 17 && (j == 8 || j == 12 || j == 16 || j == 20 || j == 22))
                    )
                {
                    gd[i, j] = new Cell(i, j);
                }
                else //if not just set empty cells
                {
                    gd[i, j] = new Cell(i, j, 0, 0.5f, false, false, false);
                }

            }
        }

    }
    /*
     * Return Cell at specific position
     */
    public Cell GetCell(int i, int j)
    {
        return gd[i, j];
    }
}
