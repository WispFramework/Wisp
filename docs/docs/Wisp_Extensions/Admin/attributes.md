# Attributes

This is a list of attributes provided by the Admin extension.

## Class Attributes

### `[GenerateCrud]`

Marks an entity type for automatic CRUD page generation.

## Property Attributes

### `[IgnoreField]`

Instructs the form generator to exclude this property from generated forms.

Properties marked with `[NotMapped]` are also ignored automatically.

The `Id` property is ignored in create forms, but shown as read-only on detail and edit pages.

### `[DisabledField]`

Renders the field as `disabled` in generated forms and prevents the extension
from attempting to write values to this property.

### `[FieldLabel(string)]`

Overrides the default field label. By default, the property name is used.

### `[FieldType(FieldType)]`

Overrides the automatically inferred field type for this property.
See [field types](field-types) for more info.

### `[ValueSource(string)]`

Specifies the name of the `DbSet` property in your `DbContext`
that provides selectable values for this property.

See [foreign keys](foreign-keys) for more info.