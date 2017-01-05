module Game

type Player = {Name:string; getMove:int[]->Async<int*int>}

type PlayerID = A|B

type Game = int[] * Player * Player

val ready : unit -> Async<unit>

val startGameFromGUI : Game -> unit

val startGameFromGUI : Game -> unit
val mutable gameEnder : (Player -> unit)