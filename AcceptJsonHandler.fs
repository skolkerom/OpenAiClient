namespace HeaderLogAuthHandler

open System.Net.Http
open System.Net.Http.Headers

type AcceptJsonHandler() =
    inherit DelegatingHandler()

    override this.SendAsync(request, cancellationToken) =
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue("application/json"))
        let response = base.SendAsync(request, cancellationToken)
        response
