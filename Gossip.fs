module Gossip

open System
open Akka.Actor
open Akka.FSharp

let system = ActorSystem.Create("FSharp")


let startGossip (actorRefArr: IActorRef[]) rumor = 
    let median = actorRefArr.Length/2
    actorRefArr.[median] <! rumor
    

let gossipSend (neighbors: int[]) rumor = 
    let rand = new Random()
    let index = rand.Next(0, neighbors.Length-1)
    let target = system.ActorSelection("akka://FSharp/user/worker"+index.ToString())
    target <! rumor


let gossipActor neighbors (mailbox : Actor<'a>) =    
    let mutable counter = 0
    let rec loop () = 
        actor {
            let! msg = mailbox.Receive()
            gossipSend neighbors msg
            counter <- counter + 1
            if counter < 10 then 
                return! loop()
            else
                Console.WriteLine("done")
        }
    loop()