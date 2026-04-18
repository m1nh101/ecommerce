# Table: `colors`

Global color catalog shared across all products and variants.

## Columns

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| `id` | `uuid` | NO | — | Primary key |
| `name` | `varchar(50)` | NO | — | Display name (e.g., "Midnight Black") |
| `hex_code` | `char(7)` | NO | — | CSS hex color (e.g., "#1A1A1A") |

## Constraints

| Type | Columns | Notes |
|------|---------|-------|
| PRIMARY KEY | `id` | |
| UNIQUE | `name` | No duplicate color names |
| CHECK | `hex_code` | Must match `#[0-9A-Fa-f]{6}` |

## Relationships

- Referenced by `product_variants.color_id`

## Sample Data

| name | hex_code |
|------|----------|
| White | #FFFFFF |
| Black | #000000 |
| Navy Blue | #001F5B |
| Charcoal Gray | #36454F |
| Olive Green | #6B7C52 |
| Burgundy | #800020 |
| Sky Blue | #87CEEB |
| Beige | #F5F0E8 |
| Red | #CC0000 |
| Camel | #C19A6B |
