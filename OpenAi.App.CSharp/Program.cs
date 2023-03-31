using Microsoft.Extensions.Configuration;
using static OpenAiClient.Contracts;
using static OpenAi.OpenAi;

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var apiToken = configuration["OpenAi:ApiToken"];
        
Connect(apiToken);
        
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

Console.ReadLine();


