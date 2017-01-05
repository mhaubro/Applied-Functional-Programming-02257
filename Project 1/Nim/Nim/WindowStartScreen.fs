module WindowStartScreen

open System.Windows.Forms
open System.Drawing
open WindowGameScreen
open HeapArrayLoader

type StartScreen (form:System.Windows.Forms.Form) =
    let _rand = new Button(Location=Point(50,65),MinimumSize=Size(100,50),
                    MaximumSize=Size(100,50),Text="Completely Random", Visible=false)    

    let _semirand = new Button(Location=Point(200,65),MinimumSize=Size(100,50),
                        MaximumSize=Size(100,50),Text="User picks heap amount", Visible=false)

    let _custom = new Button(Location=Point(350,65),MinimumSize=Size(100,50),
                      MaximumSize=Size(100,50),Text="Completely Userdefined", Visible=false)

    let _internet = new Button(Location=Point(350,65),MinimumSize=Size(100,50),
                      MaximumSize=Size(100,50),Text="Load from Internet", Visible=false)

    let _urlbox = new TextBox(Location=Point(50,25),Size=Size(400,25), Visible=false)

//Button for starting a game with a randomly defined number of heaps with random matches
    member this.startButton = _rand

//Button for starting a game with a userdefined number of heaps and
//random matches
    member this.startUserDefinedHeapButton = _semirand

//Button for starting a game with number of heaps and number of matches
//In each heap user defined
    member this.startUserDefinedAllButton = _custom

    //Button for internet
    member this.internetButton = _internet

    //Urlbox for internet
    member this.urlbox = _urlbox

//Adds the controls to the basic form. Everything is initialized as being turned off
let initializeStart (form:System.Windows.Forms.Form) (SC:StartScreen)=
    form.Controls.Add(SC.startButton)
    form.Controls.Add(SC.startUserDefinedHeapButton)
    form.Controls.Add(SC.startUserDefinedAllButton)
    form.Controls.Add(SC.internetButton)
    form.Controls.Add(SC.urlbox)

//Shows the form by setting the visibility-state.
let showStartScreen (SC:StartScreen) =
    SC.startButton.Visible <- true
    SC.startUserDefinedHeapButton.Visible <- true
    SC.startUserDefinedAllButton.Visible <- true
    SC.internetButton.Visible <- true
    SC.urlbox.Visible <- true


//Hides the form by setting the visibility-state.
let hideStartScreen (SC:StartScreen) =
    SC.startButton.Visible <- false
    SC.startUserDefinedHeapButton.Visible <- false
    SC.startUserDefinedAllButton.Visible <- false
    SC.internetButton.Visible <- false
    SC.urlbox.Visible <- false

//Adds functions related to the clicks. Goes to different state.
//As of right now, this is only used to display as an example

let loadInternetPage (SC:StartScreen) = 
    let sysuri = new System.Uri(SC.urlbox.Text)
    Async.RunSynchronously(async {return! loadFromSite sysuri})