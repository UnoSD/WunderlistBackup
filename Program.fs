module Startup

open MaybeBuilder
open ParseArguments
open CommandLineActions

[<EntryPoint>]
let main args =
    match maybe
          {
              let! action = 
                pickAction()
              
              let! credentials = 
                  match action with
                  | View -> View.pickDisplay() |> ignore
                            None
                  | Sync -> parseCredentials args
              
              credentials |>
              asyncSaveToDatabase |>
              Async.RunSynchronously
              
              return credentials
          } with
    | Some _ -> 0
    | None   -> 1