module CommandLineActions

open WunderlistBackup

type BackupAction = 
    Sync | View

let pickAction() =
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