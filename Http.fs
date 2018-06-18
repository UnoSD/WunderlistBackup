namespace WunderlistBackup

open System

type HttpMethod = Get | Post

type HttpMethod with
    member method.String = 
        match method with
            | Get -> "GET"
            | Post -> "POST"

type Segment = Folders | Lists | Tasks | Subtasks | Notes | Files

module Http =

    type Uri with
        member uri.WithSegment (segment : string) =
            sprintf "%s/%s" (uri.ToString()) segment
    
    type Segment with
        member segment.String =
            match segment with
                | Folders  -> "folders"
                | Lists    -> "lists"
                | Tasks    -> "tasks"
                | Subtasks -> "subtasks"
                | Notes    -> "notes"
                | Files    -> "files"