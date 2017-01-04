
// Temporary script to test window creation in a GUIPlayer type.

#load "AI.fs"
#load "EventQueue.fs"
#load "Game.fs"
#load "GUIPlayer.fs"

open Game
open GUIPlayer

let p1 = new GUIPlayer("bob");;
let p2 = new GUIPlayer("sven");;

let game = ([|1;5;7;3|],p1,p2);;

let _ = Async.Start(ready());;
ev.Post(Start(game));;



