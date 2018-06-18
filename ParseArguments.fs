module ParseArguments

open MaybeBuilder
open HexString
open WunderlistBackup
open System

let private createCredentials clientId token = 
    {
        endpoint = Uri "https://a.wunderlist.com/api/v1"
        clientId = clientId
        token    = token
    }

let parseCredentials (args : string[]) =
    maybe {
        let! clientId, token =
            match args with
            | [| clientId; token |] -> Some (clientId, token)
            | _                     -> None
        
        let! clientIdHex = HexString.createOption clientId
        let! tokenHex = HexString.createOption token

        return createCredentials clientIdHex tokenHex
    }