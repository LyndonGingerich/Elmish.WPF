﻿module Elmish.WPF.Samples.NewWindow.Window2Module

open Elmish.WPF


[<RequireQualifiedAccess>]
type ConfirmState =
  | Submit
  | Cancel
  | Close

type Window2 =
  { Input: string
    IsChecked: bool
    ConfirmState: ConfirmState option }

type Window2Msg =
  | SetInput of string
  | SetChecked of bool
  | Submit
  | Cancel
  | Close

[<RequireQualifiedAccess>]
type Window2OutMsg =
  | Close


module Window2 =
  let init =
    { Input = ""
      IsChecked = false
      ConfirmState = None }

  let update msg window2 =
    match msg with
    | SetInput s -> { window2 with Input = s }
    | SetChecked b -> { window2 with IsChecked = b }
    | Submit -> { window2 with ConfirmState = ConfirmState.Submit |> Some }
    | Cancel -> { window2 with ConfirmState = ConfirmState.Cancel |> Some }
    | Close  -> { window2 with ConfirmState = ConfirmState.Close  |> Some }

  let private confirmStateVisibilityBinding confirmState bindingName =
    bindingName |> Binding.oneWay (fun m -> m.ConfirmState = Some confirmState |> Bool.toVisibilityCollapsed)

  let private confirmStateToMsg confirmState msg m =
    if m.ConfirmState = Some confirmState
    then InOut.Out Window2OutMsg.Close
    else InOut.In msg

  let bindings () =
    let inBindings =
      [ "Input" |> Binding.twoWay ((fun window2 -> window2.Input), SetInput)
        "IsChecked" |> Binding.twoWay ((fun window2 -> window2.IsChecked), SetChecked)
        "SubmitMsgVisibility" |> confirmStateVisibilityBinding ConfirmState.Submit
        "CancelMsgVisibility" |> confirmStateVisibilityBinding ConfirmState.Cancel
        "CloseMsgVisibility"  |> confirmStateVisibilityBinding ConfirmState.Close ]
      |> Bindings.mapMsg InOut.In
    let inOutBindings =
      [ "Submit" |> Binding.cmd (confirmStateToMsg ConfirmState.Submit Submit)
        "Cancel" |> Binding.cmd (confirmStateToMsg ConfirmState.Cancel Cancel)
        "Close"  |> Binding.cmd (confirmStateToMsg ConfirmState.Close  Close) ]
    inBindings @ inOutBindings

let designVm = ViewModel.designInstance Window2.init (Window2.bindings ())