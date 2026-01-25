---
icon: lucide/upload
---

# File Uploads

!!! warning
    Your HTML form needs to include `enctype="multipart/form-data"` for file uploads to work correctly.

    ```html
    <form action="/upload" method="post" enctype="multipart/form-data">...</form>
    ```

Handling file uploads with Wisp is super simple. Just make sure your form is setup properly and access one
or more uploaded files through an `IHttpContextAccessor`.

Just like with any forms, you can still use argument injection to get values from other form fields.

And of course, DI works in controller method arguments too so you can inject the accessor per-method.

```csharp
public ViewResult PostUpload(string title, IHttpContextAccessor contextAccessor) 
{
    var request = contextAccessor.HttpContext?.Request;
    if(request is null) return Redirect("/error");
    
    var file = request.Files[0];
    
    File.WriteAllBytes(Path.Combine("/uploads", file.Filename), file.Data);
    
    return Redirect("/");
}
```

!!! warning
    Wisp gives you access to individual uploaded file data as `byte[]` and file names (including extension). Dealing
    with validation, checking if the data format is what you're expecting, etc. is fully up to you.