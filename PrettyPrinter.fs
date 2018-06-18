namespace WunderlistBackup

module PrettyPrinter =
    open System

    let formatForDisplay backup =
        let getLists listIds = 
            backup.lists |> 
            List.filter (fun list -> listIds |> Array.contains list.Id) 

        let getTasks listId =
            backup.tasks |>
            List.filter (fun task -> task.ListId = listId) |>
            List.sortBy (fun task -> task.Title) |>
            List.fold (fun output task -> sprintf "\t\t%s\n\t\t%s" output task.Title) String.Empty

        let formatListsForDisplay (lists : WList list) =
            lists |>
            List.sortBy (fun list -> list.Title) |>
            List.fold (fun output list -> 
                          sprintf "\t%s\n\t%s\n%s" output list.Title (getTasks list.Id)) String.Empty
    
        backup.folders |>
        List.sortBy (fun folder -> folder.Title) |>
        List.map (fun folder -> (folder.Title, folder.ListIds |> getLists)) |>
        List.fold (fun output (title, lists) -> 
                      sprintf "%s\n%s\n%s" output title (formatListsForDisplay lists)) String.Empty