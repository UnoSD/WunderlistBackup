namespace WunderlistBackup

module Serialization =
    open LiteDB
    open System.Collections.Generic
    open System
    open FSharp.Data
    open FSharp.Data.Runtime.BaseTypes
    open Newtonsoft.Json.Linq

    let bsonArray (documents : #IJsonDocument list) = 
        documents |>
        List.map (fun x -> Json.toBson x.JsonValue :> BsonValue) |>
        BsonArray

    let toBsonDocument (backup : BackUp) =
        dict[
                "_id",       BsonValue(backup.id)
                "files",     bsonArray backup.files    :> BsonValue
                "folders",   bsonArray backup.folders  :> BsonValue
                "lists",     bsonArray backup.lists    :> BsonValue
                "notes",     bsonArray backup.notes    :> BsonValue
                "subtasks",  bsonArray backup.subtasks :> BsonValue
                "tasks",     bsonArray backup.tasks    :> BsonValue
                "timestamp", BsonValue(backup.timestamp.ToString("o"))
            ]                                           |>
        (fun x -> new Dictionary<string, BsonValue>(x)) |>
        BsonDocument

    // Ugly imperative code because it's easier with Json.NET
    let toBackUp (bd : BsonDocument) =
        let parseBson (bsonValue : BsonValue) =
            let jObject = 
                bsonValue.ToString() |>
                JObject.Parse

            let replaceInArray (jo : JObject) (a : JArray) =
                a.Remove(jo) |> ignore

                let value = 
                    jo.["$numberLong"]
                      .Value<decimal>()

                a.Add(value)

            let replaceInProp (jo : JObject) (p : JProperty) =
                let value = 
                    jo.["$numberLong"]
                      .Value<decimal>()
                
                p.Value <- JValue(value)

            let toReplace = new List<(JObject * JArray)>()

            jObject.Descendants() |>
            Seq.choose (fun p -> match box p with  
                                 | :? JObject as x when x.["$numberLong"] <> null -> Some(x)
                                 | _ -> None) |>
            Seq.iter   (fun jo -> match box jo.Parent with
                                  | :? JArray as a -> toReplace.Add (jo, a)
                                  | :? JProperty as p -> replaceInProp jo p
                                  | _ -> raise(Exception()))

            toReplace |> Seq.iter (fun i -> i ||> replaceInArray)

            jObject.["id"] <- jObject.["_id"]

            jObject.ToString()

        let toObject nodeName parse =
            bd.[nodeName].AsArray |>
            Seq.map (fun i -> parse(JsonValue.Parse(parseBson i))) |>
            Seq.toList

        {
            id =        bd.["_id"].AsString
            folders =   toObject "folders"  Folder
            files =     toObject "files"    File
            lists =     toObject "lists"    WList
            notes =     toObject "notes"    Note
            subtasks =  toObject "subtasks" Subtask
            tasks =     toObject "tasks"    Task
            timestamp = DateTimeOffset.Parse   (bd.["timestamp"].ToString()
                                                                .Trim('"')
                                                                .Trim('\\'))
        }