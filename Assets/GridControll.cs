﻿using System.Collections;
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
        MrP.gameObject.transform.localScale = new Vector3(2f, 2f, 1);
        MrP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
        MsP.gameObject.transform.position = new Vector3(MsPx, MsPy, 0);
        MsP.gameObject.transform.localScale = new Vector3(2f, 2f, 1);
        MsP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            /*int player = Random.Range(1, 3);
            int direction = Random.Range(1, 5);
        */
            Moviment(2, 1);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Moviment(2, 2);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Moviment(2, 3);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Moviment(2, 4);
        }
    }

    /*Fucntion to check if moving to opponent cell
     * 
     */
    public int CheckIfBusy(int agentX, int agentY, int desiredAgentX, int desiredAgentY, int opponentx, int opponenty)
    {
        if (opponentx == desiredAgentX && opponenty == desiredAgentY) //If opponent is there
        {
            if ((grid.GetCell(agentY, agentX).Pm.Big && grid.GetCell(opponenty, opponentx).Pm.Big) || (!grid.GetCell(agentY, agentX).Pm.Big && !grid.GetCell(opponenty, opponentx).Pm.Big)) // same size
            {
                Debug.Log("Cant move there, opponent blocking ");
                return 1;
            }
            else if ((grid.GetCell(agentY, agentX).Pm.Big && !grid.GetCell(opponenty, opponentx).Pm.Big))
            {
                Debug.Log("AGENT WIN");
                return 2;
            }
            else
                Debug.Log("OPPONENT WIN");
            return 3;
        }
        else
            return 0;
    }

    /* Function to implement moviment for agents
     * @Player: 1 for Mr Pac-Man and 2 for Ms Pac-Man
     * @Direction: 1 up. 2 down, 3 left, 4 right
     */
    public void Moviment(int player, int direction)
    {
        int checker = -1;
        // Moving Mr Pac Man
        if (player == 1)
        {   //Move up
            if (direction == 1) //Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (MrPy < 17 && !grid.GetCell(MrPy + 1, MrPx).Closed)
                {
                    checker = CheckIfBusy(MrPx, MrPy, MrPx, MrPy + 1, MsPx, MsPy);
                    if(checker == 0) 
                    {
                        grid.GetCell(MrPy + 1, MrPx).Pm = grid.GetCell(MrPy, MrPx).Pm;
                        grid.GetCell(MrPy, MrPx).Pm = null;
                        MrPy++;

                        MrP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 270);
                        MrP.gameObject.transform.position = new Vector3(MrPx, MrPy, 0);
                        Debug.Log("Up: " + MrPy + " - " + MrP.transform.position);
                        PrintTable();
                    }

                }

            }
            //Move down
            else if (direction == 2) //Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (MrPy > 0 && !grid.GetCell(MrPy - 1, MrPx).Closed)
                {
                    checker = CheckIfBusy(MrPx, MrPy,MrPx, MrPy - 1, MsPx, MsPy);
                    if (checker == 0)
                    {
                        grid.GetCell(MrPy - 1, MrPx).Pm = grid.GetCell(MrPy, MrPx).Pm;
                        grid.GetCell(MrPy, MrPx).Pm = null;
                        MrPy--;

                        MrP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
                        MrP.gameObject.transform.position = new Vector3(MrPx, MrPy, 0);
                        Debug.Log("Down " + MrPy + " - " + MrP.transform.position);
                        PrintTable();
                    }
                }

            }
            //Move left
            else if (direction == 3) //Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (MrPx > 0 && !grid.GetCell(MrPy, MrPx - 1).Closed)
                {
                    checker = CheckIfBusy(MrPx, MrPy, MrPx - 1 , MrPy, MsPx, MsPy);
                    if (checker == 0)
                    {
                        grid.GetCell(MrPy, MrPx - 1).Pm = grid.GetCell(MrPy, MrPx).Pm;
                        grid.GetCell(MrPy, MrPx).Pm = null;
                        MrPx--;

                        MrP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                        MrP.gameObject.transform.position = new Vector3(MrPx, MrPy, 0);
                        Debug.Log("Down " + MrPy + " - " + MrP.transform.position);
                        PrintTable();
                    }
                }

            }
            //Move right
            else if (direction == 4) //Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (MrPx < 28 && !grid.GetCell(MrPy, MrPx + 1).Closed)
                {
                    checker = CheckIfBusy(MrPx, MrPy, MrPx + 1, MrPy , MsPx, MsPy);
                    if (checker == 0)
                    {
                        grid.GetCell(MrPy, MrPx + 1).Pm = grid.GetCell(MrPy, MrPx).Pm;
                        grid.GetCell(MrPy, MrPx).Pm = null;
                        MrPx++;

                        MrP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
                        MrP.gameObject.transform.position = new Vector3(MrPx, MrPy, 0);
                        Debug.Log("Down " + MrPy + " - " + MrP.transform.position);
                        PrintTable();
                    }
                }

            }
            else //WRONG DIRECTION NUMBER
            {
                Debug.Log("Inserted wrong direction number\nenter 1 Up, 2 Down, 3 Left, 4 Right\n Dir number: " + direction);
            }

            // STEP CHECK
            if (grid.GetCell(MrPy, MrPx).Pm.StepsNumb > 0) // If you have steps left being big, decrease number of steps left
            {
                grid.GetCell(MrPy, MrPx).Pm.StepsNumb--;
                if (grid.GetCell(MrPy, MrPx).Pm.StepsNumb == 0) //if we have no steps left, we turn small again 
                {
                    MrP.gameObject.transform.localScale = new Vector3(2f, 2f, 1); //Resset size to small
                    grid.GetCell(MrPy, MrPx).Pm.Big = false;
                }
            }
            // CHECK FOR BIG CANDY
            if (grid.GetCell(MrPy, MrPx).Candy == 2)
            {
                MrP.gameObject.transform.localScale = new Vector3(3.5f, 3.5f, 1); //Set size to BIG
                grid.GetCell(MrPy, MrPx).Pm.StepsNumb = 15;
                grid.GetCell(MrPy, MrPx).Pm.Big = true;
                grid.GetCell(MrPy, MrPx).Candy = 0;
            }
            else if (grid.GetCell(MrPy, MrPx).Candy > 0)
            {
                grid.GetCell(MrPy, MrPx).Candy = 0;
            }

        }

        //MOVING MS PAC MAN
        else if (player == 2)
        {
            //Move UP
            if (direction == 1) //Input.GetKeyDown(KeyCode.W))
            {
                if (MsPy < 17 && !grid.GetCell(MsPy + 1, MsPx).Closed)
                {
                    checker = CheckIfBusy(MsPx, MsPy, MsPx, MsPy + 1, MrPx, MrPy);
                    if (checker == 0)
                    {
                        grid.GetCell(MsPy + 1, MsPx).Pm = grid.GetCell(MsPy, MsPx).Pm;
                        grid.GetCell(MsPy, MsPx).Pm = null;
                        MsPy++;

                        MsP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 270);
                        MsP.gameObject.transform.position = new Vector3(MsPx, MsPy, 0);
                        Debug.Log("Up: " + MsPy + " - " + MsP.transform.position);
                        PrintTable();
                    }
                }

            }
            //Move DOWN
            else if (direction == 2) //Input.GetKeyDown(KeyCode.S))
            {
                if (MsPy > 0 && !grid.GetCell(MsPy - 1, MsPx).Closed)
                {
                    checker = CheckIfBusy(MsPx, MsPy, MsPx, MsPy - 1, MrPx, MrPy);
                    if (checker == 0)
                    {
                        grid.GetCell(MsPy - 1, MsPx).Pm = grid.GetCell(MsPy, MsPx).Pm;
                        grid.GetCell(MsPy, MsPx).Pm = null;
                        MsPy--;

                        MsP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
                        MsP.gameObject.transform.position = new Vector3(MsPx, MsPy, 0);
                        Debug.Log("Down " + MsPy + " - " + MsP.transform.position);
                        PrintTable();
                    }
                }

            }
            //Move LEFT
            else if (direction == 3) //Input.GetKeyDown(KeyCode.A))
            {
                if (MsPx > 0 && !grid.GetCell(MsPy, MsPx - 1).Closed)
                {
                    checker = CheckIfBusy(MsPx, MsPy, MsPx - 1, MsPy, MrPx, MrPy);
                    if (checker == 0)
                    {
                        grid.GetCell(MsPy, MsPx - 1).Pm = grid.GetCell(MsPy, MsPx).Pm;
                        grid.GetCell(MsPy, MsPx).Pm = null;
                        MsPx--;

                        MsP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                        MsP.gameObject.transform.position = new Vector3(MsPx, MsPy, 0);
                        Debug.Log("Down " + MsPy + " - " + MsP.transform.position);
                        PrintTable();
                    }
                }

            }
            //Move RIGHT
            else if (direction == 4) //Input.GetKeyDown(KeyCode.D))
            {
                if (MsPx < 28 && !grid.GetCell(MsPy, MsPx + 1).Closed)
                {
                    checker = CheckIfBusy(MsPx, MsPy, MsPx + 1, MsPy, MrPx, MrPy);
                    if (checker == 0)
                    {

                        grid.GetCell(MsPy, MsPx + 1).Pm = grid.GetCell(MsPy, MsPx).Pm;
                        grid.GetCell(MsPy, MsPx).Pm = null;
                        MsPx++;

                        MsP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
                        MsP.gameObject.transform.position = new Vector3(MsPx, MsPy, 0);
                        Debug.Log("Down " + MsPy + " - " + MsP.transform.position);
                        PrintTable();
                    }
                }

            }
            else
            {
                Debug.Log("Inserted wrong direction number\nenter 1 Up, 2 Down, 3 Left, 4 Right\n Dir number: " + direction);
            }
            // STEP CHECK
            if (grid.GetCell(MsPy, MsPx).Pm.StepsNumb > 0)                          // If you have steps left being big, decrease number of steps left
            {
                grid.GetCell(MsPy, MsPx).Pm.StepsNumb--;
                if (grid.GetCell(MsPy, MsPx).Pm.StepsNumb == 0)                     //if we have no steps left, we turn small again 
                {
                    MsP.gameObject.transform.localScale = new Vector3(2f, 2f, 1);   //Resset size to small
                    grid.GetCell(MsPy, MsPx).Pm.Big = false;
                }
            }
            // CHECK FOR BIG CANDY
            if (grid.GetCell(MsPy, MsPx).Candy == 2)
            {
                MsP.gameObject.transform.localScale = new Vector3(3.5f, 3.5f, 1);   //Set size to BIG
                grid.GetCell(MsPy, MsPx).Pm.StepsNumb = 15;                         // Reset number of steps you can be big
                grid.GetCell(MsPy, MsPx).Pm.Big = true;
                grid.GetCell(MsPy, MsPx).Candy = 0;                                 // Remove candy from cell
            }
            //Remove candy from cell
            else if (grid.GetCell(MsPy, MsPx).Candy > 0)
            {
                grid.GetCell(MsPy, MsPx).Candy = 0;
            }
        }
        else
        {
            Debug.Log("Inserted wrong player number\nenter 1 for mr and 2 for ms\n player number: " + player);
        }


    }

    public void PrintTable()
    {
        string st = "|";
        for (int i = 17; i >= 0; i--)
        {
            for (int j = 0; j < 29; j++)
            {
                if (grid.GetCell(i, j).Closed)
                {
                    st = st + "x | ";
                }
                else if (grid.GetCell(i, j).Pm != null && grid.GetCell(i, j).Pm.Mr)
                {
                    st = st + "r | ";
                }
                else if (grid.GetCell(i, j).Pm != null && !grid.GetCell(i, j).Pm.Mr)
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


public class PacMan
{
    bool big;
    int stepsBig;
    bool mr;

    public PacMan(bool big, int stepsBig, bool mr)
    {
        this.big = big;
        this.stepsBig = stepsBig;
        this.mr = mr;
    }

    public bool Mr { get => mr; set => mr = value; }
    public bool Big { get => big; set => big = value; }
    public int StepsNumb { get => stepsBig; set => stepsBig = value; }

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
    PacMan pm;
    int candy; // 0 no candy, 1 regular candy, 2 special candy

    public Cell(int row, int col, float reward, float utility, bool closed, PacMan pm, int candy)
    {
        Row = row;
        Col = col;
        Reward = reward;
        Utility = utility;
        Closed = closed;
        Pm = pm;
        Candy = candy;
    }

    public Cell(int row, int col) //Overwrite contructor to create walls
    {
        Row = row;
        Col = col;
        Reward = 0;
        Utility = 0;
        Candy = 0;
        Closed = true;
    }

    public int Row { get => row; set => row = value; }
    public int Col { get => col; set => col = value; }
    public float Reward { get => reward; set => reward = value; }
    public float Utility { get => utility; set => utility = value; }
    public bool Closed { get => closed; set => closed = value; }
    public PacMan Pm { get => pm; set => pm = value; }
    public int Candy { get => candy; set => candy = value; }
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
                    PacMan mr = new PacMan(false, 0, true);
                    gd[i, j] = new Cell(i, j, 0, 0.5f, false, mr, 1);
                }
                else if (i == 0 && j == 28)//Places Ms PacMan on (0,28)
                {
                    PacMan ms = new PacMan(false, 0, false);
                    gd[i, j] = new Cell(i, j, 0, 0.5f, false, ms, 1);
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
                else if ((i == 0 && (j == 2 || j == 15)) ||  //If it is any of these, than it has a special candy
                    (i == 2 && (j == 21)) ||
                    (i == 4 && (j == 2 || j == 9)) ||
                    (i == 7 && (j == 24)) ||
                    (i == 8 && (j == 10 || j == 17)) ||
                    (i == 12 && (j == 9 || j == 25)) ||
                    (i == 14 && (j == 15)) ||
                    (i == 15 && (j == 5)) ||
                    (i == 17 && (j == 28))
                    )
                {
                    gd[i, j] = new Cell(i, j, 0, 0.5f, false, null, 2);
                }
                else //if not just set empty cells
                {
                    gd[i, j] = new Cell(i, j, 0, 0.5f, false, null, 1);
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