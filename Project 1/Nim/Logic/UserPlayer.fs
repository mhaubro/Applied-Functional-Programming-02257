module UserPlayer

open System
open System.Windows.Forms
open System.Drawing
open Game

let processMove (heapArray:Heap[]) =
    (1,1)

type GUIPlayer (name:string) =
    inherit Player(name)
    let w = new Form(Text="HI!", Size=Size(500, 500))
    do Application.Run(w);
    member this.window = w
    override this.getMove (heapArray:Heap[]) = processMove heapArray
    


