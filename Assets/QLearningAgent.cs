using System.Collections;
using System.Collections.Generic;
using static System.Array;
using System.Linq;
using UnityEngine;
using static alglib; // use the linear progrmaming plugin

// the q learning agent -- aka Mr. PacMan
public class QLearningAgent
{

    // Training parameters
    float alpha;
    float explor = 50.0f;
    float decay;
    float gamma = 1.0f;


    // initialize q-learning algorithm
    public QLearningAgent(float explor, float decay, float learning_rate, float discount_factor){
        this.alpha = learning_rate;
        this.explor = explor;
        this.decay = decay;
        this.gamma = discount_factor;
    }

    public int getAction(Cell state){
        //with probability explor, return an action uniformly at random
        if(Random.Range(0.0f,1.0f) <= explor || state.ActionMr.Sum() < 1.0){
            return Random.Range(0, 5);
        }

        //otherwise if current state is s

        float rolling_prob_counter = 0.0f;

        float random_prob = Random.Range(0.0f, 1.0f);

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

    /**
        Does not need o -- q-learning agent does not rely on the opponent's action

    **/
    public void learn(Cell s, Cell s_prime, float reward, int a){
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


}
