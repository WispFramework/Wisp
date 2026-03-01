# About

!!! warning
    This documentation section is about an upcoming Wisp feature that is not publicly available yet.

`Wisp.Extensions.Identity` adds a DB-based identity provider and user management.

## Features

The extension comes with everything you will need to add username and password based end-user
authentication to your application.

 - Default `User` and `UserRole` objects you can extend or replace
 - Database-agnostic, EF Core-based user storage backend and session store[^1]
 - TOTP MFA support
 - Login, Signup and Password Reset forms
 - User Profile and Settings pages

[^1]: [As long as your Database is supported by EF Core](https://learn.microsoft.com/en-us/ef/core/providers/)