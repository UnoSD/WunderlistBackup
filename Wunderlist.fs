namespace WunderlistBackup

module Wunderlist =
    open Requests
    open System

    let getAllData credentials =
        let asyncGet =
            (credentials, Get) ||>
            asyncRequest

        let asyncGetParse segment parser query =
            async {
                let! json = asyncGet segment query

                return parser(json) |> Array.toList
            }

        let getAllByLists segment parse (lists : WList list) =
            async {
                let! results = 
                    lists |>
                    List.map (fun list -> list.Id.ToString()) |>
                    List.map (fun listId -> asyncGetParse segment parse [ "list_id", listId ]) |>
                    Async.Parallel

                return List.collect (fun z -> z) (results |> Array.toList)
            }

        async {
            let! folders  = asyncGetParse Folders  FoldersProvider.Parse  []
            let! lists    = asyncGetParse Lists    ListsProvider.Parse    []
            let! tasks    = getAllByLists Tasks    TasksProvider.Parse    lists
            let! subtasks = getAllByLists Subtasks SubtasksProvider.Parse lists
            let! notes    = getAllByLists Notes    NotesProvider.Parse    lists
            let! files    = getAllByLists Files    FilesProvider.Parse    lists

            return {
                id        = Guid.NewGuid().ToString()
                timestamp = DateTimeOffset.Now
                folders   = folders
                lists     = lists
                tasks     = tasks
                subtasks  = subtasks
                notes     = notes
                files     = files
            }
        }