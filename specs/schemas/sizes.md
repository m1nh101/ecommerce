# Table: `sizes`

Global size catalog. Sizes are grouped by `size_type` because clothing and shoe sizing systems differ.

## Columns

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| `id` | `uuid` | NO | — | Primary key |
| `name` | `varchar(20)` | NO | — | Display label (e.g., "M", "42", "One Size") |
| `size_type` | `varchar(20)` | NO | — | Discriminator: `clothing` \| `shoe` \| `accessory` |
| `sort_order` | `int` | NO | — | Display order within the same `size_type` |

## Constraints

| Type | Columns | Notes |
|------|---------|-------|
| PRIMARY KEY | `id` | |
| UNIQUE | `(name, size_type)` | Same label can exist in different types |

## Indexes

| Name | Columns | Purpose |
|------|---------|---------|
| `idx_sizes_type_order` | `(size_type, sort_order)` | Ordered listing per type |

## Relationships

- Referenced by `product_variants.size_id`

## Sample Data

### Clothing Sizes (`size_type = 'clothing'`)

| name | sort_order |
|------|-----------|
| XS | 1 |
| S | 2 |
| M | 3 |
| L | 4 |
| XL | 5 |
| XXL | 6 |
| XXXL | 7 |

### Shoe Sizes — EU (`size_type = 'shoe'`)

| name | sort_order |
|------|-----------|
| 35 | 1 |
| 36 | 2 |
| 37 | 3 |
| 38 | 4 |
| 39 | 5 |
| 40 | 6 |
| 41 | 7 |
| 42 | 8 |
| 43 | 9 |
| 44 | 10 |
| 45 | 11 |

### Accessory Sizes (`size_type = 'accessory'`)

| name | sort_order |
|------|-----------|
| One Size | 1 |
| S/M | 2 |
| L/XL | 3 |
