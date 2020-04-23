using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent_Handler : MonoBehaviour
{
    GridController gridController; 
    public bool agentsRunning = true; // should the agent loop run?

    QLearningAgent mrPacMan;

    // Start is called before the first frame update
    void Start()
    {
        //initalize both agents

        // access grid controller
        gridController = GetComponent<GridController>();
        mrPacMan = new QLearningAgent(.2f, 0.9999954f,1f, 0.9f);
        //yield return new WaitForSeconds(3); // wait for 3 seconds
        
        // begin taking actions
        StartCoroutine(performAgentLoop());

    }

    // The main agent loop
    IEnumerator performAgentLoop()
    {
        while(agentsRunning){
            
            yield return new WaitForSeconds(1); // wait for 1 second

            int mr_pac_man_action;
            int ms_pac_man_action;

            // get each agent's action        
            mr_pac_man_action = mrPacMan.getAction(gridController.grid.GetCell(gridController.MrPx, gridController.MrPy));
            ms_pac_man_action = Random.Range(0,6);

            // in a random order, apply each agents action
            if(Random.Range(0,2) == 0){
                applyMrPacManAction(mr_pac_man_action);
                applyMsPacManAction(ms_pac_man_action);
            }else{ //otherwise ms pac man goes first
                applyMsPacManAction(ms_pac_man_action);
                applyMrPacManAction(mr_pac_man_action);
            }

            Debug.Log("took actions");

            //each agent should recieve some kind of reward
            // @TODO

        }
        
    }

    


    /* Function to send movement decision for MrPacMan
     * @Player: 1 for Mr Pac-Man and 2 for Ms Pac-Man
     * @Direction: 0 up. 1 down, 2 left, 3 right, 4 pass
     */
    void applyMrPacManAction(int actionID){

        if(actionID == 0){
            gridController.Movement(1, 1);
        }else if(actionID ==1){
            gridController.Movement(1, 2);
        }else if(actionID == 2){
            gridController.Movement(1, 3);
        }else if(actionID == 3){
            gridController.Movement(1, 4);
        }

    }

    void applyMsPacManAction(int actionID){

        if(actionID == 0){
            gridController.Movement(2, 1);
        }else if(actionID ==1){
            gridController.Movement(2, 2);
        }else if(actionID == 2){
            gridController.Movement(2, 3);
        }else if(actionID == 3){
            gridController.Movement(2, 4);
        }

    }



}
