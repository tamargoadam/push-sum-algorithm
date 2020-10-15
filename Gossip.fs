module Gossip

open System
open Akka.Actor
open Akka.FSharp

let mutable actorRef : IActorRef list = []

let startGossip (actorRefArr: IActorRef[]) rumor = 
    let median = (actorRefArr.Length-1)/2
    // fill actorRef with all actors in network for lookups
    for i in 0..actorRefArr.Length-1 do
        actorRef <- actorRef @ [actorRefArr.[i]]

    Console.WriteLine("Sending initial rumor...")
    actorRef.[median] <! rumor
    

let getRandNum min max =
    let rand = Random()
    rand.Next(min, max)


let gossipSend (neighbors: int[]) rumor = 
    let index = getRandNum 0 neighbors.Length    
    let target = actorRef.[neighbors.[index]]
    target <! rumor


let gossipActor (neighbors: int[]) (mailbox : Actor<'a>) =    
    let mutable counter = 0
    let rec loop () = 
        actor {
            let! msg = mailbox.Receive()
            gossipSend neighbors msg
            counter <- counter + 1

            // terminate actor if rumor has been heard 10 times
            if counter < 50 then
                // let listener know this node has heard the rumor
                if counter = 1 then
                    mailbox.Context.Parent <! msg
                return! loop()
        }
    loop()