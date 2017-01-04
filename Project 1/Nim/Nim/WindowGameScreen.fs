module WindowGameScreen
open System.Windows.Forms
open System.Drawing
open System
open Game


/////////////////
// UI-ELEMENTS //
/////////////////

//Combobox containing list for drop-down menu with heaps
let ComboboxHeaps = new ComboBox(Location=Point(100,350), Visible=false, 
                        DropDownStyle=System.Windows.Forms.ComboBoxStyle.DropDownList)
//numericUpDown for matches.
let numericUpDown = new NumericUpDown(Location=Point(300,350), Visible=false)
//So, the text can only be initialized once. Or data can only be bound once.
let dataTextLabel =
    new Label(Location=Point(50,125),Size=Size(400,200),Visible=false, 
                        Text="HI\nHihi\n\n.Net's annoying")
//Ok button
let okButton = new Button(Location=Point(200,400), Text="End turn", Visible=false, 
                        MinimumSize=Size(100,50), MaximumSize=Size(100,50))
//Back button
let backButton = new Button(Location=Point(0,400), Text="Back", Visible=false, 
                        MinimumSize=Size(100,50), MaximumSize=Size(100,50))


///////////////////
// Setting up UI //
///////////////////

//Setting up the Numeric up/down for picking matches
let setUpMatchNumericUpDown max = //NumericUpDown is based on decimals
//    numericUpDown.Value <- (decimal) 1
    numericUpDown.Increment <- (decimal) 1
    numericUpDown.Minimum <- (decimal) 1
    numericUpDown.Maximum <- (decimal) max
    //numericUpDown.Visible = true

//Auxiliary function to be used when loading the heaps.
let addIfNotZero heap index =
    if (heap > 0) then ComboboxHeaps.Items.Add(index)|>ignore

//Loading the heaps from the heaparray
let loadHeaps (heaparray) =
    ComboboxHeaps.Items.Clear()
    //Sets the heaps with matches in
    Array.iteri(fun i heap -> addIfNotZero heap i) heaparray
    //Sets the box to point to the first element. If there is no elements, exception.
    if (ComboboxHeaps.Items.Count > 0) then ComboboxHeaps.SelectedIndex <- 0 else failwith("Exception e raised")


//gets the string for displaying
let getHeapString heapArray = 
    (fst (Array.fold (fun (s,i) heap -> (s + i.ToString() + ": " + heap.ToString() + "\n",i+1)) ("",0) heapArray)).Trim();;

//Event handler, To be triggered, when the heap number is changed, because
//Then the match-counter has to be changed as well.
let heapChangeEventHandler (eventargs) =
    setUpMatchNumericUpDown (Int32.Parse (ComboboxHeaps.SelectedItem.ToString()))


////////////////////
// Initialization //
////////////////////

    //Initializes UI Elements
let initializeGame (form:System.Windows.Forms.Form) =
    form.Controls.Add(ComboboxHeaps)
    form.Controls.Add(numericUpDown)
    form.Controls.Add(okButton)
    form.Controls.Add(dataTextLabel)
    form.Controls.Add(backButton)

    //Below this is only for testing, to be deleted
//    dataTextLabel.Text <- "Hi again"
//    form.Controls.Add(dataTextLabel)
    ComboboxHeaps.Items.Add(1)|>ignore
    ComboboxHeaps.Items.Add(7)|>ignore
    ComboboxHeaps.Items.Add(3)|>ignore
//    ComboboxHeaps.SelectedIndex <- 0
    ComboboxHeaps.SelectedIndex <- 0
    ComboboxHeaps.SelectedIndexChanged.Add (heapChangeEventHandler)
    setUpMatchNumericUpDown (Int32.Parse (ComboboxHeaps.SelectedItem.ToString()))

/////////////////////////
// Setting the display //
/////////////////////////

    //Finds the text for the match, and displays it
let setText heaparray =
    dataTextLabel.Text = getHeapString heaparray

    //Shows the game screen
let showGameScreen _ =
    ComboboxHeaps.Visible <- true
    numericUpDown.Visible <- true
    okButton.Visible <- true
    dataTextLabel.Visible <- true
    backButton.Visible <- true

    //Hides the game screen
let hideGameScreen _ =
    ComboboxHeaps.Visible <- false
    numericUpDown.Visible <- false
    okButton.Visible <- false
    dataTextLabel.Visible <- false
    backButton.Visible <- false    

    //This is always to be called, since it will set up the data in the game.

    //Refreshing the display
let setUpGameScreen (game:Game) = 
    let (gamearr, name1, name2) = game
    loadHeaps (gamearr)
    setText gamearr|>ignore //Returns a bool, but bool should be ignored.
    //Sets up the match-counter.
    setUpMatchNumericUpDown (Int32.Parse (ComboboxHeaps.SelectedItem.ToString()))