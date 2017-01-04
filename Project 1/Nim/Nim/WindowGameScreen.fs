module WindowGameScreen
open System.Windows.Forms
open System.Drawing
open System
open Game
open EventQueue

/////////////////
// UI-ELEMENTS //
/////////////////

type GUI (game:Game, form:System.Windows.Forms.Form) =
    
    //Game object
    let _g = game
    //Combobox
    let _c = new ComboBox(Location=Point(100,350), Visible=false, 
                                    DropDownStyle=System.Windows.Forms.ComboBoxStyle.DropDownList)
    //Form for the ui
    let _f = form
    //Numericupdown for match selection
    let _n = new NumericUpDown(Location=Point(300,350), Visible=false)
    //Label for datatext
    let _datatext = new Label(Location=Point(50,125),Size=Size(400,200),Visible=false, 
                                    Text="HI\nHihi\n\n.Net's annoying")
    //OKbutton
    let _ok = new Button(Location=Point(200,400), Text="End turn", Visible=false, 
                               MinimumSize=Size(100,50), MaximumSize=Size(100,50))
    //Backbutton
    let _back = new Button(Location=Point(0,400), Text="Back", Visible=false, 
                                 MinimumSize=Size(100,50), MaximumSize=Size(100,50))
    let _eq = AsyncEventQueue()

    member this.game = _g
    member this.parentForm = _f
    //Combobox containing list for drop-down menu with heaps
    member this.ComboboxHeaps = _c
    //numericUpDown for matches.
    member this.numericUpDown = _n
    member this.dataTextLabel = _datatext
//Ok button
    member this.okButton = _ok
//Back button
    member this.backButton = _back
//Personal Event Queue
    member this.eventQueue = _eq

///////////////////
// Setting up UI //
///////////////////

//Setting up the Numeric up/down for picking matches
let setUpMatchNumericUpDown (gui:GUI) = //NumericUpDown is based on decimals
//    numericUpDown.Value <- (decimal) 1
    gui.numericUpDown.Increment <- (decimal) 1
    gui.numericUpDown.Minimum <- (decimal) 1
    let (heaparr, _, _) = gui.game
    gui.numericUpDown.Maximum <- ((decimal) (Array.get heaparr (Int32.Parse (gui.ComboboxHeaps.SelectedItem.ToString()))))//TODO EDIT
    //numericUpDown.Visible = true

//Auxiliary function to be used when loading the heaps.
let addIfNotZero (gui:GUI) heap index =
    if (heap > 0) then gui.ComboboxHeaps.Items.Add(index)|>ignore//Returns value, but should not

//Loading the heaps from the heaparray
let loadHeaps (gui:GUI) (heaparray) =
    gui.ComboboxHeaps.Items.Clear()
    //Sets the heaps with matches in
    Array.iteri(fun i heap -> addIfNotZero gui heap i) heaparray
    //Sets the box to point to the first element. If there is no elements, exception.
    if (gui.ComboboxHeaps.Items.Count > 0) then gui.ComboboxHeaps.SelectedIndex <- 0 else failwith("Exception e raised")


//gets the string for displaying
let getHeapString heapArray = 
    (fst (Array.fold (fun (s,i) heap -> (s + i.ToString() + ": " + heap.ToString() + "\n",i+1)) ("",0) heapArray)).Trim();;

//Event handler, To be triggered, when the heap number is changed, because
//Then the match-counter has to be changed as well.
let heapChangeEventHandler (evArgs) (sender:GUI) =
    setUpMatchNumericUpDown sender

////////////////////
// Initialization //
////////////////////

    //Initializes UI Elements
let initializeGame (gui:GUI) (form:System.Windows.Forms.Form) =
    form.Controls.Add(gui.ComboboxHeaps)
    form.Controls.Add(gui.numericUpDown)
    form.Controls.Add(gui.okButton)
    form.Controls.Add(gui.dataTextLabel)
    form.Controls.Add(gui.backButton)
    //Add event handler for change in heap selection
    gui.ComboboxHeaps.SelectedIndexChanged.Add (fun evArgs -> heapChangeEventHandler evArgs gui)

/////////////////////////
// Setting the display //
/////////////////////////

    //Finds the text for the match, and displays it
let setText heaparray (gui:GUI) =
    gui.dataTextLabel.Text <- getHeapString heaparray
    ()

    //Shows the game screen
let showGameScreen (gui:GUI) =
    gui.ComboboxHeaps.Visible <- true
    gui.numericUpDown.Visible <- true
    gui.okButton.Visible <- true
    gui.dataTextLabel.Visible <- true
    gui.backButton.Visible <- true

    //Hides the game screen
let hideGameScreen (gui:GUI) =
    gui.ComboboxHeaps.Visible <- false
    gui.numericUpDown.Visible <- false
    gui.okButton.Visible <- false
    gui.dataTextLabel.Visible <- false
    gui.backButton.Visible <- false    

    //Makes the game screen inactive while waiting
let inactiveGameScreen (gui:GUI) =
    gui.ComboboxHeaps.Enabled <- false
    gui.numericUpDown.Enabled <- false
    gui.okButton.Enabled <- false
    gui.dataTextLabel.Enabled <- false
    gui.backButton.Enabled <- false    

    //Makes the game screen active again
let activeGameScreen (gui:GUI) =
    gui.ComboboxHeaps.Enabled <- true
    gui.numericUpDown.Enabled <- true
    gui.okButton.Enabled <- true
    gui.dataTextLabel.Enabled <- true
    gui.backButton.Enabled <- true
    showGameScreen//Maybe unnecessary 


    //This is always to be called, since it will set up the data in the game.
    //Refreshing the display
let setUpGameScreen (gui:GUI) = //(game:Game)
    let (gamearr, name1, name2) = gui.game
    loadHeaps gui (gamearr)
    //dataTextLabel.Text = "TestTest"
    setText gamearr gui //Returns a bool, but bool should be ignored.
    //Sets up the match-counter.
    setUpMatchNumericUpDown gui//(Int32.Parse (gui.ComboboxHeaps.SelectedItem.ToString()))
