module ViewPicker
open WindowGameScreen
open WindowStartScreen
open Game
//This is the ultimate picker of views.

let initializeForm (form:System.Windows.Forms.Form) =
    //initializeGame form
    initializeStart form
    //Picks the form to start with
    showStartScreen form

    //The following two methods are used when a click happens, and the functions
    //Are called.

let gotoGame gui = 
    showGameScreen gui
    hideStartScreen gui

let gotoStart (gui:GUI) =
    showStartScreen gui 
    hideGameScreen gui

    //Must be called to fix the functions to buttons.
let initButtons (gui:GUI) =
    //As of right now, these do the same.
    startButton.Click.Add(fun _ -> Game.startGameFromUI {heaps, {Name="User"; getMove=gui.getUserMove}})
    //startUserDefinedHeapButton.Click.Add(gotoGame)
    //startUserDefinedAllButton.Click.Add(gotoGame)
    gui.backButton.Click.Add(fun  evArgs -> gotoStart gui)

//let setViewState state =
//    | state = 0 -> ...
//    | state = 1 -> ...

