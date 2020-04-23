﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent_Handler : MonoBehaviour
{
    GridController gridController; 
    public bool agentsRunning = true; // should the agent loop run?

    // Start is called before the first frame update
    void Start()
    {
        //initalize both agents
        gridController = GetComponent<GridController>();
        
        //yield return new WaitForSeconds(3); // wait for 3 seconds
        
        // begin taking actions
        StartCoroutine(performAgentLoop());

    }

    // The main agent loop
    IEnumerator performAgentLoop()
    {
        while(agentsRunning){
            
            yield return new WaitForSeconds(1); // wait for 1 second

            // get each agent's action        
            int mr_pac_man_action;
            int ms_pac_man_action;

            // for now just use random actions
            mr_pac_man_action = Random.Range(0,5);
            ms_pac_man_action = Random.Range(0,5);

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

    

    void init_ms_pac_man(){

    }

    /* Function to call movement for MrPacMan
     * @Player: 1 for Mr Pac-Man and 2 for Ms Pac-Man
     * @Direction: 1 up. 2 down, 3 left, 4 right
     */
    void applyMrPacManAction(int actionID){

        if(actionID == 1){
            gridController.Movement(1, 1);
        }else if(actionID ==2){
            gridController.Movement(1, 2);
        }else if(actionID == 3){
            gridController.Movement(1, 3);
        }else if(actionID == 4){
            gridController.Movement(1, 4);
        }

    }

    void applyMsPacManAction(int actionID){

        if(actionID == 1){
            gridController.Movement(2, 1);
        }else if(actionID ==2){
            gridController.Movement(2, 2);
        }else if(actionID == 3){
            gridController.Movement(2, 3);
        }else if(actionID == 4){
            gridController.Movement(2, 4);
        }

    }

    // algorithm itself.
    // Variables from the environment are: the state set, S; the
    // action set, A; the opponent’s action set, O; and the discount factor, gamma. 
    
    /* The variables internal to the learner are: 
        a learning rate, alpha, which is initialized to 1.0 and decays
        over time; 
        the agent’s estimate of the-function, Q; 
        the agent’s estimate of the -function, V; 
        and the agent’s current policy for state ;, pi[s,.]. 
        
        The remaining variables are parameters of the algorithm: explor controls how
        often the agent will deviate from its current policy to ensure that the state space is adequately explored, and decay
        controls the rate at which the le
     */

    // update is Q(s,a) := r + decay * V(s')
    int determine_best_action(){

    }

}
