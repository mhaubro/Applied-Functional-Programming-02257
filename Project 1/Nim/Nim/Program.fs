// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

//Denne file skal være NEDERST i hierakiet i view. 
//Dette er nødvendigt for at få compiler til at køre.
open System
open System.Windows.Forms
open System.Drawing
open WindowStartScreen
open WindowGameScreen
open ViewPicker


[<EntryPoint>]
[<STAThread>]
let main argv = 
    let window = new Form(Text="HI!", Size=Size(500, 500))
    initializeForm(window)
    initButtons
    Application.Run(window)
    0 // return an integer exit code
