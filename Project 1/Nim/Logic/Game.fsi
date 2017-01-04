module Game


type Heap = int

type Player = {Name:string; getMove:int[]->int*int}

type PlayerID = A|B

type Game = Heap[] * Player * Player

val startGameFromGUI : Game -> unit

val startGameFromGUI : Game -> unit
val mutable gameEnder : Player -> unit