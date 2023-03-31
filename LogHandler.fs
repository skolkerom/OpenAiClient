namespace HeaderLogAuthHandler

open System.Net.Http

type LogHandler(next: HttpMessageHandler) =
    inherit DelegatingHandler(next)

    override this.SendAsync(request, cancellationToken) =
        printfn $"RequestUri: {request.RequestUri}"
        
        match request.Content with
        | :? StringContent as strContent ->
            let result = strContent.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
            printfn $"Content: {result}"
        | _ -> printfn $"Content: {request.Content}"
                
        let response = base.SendAsync(request, cancellationToken)
        
        let result = response |> Async.AwaitTask |> Async.RunSynchronously
        
        let message = result.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
        
        printfn $"Response message: {message}"
        
        response
