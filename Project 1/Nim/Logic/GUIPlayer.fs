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
    member internal this.window = new Form(Text=name, Size=Size(500, 500))
    override this.getMove (heapArray:Heap[]) = processMove heapArray
    member internal this.showWindow = this.window.Show()


