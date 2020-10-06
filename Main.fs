// Learn more about F# at http://fsharp.org
module Main

open Gossip
open System
open Akka.FSharp


let getNextPerfectSq(numNodes) = 
    numNodes


let startProtocol algorithm actorRefArr = 
    // if algorithm = "gossip" then
        Console.WriteLine("Using Gossip Algorithm")
        startGossip actorRefArr
           
    // elif algorithm = "push-sum" then
    //     Console.WriteLine("Using Push Sum Algorithm")
        
    // else    
    //     Console.WriteLine("Enter either gossip or push-sum")


let fullNetwork numNodes = 
    let actorRefArr = [|
        for i in 0 .. numNodes-1 -> 
            (spawn system ("worker"+i.ToString()) (gossipActor [|for i in 0 .. numNodes-1 -> i|]))
    |]
    actorRefArr

let buildTopology topology numNodes = 
    fullNetwork numNodes


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
