module Game

// Prelude
open System 
open System.Net 
open System.Threading

open EventQueue

open AI
//open WindowsStartScreen

type Heap = int

//[<AbstractClass>]
//type Player (name:string) =
//    member this.name = name 
//    abstract member getMove : Heap[] -> int*int

type Player = {Name:string; getMove:int[]->int*int}

type Turn = A|B

type Game = Heap[] * Player * Player

type Message =
    | Start of Game | Move of int*int | Win of Player | Error | Clear | Cancelled


let ev = AsyncEventQueue();;

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
        | Start game    -> return! getPlayerInput A (game)
        | Clear         -> return! ready()
        | _             -> failwith("ready: Unexpected Message." )}
        
and getPlayerInput turn  game = async {
    let (heapArray,playerA,playerB) = game

    let thisPlayer, nextPlayer, turn' = match turn with
                                        | A -> playerA, playerB, B
                                        | B -> playerB, playerA, A

    if (isGameEnded heapArray) then 
        ev.Post(Win(nextPlayer))
    else
        use ts = new CancellationTokenSource()
        Async.StartWithContinuations
            (async {return thisPlayer.getMove heapArray},
            (fun (id,num)   -> ev.Post (Move(id,num))),
            (fun _          -> ev.Post Error),
            (fun _          -> ev.Post Cancelled),
            ts.Token)

    // Recurs
    let! msg = ev.Receive()
    match msg with
        | Move (id,num) -> //printf "Move:%A\n" (id,num)
                          return! getPlayerInput turn' (ProcessMove game (id,num))
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




