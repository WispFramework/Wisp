# Field Types

## Type Inference

The extension will try to infer the field type for every property. You can also
override the type manually by setting `[FieldType(FieldType)]` on the property.

### Enums

Enums will be rendered as a `#!html <select>` with the item name as the displayed name
and the numerical enum value as the value.

### Reference Types

Reference type properties need `[ValueSource(string)]` to work properly. See [foreign keys](foreign-keys).

Reference types will be rendered as a `#!html <select>` with the `Id` as the value and
`ToString()` as the displayed name.

### Lists

List properties need `[ValueSource(string)]` to work properly. See [foreign keys](foreign-keys).

Lists will be rendered as a `#!html <select multiple>` with the `Id` as the value and
`ToString()` as the displayed name.

## Supported Types

### Text

Automatic for types: `string`

Rendered as `#!html <input type="text">`

### Password

Must be specified manually

Rendered as `#!html <input type="password">`

### Number

Automatic for types: `int`, `float`, `double`, `long`, `short`, `decimal`

Rendered as `#!html <input type="number">`

### Date

Automatic for types: `DateTime`, `DateTimeOffset`

Rendered as `#!html <input type="date">`

### Select

Automatic for reference types, see above.

### MultiSelect

Automatic for `List<>`, see above.

### TextArea

Must be specified manually.

Rendered as `#!html <textarea>`

### Checkbox

Must be specified manually, works the same way as [MultiSelect](#multiselect).

### Radio

Must be specified manually, works the same way as [Select](#select)

### Color

Must be specified manually.

Rendered as `#!html <select type="color">`