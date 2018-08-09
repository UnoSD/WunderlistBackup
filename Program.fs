module Startup

open CommandLineActions
open ActionsComposition
open Actions
        
[<EntryPoint>]
let main args =
    match queryUserForAction args |> run with
    | Success -> 0
    | Failure -> 1