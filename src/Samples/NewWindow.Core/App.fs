module Elmish.WPF.Samples.NewWindow.AppModule

open System.Windows

open Elmish.WPF

open Window1Module
open Window2Module


type App =
  { Window1: WindowState<string>
    Window2: Window2 option }

type AppMsg =
  | Window1Show
  | Window1Hide
  | Window1Close
  | Window1SetInput of string
  | Window2Show
  | Window2Close
  | Window2Msg of Window2Msg


module App =
  let init =
    { Window1 = WindowState.Closed
      Window2 = None }

  let update msg app =
    match msg with
    | Window1Show -> { app with Window1 = WindowState.toVisible "" app.Window1 }
    | Window1Hide -> { app with Window1 = WindowState.toHidden "" app.Window1 }
    | Window1Close -> { app with Window1 = WindowState.Closed }
    | Window1SetInput s -> { app with Window1 = app.Window1 |> WindowState.set s }
    | Window2Show -> { app with Window2 = Some Window2.init }
    | Window2Close -> { app with Window2 = None }
    | Window2Msg msg ->
      let mapWindow2 = msg |> Window2.update |> Option.map
      { app with Window2 = mapWindow2 app.Window2 }

  let bindings (createWindow1: unit -> #Window) (createWindow2: unit -> #Window) () = [
    "Window1Show" |> Binding.cmd Window1Show
    "Window1Hide" |> Binding.cmd Window1Hide
    "Window1Close" |> Binding.cmd Window1Close
    "Window2Show" |> Binding.cmd Window2Show
    "Window1" |> Binding.subModelWin(
      (fun app -> app.Window1),
      snd,
      id,
      Window1.bindings >> Bindings.mapMsg Window1SetInput,
      createWindow1)

    let mapWindow2Msg = // can't detect correct overload of `subModelWin` if inlined
      function
      | InOut.In inMsg -> inMsg |> Window2Msg
      | InOut.Out Window2OutMsg.Close -> Window2Close

    "Window2" |> Binding.subModelWin(
      (fun app -> app.Window2) >> WindowState.ofOption,
      snd,
      mapWindow2Msg,
      Window2.bindings,
      createWindow2,
      isModal = true)
  ]

let designVm =
  let fail _ = failwith "never called"
  ViewModel.designInstance App.init (App.bindings fail fail ())