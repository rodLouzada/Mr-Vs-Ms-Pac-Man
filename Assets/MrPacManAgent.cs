using System;
using System.Collections;
using System.Collections.Generic;
using static System.Array;
using System.Linq;
using UnityEngine;
using static alglib; // use the linear progrmaming plugin

// the q learning agent -- aka Mr. PacMan
public class MrPacManAgent
{

    // Training parameters
    float alpha;
    float explor = 50.0f;
    float decay;
    float gamma = 1.0f;
    int agent_type;
    bool isTraining; // is the agent training or in test mode?

    public MrPacManAgent(float explor, float decay, float learning_rate, float discount_factor, int agent_type, bool isTraining){
        this.alpha = learning_rate;
        this.explor = explor;
        this.decay = decay;
        this.gamma = discount_factor;
        this.agent_type = agent_type; // 0 => random; 1 => q-learning; 2 => minimax-q    
        this.isTraining = isTraining;

        if(this.agent_type == 0){ // if a random agent then always explore
            this.explor = 1.0f;
        } 

    }

    public int getAction(Cell state){
    
        //with probability explor, return an action uniformly at random
        if((UnityEngine.Random.Range(0.0f,1.0f) <= explor && isTraining) || state.ActionMr.Sum() < 1.0 || explor == 1.0f){
            return UnityEngine.Random.Range(0, 5);
        }

        //otherwise if current state is s

        float rolling_prob_counter = 0.0f;

        float random_prob = UnityEngine.Random.Range(0.0f, 1.0f);

        // get action based on Pi[a] for action i 
        for(int i = 0; i < 5; i++){
            rolling_prob_counter = state.ActionMr[i] + rolling_prob_counter;

            if(random_prob < rolling_prob_counter){ 
                return i;
            }

        }

        Debug.Log("Valid action not found");
        return -999; // should not ever make it here if values of Pi sum to 1.0

    }


    public void learn(Cell s, Cell s_prime, float reward, int a, int o){
        // if a random agent then don't learn
        if(this.agent_type == 0){ // random agent
            // DO NOTHING
        }else if(this.agent_type == 1){ // if agent type is q-learning
            qLearning_learn(s, s_prime, reward, a);
        }else if(this.agent_type == 2){ // if agent type is minimax q
            minimax_q_learn(s, s_prime, reward, a, o); // minimax-q agent learns using opponent's action
        }
    }

    /**
        Does not need o -- q-learning agent does not rely on the opponent's action
    **/
    public void qLearning_learn(Cell s, Cell s_prime, float reward, int a){
        // after recieving reward rew for moving from state s to s' via action a and opponent's action o

        //update q table

        // let Q[s,a, .] = (1 - alpha) * Q[s,a,o] + alpha * (rew + gamma * V[s'])
        for(int o = 0; o < 5; o++){ // need to update for all possible opponent actions; q-learning does not differentiate
            // Debug.Log("a: " + a + "  o: " + o);
            s.q_Mr[a,o] = (1 - this.alpha) * s.q_Mr[a,o] + (alpha * (s.reward + this.gamma * s_prime.Utility_Mr));
            // Debug.Log("Setting q mr at a: " + a + " and o: " + o + " to the value of: " + s.q_Mr[a,o]);
        }

        // calculate values for each action
        float [] simplified_q_table = new float[5];

        for(int a_prime = 0; a_prime < 5; a_prime++){

            simplified_q_table[a_prime] = s.q_Mr[a_prime, 0]; // use 0 for opponent's action because it isn't accounted for
            //Debug.Log("a': " + a_prime);
            //Debug.Log("Setting summation table for a' to a value of:" + simplified_q_table[a_prime]);
            //Debug.Log("q value of the action: " + s.q_Mr[a_prime,0]);
            //Debug.Log("------------------------------------------------");
        }

        
        // find the policy that maximizes value (aka use summation_table)

        // q-learning agent always chooses the best action available        

        int count = 0; // count the number of actions with this maximum value
        for(int i = 0; i < 5; i++){

            if(simplified_q_table.Max() == simplified_q_table[i]){
                count += 1;
            }
        }
        
        // for each value that does have this max value set probability of being selected to 1/count
        for(int i = 0; i < 5; i++){
            if(simplified_q_table.Max() == simplified_q_table[i]){
                s.ActionMr.SetValue(1.0f/(float)count, i);
            }else{
                s.ActionMr.SetValue(0.0f, i);
            }
        }

        s.Utility_Mr = simplified_q_table.Max(); // value is equal to the maximum possible value

        // let alpha := alpha * decay
        this.alpha = this.alpha * this.decay;
    }

    public void minimax_q_learn(Cell s, Cell s_prime, float reward, int a, int o){
        // after recieving reward rew for moving from state s to s' via action a and opponent's action o

        // let Q[s,a,o] = (1 - alpha) * Q[s,a,o] + alpha * (rew + gamma * V[s'])
        s.q_Mr[a,o] = (1 - alpha) * s.q_Mr[a,o] + (alpha * (s.reward + this.gamma * s_prime.utility_Mr));

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
                tmp_sum += ( s.action_Mr[a_prime] * s.q_Mr[a_prime, o_prime] );
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
        double[] action_Mr_LP; // the pi[s, .] that will be solved for

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
        c = new double[] {(double) s.q_Mr[0, opponent_action], 
            (double) s.q_Mr[1, opponent_action], 
            (double) s.q_Mr[2, opponent_action],
            (double) s.q_Mr[3, opponent_action],
            (double) s.q_Mr[4, opponent_action]
            }; // define the coefficients 

        // initialize Pi'
        action_Mr_LP = new double[5];

        // clone and cast pi[s, .] to pi_prime[s, .]
        for(int i = 0; i < 5; i++){ // need to cast each value of pi to a double
            action_Mr_LP.SetValue((double) s.action_Mr[i], i);
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

        alglib.minlpresults(state_LP, out action_Mr_LP, out rep); //get the results

        // output to the console for debugging
        Debug.Log("learned policy: " + alglib.ap.format(action_Mr_LP,3));

        // update the Mr pac man policy and cast back to float
        for(int i = 0; i<5;i++){
            s.action_Mr.SetValue(Convert.ToSingle(action_Mr_LP.GetValue(i)), i);
        }
        
        // let V[s] min{ o', sum{a', pi[s,a'] * Q[s,a',o']} }
        //s.utility_Ms = s.q_Ms[];
        s.utility_Mr = opponent_action_minimum_value;

        // let alpha := alpha * decay
        this.alpha = this.alpha * this.decay;
    
    
    }    
}
