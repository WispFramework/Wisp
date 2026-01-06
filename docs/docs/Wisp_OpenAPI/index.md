---
title: "About Wisp OpenAPI"
---
# Wisp.Extensions.OpenAPI

!!! warning
    This extension is in an early alpha version and some of the features mentioned in this documentation
    may not yet be fully implemented.

`Wisp.Extensions.OpenAPI` is a Wisp extensions that adds an OpenAPI spec generator and Swagger UI
to your Wisp application.

It will automatically generate an OpenAPI Specification for all routes in all controllers marked with `[ApiController]`.

## Request and Response Type Inference

For correctly typed controller actions, the generator will try to infer types for request and response data
and include it in the schema.

### Supported Sources for Request Information

**Path Variables**

The generator will find path variables and try to extract type information, if defined.

Examples:

```csharp
[Route("/hello-world/{name}")]      --> Variable `name` of type `string`
[Route("/hello-world/{id:guid}")]   --> Variable `id` of type `Guid`
```

**Query Params**

The generator will find query parameters based on the `[FromQuery]` attribtue and extract type information.

Examples:

```csharp
public async Task<ResultBox> GetHelloWorld([FromQuery] string name) 
    --> Query param `name` of type `string`

public async Task<ResultBox> GetHelloWOrld([FromQuery] Guid id)
    --> Query param `id` of type `Guid`
```

**Form Fields**

The generator will find form fields based on the `[FromForm]` attribute and extract type information.

Examples:

```csharp
public async Task<ResultBox> PostHelloWorld([FromForm] string name)
    -> Form field `name` of type `string`
```

**Headers**

The generator will find required headers and try to infer types based on the `[Header]` attribute.

Examples:

```csharp
public async Task<ResultBox> PostHelloWorld([Header] string name)
    --> Header `name` of type `string`
```

**Cookies:**

The generator will find required cookies and try to infer types based on the `[Cookie]` attribute.

```csharp
public async Task<ResultBox> PostHelloWorld([Cookie] string name)
    --> Cookie `name` of type `string`
```

**JSON Body**

The generator will find try to infer a type based on the `[FromBody]` attribute, generate a JSON schema for the
class and include it in the API spec.

Example:

```csharp
public record HelloWorldRequest(string Name);

// ...

public async Task<ResultBox> HelloWorld([FromBody] HelloWorldRequest request)
    --> JSON Body:
        {
            "Name": "string"
        }
```

### Supported Sources for Response Information

If the controller action has a concrete return type, the generator will create a JSON schema for the type and
include it in the API spec.

Example:

```csharp
public record HelloWorldResponse(string Name);

public async Task<IResultBox<HelloWorldResponse>> HelloWorld()
    --> JSON Response:
        {
            "Name": "string"
        }
```