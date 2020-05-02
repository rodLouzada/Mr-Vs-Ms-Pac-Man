using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent_Handler : MonoBehaviour
{
    GridController gridController; 
    public bool agentsRunning = true; // should the agent loop run?

    QLearningAgent mrPacMan;
    MinimaxQAgent msPacMan;

    // Start is called before the first frame update
    void Start()
    {
        //initalize both agents

        // access grid controller
        gridController = GetComponent<GridController>();
        //yield return new WaitForSeconds(3); // wait for 3 seconds
        
        // begin taking actions
        StartCoroutine(performAgentLoop());

    }

    // The main agent loop
    IEnumerator performAgentLoop()
    {
        // yield return new WaitForSeconds(2); // wait for 1 second
        mrPacMan = new QLearningAgent(.2f, 0.9999954f,.01f, 0.9f);
        msPacMan = new MinimaxQAgent(.2f, 0.9999954f,.01f, 0.9f);

        // sotre the current state and whatever state is moved into for learning
        Cell mr_curr_state;
        Cell mr_new_state;

        Cell ms_curr_state;
        Cell ms_new_state;

        mr_curr_state = gridController.grid.GetCell(gridController.MrPy, gridController.MrPx);
        ms_curr_state = gridController.grid.GetCell(gridController.MsPy, gridController.MsPx);

        while(agentsRunning){
            
            // yield return new WaitForSeconds(2); // wait for 1 second
            yield return null;

            int mr_pac_man_action;
            int ms_pac_man_action;

            // get each agent's action        
            mr_pac_man_action = mrPacMan.getAction(gridController.grid.GetCell(gridController.MrPy, gridController.MrPx));
            ms_pac_man_action = msPacMan.getAction(gridController.grid.GetCell(gridController.MsPy, gridController.MsPx));

            // in a random order, apply each agents action
            if(Random.Range(0,2) == 0){
                applyMrPacManAction(mr_pac_man_action);
                applyMsPacManAction(ms_pac_man_action);
            }else{ //otherwise ms pac man goes first
                applyMsPacManAction(ms_pac_man_action);
                applyMrPacManAction(mr_pac_man_action);
            }

            // get the new state of both players
            mr_new_state = gridController.grid.GetCell(gridController.MrPy, gridController.MrPx);
            ms_new_state = gridController.grid.GetCell(gridController.MsPy, gridController.MsPx);

            Debug.Log("took actions");

            // LEARN

            //each agent should recieve some kind of reward
            // probably use multithreading so both agents can learn in parallel
            mrPacMan.learn(mr_curr_state, mr_new_state, mr_new_state.reward, ms_pac_man_action); // q learning does not use opponent's action
            msPacMan.learn(ms_curr_state, ms_new_state, ms_new_state.reward, ms_pac_man_action, mr_pac_man_action); // minimax q 

        }
        
    }

    


    /* Function to send movement decision for MrPacMan
     * @Player: 1 for Mr Pac-Man and 2 for Ms Pac-Man
     * @Direction: 0 up. 1 down, 2 left, 3 right, 4 pass
     */
    void applyMrPacManAction(int actionID){
        Debug.Log("Taking action:" + actionID);
        
        if(actionID == 0){
            gridController.Movement(1, 1);
        }else if(actionID ==1){
            gridController.Movement(1, 2);
        }else if(actionID == 2){
            gridController.Movement(1, 3);
        }else if(actionID == 3){
            gridController.Movement(1, 4);
        }else if(actionID == 4){
            gridController.Movement(2, 5);
        }else{
            Debug.Log("entered invalid action");
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
        }else if(actionID == 4){
            gridController.Movement(2, 5);
        }else{
            Debug.Log("entered invalid action");
        }

    }



}
