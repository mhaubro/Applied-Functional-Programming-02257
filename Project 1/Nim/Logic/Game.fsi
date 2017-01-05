module Game

type Player = {Name:string; getMove:int[]->Async<int*int>}

type PlayerID = A|B

type Game = int[] * Player * Player

/// Enumeration of messages - alphabet of FSM
type Message =
    | Start of Game
    | Move of int*int
    | Win of Player
    | Error
    | Clear
    | Cancelled


val ready : unit -> Async<unit>

val cancelGameFromGUI : unit -> unit
val clearGameFromGUI : unit -> unit

val startGameFromGUI : Game -> unit

val startGameFromGUI : Game -> unit
val mutable gameEnder : (Player -> unit)