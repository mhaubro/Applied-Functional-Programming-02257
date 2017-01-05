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
open Game

[<EntryPoint>]
[<STAThread>]
let main argv = 
    let window = new Form(Text="HI!", Size=Size(500, 500))
    let game = ([|4;5;6;5|], "P1", "P2")
    let (heaps, _, _) = game
    Async.Start(Game.ready())
    let gui = new GUI(heaps, window)
    let startscreen = new StartScreen(window)
    initializeStart window startscreen
    initializeGame gui window
    initStartButtons startscreen gui heaps   
    initguiButtons gui  
    showStartScreen startscreen
    //showGameScreen gui
    //initializeGame gui window
    //setUpGameScreen gui
    //gui.dataTextLabel.Text <- "Bugtest"
    //window.Controls.Add(gui.ComboboxHeaps)
    //initializeForm(window)
    //let (heaparr, _, _) = game
    //heaparr.[2] <- 2
    //setUpGameScreen gui
    Application.Run(window)
    0 // return an integer exit code
