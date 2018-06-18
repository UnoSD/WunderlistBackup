namespace WunderlistBackup

open System
open HexString

type ClientId =
    HexString

type Token =
    HexString

type Credentials =
    {
        endpoint : Uri
        clientId : ClientId
        token : Token
    }