module GuiCallInterface
open Game
open WindowGameScreen
open System.Threading

//Might work
let getUserMove heaparray (gui:GUI) =
    //Shows the gui
    showGameScreen gui
    setUpGameScreen gui
    //Listens for gui input
    async{
        //failwith("Receive")

        let! msg = gui.eventQueue.Receive()
        
        match msg with
        | (a, b) ->    hideGameScreen gui//Hides the gui again 
                       return (a, b)//Lazy fix
        | _ ->  hideGameScreen gui//Hides the gui again
                return failwith("Unexpected user input")}
    //move
