---
icon: lucide/download
---

# Serving Files from Controllers

Sometimes you may want more control over file downloads than what the static files middleware provides.

You can serve files directly from controllers.

Simply make your controller return `void` or `Task` and write directly to the `HttpResponse`
using `IHttpContextAccessor`.

!!! warning
    The code in this example is intentionally unsafe and allows arbitrary file access.

    In real applications you must:

    - Restrict file access to a specific directory
    - Sanitize file names (`Path.GetFileName`)
    - Prevent directory traversal (`..`)
    - Validate user permissions

```csharp
// We use a greedy path variable here (`:*`) to treat everything after the 
// last slash as part of the file name.
[Route("/{fileName:*}")]
public async Task ServeFile(string fileName, IHttpContextAccessor contextAccessor)
{
    var context = contextAccessor.HttpContext 
        ?? throw new InvalidOperationException("No HttpContext available.");

    var response = context.Response;

    if (!File.Exists(fileName))
    {
        response.StatusCode = 404;
        response.ContentType = "text/plain";
        await response.Body.WriteAsync("Not Found".AsUtf8Bytes());
        return;
    }

    response.StatusCode = 200;
    response.ContentType = "application/octet-stream";

    await using var stream = File.OpenRead(fileName);
    await stream.CopyToAsync(response.Body);
}
```

In real applications, you should determine the correct Content-Type based on the file extension
instead of always using `application/octet-stream`.

!!! info
    **Pro Tip:** If you're serving files from a URL that doesn't look like a file (for example, serving `document.pdf` from
    `/files/generate-doc`), set the `Content-Disposition` header to hint the file name and extension to the browser.
    
    ```csharp
    response.Headers.Add("Content-Disposition", "attachment; filename=\"document.pdf\"");
    ```

    For more information about the `Content-Disposition` header, see 
    [the MDN doc](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Content-Disposition).