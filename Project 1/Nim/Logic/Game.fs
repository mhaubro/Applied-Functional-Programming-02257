module Game

// Prelude
open System 
open System.Net 
open System.Threading
open EventQueue

/// Player struct with name and function handle to produce moves from gamestate
type Player = {Name:string; getMove:int[]->Async<int*int>}

/// Enumaration used to alternate between player turns
type PlayerID = A|B

/// Game state with handles for player structs
type Game = int[] * Player * Player

/// Enumeration of messages - alphabet of FSM
type Message =
    | Start of Game
    | Move of int*int
    | Win of Player
    | Error
    | Clear
    | Cancelled

///Eventqueue for messages that change game FSM - if you are seeing this outside of Game.fs you are doing something wrong
let ev = AsyncEventQueue();;

///Flag to determine whether the gui is allowed to start a game
let mutable gameOn = false
///Flag to determine whether the gui is allowed to clear an ended game
let mutable gameEnded = false
///Handler for when the gui wants to cancel a game
let mutable guiCancellation = (fun () -> ())

/// Guarded function to start a game from gui
let startGameFromGUI = function game -> if not gameOn then ev.Post(Start(game)) else ()
/// Guarded function to clear an ended game from gui
let clearGameFromGUI = function () -> if gameEnded then ev.Post(Clear) else ()
/// Function to forfeit game from gui
let cancelGameFromGUI = function () -> guiCancellation()

///Handler for function to be called when the game ends
let mutable gameEnder = (fun p -> printfn "Player named %s won" p.Name)


/// Test to check if the given move is valid for the state of the game
let validateMove (heapArray:int[]) (id,num) = (id >= 0 && id < heapArray.Length && num > 0 && num <= heapArray.[id])

/// Alter the supplied game state based on a move - the move should be validated elsewhere
let ApplyMove (heapArray:int[]) ((id,num):(int*int)) = 
    heapArray.[id] <- heapArray.[id] - num
    heapArray

/// Apply a given move if it is consistent with the state of the game - throw exception otherwise
/// The returned game contains a new heapArray that is a copy of the supplied one.
let ProcessMove ((heapArray,a,b):Game) (move:(int*int)) =
    let heapArray' = if (validateMove heapArray move)
                        then ApplyMove (Array.copy heapArray) move //ensure that we return a copy of the array
                        else failwith("ProcessMove: move not valid")
    (heapArray',a,b)

///Test to determine whether the game can continue
let isGameEnded = function hl -> Array.fold (+) 0 hl = 0;;

///Initial state - awaits a game posted to eventqueue, possibly through startGameFromGUI
let rec ready() = async {
    // GUI Setup

    // Recurs
    let! msg = ev.Receive()
    match msg with
        | Start game    -> gameOn <- true
                           return! Turn A (game)
        | Clear         -> return! ready()
        | _             -> failwith("ready: Unexpected Message." )}
/// State representing turn of player represented by p   
and Turn p game = async {
    let (heapArray,playerA,playerB) = game

    let thisPlayer, nextPlayer, p' = match p with
                                        | A -> playerA, playerB, B
                                        | B -> playerB, playerA, A

    if (isGameEnded heapArray) then 
        ev.Post(Win(nextPlayer))
    else
        let ts = new CancellationTokenSource()
        guiCancellation <- (fun () -> ts.Cancel() )
        Async.StartWithContinuations
            (async {return! thisPlayer.getMove heapArray},
            (fun (id,num)   -> match id,num with
                                |0,0-> ()
                                | _,_ -> ev.Post(Move(id,num))),
            (fun _          -> ev.Post Error),
            (fun _          -> ev.Post(Win(nextPlayer))),
            ts.Token)

    // Recurs
    let! msg = ev.Receive()
    //guiCancellation <- fun ()->()//reset cancellation handle
    match msg with
        | Move (id,num) -> return! Turn p' (ProcessMove game (id,num))
        | Win e         -> return! _end(e)
        | _ -> failwith("getUserInput: Unexpected Message.")}

/// End state, notifies someone of a winner through gameEnder and waits to be cleared
and _end(e) = async {
    // GUI Setup
    gameEnded <- true
    gameEnder(e)
    // Recurs
    let! msg = ev.Receive()
    match msg with
        | Clear -> gameOn<- false
                   return! ready()
        | _     -> failwith("gameEnded: Unexpected Message.")};;




