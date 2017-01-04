
open System.Windows.Forms
open System.Drawing
open System

type GUI(name:string) as _this = 
    let _window = new Form(Text="bib", Size=Size(500,500))
    let _button = new Button(Location=Point(200,400), Text="bob", Visible=true, 
                      MinimumSize=Size(100,50), MaximumSize=Size(100,50))
    do
        _button.Click.Add(_this.pushButton)
        _window.Controls.Add(_button)
        _window.Closing.Add(_this.onClose)
        _window.Show()

    member this.name = "bib"
    member this.window = _window
    member this.button = _button
    member private this.pushButton _ = 
        let temp = this.window.Text
        this.window.Text <- this.button.Text
        this.button.Text <- temp
    member private this.onClose _ = ()
        

;;

let gui = new GUI("hab");;

//let form = new Form(Text="bib", Size=Size(500,500));;
//
//let button = new Button(Location=Point(200,400), Text="bob", Visible=true, 
//                        MinimumSize=Size(100,50), MaximumSize=Size(100,50));;
//
//let pushButton _ = 
//    let temp = form.Text
//    form.Text <- button.Text
//    button.Text <- temp;;
//
//button.Click.Add(pushButton);;
//form.Controls.Add(button);;
//form.Show();;
//    
//        
