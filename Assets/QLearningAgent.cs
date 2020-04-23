using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QLearningAgent : MonoBehaviour
{

    List<int> S;
    List<int> A; // move N, S, E, W
    List<int> O; // move N, S, E, W

    List<Double> Q;
    List<Double> pi;
    List<Integer> v;
    double alpha;

    double gamma = 1.0;

    double explor = 50.0;

    // initialize q-learning algorithm
    void init_mr_pac_man(List<int> S, List<int> A, List<int> O){
        base.S = this.S;
        base.A = this.A;
        base.O = this.O;

        // for all s in S, a in A, and o in O
        foreach (int s in S){
            foreach (int a in A){
                foreach(int o in O){
                    Q[s,a,o] = 1
                }
            }
        }    

        // for all s in S
        foreach (int s in S){
            V[s] = 1
        }

        // for all s in S, a in A
        foreach (int s in S){
            foreach (int a in A){
                pi[s,a] = 1.0/A.sum();
            }
        }

        // let alpha = 1.0
        alpha = 1.0

    }

    int get_action(int s){
        //with probability explor, return an action uniformly at random
        if(Random.Range(0.0,100.0) <= explor){
            return A[Random.Range(0.0, len(A))]
        }

        //otherwise if current state is s
        foreach (int a in A){
            //return action a with probability pi[s,a]
            if(Random.Range(0.0, 1.0) < pi[s, a]){
                return a;
            }  
        }
    }

    void learn(int s, int s_prime, float reward, int a, int o){
        // after recieving reward rew for moving from state s to s' via action a and opponent's action o

        // let Q[s,a,o] = (1 - alpha) * Q[s,a,o] + alpha * (rew + gamma * V[s'])
        Q[s, a, o] = (1 - alpha) * Q[s,a,o] + alpha * (reward + gamma * V[s_prime])

        // use linear programming to find pi[s,.] such that: 
            // pi[x,.] = argmax{pi'[s,.], min{o', sum{a', pi[s,a'] * Q[s,a’,o’] }}}
        

        // let V[s] min{ o', sum{a', pi[s,a'] * Q[s,a',o']} }
        // let alpha := alpha * decay
    }


}
