# OpenAiClient
OpenAI API provider for .NET Core


### Configuration
Add appsettings.json 

```json
{
    "OpenAi": {
        "ApiToken": "<API_TOKEN>"
    }
}
```

### C#

Read API token from appsettings.json
```cs
using Microsoft.Extensions.Configuration;
using static OpenAiClient.Contracts;
using static OpenAi.OpenAi;

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var apiToken = configuration["OpenAi:ApiToken"];
Connect(apiToken);
```

#### Usage
```cs
var models = await ListModels();

var model = await RetrieveModel("text-davinci-003");

var completion = await CreateCompletion(model: "text-davinci:001", prompt: "Say this is a test", maxTokens: 7, temperature: 0.0f, topP: 1.0f, n: 1, stream: false);

var chatCompletion = await CreateChatCompletion(model: "gpt-3.5-turbo", messages: new[] { new ChatCompletionMessage { Name = "test", Role = "user", Content = "Hello!" } });

var edit = await CreateEdit(model: "text-davinci-edit-001", instruction: "Fix the spelling mistakes", input: "What day of the wek is it?");

var image = await CreateImage(prompt: "A cute baby sea otter", n: 1, size: "1024x1024");

var embedding = await CreateEmbedding(model: "text-embedding-ada-002", input: new[]{"The food was delicious and the waiter..."}, user: "test");

var fineTuneFile = await CreateFile("training_data.jsonl", "fine-tune");

var fineTuneJob = await CreateFineTune(fineTuneFile.Id);

var retrievedFile = await RetrieveFile(fineTuneFile.Id);

var files = await ListFiles();

var fileContents = await DownloadFile(fineTuneFile.Id); //for free accounts it generates error "To help mitigate abuse, downloading of fine-tune training files is disabled for free accounts."

var fineTuneEvents = await ListFineTuneEvents(fineTuneJob.Id, false);

var moderation = await CreateModeration("I want to kill them.");

```

### F#

Read API token from appsettings.json

```fs
open System
open Microsoft.Extensions.Configuration
open OpenAiClient.Contracts
open OpenAi

let configuration = ConfigurationBuilder().AddJsonFile("appsettings.json").Build()
let apiToken = configuration.["OpenAi:ApiToken"]
Connect(apiToken)
```

#### Usage
```fs
let models = ListModels() |> Async.AwaitTask |> Async.RunSynchronously

let model = RetrieveModel("text-davinci-003") |> Async.AwaitTask |> Async.RunSynchronously

let completion = CreateCompletion(model = "text-davinci:001", prompt = "Say this is a test", maxTokens = 7, temperature = 0.0f, topP = 1.0f, n = 1, stream = false) |> Async.AwaitTask |> Async.RunSynchronously

let chatCompletion = CreateChatCompletion(model = "gpt-3.5-turbo", messages = [| ChatCompletionMessage(Name = "test", Role = "user", Content = "Hello!") |]) |> Async.AwaitTask |> Async.RunSynchronously

let edit = CreateEdit(model = "text-davinci-edit-001", instruction = "Fix the spelling mistakes", input = "What day of the wek is it?") |> Async.AwaitTask |> Async.RunSynchronously

let image = CreateImage(prompt = "A cute baby sea otter", n = 1, size = "1024x1024") |> Async.AwaitTask |> Async.RunSynchronously

let embedding = CreateEmbedding(model = "text-embedding-ada-002", input = [| "The food was delicious and the waiter..." |], user = "test") |> Async.AwaitTask |> Async.RunSynchronously

let fineTuneFile = CreateFile("training_data.jsonl", "fine-tune") |> Async.AwaitTask |> Async.RunSynchronously

let fineTuneJob = CreateFineTune(fineTuneFile.Id) |> Async.AwaitTask |> Async.RunSynchronously

let retrievedFile = RetrieveFile(fineTuneFile.Id) |> Async.AwaitTask |> Async.RunSynchronously

let files = ListFiles() |> Async.AwaitTask |> Async.RunSynchronously

let fileContents = DownloadFile(fineTuneFile.Id) //for free accounts it generates error "To help mitigate abuse, downloading of fine-tune training files is disabled for free accounts."

let fineTuneEvents = ListFineTuneEvents(fineTuneJob.Id, false) |> Async.AwaitTask |> Async.RunSynchronously

let moderation = CreateModeration("I want to kill them.") |> Async.AwaitTask |> Async.RunSynchronously
```
