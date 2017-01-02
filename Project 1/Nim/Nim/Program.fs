// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System
open System.Windows.Forms

[<EntryPoint>]
[<STAThread>]//Necessary for UI elements. Indicates single-threaded apartment
let main argv = //Main-method
    use form = new Form()
 
    Application.Run(form);
    0 // return an integer exit code
