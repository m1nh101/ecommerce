# Table: `product_images`

Stores image URLs attached to either a product (shared across all variants) or a specific variant (e.g., color swatch photo).

## Columns

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| `id` | `uuid` | NO | — | Primary key |
| `product_id` | `uuid` | NO | — | FK → `products.id` |
| `variant_id` | `uuid` | YES | NULL | FK → `product_variants.id`; NULL = product-level image |
| `url` | `varchar(500)` | NO | — | Absolute or CDN-relative URL |
| `is_primary` | `boolean` | NO | `false` | Main display image (one per scope) |
| `sort_order` | `int` | NO | `0` | Display order within the same scope |

## Constraints

| Type | Columns | Notes |
|------|---------|-------|
| PRIMARY KEY | `id` | |
| FOREIGN KEY | `product_id` → `products.id` | CASCADE DELETE |
| FOREIGN KEY | `variant_id` → `product_variants.id` | CASCADE DELETE |

## Indexes

| Name | Columns | Purpose |
|------|---------|---------|
| `idx_product_images_product_id` | `product_id` | Load all images for a product |
| `idx_product_images_variant_id` | `variant_id` | Load images for a specific variant |

## Image Scoping Rules

| `variant_id` | Meaning |
|-------------|---------|
| `NULL` | Shared product image — shown for all variants unless variant has its own |
| set | Variant-specific image — typically color swatch or color-specific lifestyle shot |

## Primary Image Logic

- At most **one** `is_primary = true` per `(product_id, variant_id)` scope.
- If a variant has no primary image, the product-level primary image is used as fallback.
- Enforced at the application / domain layer (not by a DB constraint).

## Relationships

| Relation | Direction | Notes |
|----------|-----------|-------|
| `products` | Many-to-one | Parent product |
| `product_variants` | Many-to-one (optional) | Parent variant if variant-scoped |
