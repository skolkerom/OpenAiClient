module OpenAiClient.RecordMapper

open System.Reflection

let convertRecord (source: 'a) : 'b =
    let sourceType = typeof<'a>
    let targetType = typeof<'b>
    let targetTypeConstructor = targetType.GetConstructor(Array.empty)

    let properties =
        sourceType.GetProperties(BindingFlags.Public ||| BindingFlags.Instance)
        |> Array.filter (fun p -> p.GetGetMethod().IsPublic && p.GetSetMethod().IsPublic)

    let target = targetTypeConstructor.Invoke(Array.empty) :?> 'b

    for p in properties do
        let value = p.GetValue(source)
        targetType.GetProperty(p.Name).SetValue(target, value)

    target