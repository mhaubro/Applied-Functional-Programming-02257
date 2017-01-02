module WindowStartScreen

open System.Windows.Forms
open System.Drawing

//let WindowStartForm = new Form(Text="HI13", Size=Size(300,300), Visible=true)

let urlBox =
  new TextBox(Location=Point(50,25),Size=Size(400,25), Visible=true)

let ansBox =
  new TextBox(Location=Point(150,150),Size=Size(200,25), Visible=true)

let startButton =
  new Button(Location=Point(50,65),MinimumSize=Size(100,50),
              MaximumSize=Size(100,50),Text="START", Visible=true)

let clearButton =
  new Button(Location=Point(200,65),MinimumSize=Size(100,50),
              MaximumSize=Size(100,50),Text="CLEAR", Visible=true)

let cancelButton =
  new Button(Location=Point(350,65),MinimumSize=Size(100,50),
              MaximumSize=Size(100,50),Text="CANCEL", Visible=true)

let setUpForm (form:System.Windows.Forms.Form) =
    form.Controls.Add(urlBox)
    form.Controls.Add(ansBox)
    form.Controls.Add(startButton)
    form.Controls.Add(clearButton)
    form.Controls.Add(cancelButton)
//    form = WindowStartForm
//
let hideForm _ =
    urlBox.Visible <- false
    ansBox.Visible <- false


startButton.Click.Add(hideForm)

