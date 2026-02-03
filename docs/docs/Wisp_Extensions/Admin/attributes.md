# Attributes

This is a list of attributes provided by the Admin extension.

## Class Attributes

### `[GenerateCrud]`

Tells the extension to generate CRUD pages for this type.

## Property Attributes

### `[IgnoreField]`

Tells the form builder to ignore this property. The form builder will also ignore
propeties with `[NotMapped]`. It will also ignore the `Id` property everywhere
except for the detail and edit pages, where it will be shown as readonly.

### `[DisabledField]`

The field for this property will be rendered as `disabled` and the extension will
never try to write to it.

### `[FieldLabel(string)]`

Allows you to explicitly set a label for the field. By default, the label is the
property name.

### `[FieldType(FieldType)]`

Allows you to override the field type, in case the extension determines it wrong.
See [field types](field-types) for more info.

### `[ValueSource(string)]`

The name of the `DbSet` property in your `DbContext` that contains the items for
this property.

See [foreign keys](foreign-keys) for more info.