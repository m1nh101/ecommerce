# Table: `products`

Product master record. Holds identity, classification, and base pricing. Actual stock and per-variant pricing live in `product_variants`.

## Columns

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| `id` | `uuid` | NO | — | Primary key |
| `name` | `varchar(200)` | NO | — | Product display name |
| `description` | `text` | YES | NULL | Full product description / marketing copy |
| `brand` | `varchar(100)` | YES | NULL | Brand name (e.g., "Nike", "Zara") |
| `category_id` | `uuid` | YES | NULL | FK → `categories.id` |
| `gender` | `varchar(20)` | NO | — | Target gender: `Men` \| `Women` \| `Unisex` \| `Kids` |
| `base_price_amount` | `decimal(18,2)` | NO | — | Default price; variants may override |
| `base_price_currency` | `varchar(3)` | NO | `'USD'` | ISO 4217 currency code |
| `is_active` | `boolean` | NO | `true` | Soft-delete / hide from storefront |
| `created_at` | `timestamptz` | NO | — | Row creation timestamp |
| `updated_at` | `timestamptz` | NO | — | Last update timestamp |

## Constraints

| Type | Columns | Notes |
|------|---------|-------|
| PRIMARY KEY | `id` | |
| FOREIGN KEY | `category_id` → `categories.id` | SET NULL on category delete |
| CHECK | `base_price_amount >= 0` | |
| CHECK | `gender IN ('Men','Women','Unisex','Kids')` | |

## Indexes

| Name | Columns | Purpose |
|------|---------|---------|
| `idx_products_category_id` | `category_id` | Filter by category |
| `idx_products_gender` | `gender` | Filter by gender |
| `idx_products_brand` | `brand` | Filter / search by brand |
| `idx_products_is_active` | `is_active` | Storefront visibility filter |

## Relationships

| Relation | Direction | Notes |
|----------|-----------|-------|
| `categories` | Many-to-one | A product belongs to one category |
| `product_variants` | One-to-many | A product has many size×color variants |
| `product_images` | One-to-many | Product-level images (`variant_id IS NULL`) |

## Notes

- `base_price_amount` is the fallback price. A variant with `price_override_amount IS NULL` inherits this value.
- `updated_at` should be maintained by the application (domain event / EF interceptor), not a DB trigger.
