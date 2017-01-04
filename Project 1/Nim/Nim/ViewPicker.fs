module ViewPicker
open WindowGameScreen
open WindowStartScreen

//This is the ultimate picker of views.

let initializeForm (form:System.Windows.Forms.Form) =
    initializeGame form
    initializeStart form
    //Picks the form to start with
    showStartScreen form

    //The following two methods are used when a click happens, and the functions
    //Are called.

let gotoGame event = 
    showGameScreen event
    hideStartScreen event

let gotoStart event =
    showStartScreen event 
    hideGameScreen event

    //Must be called to fix the functions to buttons.
let initButtons =
    //As of right now, these do the same.
    startButton.Click.Add(gotoGame)
    startUserDefinedHeapButton.Click.Add(gotoGame)
    startUserDefinedAllButton.Click.Add(gotoGame)
    backButton.Click.Add(gotoStart)

//let setViewState state =
//    | state = 0 -> ...
//    | state = 1 -> ...

