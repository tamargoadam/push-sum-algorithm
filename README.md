# Gossip and Push-Sum Simulations

This program simulates the asynchronous Gossip and [Push-Sum](https://www.cs.cornell.edu/johannes/papers/2003/focs2003-gossip.pdf) algorithms. This implementation uses the actor model of concurrent computation, powered by [Akka](https://getakka.net/).

##### Command to run: 
`dotnet run <num_nodes> <topology> <algorithm>`

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
