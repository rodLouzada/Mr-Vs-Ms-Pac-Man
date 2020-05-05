using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine.UI;
using System.IO;
using System.Globalization;
using TMPro;
using UnityEngine.SceneManagement;

public class GridController : MonoBehaviour
{
    public int table; //table set 0 for maze, 1 empty no maze, 2 small board

    public Table grid;

    public GameObject MrP;
    public GameObject MsP;
    public int MrPx = 0;
    public int MrPy = 17;

    public int MsPx = 28;
    public int MsPy = 0;

    public float mr_reward = 0.0f;

    public float ms_reward = 0.0f;

    public GameObject sCandy;
    public GameObject bCandy;

    public TMP_InputField file_name_txt;
    public TMP_InputField load_file_name_txt;

    // display the number of points
    public Text txtMrScore;
    public Text txtMsScore;
    public Text txtMrGamesWon;
    public Text txtMsGamesWon;
    public Text txtStep_MS;
    public Text txtStep_MR;

    public float current_score_mr = 0.0f;
    public float current_score_ms = 0.0f;

    public float curr_match_score_mr = 0.0f;
    public float curr_match_score_ms = 0.0f;
    public int games_won_mr = 0;
    public int games_won_ms = 0;

    // log files of wins and points scored
    public string ms_pac_man_wins_log = "";
    public string mr_pac_man_wins_log = "";
    public string ms_pac_man_points_log = "";
    public string mr_pac_man_points_log = "";

