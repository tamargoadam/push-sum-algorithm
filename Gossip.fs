module Gossip

open System
open Akka.Actor
open Akka.FSharp

let system = ActorSystem.Create("FSharp")

let mutable actorRef : IActorRef list = []

let startGossip (actorRefArr: IActorRef[]) rumor = 
    let median = actorRefArr.Length/2
    for i in 0..actorRefArr.Length-1 do
        actorRef <- actorRef @ [actorRefArr.[i]]
    actorRefArr.[median] <! rumor
    

let gossipSend (neighbors: int[]) rumor = 
    Console.WriteLine("gossip_send")
    let rand = new Random()
    let index = rand.Next(0, neighbors.Length-1)
    
    let target = actorRef.[index]
    Console.WriteLine("{0} {1}", index, target)
    target <! rumor


let gossipActor (neighbors: int[]) (mailbox : Actor<'a>) =    
    let mutable counter = 0
    // for i in 0..neighbors.Length-1 do
    //     Console.WriteLine("{0}", neighbors.[i])

    Console.WriteLine("heya")

    let rec loop () = 
        actor {
            let! msg = mailbox.Receive()
            gossipSend neighbors msg
            counter <- counter + 1
            if counter < 10 then 
                // Console.WriteLine("{0} c", counter)
                return! loop()
            else
                Console.WriteLine("done")
        }
    loop()