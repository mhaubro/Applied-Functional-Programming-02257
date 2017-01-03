module WindowStartScreen

open System.Windows.Forms
open System.Drawing
open WindowGameScreen

//Button for starting a game with a randomly defined number of heaps with
//Randomly defined matches (1-6 heaps with 1-6 matches)
let startButton =
  new Button(Location=Point(50,65),MinimumSize=Size(100,50),
              MaximumSize=Size(100,50),Text="Completely Random", Visible=false)

//Button for starting a game with a userdefined number of heaps and
//random matches
let startUserDefinedHeapButton =
  new Button(Location=Point(200,65),MinimumSize=Size(100,50),
              MaximumSize=Size(100,50),Text="User picks heap amount", Visible=false)

//Button for starting a game with number of heaps and number of matches
//In each heap user defined
let startUserDefinedAllButton =
  new Button(Location=Point(350,65),MinimumSize=Size(100,50),
              MaximumSize=Size(100,50),Text="Completely Userdefined", Visible=false)

//Adds the controls to the basic form. Everything is initialized as being turned off
let initializeStart (form:System.Windows.Forms.Form) =
    form.Controls.Add(startButton)
    form.Controls.Add(startUserDefinedHeapButton)
    form.Controls.Add(startUserDefinedAllButton)

//Shows the form by setting the visibility-state.
let showStartScreen _ =
    startButton.Visible <- true
    startUserDefinedHeapButton.Visible <- true
    startUserDefinedAllButton.Visible <- true

//Hides the form by setting the visibility-state.
let hideStartScreen _ =
    startButton.Visible <- false
    startUserDefinedHeapButton.Visible <- false
    startUserDefinedAllButton.Visible <- false

//Adds functions related to the clicks. Goes to different state.
//As of right now, this is only used to display as an example

