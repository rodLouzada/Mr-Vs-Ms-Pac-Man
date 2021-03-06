﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Agent_Handler : MonoBehaviour
{
    GridController gridController; 
    public bool agentsRunning = true; // should the agent loop run?
    bool start = false;
    bool isTraining = true; // should the agents be in training mode? or just testing mode?
    public TMP_InputField training_if_txt;
    public TMP_InputField test_if_txt;
    MrPacManAgent mrPacMan;
    MsPacManAgent msPacMan;
    public int max_steps = 2500; // Board steps
    int max_training_steps = 10000; //After these steps traning will stop

    public Toggle rdm_tgl, q_tgl,mm_tgl, o_rdm_tgl, o_q_tgl, o_mm_tgl;
    public bool rdm_select, q_select, mm_select, o_rdm_select, o_q_select, o_mm_select;

    public int curr_step = 0;
    public int training_curr_step = 0;

    // sotre the current state and whatever state is moved into for learning
    Cell mr_curr_state;
    Cell mr_new_state;

    Cell ms_curr_state;
    Cell ms_new_state;

    int action_return_mr;
    int action_return_ms;


    // Start is called before the first frame update
    void Start()
    {
        //initalize both agents

        // access grid controller
        gridController = GetComponent<GridController>();
        //yield return new WaitForSeconds(3); // wait for 3 seconds
        
        // begin taking actions
       // StartCoroutine(performAgentLoop());

    }

    private void Update()
    {
        
        if (start)
        {


            gridController.grid.Decay = Mathf.Pow(10, (Mathf.Log10(0.01f ) / max_training_steps));

            gridController.ClearLogs();

            curr_step = 0;
            training_curr_step = 0;


            // check for strategy toggle button

            rdm_select = rdm_tgl.isOn;
            q_select = q_tgl.isOn;
            mm_select = mm_tgl.isOn;

            o_rdm_select = o_rdm_tgl.isOn;
            o_q_select = o_q_tgl.isOn;
            o_mm_select = o_mm_tgl.isOn;

            StartCoroutine(performAgentLoop());
            start = false;
        }

    }
    // The main agent loop
    IEnumerator performAgentLoop()
    {   
        

        float mr_step_reward;
        float ms_step_reward;

        
        int opponent_agent_strategy_type = -1; // 0 => random; 1 => q-learning; 2 => minimax-q
        if (o_rdm_select)
        {
            opponent_agent_strategy_type = 0;
            //Debug.Log("opponent random");
        }
        else if (o_q_select)
        {
            opponent_agent_strategy_type = 1;
            //Debug.Log("opponent q");

        }
        else if (o_mm_select)
        {
            opponent_agent_strategy_type = 2;
            //Debug.Log("opponent minimax");

        }

        int ms_pac_man_agent_startegy_type = -1;
        if (rdm_select)
        {
            ms_pac_man_agent_startegy_type = 0;
            //Debug.Log("agent random");

        }
        else if (q_select)
        {
            ms_pac_man_agent_startegy_type = 1;
            //Debug.Log("agent qlearning");

        }
        else if (mm_select)
        {
            ms_pac_man_agent_startegy_type = 2;
            //Debug.Log("agent minimax");

        }

        // yield return new WaitForSeconds(2); // wait for 1 second
        mrPacMan = new MrPacManAgent(gridController.grid.Explor, gridController.grid.Decay, gridController.grid.Learning_rate, gridController.grid.Discount_factor, opponent_agent_strategy_type, isTraining);
        msPacMan = new MsPacManAgent(gridController.grid.Explor, gridController.grid.Decay, gridController.grid.Learning_rate, gridController.grid.Discount_factor, ms_pac_man_agent_startegy_type, isTraining);

        



        while(agentsRunning){
            
            // yield return new WaitForSeconds(2); // wait for 1 second
            yield return null;
            
            // get the current state
            mr_curr_state = gridController.grid.GetCell(gridController.MrPy, gridController.MrPx);
            ms_curr_state = gridController.grid.GetCell(gridController.MsPy, gridController.MsPx);

            int mr_pac_man_action;
            int ms_pac_man_action;

            // get each agent's action        
            mr_pac_man_action = mrPacMan.getAction(gridController.grid.GetCell(gridController.MrPy, gridController.MrPx));
            ms_pac_man_action = msPacMan.getAction(gridController.grid.GetCell(gridController.MsPy, gridController.MsPx));

            // in a random order, apply each agents action
            if(UnityEngine.Random.Range(0,2) == 0){
                
                // fist player will always get a chance to move
                // calculate reward based on chosen step and current state
                mr_step_reward = calculateStepReward(mr_curr_state, mr_pac_man_action,1); // update mr pac man's score before moving
                action_return_mr = applyMrPacManAction(mr_pac_man_action);
                //Debug.Log("mr step reward: " + mr_step_reward);


                // second player might have been eaten
                if(action_return_mr == 2){
                    mr_step_reward = 100f;
                    ms_step_reward = -100f;
                }else if(action_return_mr == 3)
                { // or get to eat
                    mr_step_reward = -100f;
                    ms_step_reward = 100f;
                }
                else{
                    // calculate the reward after the other agent has already completed their action, but before taking the action
                    ms_step_reward = calculateStepReward(ms_curr_state, ms_pac_man_action,0);
                    action_return_ms = applyMsPacManAction(ms_pac_man_action);
                }

            }else{ //otherwise ms pac man goes first
                ms_step_reward = calculateStepReward(ms_curr_state, ms_pac_man_action,0);
                action_return_ms =  applyMsPacManAction(ms_pac_man_action);
                //Debug.Log("ms step reward: " + ms_step_reward);
                if (action_return_ms == 2)
                {
                    mr_step_reward = -100f;
                    ms_step_reward = 100f;
                }
                else if (action_return_ms == 3)
                { // or get to eat
                    mr_step_reward = 100f;
                    ms_step_reward = -100f;
                }
                else
                { // most of the time mr will just move
                    
                    mr_step_reward = calculateStepReward(mr_curr_state, mr_pac_man_action,1); // update mr pac man's score before moving
                    action_return_mr =  applyMrPacManAction(mr_pac_man_action);
                }
            }

            // update global score
            gridController.AddPoints(1, mr_step_reward);
            //gridController.mr_reward += mr_step_reward;
            gridController.AddPoints(0, ms_step_reward);
            //gridController.ms_reward += ms_step_reward;
            training_curr_step += 1;

            gridController.txtStep_MS.text = training_curr_step.ToString();
            gridController.txtStep_MR.text = training_curr_step.ToString();

            // get the new state of both players
            mr_new_state = gridController.grid.GetCell(gridController.MrPy, gridController.MrPx);
            ms_new_state = gridController.grid.GetCell(gridController.MsPy, gridController.MsPx);

            //Debug.Log("took actions");

            // LEARN

            //each agent should recieve some kind of reward
            // probably use multithreading so both agents can learn in parallel
            if(isTraining){ // only learn if in training mode, not testing mode
                mrPacMan.learn(mr_curr_state, mr_new_state, mr_step_reward, mr_pac_man_action, ms_pac_man_action); // q learning does not use opponent's action
                msPacMan.learn(ms_curr_state, ms_new_state, ms_step_reward, ms_pac_man_action, mr_pac_man_action); // minimax q 
            }

            if(curr_step >= max_steps){
                //agentsRunning = false; // stop the current thread from running
                gridController.ResetTable(); // reset the game table
                curr_step = 0;
                //StartCoroutine(performAgentLoop()); // start a new loop of training on the new table
            }else{
                curr_step++;
            }

            if(gridController.grid.isNoCandies() == true){
                if(gridController.curr_match_score_mr > gridController.curr_match_score_ms){ // if mr has more points this match
                    gridController.AddWin(1); // he gets a win
                }else if(gridController.curr_match_score_ms > gridController.curr_match_score_mr){ // if ms has more points this match
                    gridController.AddWin(0);
                    //gridController.games_won_ms += 1; // she gets a win
                }
                gridController.ResetTable();
            }

            // if game over the stop the agents from running
            if(training_curr_step >= max_training_steps){
                // exit training
                //Debug.Log("QUIT TRAINING");
                agentsRunning = false;
                gridController.ResetTable();
            }
        }
        
    }

    


    /* Function to send movement decision for MrPacMan
     * @Player: 1 for Mr Pac-Man and 2 for Ms Pac-Man
     * @Direction: 0 up. 1 down, 2 left, 3 right, 4 pass
     */
    int applyMrPacManAction(int actionID){
        //Debug.Log("Taking action:" + actionID);
        
        if(actionID == 0){
            return gridController.Movement(1, 1);
        }else if(actionID ==1){
            return gridController.Movement(1, 2);
        }else if(actionID == 2){
            return gridController.Movement(1, 3);
        }else if(actionID == 3){
            return gridController.Movement(1, 4);
        }else if(actionID == 4){
            return gridController.Movement(2, 5);
        }else{
            //Debug.Log("entered invalid action");
            return -1;
        }

    }

    int applyMsPacManAction(int actionID){

        if(actionID == 0){
            return gridController.Movement(2, 1);
        }else if(actionID ==1){
            return gridController.Movement(2, 2);
        }else if(actionID == 2){
            return gridController.Movement(2, 3);
        }else if(actionID == 3){
            return gridController.Movement(2, 4);
        }else if(actionID == 4){
            return gridController.Movement(2, 5);
        }else{
            //Debug.Log("entered invalid action");
            return -1;
        }

    }

    /*
     * Get text from input field to set the number of steps
     */
    public void setMaxTrainingSteps()
    {

        max_training_steps = int.Parse(training_if_txt.text);
        agentsRunning = true;
        isTraining = true;
        //max_steps = 2500;
        start = true;
    }
    
    public void setToTestMode(){
        gridController.ResetTable();
        isTraining = false;
        max_training_steps = int.Parse(test_if_txt.text);
        max_steps = int.MaxValue;
        agentsRunning = true;
        start = true;
    }

    /**public void setMaxTestingSteps(int numSteps){
        max_training_steps = numSteps;
    }**/
    
    
    // 
    float calculateStepReward(Cell curr_cell, int action_index, int playerID){
        // Cell curr_cell;
        Cell new_cell;
        int curr_coord_x = curr_cell.Col;
        int curr_coord_y = curr_cell.Row;


        Debug.Log("curr_coord_x: "+ curr_coord_x + " curr_coord_y:" + curr_coord_y);
        int new_coord_x;
        int new_coory_y;

        // curr_cell = gridController.grid.GetCell(curr_coord_y, curr_coord_x);
        
        if(curr_cell.Pm == null){
            Cell ms_curr_state = gridController.grid.GetCell(gridController.MsPy, gridController.MsPx);
            Cell mr_curr_state = gridController.grid.GetCell(gridController.MrPy, gridController.MrPx);
        
            if(playerID == 0){
                curr_cell = ms_curr_state;
            }else if(playerID == 1){
                curr_cell = mr_curr_state;
            }
        
        }


        //Debug.Log("current coordinate x: " + curr_coord_x + " coord y : " + curr_coord_y + "  action: " + action_index);
        if (curr_cell.Pm == null)
        {
            //Debug.Log("#######GOTCHA######");
            mr_curr_state = gridController.grid.GetCell(gridController.MrPy, gridController.MrPx);
            ms_curr_state = gridController.grid.GetCell(gridController.MsPy, gridController.MsPx);
            curr_cell = gridController.grid.GetCell(curr_coord_y, curr_coord_x);
            return -100f;
        }

        // get direction based on action
        if (action_index == 0){
            if(curr_coord_y == gridController.grid.row-1){
                new_cell = gridController.grid.GetCell(curr_coord_y, curr_coord_x);
            }else{
                new_cell = gridController.grid.GetCell(curr_coord_y+1, curr_coord_x); // determine new cell coordinates
            }

        }else if(action_index == 1){
            if(curr_coord_y == 0){
                new_cell = gridController.grid.GetCell(curr_coord_y, curr_coord_x);
            }else{
                new_cell = gridController.grid.GetCell(curr_coord_y-1, curr_coord_x);
            }
        }else if(action_index == 2){
            if(curr_coord_x == 0){
                new_cell = gridController.grid.GetCell(curr_coord_y, curr_coord_x);
            }else{
                new_cell = gridController.grid.GetCell(curr_coord_y, curr_coord_x-1);
            }
        }else if(action_index == 3){
            if(curr_coord_x >= gridController.grid.col -1){
                new_cell = gridController.grid.GetCell(curr_coord_y, curr_coord_x);
            }else{
                new_cell = gridController.grid.GetCell(curr_coord_y, curr_coord_x+1);
            }
        }else{
            new_cell = gridController.grid.GetCell(curr_coord_y, curr_coord_x);
        }        

        //Debug.Log("is the current cell a big pm? :" + curr_cell.Pm.Big);
        //Debug.Log("is there a pm in new cell : " + (new_cell.Pm != null));

        // is there a small or big orb in the new cell
        if (new_cell.Candy == 1){ // small candy
            return 1.0f;
        }else if(new_cell.Candy == 2){ // big candy
            return 5.0f;

        }else if(new_cell.Pm != null && !new_cell.Closed){ // is there a player in the new cell?
            // is the player in the current cell big or small?
            if(new_cell.Pm.Big && curr_cell.Pm.Big == false){ // is the new cell player big and I'm small
                return -100f;
            }else if(new_cell.Pm.Big == false && curr_cell.Pm.Big){
                return 100f; //eat 'em
            }
        }






        return -0.05f;

    }



}
