// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

//Denne file skal være NEDERST i hierakiet i view. 
//Dette er nødvendigt for at få compiler til at køre.
open System
open System.Windows.Forms
open WindowStartScreen
open WindowGameScreen


[<EntryPoint>]
[<STAThread>]
let main argv = 
    use form = new Form()
 
    Application.Run(form);
    0 // return an integer exit code
