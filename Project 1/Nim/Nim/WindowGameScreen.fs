module WindowGameScreen
open System.Windows.Forms
open Game
//This module describes the screen that's displayed when the user is playing.
//First a simple description of how heaps look is developed.
//type Game = Heap list * User * Opponent

//Combobox containing list for drop-down menu with heaps
let ComboboxHeaps = new ComboBox()
//Combobox for dropdown menu with matches.
let ComboboxMatches = new ComboBox()

//Genererating heaps
let loadHeaps (heaplist:List<int>) =
    ComboboxHeaps.Items.Clear() 
    ComboboxHeaps.Items.Add(heaplist)

//Gives the amount of matches from the heap selected in the first comboboxheaps.
//let numberMatches =

//Finds the amount of matches to be selected, such that one can remove from the heap.
let loadMatchRemoval = 
    ComboboxMatches.Items.Clear()
//    ComboboxMatches.Items.Add(Seq.toList(seq {1 ..  numberMatches}))
    

//heapVisuals contains the elements to be shown.
//It's to be cleared and re-initialized every time a new game is started.
//let heapVisuals = []//Dummy value until now.

//Dropdown for choosing heap must be calculated, since empty heaps shouldn't be
//Selectable

//let calcDropDown = 
//...

//Bind this to a combobox

//Maybe create it lazily or work hard?

