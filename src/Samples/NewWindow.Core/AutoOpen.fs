[<AutoOpen>]
module AutoOpen



[<RequireQualifiedAccess>]
module Bool =
  open System.Windows
  let toVisibilityCollapsed = function
    | true  -> Visibility.Visible
    | false -> Visibility.Collapsed


[<RequireQualifiedAccess>]
type InOut<'a, 'b> =
  | In of 'a
  | Out of 'b
