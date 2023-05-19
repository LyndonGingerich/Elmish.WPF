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
  module Window1 =
    let lens = lens (fun app -> app.Window1) (fun v app -> { app with Window1 = v })
  module Window2 =
    let lens = lens (fun app -> app.Window2) (fun v app -> { app with Window2 = v })

  let init =
    { Window1 = WindowState.Closed
      Window2 = None }

  let update = function
    | Window1Show -> "" |> WindowState.toVisible |> Window1.lens
    | Window1Hide -> "" |> WindowState.toHidden  |> Window1.lens
    | Window1Close -> fun app -> { app with Window1 = WindowState.Closed }
    | Window1SetInput s -> s |> WindowState.set |> Window1.lens
    | Window2Show -> Window2.init |> Some |> fun v app -> { app with Window2 = v }
    | Window2Close -> fun app -> { app with Window2 = None }
    | Window2Msg msg -> msg |> Window2.update |> Option.map |> Window2.lens

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