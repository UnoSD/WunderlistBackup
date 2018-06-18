namespace WunderlistBackup

module Requests =
    open FSharp.Data
    open WunderlistBackup.Http

    let asyncRequest credentials (method : HttpMethod) (segment : Segment) query =
        Http.AsyncRequestString
            (
                url = credentials.endpoint.WithSegment segment.String,
                query = query,
                headers = [ "X-Client-ID", credentials.clientId.value
                            "X-Access-Token", credentials.token.value ],
                httpMethod = method.String
            )
    
    //open Newtonsoft.Json.Linq
    //let asyncRequest _ _ (segment : Segment) query =
    //    async {
    //        return
    //            match (segment, query) with
    //            | (Folders, _) ->  System.IO.File.ReadAllText "FoldersJsonSample.json"
    //            | (Lists, _) ->    System.IO.File.ReadAllText "ListsJsonSample.json"
    //            //| (Tasks, [ ("list_id", id) ]) -> (JArray.Parse(System.IO.File.ReadAllText "TasksJsonSample.json") |>
    //            //                                   Seq.filter (fun jt -> jt.["list_id"].ToString() = id) |>
    //            //                                   Seq.toArray |>
    //            //                                   JArray).ToString()
    //            | (Tasks, _) ->    System.IO.File.ReadAllText "TasksJsonSample.json"
    //            | (Subtasks, _) -> System.IO.File.ReadAllText "SubtasksJsonSample.json"
    //            | (Notes, _) ->    System.IO.File.ReadAllText "NotesJsonSample.json"
    //            | (Files, _) ->    System.IO.File.ReadAllText "FilesJsonSample.json"
    //    }