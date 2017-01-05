module GuiCallInterface
open Game
open WindowGameScreen

//Might work
let getUserMove heaparray (gui:GUI) =
    //Shows the gui
    showGameScreen gui
    setUpGameScreen gui
    //Listens for gui input
    let mutable move = (0,0)
    Async.StartImmediate(async{
        //failwith("Receive")

        let! msg = gui.eventQueue.Receive()
        
        match msg with
        | (a, b) ->    hideGameScreen gui//Hides the gui again 
                       do move <- (a, b)//Lazy fix
        | _ ->  hideGameScreen gui//Hides the gui again
                return failwith("Unexpected user input")})
    move
