// Learn more about F# at http://fsharp.org
module Main

open Gossip
open PushSum
open System
open Akka.Actor
open Akka.FSharp
open System.Collections.Generic

let system = ActorSystem.Create("FSharp")

let neighbors = new Dictionary<int, int []>( )

let getNextPerfectSq(numNodes) = 
    let sqroot:int = int(Math.Ceiling(sqrt(float(numNodes)))**2.0)
    sqroot

let full_network numNodes = 
    for i in 0..numNodes-1 do
        let mutable adjList : int list = []
        for j in 0..numNodes-1 do
            if i <> j then
                adjList <- j :: adjList
        let adjArray = adjList |> List.toArray
        neighbors.Add(i, adjArray)

let twoD_network numNodes sqroot =
    for i in 0..numNodes-1 do
        let mutable adjList : int list = []
        // For the right neighbors
        if (i+1) % sqroot <> 0 then
            adjList <- i+1 :: adjList

        // For the lower neighbors
        if i > sqroot-1 then
            adjList <- i-sqroot :: adjList

        // For the upper neighbors
        if numNodes-i > sqroot then
            adjList <- i+sqroot :: adjList

        // For the left neighbors
        if i % sqroot <> 0 then
            adjList <- i-1 :: adjList

        let adjArray = adjList |> List.toArray
        neighbors.Add(i, adjArray)


let line_network numNodes = 
    for i in 0..numNodes-1 do
        let mutable adjList : int list = []
        if i = 0 then
            adjList <- i+1 :: adjList
        elif i = numNodes-1 then
            adjList <- i-1 :: adjList
        else
            adjList <- i+1 :: adjList
            adjList <- i-1 :: adjList

        let adjArray = adjList |> List.toArray
        neighbors.Add(i, adjArray)


let impTwoD_network numNodes sqroot =
    for i in 0..numNodes-1 do
        let mutable adjList : int list = []

        let rand = new Random()
        let mutable index = i

        while index = i do
            index <- rand.Next(0, numNodes-1)

        // For the random neighbor
        adjList <- index :: adjList

        // For the right neighbors
        if (i+1) % sqroot <> 0 then
            adjList <- i+1 :: adjList

        // For the lower neighbors
        if i > sqroot-1 then
            adjList <- i-sqroot :: adjList

        // For the upper neighbors
        if numNodes-i > sqroot then
            adjList <- i+sqroot :: adjList

        // For the left neighbors
        if i % sqroot <> 0 then
            adjList <- i-1 :: adjList

        let adjArray = adjList |> List.toArray
        neighbors.Add(i, adjArray)


let createRefArr (algorithm: string) (numNodes: int) (mailbox : Actor<'a>)=   
    if algorithm = "gossip" then
        [|
            for i in 0 .. numNodes-1 -> 
                (spawn mailbox ("worker"+i.ToString()) (gossipActor (neighbors.Item(i))))
        |]
    else
        [|
            for i in 0 .. numNodes-1 -> 
                (spawn mailbox ("worker"+i.ToString()) (pushSumActor (float i) (neighbors.Item(i))))
        |]

let startProtocol (algorithm: string) (refArr: IActorRef []) (numNodes: int) = 
    if algorithm = "gossip" then
        Console.WriteLine("Using Gossip Algorithm...")
        startGossip refArr "rumor"
           
    elif algorithm = "push-sum" then
        Console.WriteLine("Using Push Sum Algorithm...")
        startPushSum refArr

    else    
        Console.WriteLine("Enter either gossip or push-sum")

let supervisorActor (algorithm: string) (numNodes: int) (mailbox : Actor<'a>)= 
    let refArr = createRefArr algorithm numNodes mailbox
    startProtocol algorithm refArr numNodes
    let mutable numHeard = 0
    let mutable returnAddress = mailbox.Context.Parent
    let rec loop () = 
        actor {
            let! msg = mailbox.Receive()
            let sender = mailbox.Sender()
            if returnAddress = mailbox.Context.Parent then
                returnAddress <- sender
                return! loop()
            else
                numHeard <- numHeard + 1
                if numHeard < numNodes then 
                    // if algorithm = "gossip" then
                    //     Console.WriteLine("{0} heard the rumor, '{1}'!. ({2})", sender, msg, numHeard)
                    // elif algorithm = "push-sum" then
                    //     Console.WriteLine("{0} converged to the sum, {1}!. ({2})", sender, msg, numHeard)
                    return! loop()
                else
                    if algorithm = "gossip" then
                        Console.WriteLine("All {0} nodes heard the rumor!", numHeard)
                    elif algorithm = "push-sum" then
                        Console.WriteLine("All {0} nodes converged to the sum! (~{1})", numHeard, msg)
                    returnAddress <! "done!"
        }
    loop()



let buildTopology topology numNodes = 
    if topology = "full" then
        full_network(numNodes)

    elif topology = "2D" then
        let sqroot:int = int(sqrt(float(numNodes)))
        twoD_network numNodes sqroot

    elif topology = "line" then
        line_network(numNodes)

    else 
        let sqroot:int = int(sqrt(float(numNodes)))   
        impTwoD_network numNodes sqroot

[<EntryPoint>]
let main argv =
    if argv.Length <> 3 then
        Console.WriteLine("Invalid Input Provided")
        Console.WriteLine("Ex.: project2 100 2D gossip")
    else

        let mutable numNodes:int = int argv.[0]
        let topology = argv.[1]
        let algorithm = argv.[2]

        if topology = "2D" || topology = "imp2D" then
            numNodes <- getNextPerfectSq(numNodes)
        
        
        buildTopology topology numNodes

        let stopWatch = System.Diagnostics.Stopwatch.StartNew()

        let supervisor = spawn system "supervisorActor" (supervisorActor algorithm numNodes)
        let res = supervisor <? "done?" |> Async.RunSynchronously

        stopWatch.Stop()

        Console.WriteLine("Time to complete: {0} ms", stopWatch.Elapsed.TotalMilliseconds)

        
    
    0 // return an integer exit code
