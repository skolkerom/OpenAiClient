module OpenAiClient.Contracts

type ChatCompletionMessage() =
    member val Name : string = null with get, set
    member val Role : string = null with get, set
    member val Content : string = null with get, set