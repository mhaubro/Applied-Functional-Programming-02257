module Game

// Prelude
open System 
open System.Net 
open System.Threading

open EventQueue
//open WindowsStartScreen

type Heap = int
type User = string
type Opponent = string

type Game = Heap list * User * Opponent

// Events for the AsyncEventQueue
type End = Win | Lose
type Message = 
    | Start of Game | UserMove of Game | OpponentMove of Game | End | Error | Clear 
let ev = AsyncEventQueue()
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

    // Recurs
    let! msg = ev.Receive()
    match msg with
        | UserMove game -> return! getOpponentInput(game)
        | End e -> return! gameEnded(e)
        | _ -> failwith("getUserInput: Unexpected Message.")}

and getOpponentInput(game) = async {
    // GUI Setup

    // Recurs
    let! msg = ev.Receive()
    match msg with
        | OpponentMove game -> return! getUserInput(game)
        | End e -> return! gameEnded(e)
        | _ -> failwith("getOpponentInput: Unexpected Message.")}

and gameEnded(e) = async {
    // GUI Setup

    // Recurs
    let! msg = ev.Receive()
    match msg with
        | Clear -> ready()
        | _ -> failwith("gameEnded: Unexpected Message.")}

