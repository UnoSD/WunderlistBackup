module Startup

open WunderlistBackup
open ParseArguments

type Action =
    | ViewAction
    | BackupAction

type Result =
    | Success
    | Failure

module UserPrompt =
    type Instruction<'a> =
        | GetAction      of (Action -> 'a)
        | GetCredentials of (string[] * (Credentials -> 'a))

    type Program<'a> =
        | Free of Instruction<Program<'a>>
        | Pure of 'a

    let getAction () =
        Free (GetAction Pure)

    let getCredentials args =
        Free (GetCredentials (args, Pure))

    let private mapI f = function
        | GetAction      (next)       -> GetAction      (next >> f)
        | GetCredentials (args, next) -> GetCredentials (args, next >> f)

    let rec bind f = function
        | Free instruction -> instruction |> mapI (bind f) |> Free
        | Pure x -> f x

    type UserPromptBuilder() =
        member __.Bind(x, f) = bind f x
        member __.ReturnFrom x = x
        member __.Zero() = Pure ()
        member __.Return(x) = Pure x

[<AutoOpen>]
module UserPromptComputationExpression =
    open UserPrompt

    let userPrompt = new UserPromptBuilder()

[<EntryPoint>]
let main args =
    let rec interpret =
        function 
        | UserPrompt.Program.Pure x -> x
        | UserPrompt.Program.Free (UserPrompt.Instruction.GetAction next) ->
            let choice =
                CommandLine.pick [ BackupAction; ViewAction ]
                         (fun ba -> match ba with 
                                    | BackupAction -> "Start new backup"
                                    | ViewAction -> "View backups")

            choice |> 
            Option.defaultValue ViewAction |>
            next |>
            interpret
        | UserPrompt.Program.Free (UserPrompt.Instruction.GetCredentials (args, next)) ->
            args |> 
            parseCredentials |>
            Option.defaultValue
                {
                    endpoint = System.Uri("")
                    clientId = (HexString.HexString.createOption "").Value
                    token = (HexString.HexString.createOption "").Value
                } |>
            next |>
            interpret

    let view =
        View.pickDisplay() |> ignore
        Success

    let backup credentials =
        CommandLineActions.asyncSaveToDatabase credentials |>
        Async.RunSynchronously
        Success

    let userRequest =
        userPrompt {
            let! action = UserPrompt.getAction()

            return!
                match action with
                | ViewAction -> userPrompt { return view }
                | BackupAction -> 
                    userPrompt {
                                   let! credentials = UserPrompt.getCredentials args
                                   
                                   return backup credentials
                               }
        }

    match interpret userRequest with
    | Success -> 0
    | Failure -> 1