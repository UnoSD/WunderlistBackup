module Json

open System.Collections.Generic
open FSharp.Data
open LiteDB

let toBson (value) =
    let rec toBsonValue value =
        let toBsonDictionary (record : (string * JsonValue)[]) =
            record |>
            Array.map (fun (name, value) -> 
                           KeyValuePair((if name = "id" then "_id" else name), toBsonValue value)) |>
            (fun x -> new Dictionary<string, BsonValue>(x))

        let toBsonArray (array : JsonValue[]) =
            array |>
            Array.map (fun value -> toBsonValue value)

        let (|Integer|Decimal|) input =
            match System.Int64.TryParse (input.ToString()) with
            | true, value -> Integer value
            | false, _ -> Decimal input

        match value with
            | JsonValue.String  value -> BsonValue    (value)
            | JsonValue.Number  value -> match value with
                                         | Integer i -> BsonValue(i)
                                         | Decimal d -> BsonValue(d)
            | JsonValue.Float   value -> BsonValue    (value)
            | JsonValue.Boolean value -> BsonValue    (value)
            | JsonValue.Record  value -> BsonDocument (toBsonDictionary value) :> BsonValue
            | JsonValue.Array   value -> BsonArray    (toBsonArray      value) :> BsonValue
            | JsonValue.Null          -> BsonValue    ()

    toBsonValue value :?> BsonDocument