    // Start is called before the first frame update
    void Start()
    {
        if (table == 0)
        {
            grid = new Table();
        }
        else if (table == 1)
        {
            grid = new Table(1);
            ResetTable();
        }
        else if (table == 2)
        {
            grid = new Table("a");
            MrPx = 0;
            MrPy = 6;

            MsPx = 4;
            MsPy = 0;
            ResetTable();
        }

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
            Movement(2, 1);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Movement(2, 2);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Movement(2, 3);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Movement(2, 4);
        }
    }


    /*Function to check if moving to opponent cell
     * 
     */
    public int CheckIfBusy(int agentX, int agentY, int desiredAgentX, int desiredAgentY, int opponentx, int opponenty)
    {
        if (opponentx == desiredAgentX && opponenty == desiredAgentY) //If opponent is there
        {
            if ((grid.GetCell(agentY, agentX).Pm.Big && grid.GetCell(opponenty, opponentx).Pm.Big) || (!grid.GetCell(agentY, agentX).Pm.Big && !grid.GetCell(opponenty, opponentx).Pm.Big)) // same size
            {
                //Debug.Log("Cant move there, opponent blocking ");
                return 1;
            }
            else if ((grid.GetCell(agentY, agentX).Pm.Big && !grid.GetCell(opponenty, opponentx).Pm.Big))
            {   
                //Debug.Log("AGENT WIN");
                this.AddWin(0); // ms pac man wins
                
                this.ResetTable();
                return 2;
            }
            else
                //Debug.Log("OPPONENT WIN");
                this.AddWin(1); // mr pac man wins
                this.ResetTable();
                return 3;
        }
        else

            return 0;
    }

    /* Function to implement moviment for agents
     * @Player: 1 for Mr Pac-Man and 2 for Ms Pac-Man
     * @Direction: 1 up. 2 down, 3 left, 4 right
     */
    public int Movement(int player, int direction)
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
                    if (checker == 0) // no problem
                    {
                        grid.GetCell(MrPy + 1, MrPx).Pm = grid.GetCell(MrPy, MrPx).Pm;
                        grid.GetCell(MrPy, MrPx).Pm = null;
                        MrPy++;

                        MrP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 270);
                        MrP.gameObject.transform.position = new Vector3(MrPx, MrPy, 0);
                        //Debug.Log("Up: " + MrPy + " - " + MrP.transform.position);
                        PrintTable();
                    }

                }

            }
            //Move down
            else if (direction == 2) //Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (MrPy > 0 && !grid.GetCell(MrPy - 1, MrPx).Closed)
                {
                    checker = CheckIfBusy(MrPx, MrPy, MrPx, MrPy - 1, MsPx, MsPy);
                    if (checker == 0)
                    {
                        grid.GetCell(MrPy - 1, MrPx).Pm = grid.GetCell(MrPy, MrPx).Pm;
                        grid.GetCell(MrPy, MrPx).Pm = null;
                        MrPy--;

                        MrP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
                        MrP.gameObject.transform.position = new Vector3(MrPx, MrPy, 0);
                        //Debug.Log("Down " + MrPy + " - " + MrP.transform.position);
                        PrintTable();
                    }
                }

            }
            //Move left
            else if (direction == 3) //Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (MrPx > 0 && !grid.GetCell(MrPy, MrPx - 1).Closed)
                {
                    checker = CheckIfBusy(MrPx, MrPy, MrPx - 1, MrPy, MsPx, MsPy);
                    if (checker == 0)
                    {
                        grid.GetCell(MrPy, MrPx - 1).Pm = grid.GetCell(MrPy, MrPx).Pm;
                        grid.GetCell(MrPy, MrPx).Pm = null;
                        MrPx--;

                        MrP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                        MrP.gameObject.transform.position = new Vector3(MrPx, MrPy, 0);
                        //Debug.Log("Down " + MrPy + " - " + MrP.transform.position);
                        PrintTable();
                    }
                }

            }
            //Move right
            else if (direction == 4) //Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (MrPx < 28 && !grid.GetCell(MrPy, MrPx + 1).Closed)
                {
                    checker = CheckIfBusy(MrPx, MrPy, MrPx + 1, MrPy, MsPx, MsPy);
                    if (checker == 0)
                    {
                        grid.GetCell(MrPy, MrPx + 1).Pm = grid.GetCell(MrPy, MrPx).Pm;
                        grid.GetCell(MrPy, MrPx).Pm = null;
                        MrPx++;

                        MrP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
                        MrP.gameObject.transform.position = new Vector3(MrPx, MrPy, 0);
                        //Debug.Log("Down " + MrPy + " - " + MrP.transform.position);
                        PrintTable();
                    }
                }

            }
            else //WRONG DIRECTION NUMBER
            {
                //Debug.Log("This stay in place");
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
                grid.GetCell(MrPy, MrPx).Pm.StepsNumb = 100;
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
                        //Debug.Log("Up: " + MsPy + " - " + MsP.transform.position);
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
                        //Debug.Log("Down " + MsPy + " - " + MsP.transform.position);
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
                        //Debug.Log("Down " + MsPy + " - " + MsP.transform.position);
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
                        //Debug.Log("Down " + MsPy + " - " + MsP.transform.position);
                        PrintTable();
                    }
                }

            }
            else
            {
                //Debug.Log("This stay in place");
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
                grid.GetCell(MsPy, MsPx).Pm.StepsNumb = 100;                         // Reset number of steps you can be big
                grid.GetCell(MsPy, MsPx).Pm.Big = true;
                grid.GetCell(MsPy, MsPx).Candy = 0;                                 // Remove candy from cell

                grid.GetCell(MsPy, MsPx).Reward = -0.05f;
            }
            //Remove candy from cell
            else if (grid.GetCell(MsPy, MsPx).Candy > 0)
            {
                grid.GetCell(MsPy, MsPx).Candy = 0;
                grid.GetCell(MsPy, MsPx).Reward = -0.05f;
            }
        }
        else
        {
            //Debug.Log("Inserted wrong player number\nenter 1 for mr and 2 for ms\n player number: " + player);
        }

        return checker;

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
        //Debug.Log(st);
    }

    public void ClearLogs(){
        this.ms_pac_man_wins_log = "";
        this.mr_pac_man_wins_log = "";
        this.ms_pac_man_points_log = "";
        this.mr_pac_man_points_log = "";
    }

    /* This method reset candies and agents positions without erasing what have been learned already
     */
    public void ResetTable()
    {   

        // reset current match scores
        this.curr_match_score_mr = 0.0f;
        this.curr_match_score_ms = 0.0f;

        if (table == 0)
        {
            // Sending agetns to the right place

            int OldMrPx = MrPx;
            int OldMrPy = MrPy;

            int OldMsPx = MsPx;
            int OldMsPy = MsPy;
            int MrnewPosX;
            int MrnewPosy;
            int MsnewPosX;
            int MsnewPosy;

            do
            {
                MrnewPosX = (int)Random.Range(0.0f, 28.0f);//0;
                MrnewPosy = (int)Random.Range(0.0f, 17.0f);//17;
            } while (grid.GetCell(MrnewPosy, MrnewPosX).Closed);

            MrPx = MrnewPosX;
            MrPy = MrnewPosy;

            do
            {
                MsnewPosX = (int)Random.Range(0.0f, 28.0f);
                MsnewPosy = (int)Random.Range(0.0f, 17.0f);

            } while (((MsnewPosX == MrPx) && (MsnewPosy == MrPy)) || (grid.GetCell(MsnewPosy, MsnewPosX).Closed));

            MsPx = MsnewPosX;
            MsPy = MsnewPosy;//0;

            MrP.gameObject.transform.position = new Vector3(MrPx, MrPy, 0);
            MrP.gameObject.transform.localScale = new Vector3(2f, 2f, 1);
            MrP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
            MsP.gameObject.transform.position = new Vector3(MsPx, MsPy, 0);
            MsP.gameObject.transform.localScale = new Vector3(2f, 2f, 1);
            MsP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

            //Setting them candies back to place
            PacMan MRpm = grid.GetCell(OldMrPy, OldMrPx).Pm; //making copy of the adress of the PacMan objects 
            PacMan MSpm = grid.GetCell(OldMsPy, OldMsPx).Pm;  //making copy of the adress of the PacMan objects 
            grid.GetCell(OldMrPy, OldMrPx).Pm = null;        // Erasing PM aderess from that cell, pacMan is no longer there
            grid.GetCell(OldMsPy, OldMsPx).Pm = null;        // Erasing PM aderess from that cell, pacMan is no longer there

            MRpm.StepsNumb = 0;
            MSpm.StepsNumb = 0;

            for (int i = 0; i < 18; i++)
            {
                for (int j = 0; j < 29; j++)
                {
                    if (i == MrnewPosy && j == MrnewPosX) //Places Mr PacMan on new cell
                    {
                        MRpm.Big = false;
                        grid.GetCell(i, j).Pm = MRpm;
                    }
                    else if (i == MsnewPosy && j == MsnewPosX)//Places Ms PacMan on new cell
                    {
                        MSpm.Big = false;
                        grid.GetCell(i, j).Pm = MSpm;
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
                        // dont do anything, live your life or something. Here there is only walls 
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
                        grid.GetCell(i, j).Candy = 2;
                        Instantiate(bCandy, new Vector3(j, i, 0), Quaternion.identity);
                    }
                    else //if not just set empty cells
                    {
                        grid.GetCell(i, j).Candy = 1;
                        Instantiate(sCandy, new Vector3(j, i, 0), Quaternion.identity);
                    }

                }
            }
        }
        //////////////////////////////////////////////////////
        ///FOR AN EMPTY SECOND TABLE
        else if(table == 1)
        {
            // Sending agetns to the right place

            int OldMrPx = MrPx;
            int OldMrPy = MrPy;

            int OldMsPx = MsPx;
            int OldMsPy = MsPy;
            int MrnewPosX;
            int MrnewPosy;
            int MsnewPosX;
            int MsnewPosy;

            MrPx = 0;
            MrPy = 17;

            MsPx = 28;
            MsPy = 0;//0;

            MrP.gameObject.transform.position = new Vector3(MrPx, MrPy, 0);
            MrP.gameObject.transform.localScale = new Vector3(2f, 2f, 1);
            MrP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
            MsP.gameObject.transform.position = new Vector3(MsPx, MsPy, 0);
            MsP.gameObject.transform.localScale = new Vector3(2f, 2f, 1);
            MsP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

            //Setting them candies back to place
            PacMan MRpm = grid.GetCell(OldMrPy, OldMrPx).Pm; //making copy of the adress of the PacMan objects 
            PacMan MSpm = grid.GetCell(OldMsPy, OldMsPx).Pm;  //making copy of the adress of the PacMan objects 
            grid.GetCell(OldMrPy, OldMrPx).Pm = null;        // Erasing PM aderess from that cell, pacMan is no longer there
            grid.GetCell(OldMsPy, OldMsPx).Pm = null;        // Erasing PM aderess from that cell, pacMan is no longer there

            MRpm.StepsNumb = 0;
            MSpm.StepsNumb = 0;

            for (int i = 0; i < 18; i++)
            {
                for (int j = 0; j < 29; j++)
                {
                    if (i == 17 && j == 0) //Places Mr PacMan on new cell
                    {
                        MRpm.Big = false;
                        grid.GetCell(i, j).Pm = MRpm;
                    }
                    else if (i == 0 && j == 28)//Places Ms PacMan on new cell
                    {
                        MSpm.Big = false;
                        grid.GetCell(i, j).Pm = MSpm;
                    }
                    else if ((i == 1 && (j == 1 || j == 27 || j == 4 || j == 24 || j == 7 || j == 21 || j == 10 || j == 18 || j == 13 || j == 15)) ||  //If it is any of these, than it has a special candy
                        (i == 16 && (j == 1 || j == 27 || j == 4 || j == 24 || j == 7 || j == 21 || j == 10 || j == 18 || j == 13 || j == 15)) ||
                        (i == 4 && (j == 1 || j == 27 || j == 4 || j == 24 || j == 7 || j == 21 || j == 10 || j == 18 || j == 13 || j == 15)) ||
                        (i == 13 && (j == 1 || j == 27 || j == 4 || j == 24 || j == 7 || j == 21 || j == 10 || j == 18 || j == 13 || j == 15)) ||
                        (i == 7 && (j == 1 || j == 27 || j == 4 || j == 24 || j == 7 || j == 21 || j == 10 || j == 18 || j == 13 || j == 15)) ||
                        (i == 10 && (j == 1 || j == 27 || j == 4 || j == 24 || j == 7 || j == 21 || j == 10 || j == 18 || j == 13 || j == 15))
                        )
                    {
                        grid.GetCell(i, j).Candy = 2;
                        Instantiate(bCandy, new Vector3(j, i, 0), Quaternion.identity);
                    }
                    else //if not just set empty cells
                    {
                        grid.GetCell(i, j).Candy = 1;
                        Instantiate(sCandy, new Vector3(j, i, 0), Quaternion.identity);
                    }

                }
            }
        }
        ///////////////////////////////////////////////////
        /// IF ITS THE SMALL BOARD
        else if(table == 2)
        {

            // Sending agetns to the right place

            int OldMrPx = MrPx;
            int OldMrPy = MrPy;

            int OldMsPx = MsPx;
            int OldMsPy = MsPy;
            int MrnewPosX;
            int MrnewPosy;
            int MsnewPosX;
            int MsnewPosy;

            MrPx = 0;
            MrPy = 6;

            MsPx = 4;
            MsPy = 0;//0;

            MrP.gameObject.transform.position = new Vector3(MrPx, MrPy, 0);
            MrP.gameObject.transform.localScale = new Vector3(2f, 2f, 1);
            MrP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
            MsP.gameObject.transform.position = new Vector3(MsPx, MsPy, 0);
            MsP.gameObject.transform.localScale = new Vector3(2f, 2f, 1);
            MsP.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

            //Setting them candies back to place
            PacMan MRpm = grid.GetCell(OldMrPy, OldMrPx).Pm; //making copy of the adress of the PacMan objects 
            PacMan MSpm = grid.GetCell(OldMsPy, OldMsPx).Pm;  //making copy of the adress of the PacMan objects 
            grid.GetCell(OldMrPy, OldMrPx).Pm = null;        // Erasing PM aderess from that cell, pacMan is no longer there
            grid.GetCell(OldMsPy, OldMsPx).Pm = null;        // Erasing PM aderess from that cell, pacMan is no longer there

            MRpm.StepsNumb = 0;
            MSpm.StepsNumb = 0;

            for (int i = 0; i < 18; i++)
            {
                for (int j = 0; j < 29; j++)
                {
                    if (i == 6 && j == 0) //Places Mr PacMan on new cell
                    {
                        MRpm.Big = false;
                        grid.GetCell(i, j).Pm = MRpm;
                    }
                    else if (i == 0 && j == 4)//Places Ms PacMan on new cell
                    {
                        MSpm.Big = false;
                        grid.GetCell(i, j).Pm = MSpm;
                    }
                    else if (i == 3 && j == 2)
                    {
                        grid.GetCell(i, j).Candy = 2;
                        Instantiate(bCandy, new Vector3(j, i, 0), Quaternion.identity);
                    }
                    else if ((i == 0 && j < 5) ||  //If it is simple candy
                (i == 1 && j < 5) ||
                (i == 2 && j < 5) ||
                (i == 3 && j != 2 && j < 5) ||
                (i == 4 && j < 5) ||
                (i == 5 && j < 5) ||
                (i == 6 && j < 5)
                )
                    {
                        grid.GetCell(i, j).Candy = 1;
                        Instantiate(sCandy, new Vector3(j, i, 0), Quaternion.identity);
                    }

                }
            }
        }
    }

    //xml variables
    public ListsGrid xml_grid;

    /*
     * This  Method is not pretty but it works
     * I did tried to creat a proper XML file with all the pretty nodes, but this file would get too large and crash unity
     * To solve this I decided to reduce the number of nodes to 4 and store data as really long senquences of digits
     * Then I will create my own parser to break this string in subsections and insert info back do the grid
     * To correctly identify later what belongs to where I used the following special char:
     * # Represents the end of data of a cell
     * ; Represents a new value
     * ! Works for a 2d array, it represents the end of a row
     */
    public void CreateXMLItems()
    {

        for (int i = 0; i < 18; i++)
        {
            for (int j = 0; j < 29; j++)
            {
                if (!grid.GetCell(i, j).Closed)
                {


                    for (int a = 0; a < 5; a++)
                    {
                        xml_grid.MrActionList = xml_grid.MrActionList + grid.GetCell(i, j).action_Mr[a] + ";";
                        xml_grid.MsActionList = xml_grid.MsActionList + grid.GetCell(i, j).action_Ms[a] + ";";
                    }

                    for (int qa = 0; qa < 5; qa++)
                    {
                        for (int qb = 0; qb < 5; qb++)
                        {
                            xml_grid.MrQ = xml_grid.MrQ + grid.GetCell(i, j).Q_Mr[qa, qb] + ";";
                            xml_grid.MsQ = xml_grid.MsQ + grid.GetCell(i, j).Q_Ms[qa, qb] + ";";
                        }
                        xml_grid.MrQ = xml_grid.MrQ  +"!";
                        xml_grid.MsQ = xml_grid.MsQ + "!";
                    }

                    xml_grid.MrActionList = xml_grid.MrActionList + "#";
                    xml_grid.MsActionList = xml_grid.MsActionList + "#";

                    xml_grid.MrQ = xml_grid.MrQ + "#";
                    xml_grid.MsQ = xml_grid.MsQ + "#";
                }

            }
        }
    }

    /*
     * This  Method is not pretty but work
     * It gets the very long string I created and goes spliting on special characters and adding to the correct cells
     * # Represents the end of data of a cell
     * ; Represents a new value
     * ! Works for a 2d array, it represents the end of a row
     */
    public void ParseXMLItems()
    {
       /* xml_grid.MrActionList;
        xml_grid.MsActionList;
        xml_grid.MrQ;
        xml_grid.MsQ;*/
        string mral, msal, mrql, msql, auxr,auxs;
        int lindex;
        System.String[] mrai, msai, mrqi, msqi;
        for (int i = 0; i < 18; i++)
        {
            for (int j = 0; j < 29; j++)
            {
                //Execute this per each cell
                if (!grid.GetCell(i, j).Closed)
                {

                    lindex = xml_grid.MrActionList.IndexOf("#");                       // Finds where this cell's data ends
                    mral = xml_grid.MrActionList.Substring(0, lindex);                 // Get the data of this cell as one smaller bus still long string
                    xml_grid.MrActionList.Remove(0, lindex + 1);                       // Remove the part we just got from original string
                    mrai = System.Text.RegularExpressions.Regex.Split(mral, ";");      // Use split method to get array of values

                    lindex = xml_grid.MsActionList.IndexOf("#");                       // Repeat exactly the same for Ms Pacman information
                    msal = xml_grid.MsActionList.Substring(0, lindex);
                    xml_grid.MsActionList.Remove(0, lindex + 1);
                    msai = System.Text.RegularExpressions.Regex.Split(msal, ";");

                    for (int a = 0; a < 5; a++)
                    {
                        grid.GetCell(i, j).action_Mr[a] = float.Parse(mrai[a]);        // Now send data back to the grid
                        grid.GetCell(i, j).action_Ms[a] = float.Parse(msai[a]);
                    }

                    lindex = xml_grid.MrQ.IndexOf("#");                               // Similarly to actions, get all the data from this cell
                    mrql = xml_grid.MrQ.Substring(0, lindex);
                    xml_grid.MrQ.Remove(0, lindex + 1);

                    lindex = xml_grid.MsQ.IndexOf("#");                              // And do the same for Ms. P
                    msql = xml_grid.MsQ.Substring(0, lindex);
                    xml_grid.MsQ.Remove(0, lindex + 1);

                    for (int qa = 0; qa < 5; qa++)
                    {
                        lindex = mrql.IndexOf("!");                                 // Now the data we got is a 2D array, we need to do further splinting so we can feed each row of this array
                        auxr = mrql.Substring(0, lindex);
                        mrql.Remove(0, lindex + 1);
                        mrqi = System.Text.RegularExpressions.Regex.Split(auxr, ";");

                        lindex = msql.IndexOf("!");
                        auxs = msql.Substring(0, lindex);
                        msql.Remove(0, lindex + 1);
                        msqi = System.Text.RegularExpressions.Regex.Split(auxs, ";");


                        for (int qb = 0; qb < 5; qb++)
                        {
                            grid.GetCell(i, j).Q_Mr[qa, qb] = float.Parse(mrqi[qb]);
                            grid.GetCell(i, j).Q_Ms[qa, qb] = float.Parse(msqi[qb]); ;
                        }
                    }

                }

            }
        }
    }

    public void WriteXML()
    {
        CreateXMLItems();
        XmlSerializer serializer = new XmlSerializer(typeof(ListsGrid));
        FileStream stream = new FileStream( "C:/xml/" + file_name_txt.text + ".xml", FileMode.Create); //new FileStream(Application.dataPath + "/xml/"+ file_name_txt.text + ".xml", FileMode.Create);//+"+ System.DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss") + ".xml", FileMode.Create);
        serializer.Serialize(stream, xml_grid);
        stream.Close();
    }
    public string MrActionList = "";
    public string MsActionList = "";
    public string MrQ = "";
    public string MsQ = "";
    public void LoadXML()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(ListsGrid));
        FileStream stream = new FileStream("C:/xml/" + load_file_name_txt.text+ ".xml", FileMode.Open);
        xml_grid = serializer.Deserialize(stream) as ListsGrid;
        MrActionList = xml_grid.MrActionList;
        MsActionList = xml_grid.MsActionList;
        MrQ = xml_grid.MrQ;
        MsQ = xml_grid.MsQ;
        //Debug.Log("Started Parsing");
        ParseXMLItems();
        //Debug.Log("Finished parsing");
        stream.Close();
    }


    [System.Serializable]
    public class ListsGrid
    {
        [XmlElement("cells")]
        public string MrActionList = "";
        public string MsActionList = "";
        public string MrQ = "";
        public string MsQ = "";
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddWin(int playerID){
        if(playerID == 0){ // ms pac man wins
            this.games_won_ms += 1;
            txtMsGamesWon.text = this.games_won_ms.ToString();

        }else if(playerID == 1){
            this.games_won_mr += 1;
            txtMrGamesWon.text = this.games_won_mr.ToString();
        }

        ms_pac_man_wins_log += ((this.games_won_ms).ToString() + ";");
        mr_pac_man_wins_log += ((this.games_won_mr).ToString() + ";");

        ms_pac_man_points_log += ((this.current_score_ms).ToString() + ";");
        mr_pac_man_points_log += ((this.current_score_mr).ToString() + ";");

    }

    public void AddPoints(int playerID, float points){
        if(playerID == 0){
            this.curr_match_score_ms += points;
            this.current_score_ms += points;
            this.txtMsScore.text = this.current_score_ms.ToString();
            // txtMsScore.GetComponent<Text> = this.curr_match_score_ms;
            

        }
        else if(playerID == 1){
            this.curr_match_score_mr += points;
            this.current_score_mr += points;
            this.txtMrScore.text = this.current_score_mr.ToString();

            // txtMrScore = this.current_score_mr;
        }
        
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
    public float reward;                       // R[s]
    public float utility_Mr;                      // V[s]
    public float utility_Ms;
    bool closed;
    PacMan pm;
    int candy;                           // 0 no candy, 1 regular candy, 2 special candy
    public float[] action_Mr;                        // pi: probability of each action being taken (0 stand, 1 up, 2 down, 3 left, 4 right)
    public float[] action_Ms;
    public float[,] q_Mr = new float[5, 5];        // Q[s,a,o] = 5X5 matrix to each state
    public float[,] q_Ms = new float[5, 5];



    public Cell(int row, int col, float reward, float utility, bool closed, PacMan pm, int candy)
    {
        Row = row;
        Col = col;
        Reward = reward;
        utility_Mr = utility;
        utility_Ms = utility;
        Closed = closed;
        Pm = pm;
        Candy = candy;
        action_Mr = new float[] { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f }; // 1/5 for each value
        action_Ms = new float[] { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f };

        for (int a = 0; a < q_Mr.GetLength(0); a++)
        {
            for (int b = 0; b < q_Mr.GetLength(1); b++)
            {
                q_Mr[a, b] = 1.0f;
                q_Ms[a, b] = 1.0f;
            }
        }
    }

    public Cell(int row, int col) //Overwrite contructor to create walls
    {
        Row = row;
        Col = col;
        Reward = 0;
        utility_Mr = 0;
        utility_Ms = 0;
        Candy = 0;
        Closed = true;
    }

    public int Row { get => row; set => row = value; }
    public int Col { get => col; set => col = value; }
    public float Reward { get => reward; set => reward = value; }
    public float Utility_Mr { get => utility_Mr; set => utility_Mr = value; }
    public float Utility_Ms { get => utility_Ms; set => utility_Ms = value; }
    public bool Closed { get => closed; set => closed = value; }
    public PacMan Pm { get => pm; set => pm = value; }
    public int Candy { get => candy; set => candy = value; }
    public float[] ActionMr { get => action_Mr; set => action_Mr = value; }
    public float[] ActionMs { get => action_Ms; set => action_Ms = value; }
    public float[,] Q_Mr { get => q_Mr; set => q_Mr = value; }
    public float[,] Q_Ms { get => q_Ms; set => q_Ms = value; }

    public float GetActionMr(int x) => ActionMr[x];
    public float GetActionMs(int x) => ActionMs[x];
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
    public int row = 18;
    public int col = 29;

    float explor;
    float decay;
    float learning_rate;
    float discount_factor;



    Cell[,] gd = new Cell[18, 29];

    public Table() //Constructor of the game maze
    {
        //Setting learning and training variables
        Explor = 0.2f;                             // DEFINITION OF EXPLORATION FACTOR
        Decay = 0.9999954f;                        // DECAY 
        Learning_rate = 1f;                        // LEARNING RATE
        Discount_factor = 0.9f;                    // DISCOUNT FACTOR

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


    public Table(int a) //Constructor of the game maze
    {
        //Setting learning and training variables
        Explor = 0.2f;                             // DEFINITION OF EXPLORATION FACTOR
        Decay = 0.9999954f;                        // DECAY 
        Learning_rate = 1f;                        // LEARNING RATE
        Discount_factor = 0.9f;                    // DISCOUNT FACTOR

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
                else if ((i == 1 && (j == 1 || j == 27 || j == 4 || j == 24 || j == 7 || j == 21 || j == 10 || j == 18 || j == 13 || j == 15)) ||  //If it is any of these, than it has a special candy
                    (i == 16 && (j == 1 || j == 27 || j == 4 || j == 24 || j == 7 || j == 21 || j == 10 || j == 18 || j == 13 || j == 15)) ||
                    (i == 4 && (j == 1 || j == 27 || j == 4 || j == 24 || j == 7 || j == 21 || j == 10 || j == 18 || j == 13 || j == 15)) ||
                    (i == 13 && (j == 1 || j == 27 || j == 4 || j == 24 || j == 7 || j == 21 || j == 10 || j == 18 || j == 13 || j == 15)) ||
                    (i == 7 && (j == 1 || j == 27 || j == 4 || j == 24 || j == 7 || j == 21 || j == 10 || j == 18 || j == 13 || j == 15)) ||
                    (i == 10 && (j == 1 || j == 27 || j == 4 || j == 24 || j == 7 || j == 21 || j == 10 || j == 18 || j == 13 || j == 15))
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

    public Table(string a) //Constructor of the game maze
    {
        row = 7;
        col = 5;
        //Setting learning and training variables
        Explor = 0.2f;                             // DEFINITION OF EXPLORATION FACTOR
        Decay = 0.9999954f;                        // DECAY 
        Learning_rate = 1f;                        // LEARNING RATE
        Discount_factor = 0.9f;                    // DISCOUNT FACTOR

        for (int i = 0; i < 18; i++)
        {
            for (int j = 0; j < 29; j++)
            {
                if (i == 6 && j == 0) //Places Mr PacMan on (17,0)
                {
                    PacMan mr = new PacMan(false, 0, true);
                    gd[i, j] = new Cell(i, j, 0, 0.5f, false, mr, 1);
                }
                else if (i == 0 && j == 4)//Places Ms PacMan on (0,28)
                {
                    PacMan ms = new PacMan(false, 0, false);
                    gd[i, j] = new Cell(i, j, 0, 0.5f, false, ms, 1);
                }
                else if (i == 3 && j == 2)
                {
                    gd[i, j] = new Cell(i, j, 0, 0.5f, false, null, 2);
                }
                else if ((i == 0 && j < 5) ||  //If it is simple candy
                    (i == 1 && j < 5) ||
                    (i == 2 && j < 5) ||
                    (i == 3 && j != 2 && j < 5) || 
                    (i == 4 && j < 5) ||
                    (i == 5 && j < 5) ||
                    (i == 6 && j < 5)
                    )
                {
                    gd[i, j] = new Cell(i, j, 0, 0.5f, false, null, 1);
                }
                else //if not just set walls
                {
                    gd[i, j] = new Cell(i, j);
                }

            }
        }

    }


    public float Explor { get => explor; set => explor = value; }
    public float Decay { get => decay; set => decay = value; }
    public float Learning_rate { get => learning_rate; set => learning_rate = value; }
    public float Discount_factor { get => discount_factor; set => discount_factor = value; }

    /*
* Return Cell at specific position
*/
    public Cell GetCell(int i, int j)
    {
        return gd[i, j];
    }

    // are there candies remaining on the board?
    public bool isNoCandies(){

        for (int y = 0; y< this.gd.GetLength(0); y++){

            for(int x = 0; x < this.gd.GetLength(1); x++){
                if(gd[y,x].Candy > 0){
                    return false; // there is a candy left
                }
            }

        }
        return true; // there are no candies left


    }
}

