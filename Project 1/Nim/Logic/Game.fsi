module Game

/// Player struct with name and function handle to produce moves from gamestate
type Player = {Name:string; getMove:int[]->Async<int*int>}

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

///Initial state - awaits a game posted through startGameFromGUI
val ready : unit -> Async<unit>

/// Function to forfeit game from gui
val cancelGameFromGUI : unit -> unit

/// Guarded function to clear an ended game from gui
val clearGameFromGUI : unit -> unit

/// Guarded function to start a game from gui
val startGameFromGUI : Game -> unit

///Handler for function to be called when the game ends - should maybe be 
val mutable gameEnder : (Player -> unit)