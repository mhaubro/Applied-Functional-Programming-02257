// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
#load "AI.fs"
#load "EventQueue.fs"
#load "Game.fs"
open EventQueue
open Game

let _ = Async.Start(ready());;
let mutable counter = 1;;
while counter < 1000 do
    ev.Post(Start(([|1..100|],"Q","Z")))
    Async.Sleep 100 |> ignore
    ev.Post(Clear)
    counter <- counter + 1