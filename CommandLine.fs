namespace WunderlistBackup

module CommandLine =

    open System
    open System.Linq

    let integers upTo =
        Enumerable.Range(1, upTo)

    let numberedCollection (collection : 'a list) =
        collection |>
        Seq.zip (integers collection.Length) |>
        Seq.toList

    let pick collection display =
        let numbered = 
            numberedCollection collection
            
        numbered |> 
        List.map (fun (number, backup) -> sprintf "[%i] %A" number (display backup)) |>
        List.fold (fun output line -> sprintf "%s\n%s" output line) String.Empty |>
        Console.WriteLine

        match Console.ReadKey() with
            | key when key.Key > ConsoleKey.D0 && key.Key <= ConsoleKey.D9 ->
                numbered |>
                List.find (fun (n, _) -> n = (int key.Key) - (int ConsoleKey.D0)) |>
                snd |>
                Some
            | _ -> None