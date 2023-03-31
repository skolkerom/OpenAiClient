namespace JsonSerializationHandler

open System.Net.Http
open System.Text
open System.Text.Json
open System.Text.Json.Nodes
open System.Threading
open System.Threading.Tasks


type JsonSerializationHandler(next: HttpMessageHandler) =
    inherit DelegatingHandler(next)

    let removeNullProperties (jsonElement: JsonElement) : JsonNode =
        let jsonObject = JsonObject()

        jsonElement.EnumerateObject()
        |> Seq.filter (fun property -> not <| property.Value.ValueKind.Equals(JsonValueKind.Null))
        |> Seq.iter (fun property -> jsonObject.Add(property.Name, JsonNode.Parse(property.Value.GetRawText())))

        jsonObject
    
    override _.SendAsync(request: HttpRequestMessage, cancellationToken: CancellationToken) : Task<HttpResponseMessage> =
        
        if request.Content <> null && request.Content.Headers.ContentType.MediaType = "application/json"
        then
            let jsonContent = request.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
            let jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonContent)
            let cleanedJsonElement = removeNullProperties jsonElement
            let cleanedJsonContent = JsonSerializer.Serialize(cleanedJsonElement)
            request.Content <- new StringContent(cleanedJsonContent, Encoding.UTF8, "application/json")

        base.SendAsync(request, cancellationToken) 
        

