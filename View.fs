module View

open LiteDB
open WunderlistBackup
open Serialization
open MaybeBuilder
open System

type DiskFile = 
    IO.File

type Path =
    IO.Path

type TaskItem =
    Subtask of Subtask | Note of Note | File of File

type TaskItemDisplayable =
    TaskItem * string
    
let pickDisplay() =
    use db = new LiteDatabase("wunderlist.db")

    let allBackUps =
        db.GetCollection("BackUp")
          .FindAll() |>
        Seq.map toBackUp |>
        Seq.toList

    maybe {
        let! backup = CommandLine.pick allBackUps (fun b -> b.timestamp)

        let folders = None :: (backup.folders |> List.map Some)

        let! folder = CommandLine.pick folders (fun f -> match f with
                                                         | Some f -> f.Title
                                                         | None   -> "No folder")

        let folderLists =
            backup.lists |> List.filter (fun list -> match folder with
                                                     | Some folder -> folder.ListIds |>
                                                                      Array.contains list.Id
                                                     | None -> backup.folders |>
                                                               Seq.collect (fun f -> f.ListIds) |>
                                                               Seq.contains list.Id |>
                                                               not)

        let! list = CommandLine.pick folderLists (fun l -> l.Title)
            
        let listTasks =
            backup.tasks |> List.filter (fun task -> task.ListId = list.Id)

        let! task = CommandLine.pick listTasks (fun t -> t.Title)

        let subtasks =
            backup.subtasks |>
            List.filter (fun subtask -> subtask.TaskId = task.Id) |>
            List.map Subtask

        let notes =
            backup.notes |> 
            List.filter (fun note -> note.TaskId = task.Id) |>
            List.map Note

        let files =
            backup.files |>
            List.filter (fun file -> file.TaskId = task.Id) |>
            List.map File

        let items = 
            [ subtasks; files; notes ] |> List.concat
            
        let! item =
            CommandLine.pick items (fun i -> match i with
                                             | Subtask s -> sprintf "Subtask: %s" s.Title
                                             | Note n -> sprintf "Note: %s" n.Content
                                             | File f -> sprintf "File: %s" f.FileName)

        return match item with 
               | File f -> let tempPath = sprintf "%s.%s" (Path.GetTempFileName()) f.FileName
                           let fileStream = DiskFile.OpenWrite(tempPath)
                           db.FileStorage.Download(f.Id.ToString(), fileStream) |> ignore
                           fileStream.Flush()
                           fileStream.Dispose()
                           //Diagnostics.Process.Start(tempPath) |> ignore
                           Console.WriteLine tempPath
               | _ -> ignore()
    }
