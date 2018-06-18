namespace WunderlistBackup

open System

[<CLIMutable>]
type BackUp =
    {
        id        : string
        timestamp : DateTimeOffset

        folders   : Folder  list
        lists     : WList   list
        tasks     : Task    list
        subtasks  : Subtask list
        notes     : Note    list
        files     : File    list
    }

