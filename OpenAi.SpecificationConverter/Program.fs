module OpenAi.SpecificationConverter

open SpecificationConverter

[<EntryPoint>]
let main _ =
    ConvertYamlOpenApiSpecToJson()
    0