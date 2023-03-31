module OpenAiProvider

open SwaggerProvider

type OpenAiProvider = OpenApiClientProvider<"openai.json", PreferAsync=true, PreferNullable=true>