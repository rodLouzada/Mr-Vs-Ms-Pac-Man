using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static alglib; // use the linear progrmaming plugin


// the minimax-q agent -- aka Ms. PacMan
public class MinimaxQAgent
{

    // Training parameters
    float alpha;
    float explor = 50.0f;
    float decay;
    float gamma = 1.0f;
    bool isTraining; // is training or in test mode?


    // initialize minimax-q algorithm
    public MinimaxQAgent(float explor, float decay, float learning_rate, float discount_factor, bool isTraining){
        this.alpha = learning_rate;
        this.explor = explor;
        this.decay = decay;
        this.gamma = discount_factor;
        this.isTraining = isTraining;
    }

    public int getAction(Cell state){
        //with probability explor, return an action uniformly at random
        if(UnityEngine.Random.Range(0.0f,1.0f) <= explor && isTraining){
            return UnityEngine.Random.Range(0, 5);
        }

        //otherwise if current state is s
        float rolling_prob_counter = 0.0f;

        float random_prob = UnityEngine.Random.Range(0.0f, 1.0f);

        // get action based on Pi[a] for action i 
        for(int i = 0; i < 5; i++){
            rolling_prob_counter = state.ActionMs[i] + rolling_prob_counter;

            if(random_prob < rolling_prob_counter){ 
                return i;
            }

        }

        return -999; // should not ever make it here if values of Pi sum to 1.0

    }
    
    public void learn(Cell s, Cell s_prime, float reward, int a, int o){
        // after recieving reward rew for moving from state s to s' via action a and opponent's action o

        // let Q[s,a,o] = (1 - alpha) * Q[s,a,o] + alpha * (rew + gamma * V[s'])
        s.q_Ms[a,o] = (1 - alpha) * s.q_Ms[a,o] + (alpha * (s.reward + this.gamma * s_prime.utility_Ms));

        // use linear programming to find pi[s,.] such that: 
            // pi[x,.] = argmax{pi'[s,.], min{o', sum{a', pi[s,a'] * Q[s,a’,o’] }}}
        

        // linear programming == https://www.alglib.net/
        // minlp subpackage
        
        // for each opponent action get the value of this action that leads to the lowest value for the current agent (minimaxQ)
        float opponent_action_minimum_value = Mathf.Infinity;     // a float [] to store the opponents smallest possible value that it can force the player to have
        int opponent_action = 0;                                     // the action the opponent will choose
        float tmp_sum;                                              // the sum of the current a_prime value

        // for each opponent action get the minimum value it could cause this agent to have
        for(int o_prime = 0; o_prime < 5; o_prime++){
            tmp_sum = 0;
            
            // summation
            for(int a_prime = 0; a_prime < 5; a_prime++){
                tmp_sum += ( s.action_Ms[a_prime] * s.q_Ms[a_prime, o_prime] );
            }
            
            // if the current o_prime value is less than the current least then replace the value
            if(opponent_action_minimum_value > tmp_sum){
                opponent_action_minimum_value = tmp_sum;
                opponent_action = o_prime;
            }
        } 
        // MAXIMIZE Pi

        // find the policy that maximizes for all a pi[s,a'] * Q[s,a',o']
        
        // define all necessary variables
        double[] action_Ms_LP; // the pi[s, .] that will be solved for

        // define constraints
        double[] lower_bound; // a lower bound for each variable
        double[] upper_bound; // an upper bound for each variable

        double[,] sum_constraint; // a coefficient for each variable in the sum contraint
        double[] sum_constraint_AU; // a maximum value for a sum constraint (1.0)
        double[] sum_constraint_AL; // a minimum value for a sum constraint (1.0)
        // sum lower and higher are equal so equality will be used

        // define the cost function maximize( pi_prime[s,0] x q[s,0, o_prime] + ... ) 
        double [] c;

        // scales of the variables
        double [] scale;

        // alglib required variables
        minlpstate state_LP = new minlpstate(); // algorithm state
        minlpreport rep = new minlpreport(); // optimization report -- useful for debugging


        // INIT

        // initialize constraints
        // pi[n] >= 0.0 and pi[n] <= 1.0
        lower_bound = new double[] {0.0, 0.0, 0.0, 0.0, 0.0}; // lower bound of 0.0f for each action
        upper_bound = new double[] {1.0, 1.0, 1.0, 1.0, 1.0}; // lower bound of 0.0f for each action

        // sum(pi[n]) == 1.0
        sum_constraint = new double[,] {{1.0, 1.0,1.0,1.0,1.0}}; // coefficient of each variable is 1.0
        sum_constraint_AU = new double[] {1.0}; // AL = AU so use equality check
        sum_constraint_AL = new double[] {1.0}; // -> check that ... = 1.0f
        
        // init variable scales
        scale = new double[] {1,1,1,1,1};

        // init the cost function maximize( pi_prime[s,0] x q[s,0, o_prime] + ... ) 
        c = new double[] {(double) s.q_Ms[0, opponent_action], 
            (double) s.q_Ms[1, opponent_action], 
            (double) s.q_Ms[2, opponent_action],
            (double) s.q_Ms[3, opponent_action],
            (double) s.q_Ms[4, opponent_action]
            }; // define the coefficients 

        // initialize Pi'
        action_Ms_LP = new double[5];

        // clone and cast pi[s, .] to pi_prime[s, .]
        for(int i = 0; i < 5; i++){ // need to cast each value of pi to a double
            action_Ms_LP.SetValue((double) s.action_Ms[i], i);
        }

        // create the linear program
        alglib.minlpcreate(5, out state_LP); // create an LP solver with a problem size of 5 and the optimizer in the defaul state

        // SET CONSTRAINTS
        // set costs
        alglib.minlpsetcost(state_LP, c);

        // set bounds on each variable [0.0, 1.0]
        alglib.minlpsetbc(state_LP, lower_bound, upper_bound);

        // sum(pi[s,.]) = 1.0 ; one equality constraint
        alglib.minlpsetlc2dense(state_LP, sum_constraint,sum_constraint_AL,sum_constraint_AU,1); 
        

        // set scale of the parameters
        alglib.minlpsetscale(state_LP, scale); // sets the scaling coeffiients


        // solve
        alglib.minlpoptimize(state_LP); // solve the LP problem

        alglib.minlpresults(state_LP, out action_Ms_LP, out rep); //get the results

        // output to the console for debugging
        Debug.Log("learned policy: " + alglib.ap.format(action_Ms_LP,3));

        // update the Ms pac man policy and cast back to float
        for(int i = 0; i<5;i++){
            s.action_Ms.SetValue(Convert.ToSingle(action_Ms_LP.GetValue(i)), i);
        }
        
        // let V[s] min{ o', sum{a', pi[s,a'] * Q[s,a',o']} }
        //s.utility_Ms = s.q_Ms[];
        s.utility_Ms = opponent_action_minimum_value;

        // let alpha := alpha * decay
        this.alpha = this.alpha * this.decay;
    
    
    }    
}



