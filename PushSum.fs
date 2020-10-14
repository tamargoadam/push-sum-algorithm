module PushSum

open System
open Akka.Actor
open Akka.FSharp

let mutable actorRef : IActorRef list = []

let startPushSum (actorRefArr: IActorRef[]) = 
    let median = actorRefArr.Length-1
    // fill actorRef with all actors in network for lookups
    for i in 0..actorRefArr.Length-1 do
        actorRef <- actorRef @ [actorRefArr.[i]]

    Console.WriteLine("Sending initial values...")
    actorRef.[median] <! (0.0, 0.0)
    

let getRandNum min max =
    let rand = Random()
    rand.Next(min, max)


let pushSumSend (neighbors: int[]) rumor (self: IActorRef) = 
    let index = getRandNum 0 neighbors.Length    
    let target = actorRef.[index]
    target <! rumor
    self <! rumor



let checkConverge (oldVals: float * float) (newVals: float * float) = 
    let oldV = fst(oldVals)/snd(oldVals)
    let newV = ((fst(oldVals) + fst(newVals))/2.0)/((snd(oldVals) + snd(newVals))/2.0)
    oldV - newV < 10.0**(-10.0) && newV - oldV < 10.0**(-10.0)


let pushSumActor (value: float) (neighbors: int[]) (mailbox : Actor<float * float>) =    
    let mutable convCounter = 0
    let mutable s = value
    let mutable w = 1.0
    let rec loop () = 
        actor {
            let! msg = mailbox.Receive()
            if checkConverge (s, w) msg then
                convCounter <- convCounter + 1
            else
                convCounter <- 0
            s <- s + fst(msg)
            w <- w + snd(msg)
            
            // Console.WriteLine("Current sum val: {0}", (s/w))
            pushSumSend neighbors (s/2.0, w/2.0) mailbox.Self

            // terminate actor if it has converged at sum
            if convCounter < 10 then
                return! loop()
            else
                mailbox.Context.Parent <! (s/w)
        }
    loop()