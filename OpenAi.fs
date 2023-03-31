module OpenAi

open System
open System.IO
open System.Runtime.InteropServices
open OpenAiClient.Contracts
open OpenAiClient.RecordMapper
open OpenAiProvider

[<AutoOpen>]
type OpenAi() =
    static let mutable client = null
    
    static member Connect apiToken = 
        client <- OpenAiClientFactory.Create(apiToken)
    
    static member ListModels() =
        async {
            let! models = client.ListModels()
            return models
        } |> Async.StartAsTask
        
    static member RetrieveModel(modelId: string) =
        async {
            let! model = client.RetrieveModel(modelId)
            return model
        } |> Async.StartAsTask

    static member CreateCompletion(model, [<Optional>] prompt, [<Optional>] suffix, [<Optional>] maxTokens : Nullable<int>, [<Optional>] temperature : Nullable<float32>, [<Optional>] topP : Nullable<float32>, [<Optional>] n : Nullable<int>, [<Optional>] stream : Nullable<bool>, [<Optional>] logProbs : Nullable<int>, [<Optional>] echo : Nullable<bool>, [<Optional>] stop, [<Optional>] presencePenalty : Nullable<float32>, [<Optional>] frequencyPenalty : Nullable<float32>, [<Optional>] bestOf : Nullable<int>, [<Optional>] logitBias : string, [<Optional>] user) = 
        let r = OpenAiProvider.CreateCompletionRequest(Model = model)
        if prompt <> null && prompt <> "" then r.Prompt <- prompt
        if suffix <> null && suffix <> "" then r.Suffix <- suffix
        if maxTokens.HasValue then r.MaxTokens <- maxTokens.Value
        if temperature.HasValue then r.Temperature <- temperature.Value
        if topP.HasValue then r.TopP <- topP.Value
        if n.HasValue then r.N <- n.Value
        if stream.HasValue then r.Stream <- stream.Value
        if logProbs.HasValue then r.Logprobs <- logProbs.Value
        if echo.HasValue then r.Echo <- echo.Value
        if stop <> null && stop <> "" then r.Stop <- stop
        if presencePenalty.HasValue then r.PresencePenalty <- presencePenalty.Value
        if frequencyPenalty.HasValue then r.FrequencyPenalty <- frequencyPenalty.Value
        if bestOf.HasValue then r.BestOf <- bestOf.Value
        if logitBias <> null && logitBias <> "" then r.LogitBias <- logitBias
        if user <> null && user <> "" then r.User <- user
        async {
            let! result = client.CreateCompletion(r)
            return result
        } |> Async.StartAsTask

    static member CreateChatCompletion(model, messages : ChatCompletionMessage[], [<Optional>] temperature : Nullable<float32>, [<Optional>] topP : Nullable<float32>, [<Optional>] n : Nullable<int>, [<Optional>] stream  : Nullable<bool>, [<Optional>] stop : string[], [<Optional>] maxTokens : Nullable<int>, [<Optional>] presencePenalty : Nullable<float32>, [<Optional>] frequencyPenalty : Nullable<float32>) =
        let mappedMessages = messages |> Array.map(convertRecord)
        
        let r = OpenAiProvider.CreateChatCompletionRequest(
            Model = model,
            Messages = mappedMessages)
        
        if maxTokens.HasValue then r.MaxTokens <- maxTokens.Value
        if temperature.HasValue then r.Temperature <- temperature.Value
        if topP.HasValue then r.TopP <- topP.Value
        if n.HasValue then r.N <- n.Value
        if stream.HasValue then r.Stream <- stream.Value
        if stop <> null && stop.Length > 0 then r.Stop <- stop
        if presencePenalty.HasValue then r.PresencePenalty <- presencePenalty.Value
        if frequencyPenalty.HasValue then r.FrequencyPenalty <- frequencyPenalty.Value
        async {
            let! result = client.CreateChatCompletion(r)
            return result
        } |> Async.StartAsTask

    static member CreateEdit(model, instruction, [<Optional>] input, [<Optional>] n : Nullable<int>, [<Optional>] temperature : Nullable<float32>, [<Optional>] topP : Nullable<float32>) =
        let r = OpenAiProvider.CreateEditRequest(
            Model = model,
            Instruction = instruction)
        if input <> null && input <> "" then r.Input <- input
        if temperature.HasValue then r.Temperature <- temperature.Value
        if topP.HasValue then r.TopP <- topP.Value
        if n.HasValue then r.N <- n.Value
        async {
            let! result = client.CreateEdit(r)
            return result
        } |> Async.StartAsTask

    static member CreateImage(prompt, [<Optional>] n : Nullable<int>, [<Optional>] size, [<Optional>] responseFormat, [<Optional>] user) =
        let r = OpenAiProvider.CreateImageRequest(Prompt = prompt)
        if n.HasValue then r.N <- n.Value
        if size <> null && size <> "" then r.Size <- size
        if responseFormat <> null && responseFormat <> "" then r.ResponseFormat <- responseFormat
        if user <> null && user <> "" then r.User <- user
        async {
            return! client.CreateImage(r)
        } |> Async.StartAsTask

    static member CreateImageEdit(image, prompt, [<Optional>] mask : Stream, [<Optional>] n : Nullable<int>, [<Optional>] size, [<Optional>] responseFormat, [<Optional>] user) =
        let r = OpenAiProvider.CreateImageEditRequest(Image = image, Prompt = prompt)
        if mask <> null then r.Mask <- mask
        if n.HasValue then r.N <- n.Value
        if size <> null && size <> "" then r.Size <- size
        if responseFormat <> null && responseFormat <> "" then r.ResponseFormat <- responseFormat
        if user <> null && user <> "" then r.User <- user
        async {
            return! client.CreateImageEdit(r)
        } |> Async.StartAsTask

    static member CreateImageVariation(image, [<Optional>] n : Nullable<int>, [<Optional>] size, [<Optional>] responseFormat, [<Optional>] user) =
        let r = OpenAiProvider.CreateImageVariationRequest(Image = image)
        if n.HasValue then r.N <- n.Value
        if size <> null && size <> "" then r.Size <- size
        if responseFormat <> null && responseFormat <> "" then r.ResponseFormat <- responseFormat
        if user <> null && user <> "" then r.User <- user
        async {
            return! client.CreateImageVariation(r)
        } |> Async.StartAsTask

    static member CreateEmbedding(model, input : Object[], [<Optional>] user) =
        let r = OpenAiProvider.CreateEmbeddingRequest(Model = model, Input = input)
        if user <> null && user <> "" then r.User <- user
        async {
            return! client.CreateEmbedding(r)
        } |> Async.StartAsTask

    static member CreateTranscription(file, model, [<Optional>] prompt, [<Optional>] responseFormat, [<Optional>] temperature : Nullable<float32>, [<Optional>] language) =
        let r = OpenAiProvider.CreateTranscriptionRequest(
            File = file,
            Model = model)
        if prompt <> null && prompt <> "" then r.Prompt <- prompt
        if responseFormat <> null && responseFormat <> "" then r.ResponseFormat <- responseFormat
        if temperature.HasValue then r.Temperature <- temperature.Value
        if language <> null && language <> "" then r.Language <- language
        async {
            return! client.CreateTranscription(r)
        } |> Async.StartAsTask

    static member CreateTranslation(file, model, [<Optional>] prompt, [<Optional>] responseFormat, [<Optional>] temperature : Nullable<float32>) =
        let r = OpenAiProvider.CreateTranslationRequest(
            Model = model,
            File = file)
        if prompt <> null && prompt <> "" then r.Prompt <- prompt
        if responseFormat <> null && responseFormat <> "" then r.ResponseFormat <- responseFormat
        if temperature.HasValue then r.Temperature <- temperature.Value
        async {
            return! client.CreateTranslation(r)
        } |> Async.StartAsTask

    static member ListFiles() =
        async {
            return! client.ListFiles()
        } |> Async.StartAsTask

    static member CreateFile(file : string, purpose) =
        async {
            use fileStream = new FileStream(file, FileMode.Open, FileAccess.Read)
            let r = OpenAiProvider.CreateFileRequest(File = fileStream, Purpose = purpose)
            return! client.CreateFile(r)
        } |> Async.StartAsTask
    
    static member RetrieveFile(fileId) =
        async {
            let! file = client.RetrieveFile(fileId)
            return file
        } |> Async.StartAsTask
        
    static member DeleteFile(fileId) =
        async {
            let! result = client.DeleteFile(fileId)
            return result
        } |> Async.StartAsTask
        
    static member DownloadFile(fileId) =
        async {
            let! stream = client.DownloadFile(fileId)
            return stream
        } |> Async.StartAsTask
        
    static member CreateFineTune(trainingFile, [<Optional>] validationFile, [<Optional>] model, [<Optional>] nEpochs : Nullable<int>, [<Optional>] batchSize : Nullable<int>, [<Optional>] learningRateMultiplier : Nullable<float32>, [<Optional>] promptLossWeight : Nullable<float32>, [<Optional>] computeClassificationMetrics : Nullable<bool>, [<Optional>] classificationNClasses : Nullable<int>, [<Optional>] classificationPositiveClass, [<Optional>] classificationBetas, [<Optional>] suffix) =
        let r = OpenAiProvider.CreateFineTuneRequest(TrainingFile = trainingFile)
        if validationFile <> null && validationFile <> "" then r.ValidationFile <- validationFile
        if model |> Option.isSome then r.Model <- model.Value
        if nEpochs.HasValue then r.NEpochs <- nEpochs.Value
        if batchSize.HasValue then r.BatchSize <- batchSize.Value
        if learningRateMultiplier.HasValue then r.LearningRateMultiplier <- learningRateMultiplier.Value
        if promptLossWeight.HasValue then r.PromptLossWeight <- promptLossWeight.Value
        if computeClassificationMetrics.HasValue then r.ComputeClassificationMetrics <- computeClassificationMetrics.Value
        if classificationNClasses.HasValue then r.ClassificationNClasses <- classificationNClasses.Value
        if classificationPositiveClass <> null && classificationPositiveClass <> "" then r.ClassificationPositiveClass <- classificationPositiveClass
        if classificationBetas |> Option.isSome then r.ClassificationBetas <- classificationBetas.Value
        if suffix <> null && suffix <> "" then r.Suffix <- suffix
        async {
            let! result = client.CreateFineTune(r)
            return result
        } |> Async.StartAsTask
        
    static member ListFineTunes() =
        async {
            let! fineTunes = client.ListFineTunes()
            return fineTunes
        } |> Async.StartAsTask
        
    static member RetrieveFineTune(fineTuneId) =
        async {
            let! fineTune = client.RetrieveFineTune(fineTuneId)
            return fineTune
        } |> Async.StartAsTask
        
    static member CancelFineTune(fineTuneId) =
        async {
            let! result = client.CancelFineTune(fineTuneId)
            return result
        } |> Async.StartAsTask
        
    static member ListFineTuneEvents(fineTuneId, stream : Nullable<bool>) =
        async {
            let! events = client.ListFineTuneEvents(fineTuneId, stream)
            return events
        } |> Async.StartAsTask
        
    static member DeleteFineTuneModel(model) =
        async {
            let! result = client.DeleteModel(model)
            return result
        } |> Async.StartAsTask
        
    static member CreateModeration(input, [<Optional>]model) =
        async {
            let r = OpenAiProvider.CreateModerationRequest(Input = input)
            if model <> null && model <> "" then r.Model <- model
            let! result = client.CreateModeration(r)
            return result
        } |> Async.StartAsTask