module WindowGameScreen
open System.Windows.Forms
open Game
//This module describes the screen that's displayed when the user is playing.
//First a simple description of how heaps look is developed.
//type Game = Heap [] * User * Opponent

//Combobox containing list for drop-down menu with heaps
let ComboboxHeaps = new ComboBox()
//numericUpDown for matches.
let numericUpDown = new NumericUpDown()

//NumericUPDown
let setUpMatchNumericUpDown max = //NumericUpDown is based on decimals
    numericUpDown.Value <- (decimal) 1
    numericUpDown.Increment <- (decimal) 1
    numericUpDown.Minimum <- (decimal) 1
    numericUpDown.Maximum <- (decimal) max

let addIfNotZero heap index =
    if (heap > 0) then ComboboxHeaps.Items.Add(index)|>ignore

//Genererating heaps
let loadHeaps (heaparray) =
    ComboboxHeaps.Items.Clear()
    Array.iteri(fun i heap -> addIfNotZero heap i) heaparray


let setUpGameScreen (game:Game) = 
    loadHeaps (fst game)
    setUpMatchNumericUpDown ComboboxHeaps.SelectedValue
    


















//Gets the nth element of a list.
//let rec getNth list n = 
//    match n with
//    | head::tail N when N = 0 -> head
//    | head::tail N when N > 0 -> getNth(tail, N)
//    | _ -> failwith("Error")


//Finds the amount of matches to be selected, such that one can remove from the heap.
    

//heapVisuals contains the elements to be shown.
//It's to be cleared and re-initialized every time a new game is started.
//let heapVisuals = []//Dummy value until now.

//Dropdown for choosing heap must be calculated, since empty heaps shouldn't be
//Selectable

//let calcDropDown = 
//...

//Bind this to a combobox

//Maybe create it lazily or work hard?

let getHeapString heapArray = 
    (fst (Array.fold (fun (s,i) heap -> (s + i.ToString() + ": " + heap.ToString() + "\n",i+1)) ("",0) heapArray)).Trim();;


