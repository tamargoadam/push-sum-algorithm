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
    let mutable x = 0
    Console.WriteLine("Sending initial rumor...")
    actorRef.[median] <! rumor
    
let getRandNum min max =
    let rand = Random()
    rand.Next(min, max)

let gossipSend (neighbors: int[]) rumor = 
    let index = getRandNum 0 neighbors.Length
    
    let target = actorRef.[index]
    Console.WriteLine("Sending msg to {0}: {1}", index, target)
    target <! rumor


let gossipActor (neighbors: int[]) (mailbox : Actor<'a>) =    
    let mutable counter = 0
    // for i in 0..neighbors.Length-1 do
    //     Console.WriteLine("{0}", neighbors.[i])

    Console.WriteLine("Actor started")

    let rec loop () = 
        actor {
            let! msg = mailbox.Receive()
            gossipSend neighbors msg
            counter <- counter + 1
            if counter < 10 then 
                Console.WriteLine("counter: {0}", counter)
                return! loop()
            else
                Console.WriteLine("done")
        }
    loop()