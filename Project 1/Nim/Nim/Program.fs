// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

// Prelude
open System 
open System.Net 
open System.Threading 
open System.Windows.Forms 
open System.Drawing 

let window =
    new Form(Text="Web Source Length", Size=Size(525,225));;

[<EntryPoint>]
let main argv = 
    window.Show()
    0 // return an integer exit code
