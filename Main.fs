// Learn more about F# at http://fsharp.org
module Main

open Gossip
open System
open Akka.FSharp

open System.Collections.Generic
let neighbors = new Dictionary<int, Array>()

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



let startProtocol algorithm actorRefArr = 
    if algorithm = "gossip" then
        Console.WriteLine("Using Gossip Algorithm")
        // startGossip actorRefArr
           
    elif algorithm = "push-sum" then
        Console.WriteLine("Using Push Sum Algorithm")
        
    else    
        Console.WriteLine("Enter either gossip or push-sum")


let fullNetwork numNodes = 
    let actorRefArr = [|
        for i in 0 .. numNodes-1 -> 
            (spawn system ("worker"+i.ToString()) (gossipActor [|for i in 0 .. numNodes-1 -> i|]))
    |]
    actorRefArr

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

        let actorRefArr = buildTopology topology numNodes

        let stopWatch = System.Diagnostics.Stopwatch.StartNew()

        let p = startProtocol algorithm actorRefArr

        stopWatch.Stop()

        Console.WriteLine("{0}", stopWatch.Elapsed.TotalMilliseconds)

        
    
    0 // return an integer exit code
