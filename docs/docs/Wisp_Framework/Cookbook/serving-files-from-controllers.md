---
icon: lucide/download
---

# Serving Files from Controllers

Sometimes, you might want to exercise more control over file downloads than you get from the static files
middleware.

You can serve files directly from controllers.

Simply make your controller return `void` or `Task` and write to the response directly using
`IHttpContextAccessor`.

!!! warning
    The code in this example is unsafe as it allows users to access arbitrary data from the host.
    In a real life usecase, always make sure to sanitize paths and check appropriate permissions.

```csharp
// We use a greedy path variable here (`:*`) to treat everything after the 
// last slash as part of the file name.
[Route("/{fileName:*}")]
public async Task ServeFile(string fileName, IHttpContextAccessor contextAccessor)
{
    var response = accessor.HttpContext?.Response 
        ?? throw new ArgumentNullException(nameof(accessor));
        
    var file = await File.ReadAllTextAsync(fileName);
    if (file is null)
    {
        response.StatusCode = 404;
        response.ContentType = "text/plain";
        response.Body = new MemoryStream("".AsUtf8Bytes());
        return;
    }

    response.StatusCode = 200;
    response.ContentType = "image/png";
    response.Body = new MemoryStream(file);
}
```

!!! info
    **Pro Tip:** If you're serving files from a URL that doesn't look like a file (for example, serving `document.pdf` from
    `/files/generate-doc`), set the `Content-Disposition` header to hint the file name and extension to the browser.
    
    ```csharp
    response.Headers.Add("Content-Disposition", "attachment; filename=\"document.pdf\"");
    ```

    For more information about the `Content-Disposition` header, see 
    [the MDN doc](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Content-Disposition).