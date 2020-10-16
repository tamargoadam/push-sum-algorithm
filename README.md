# Gossip and Push-Sum Simulation

#### Command to run: 
`dotnet run numNodes topology algorithm`

##### Available topologies:
 - `full`
   - Every actor is a neighbor of all other actors.
 - `2D`
   - Actors form a 2D grid.
 - `line`
   - Actors are arranged in a line.
 - `imp2D`
   - Actors are arranged in a grid but one random other neighbor
      is selected from the list of all actors.

##### Available algorithms:
- `gossip`
- `push-sum` 