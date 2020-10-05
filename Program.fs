// Learn more about F# at http://fsharp.org

open System

let getNextPerfectSq(numNodes) = 
    numNodes


let buildTopology(topology, numNodes) = 
    numNodes

let startProtocol(algorithm) = 
    if algorithm = "gossip" then
        Console.WriteLine("Using Gossip Algorithm")
           
    elif algorithm = "push-sum" then
        Console.WriteLine("Using Push Sum Algorithm")
        
    else    
        Console.WriteLine("Enter either gossip or push-sum")



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

        buildTopology(topology, numNodes) |> ignore

        let stopWatch = System.Diagnostics.Stopwatch.StartNew()

        startProtocol(algorithm)

        stopWatch.Stop()

        Console.WriteLine("{0}", stopWatch.Elapsed.TotalMilliseconds)

        
    
    0 // return an integer exit code
