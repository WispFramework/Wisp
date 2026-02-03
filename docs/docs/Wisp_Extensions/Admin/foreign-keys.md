# Foreign Keys

The extension supports generating selects, radios and checkboxes from foreign
keys.

## X-To-One

For X-To-One relationships, the extension will try to generate a `#!html <select>`, listing
all available options from the database.

The property must have a `[ValueSource(string)]` on it for this to work correctly.

## X-To-Many

For X-To-Many relationsips, the extension will try to generate a `#!html <select multiple>`,
listing all available options from the database.

An x-to-many property must be a `List<>` and as with x-to-one, must have a `[ValueSource(string)]`
attribute.

## Enums

The extension will also generate a `#!html <select>` for enums, where the value
is the numerical value of the enum item and the displayed name is the item name.

Enums do not need the `[ValueSource(string)]` attribute.

## The `[ValueSource(string)]` Attribute

This attribute tells the extension where to get data for the foreign key. The
argument is the name of a `DbSet<>` property in your `DbContext`.

The attribute is technically not required, as the extension will attempt to
infer the property name by pluralizing the name of the type, but for reliability,
setting it manually is strongly required.

```csharp
public class AppDbContext : DbContext
{
    public DbSet<Category> Categories;
}

[GenerateCrud]
public class DemoEntity : CrudEntity
{
    [ValueSource(nameof(AppDbContext.Categories))]
    public Category Category { get; set; }
}
```