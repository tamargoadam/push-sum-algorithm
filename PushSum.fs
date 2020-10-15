module PushSum

open System
open Akka.Actor
open Akka.FSharp

let mutable actorRef : IActorRef list = []

let startPushSum (actorRefArr: IActorRef[]) = 
    let median = (actorRefArr.Length-1)/2
    // fill actorRef with all actors in network for lookups
    for i in 0..actorRefArr.Length-1 do
        actorRef <- actorRef @ [actorRefArr.[i]]

    Console.WriteLine("Sending initial values...")
    actorRef.[median] <! (-1.0, -1.0)
    

let getRandNum min max =
    let rand = Random()
    rand.Next(min, max)


let pushSumSend (neighbors: int[]) rumor = 
    let index = getRandNum 0 neighbors.Length    
    let target = actorRef.[neighbors.[index]]
    target <! rumor



let checkConverge (oldVals: float * float) (newVals: float * float) = 
    let oldV = fst(oldVals)/snd(oldVals)
    let newV = ((fst(oldVals) + fst(newVals))/2.0)/((snd(oldVals) + snd(newVals))/2.0)
    abs (oldV - newV) < 10.0**(-10.0)

let pushSumActor (value: float) (neighbors: int[]) (mailbox : Actor<float * float>) =    
    let mutable convCounter = 0
    let mutable s = value
    let mutable w = 0.0

    let rec loop () = 
        actor {
            let! msg = mailbox.Receive()

            if msg = (-1.0, -1.0) then
                s <- s/2.0
                w <- 0.5
                mailbox.Self <! (s, w)
                return! loop()

            if checkConverge (s, w) msg then
                    convCounter <- convCounter + 1
            else
                convCounter <- 0
            
            s <- (s + fst(msg))/2.0
            w <- (w + snd(msg))/2.0
            
            pushSumSend neighbors (s, w)

            // terminate actor if it has converged at sum
            if convCounter <> 3 then
                //Console.WriteLine("{0}: {1}", mailbox.Context.Self, (s/w))
                return! loop()
            else
                mailbox.Context.Parent <! (s/w)
        }
    loop()