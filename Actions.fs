module Actions

type Result =
    Success | Failure

type BackupAction = 
    Sync | View

let syncAction parseCredentials asyncSaveToDatabase args =
    let credentials = 
        parseCredentials args
    
    match credentials with
    | Some credentials -> credentials |> 
                          asyncSaveToDatabase |>
                          Async.RunSynchronously
                          Success
    | None             -> Failure

let viewAction () =
    View.pickDisplay() |> ignore
    Success

let mapActionTypeToAction viewAction syncAction actionType =
    match actionType with
    | View -> viewAction
    | Sync -> syncAction