module ViewPicker
open WindowGameScreen
open WindowStartScreen
open Game
open GuiCallInterface
open System.Threading

let initguiButtons (gui:GUI) (SC:StartScreen) =
    gui.backButton.Click.Add(fun  evArgs -> 
                                            hideGameScreen gui
                                            Game.cancelGameFromGUI ()
                                            gui.eventQueue.Post(0,0)
                                            gui.eventQueue.clear()
                                            Game.clearGameFromGUI()
                                            showStartScreen SC
                                            setGameBackScreen gui
                                            )

    gui.okButton.Click.Add(fun _ -> gui.eventQueue.Post (getSelected gui))
    Game.gameEnder <- (fun player -> setGameEndScreen gui player)
    ()

let initStartButtons (SC:StartScreen) (gui:GUI) (heaps) =
    //The gameender is to be removed when the GUI is appended

    SC.internetButton.Click.Add(fun _ ->    try
                                                startGameFromGUI (loadInternetPage SC, {Name="User"; getMove=(fun heapsfun -> getUserMove heapsfun gui)}, {Name="AI"; getMove=AI.getAIMove})
                                            with
                                            | :? System.Exception -> ()   
                                            | _ ->  activeGameScreen gui
                                                    hideStartScreen SC)
                                            
    SC.startButton.Click.Add(fun _ -> hideStartScreen SC
                                      activeGameScreen gui
                                      Game.startGameFromGUI (heaps(), {Name="User"; getMove=(fun heapsfun -> getUserMove heapsfun gui)}, {Name="AI"; getMove=AI.getAIMove}))


