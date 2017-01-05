module Game

// Prelude
open System 
open System.Net 
open System.Threading

open EventQueue

//open WindowsStartScreen

type Heap = int

//[<AbstractClass>]
//type Player (name:string) =
//    member this.name = name 
//    abstract member getMove : Heap[] -> int*int

type Player = {Name:string; getMove:int[]->int*int}

type PlayerID = A|B

type Game = Heap[] * Player * Player

type Message =
    | Start of Game | Move of int*int | Win of Player | Error | Clear | Cancelled


let ev = AsyncEventQueue();;

let mutable gameOn = false
let startGameFromGUI = function | game -> if not gameOn then ev.Post(Start(game)) else ()

let clearGameFromGUI = function () -> ev.Post(Clear)

let mutable gameEnder = (fun p -> printfn "Player named %s won" p.Name)

let mutable guiCancellation = (fun () -> ())

let validateMove ((heapArray,_,_):Game) ((id,num):(int*int)) = 
    (id >= 0 && id < heapArray.Length && num > 0 && num <= heapArray.[id])

let ApplyMove ((heapArray,a,b):Game) ((id,num):(int*int)) = 
    heapArray.[id] <- heapArray.[id] - num
    (heapArray, a, b)

let ProcessMove (game:Game) (move:(int*int)) =
    if (validateMove game move) then ApplyMove game move
    else failwith("ProcessMove: move not valid")

let isGameEnded = function
                    | hl -> Array.fold (+) 0 hl = 0;;

let rec ready() = async {
    // GUI Setup

    // Recurs
    let! msg = ev.Receive()
    match msg with
        | Start game    -> gameOn <- true
                           return! Turn A (game)
        | Clear         -> return! ready()
        | _             -> failwith("ready: Unexpected Message." )}
        
and Turn p game = async {
    let (heapArray,playerA,playerB) = game

    let thisPlayer, nextPlayer, p' = match p with
                                        | A -> playerA, playerB, B
                                        | B -> playerB, playerA, A

    if (isGameEnded heapArray) then 
        ev.Post(Win(nextPlayer))
    else
        use ts = new CancellationTokenSource()
        guiCancellation <- (fun () -> ts.Cancel())
        Async.StartWithContinuations
            (async {return thisPlayer.getMove heapArray},
            (fun (id,num)   -> ev.Post (Move(id,num))),
            (fun _          -> ev.Post Error),
            (fun _          -> ev.Post(Win(nextPlayer))),
            ts.Token)

    // Recurs
    let! msg = ev.Receive()
    match msg with
        | Move (id,num) -> //printf "Move:%A\n" (id,num)
                          return! Turn p' (ProcessMove game (id,num))
        | Win e -> return! _end(e)
        | _ -> failwith("getUserInput: Unexpected Message.")}

and _end(e) = async {
    // GUI Setup
    gameEnder(e)
    // Recurs
    let! msg = ev.Receive()
    match msg with
        | Clear -> 
                gameOn<- false
                return! ready()
        | _ -> failwith("gameEnded: Unexpected Message.")};;




