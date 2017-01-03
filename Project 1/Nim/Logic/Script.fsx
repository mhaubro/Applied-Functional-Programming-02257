// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
#load "AI.fs"
#load "EventQueue.fs"
#load "Game.fs"
open EventQueue
open Game

let _ = getUserInput([1..100],"A","B");;

let f = async{
    let! msg = ev.Receive()
    printf "%A" ("Bob")
    };;

let _ = Async.StartImmediate(f);;
// Define your library scripting code here

