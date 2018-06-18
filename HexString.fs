module HexString

open System

type HexString =
    private HexString of string
    
type HexString with
    member hs.value =
        (function HexString(value) -> value)(hs)
    
    static member createOption value = 
        match value with
        | x when String.IsNullOrEmpty(x)                                -> None
        | x when Seq.filter (fun (c : char) -> c < 'a' || c > 'f') x |>
                 Seq.isEmpty                                            -> None
        | x                                                             -> Some (HexString x)