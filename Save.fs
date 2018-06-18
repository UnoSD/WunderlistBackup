module Save

open LiteDB
open FSharp.Data
open WunderlistBackup
open Serialization

let saveToDb backup =
    async {
        use db = new LiteDatabase("wunderlist.db")
            
        backup |>
        toBsonDocument |>
        db.GetCollection("BackUp").Insert |>
        ignore
            
        let! fileStreams = 
            backup.files |>
            List.map (fun file -> async {
                                        let! response = Http.AsyncRequestStream(file.Url)
                                        return (file, response.ResponseStream)
                                    }) |>
            Async.Parallel
                
        fileStreams |>
        Array.iter (fun (file, stream) -> db.FileStorage
                                            .Upload(file.Id.ToString(), file.FileName, stream) |>
                                          ignore)
    }