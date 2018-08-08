module Startup

open CommandLineActions
open WunderlistBackup

type Result =
    Success | Failure

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

let commandLinePick () =
    CommandLine.pick [ Sync; View ]
                     (fun ba -> match ba with 
                                | Sync -> "Start new backup"
                                | View -> "View backups")  

let queryUserForAction commandLinePick mapActionTypeToAction () =
    let actionTypeOption = commandLinePick()       

    let action =
        match actionTypeOption with
        | None -> None
        | Some actionType -> Some (mapActionTypeToAction actionType)

    action

let run queryUserForAction =
    let userSelection = queryUserForAction()

    match userSelection with
    | None -> Failure
    | Some action -> action()
        
[<EntryPoint>]
let main args =
    let syncAction () =
        syncAction ParseArguments.parseCredentials CommandLineActions.asyncSaveToDatabase args

    let mapActionTypeToAction actionType = 
        mapActionTypeToAction viewAction syncAction actionType

    let queryUserForAction = 
        queryUserForAction commandLinePick mapActionTypeToAction

    match run queryUserForAction with
    | Success -> 0
    | Failure -> 1