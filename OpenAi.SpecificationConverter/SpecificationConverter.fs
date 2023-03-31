module SpecificationConverter

open System
open System.Linq
open System.Globalization
open System.IO
open System.Net.Http
open System.Text.RegularExpressions
open System.Threading
open Microsoft.OpenApi.Readers
open Microsoft.OpenApi.Writers

let ConvertYamlOpenApiSpecToJson() =
    Thread.CurrentThread.CurrentCulture <- CultureInfo("en-US")
    let url = "https://raw.githubusercontent.com/openai/openai-openapi/master/openapi.yaml"

    let yamlOaiMetaString = async{
        use client = new HttpClient()
        let! response = client.GetAsync(url) |> Async.AwaitTask
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        return content
    } 
    
    let yamlEmptyLines = (yamlOaiMetaString |> Async.RunSynchronously)

    let yamlNoEmptyLines = Regex.Replace(yamlEmptyLines, "^[ \t]*\r?$\n?", String.Empty, RegexOptions.Multiline);
    
    let yaml = Regex.Replace(yamlNoEmptyLines, "x-oaiMeta:.*(?:\r?\n(?!\s*response:\s*\|).*)*\r?\n\s*response:\s\|\n.*\{(?:[^{}]*|\{(?:[^{}]*|\{(?:[^{}]*|\{[^{}]*\})*\})*\})*\}", "", RegexOptions.Multiline)
    
    use writer = new StreamWriter("openai.yaml")
    writer.Write(yaml)
    writer.Flush()
    
    let openApiReaderSettings = OpenApiReaderSettings()
    
    let (openApiDocument, diagnostics) = OpenApiStringReader(openApiReaderSettings).Read(yaml)
    
    if(diagnostics.Errors.Any())
    then
        printfn "There were errors while parsing yaml file"
    else
        use outputStream = new FileStream("openai.json", FileMode.Create)
        use writer = new StreamWriter(outputStream)
        let apiWriter = OpenApiJsonWriter(writer)
        openApiDocument.SerializeAsV3(apiWriter)