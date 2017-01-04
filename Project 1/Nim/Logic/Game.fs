module Game

// Prelude
open System 
open System.Net 
open System.Threading

open EventQueue

open AI
//open WindowsStartScreen

type Heap = int

[<AbstractClass>]
type Player (name:string) =
    member this.name = name 
    abstract member getMove : Heap[] -> int*int


type Game = Heap[] * Player * Player

type Message =
    | Start of Game | Move of int*int | Win of Player | Error | Clear | Cancelled


let ev = AsyncEventQueue();;

let validateMove ((heapArray,_,_):Game) ((id,num):(int*int)) = 
    (id > 0 && id < heapArray.Length && num > 0 && num < heapArray.[id])

let ApplyMove ((heapArray,A,B):Game) ((id,num):(int*int)) = 
    heapArray.[id] <- heapArray.[id] - num
    (heapArray, A, B)

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
        | Start game    -> return! getPlayerAInput(game)
        | Clear         -> return! ready()
        | _             -> failwith("ready: Unexpected Message." )}

and getPlayerAInput(game) = async {
    let (heapArray,PlayerA,PlayerB) = game

    if (isGameEnded heapArray) then 
        ev.Post(Win(PlayerB))
    else
        use ts = new CancellationTokenSource()
        Async.StartWithContinuations
            (async {return PlayerA.getMove heapArray},
            (fun (id,num)   -> ev.Post (Move(id,num))),
            (fun _          -> ev.Post Error),
            (fun _          -> ev.Post Cancelled),
            ts.Token)

    // Recurs
    let! msg = ev.Receive()
    match msg with
        | Move (id,num) -> return! getPlayerBInput(ProcessMove game (id,num))
        | Win e -> return! gameEnded(e)
        | _ -> failwith("getUserInput: Unexpected Message.")}

and getPlayerBInput(game) = async {
    let (heapArray,PlayerA,PlayerB) = game

    if (isGameEnded heapArray) then 
        ev.Post(Win(PlayerA))
    else
        use ts = new CancellationTokenSource()
        Async.StartWithContinuations
            (async {return PlayerB.getMove heapArray},
            (fun (id,num)   -> ev.Post (Move(id,num))),
            (fun _          -> ev.Post Error),
            (fun _          -> ev.Post Cancelled),
            ts.Token)

    // Recurs
    let! msg = ev.Receive()
    match msg with
        | Move (id,num) -> return! getPlayerAInput(ProcessMove game (id,num))
        | Win e -> return! gameEnded(e)
        | _ -> failwith("getUserInput: Unexpected Message.")}

and gameEnded(e) = async {
    // GUI Setup
    printf "%A wins\n" e
    System.Console.Out.Flush |> ignore
    // Recurs
    let! msg = ev.Receive()
    match msg with
        | Clear -> return! ready()
        | _ -> failwith("gameEnded: Unexpected Message.")};;




