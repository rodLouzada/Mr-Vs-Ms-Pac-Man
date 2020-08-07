<h1 align="center"> Mr. Vs. Ms. Pac-Man </h1>

Exploration of Game Theory and Q-learning algorithm for decision making.

This project was based on Littman paper on Multiagent Reinforcement Learning.

M. Littman, "Markov Games as a Framework for Multiagent Reinforcement Learning", International Conference on Machine Learning, pp. 157-163, 1994


## Goal

The goal of this project has 3 main parts. Firstly, to investigate the Markov game environment by implementing our own our own multi agent environment in the Unity game engine; To evaluate the q-learning and minimax-q algorithms by pitting them against each other in this environment; And lastly to produce a tool for visual decision making and give us instant feedback about how well the agents perform so that we can respond and make changes to various environmental parameters. 

## Requirement 

This project was developed using [Unity 3D engine](https://unity.com/)

## Instalation

Once you have Unity installed, you can simply import this project.

## Usage
![3 Options](https://i.imgur.com/a6gPHxb.gif)

The user can select a small or a larger table with only orbs and no obstacles, or a maze table which reseambles the original pacman game. 

The playing table consists of: 

- Small orbs which will grant a small reward to the agent that eats it.
- Larger orb with greater reward, and once an agent consume it that agent "grows" and stays that size for a certain number of steps. While big, this agent can "eat" the opponent. If both agents are enlarged then nothing happens and one blocks the other from moving to its position. 
- Two agents with goal to rank as many points as possible. These agents move in the up, down, left, and right. 

The user can select training parameters, selecting which algorithm the agents will perform, as well as the number of steps the training will execute. To improve this phase, the table will reset after a set number of steps (steps which will be incremented as training proceeds). We decided to use this approach to prevent platoing. 

After training is executed, the user must save the learned policy. This will generate a XML file.

To test your training, the user needs to load a previously saved training XML file.

![screen](https://i.imgur.com/cFPpdba.png)

On the bottom right the user can hard reset the simulation.

The user can export training and testing logs. This is a XML file with information of games won and points acquired. However we need to implement the visualization of such information.

# Algorithms 

Chart of how agents behave
![agents](https://i.imgur.com/3MklxYu.png)

Chart of learning algorithms
![agents](https://i.imgur.com/Wre5Pi5.png)

