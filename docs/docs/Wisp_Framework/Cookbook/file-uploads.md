---
icon: lucide/upload
---

# File Uploads

!!! warning
    Your HTML form needs to include `enctype="multipart/form-data"` for file uploads to work correctly.

    ```html
    <form action="/upload" method="post" enctype="multipart/form-data">...</form>
    ```

Handling file uploads with Wisp is straightforward. Just make sure your form is set up properly and access one
or more uploaded files through an `IHttpContextAccessor`.

Just like with any forms, you can still use argument injection to get values from other form fields.

Form fields can still be injected as method parameters, and other services can be resolved via
dependency injection.

Files are fully buffered in memory before being exposed as `byte[]`. Streming is not yet available.

```csharp
public ViewResult PostUpload(string title, IHttpContextAccessor contextAccessor) 
{
    var request = contextAccessor.HttpContext?.Request;
    if(request is null) return Redirect("/error");
    
    if(request.Files.Count == 0)
        return Redirect("/error");

    var file = request.Files[0];
    
    // Make sure you sanitize the paths safely. This example will prevent path traversal attacks
    // where the client sends a path like `../../appsettings.json`.
    var safeFileName = Path.GetFileName(file.Filename);
    var uploadPath = Path.Combine("uploads", safeFileName);

    File.WriteAllBytes(uploadPath, file.Data);
    
    return Redirect("/");
}
```

!!! warning
    **Uploaded file names and file contents are untrusted input.**
    
    You must:

    - Sanitize file names (e.g. use `Path.GetFileName`)
    - Validate file size
    - Validate content type or file signature
    - Prevent overwriting sensitive files
    - Avoid storing executable files in publicly accessible directories