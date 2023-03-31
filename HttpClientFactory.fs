module OpenAiClientFactory

open System
open System.Net.Http
open OpenAiProvider
open HeaderLogAuthHandler
open JsonSerializationHandler

let Create apiToken = 
    let acceptJsonHandler = new AcceptJsonHandler()
    acceptJsonHandler.InnerHandler <- new HttpClientHandler()
    let jsonSerializationHandler = new JsonSerializationHandler(acceptJsonHandler)
    let logHandler = new LogHandler(jsonSerializationHandler)
    let httpClient = new HttpClient(logHandler)
    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiToken}")
    httpClient.BaseAddress <- Uri("https://api.openai.com/v1/")
    
    let client = OpenAiProvider.Client()
    client.HttpClient <- httpClient
    
    client