using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    int max_steps = 2500; // Board steps
    int max_training_steps = 10000; //After these steps traning will stop

    public Toggle rdm_tgl, q_tgl,mm_tgl, o_rdm_tgl, o_q_tgl, o_mm_tgl;
    public bool rdm_select, q_select, mm_select, o_rdm_select, o_q_select, o_mm_select;
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
        int curr_step = 0;
        int training_curr_step = 0;

        float mr_step_reward;
        float ms_step_reward;

        
        int opponent_agent_strategy_type = -1; // 0 => random; 1 => q-learning; 2 => minimax-q
        if (o_rdm_select)
        {
            opponent_agent_strategy_type = 0;
        }
        else if (o_q_select)
        {
            opponent_agent_strategy_type = 1;
        }
        else if (o_mm_select)
        {
            opponent_agent_strategy_type = 2;
        }

        int ms_pac_man_agent_startegy_type = -1;
        if (rdm_select)
        {
            opponent_agent_strategy_type = 0;
        }
        else if (q_select)
        {
            opponent_agent_strategy_type = 1;
        }
        else if (mm_select)
        {
            opponent_agent_strategy_type = 2;
        }

        // yield return new WaitForSeconds(2); // wait for 1 second
        mrPacMan = new MrPacManAgent(.2f, 0.9999954f,.01f, 0.9f, opponent_agent_strategy_type, isTraining);
        msPacMan = new MsPacManAgent(.2f, 0.9999954f,.01f, 0.9f, ms_pac_man_agent_startegy_type, isTraining);

        // sotre the current state and whatever state is moved into for learning
        Cell mr_curr_state;
        Cell mr_new_state;

        Cell ms_curr_state;
        Cell ms_new_state;



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

            // calculate reward based on chosen step and current state
            mr_step_reward = calculateStepReward(mr_curr_state.Row, mr_curr_state.Col, mr_pac_man_action); // update mr pac man's score before moving
            ms_step_reward = calculateStepReward(ms_curr_state.Row, ms_curr_state.Col, ms_pac_man_action);

            // update global score
            gridController.mr_reward += mr_step_reward;
            gridController.ms_reward += ms_step_reward;
            
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
                gridController.ResetTable();
            }
            
            training_curr_step += 1;

            // if game over the stop the agents from running
            if(training_curr_step >= max_training_steps){
                // exit training
                Debug.Log("QUIT TRAINING");
                agentsRunning = false;
                gridController.ResetTable();
            }
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

    /*
     * Get text from input field to set the number of steps
     */
    public void setMaxTrainingSteps()
    {

        max_training_steps = int.Parse(training_if_txt.text);
        start = true;
        agentsRunning = true;
        isTraining = true;
        max_steps = 2500;
        agentsRunning = true;
    }
    
    public void setToTestMode(){
        isTraining = false;
        max_training_steps = int.Parse(test_if_txt.text);
        max_steps = int.MaxValue;
        start = true;
        agentsRunning = true;
    }

    /**public void setMaxTestingSteps(int numSteps){
        max_training_steps = numSteps;
    }**/
    
    
    // 
    float calculateStepReward(int curr_coord_y, int curr_coord_x, int action_index){
        Cell curr_cell;
        Cell new_cell;

        curr_cell = gridController.grid.GetCell(curr_coord_y, curr_coord_x);
        
        Debug.Log("current coordinate x: " + curr_coord_x + " coord y : " + curr_coord_y + "  action: " + action_index);

        // get direction based on action
        if(action_index == 0){
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
            if(curr_coord_x == gridController.grid.col -1){
                new_cell = gridController.grid.GetCell(curr_coord_y, curr_coord_x);
            }else{
                new_cell = gridController.grid.GetCell(curr_coord_y, curr_coord_x+1);
            }
        }else{
            new_cell = gridController.grid.GetCell(curr_coord_y, curr_coord_x);
        }



        // is there a small or big orb in the new cell
        if(new_cell.Candy == 1){ // small candy
            return 1.0f;
        }else if(new_cell.Candy == 2){ // big candy
            return 5.0f;
        }else if(new_cell.Pm != null){ // is there a player in the new cell?
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
