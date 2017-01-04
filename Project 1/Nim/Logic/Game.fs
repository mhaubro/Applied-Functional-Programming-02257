module Game

// Prelude
open System 
open System.Net 
open System.Threading

open EventQueue

open AI
//open WindowsStartScreen

type Heap = int
type User = string
type Opponent = string

type Game = Heap[] * User * Opponent

// Events for the AsyncEventQueue
//type End = Win | Lose
type Message = 
    | Start of Game | UserMove of Game | OpponentMove of Game | End of User | Error | Clear
let ev = AsyncEventQueue();;

let isGameEnded = function
                    | hl -> Array.fold (+) 0 hl = 0;;

let rec ready() = async {
    // GUI Setup

    // Recurs
    let! msg = ev.Receive()
    match msg with
        | Start game    -> return! getUserInput(game)
        | Clear         -> return! ready()
        | _             -> failwith("ready: Unexpected Message." )}

and getUserInput(game) = async {
    // GUI Setup
    let (hl,u,o)=game

    if isGameEnded hl then ev.Post(End(o))
    else
        ev.Post(UserMove(AI.opponentAI game))

    // Recurs
    let! msg = ev.Receive()
    match msg with
        | UserMove game' -> return! getOpponentInput(game')
        | End e -> return! gameEnded(e)
        | _ -> failwith("getUserInput: Unexpected Message.")}

and getOpponentInput(game) = async {
    // GUI Setup
    
    let (hl,u,o)=game
    if isGameEnded hl then ev.Post(End(u))
    else
        ev.Post(OpponentMove(AI.opponentAI game))

    // Recurs
    let! msg = ev.Receive()
    match msg with
        | OpponentMove game' -> return! getUserInput(game')
        | End e -> return! gameEnded(e)
        | _ -> failwith("getOpponentInput: Unexpected Message.")}

and gameEnded(e) = async {
    // GUI Setup
    printf "%A wins\n" e
    System.Console.Out.Flush |> ignore
    // Recurs
    let! msg = ev.Receive()
    match msg with
        | Clear -> return! ready()
        | _ -> failwith("gameEnded: Unexpected Message.")};;
