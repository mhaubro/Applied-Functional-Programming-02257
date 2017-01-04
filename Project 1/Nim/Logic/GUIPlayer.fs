module GUIPlayer

open System
open System.Windows.Forms
open System.Drawing
open Game

let processMove (heapArray:Heap[]) =
    (1,1)

type GUIPlayer (name:string) as this =
    inherit Player(name)
    do this.showWindow
    member private this.window = new Form(Text=name, Size=Size(500, 500))
    member private this.showWindow = this.window.Show()
    override this.getMove (heapArray:Heap[]) = processMove heapArray
    