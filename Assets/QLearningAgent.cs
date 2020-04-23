using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if(Random.Range(0.0f,100.0f) <= explor){
            return Random.Range(0, 6);
        }

        //otherwise if current state is s
        while(true){
            for(int i = 0; i < state.ActionMr.Length; i++){
                //return action a with probability pi[s,a]
                if(Random.Range(0.0f, 1.0f) < state.ActionMr[i]){
                    return i;
                } 
            }
        }
    }

    void learn(Cell s, Cell s_prime, float reward, int a, int o){
        // after recieving reward rew for moving from state s to s' via action a and opponent's action o

        // let Q[s,a,o] = (1 - alpha) * Q[s,a,o] + alpha * (rew + gamma * V[s'])
        s.q_Mr[a,o] = (1 - this.alpha) * s.q_Mr[a,o] + alpha * (s.reward + this.gamma * s_prime.utility);

        // use linear programming to find pi[s,.] such that: 
            // pi[x,.] = argmax{pi'[s,.], min{o', sum{a', pi[s,a'] * Q[s,a’,o’] }}}
        

        // let V[s] min{ o', sum{a', pi[s,a'] * Q[s,a',o']} }
        // let alpha := alpha * decay
    }


}
