namespace WunderlistBackup

module Download =
    open System
    open FSharp.Data

    let asyncDownload url outputStream =
        async {
            let! response = Http.AsyncRequestStream(url)

            use stream = response.ResponseStream

            do! stream.CopyToAsync(outputStream) |> Async.AwaitTask
        }

    let asyncDownloadFile (file : File) =
        async {
            use outputStrem = IO.File.Open(file.FileName.Replace(':', '_'), IO.FileMode.CreateNew)
    
            do! asyncDownload file.Url outputStrem
        }