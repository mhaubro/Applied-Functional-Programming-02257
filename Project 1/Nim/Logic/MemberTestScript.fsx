
open System.Windows.Forms
open System.Drawing
open System
open System.Threading

type GUI(name:string, form:System.Windows.Forms.Form) as _this = 
    let _window = form
    let _button1 = new Button(Location=Point(200,400), Text="A", Visible=false, 
                      MinimumSize=Size(100,50), MaximumSize=Size(100,50))
    let _button2 = new Button(Location=Point(300,400), Text="B", Visible=false, 
                      MinimumSize=Size(100,50), MaximumSize=Size(100,50))
    do
        _button1.Click.Add(fun evtargs -> _this.pushButton _this.button1)
        _button2.Click.Add(fun evtargs -> _this.pushButton _this.button2)
        _window.Controls.Add(_this.button1)
        _window.Controls.Add(_this.button2)
        _window.Closing.Add(_this.onClose)
        _window.Show()
        _button1.Visible <- true
        _button2.Visible <- true
        _window.Text <- _this.name

    member this.name = "C"
    member this.window = _window
    member this.button1 = _button1
    member this.button2 = _button2
    member this.pushButton (sender:System.Windows.Forms.Button) = 
        sender.Enabled <- false
        let temp = this.window.Text
        this.window.Text <- sender.Text
        sender.Text <- temp
        Async.Start(async{
            do! Async.Sleep(1000)
            sender.Enabled <- true
        })  
    member this.onClose _ = ()
        
;;

let form = new Form(Text="this is a form", Size=Size(500,500));;

let gui = new GUI("hab",form);;

      
