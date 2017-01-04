module Game

type Heap = int

[<AbstractClass>]
type Player =
    class
        new : name:string -> Player
        member name : string
        abstract member getMove : Heap[] -> int*int
    end

type Game = Heap[] * Player * Player


