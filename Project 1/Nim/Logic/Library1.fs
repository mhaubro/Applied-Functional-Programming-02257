namespace Logic

type Heap = int;;
type Player = string;;

type GameInfo = Heap list;;
type Game = Player * Player * GameInfo;;

type Class1() = 
    member this.X = "F#"
