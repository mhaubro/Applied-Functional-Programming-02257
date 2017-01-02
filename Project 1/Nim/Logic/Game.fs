module Game

// Prelude
open System 
open System.Net 
open System.Threading

type Heap = int
type User = string
type Opponent = string

type Game = Heap list * User * Opponent

// Events for the AsyncEventQueue
type Message = 
    | Start of Game | UserMove of Game | OpponentMove of Game | End | Error | Clear
let ev = AsyncEventQueue()
let rec ready() = async {}
and userInput() = async {}
and opponentInput() = async {}
and gameEnded() = async {}

