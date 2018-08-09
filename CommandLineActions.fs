module CommandLineActions

open WunderlistBackup
open Actions

let commandLinePick () =
    CommandLine.pick [ Sync; View ]
                     (fun ba -> match ba with 
                                | Sync -> "Start new backup"
                                | View -> "View backups") 

let asyncBackup credentials =
    credentials |>
    Wunderlist.getAllData

let asyncSaveToDatabase credentials =
    async {
        let! backup = asyncBackup credentials

        do! Save.saveToDb backup
    }

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