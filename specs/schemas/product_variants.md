# Table: `product_variants`

Each row represents a unique size × color combination of a product. This is the leaf-level sellable unit with its own SKU, stock, and optional price override.

## Columns

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| `id` | `uuid` | NO | — | Primary key |
| `product_id` | `uuid` | NO | — | FK → `products.id` |
| `color_id` | `uuid` | NO | — | FK → `colors.id` |
| `size_id` | `uuid` | NO | — | FK → `sizes.id` |
| `sku` | `varchar(100)` | NO | — | Stock-keeping unit; globally unique |
| `price_override_amount` | `decimal(18,2)` | YES | NULL | When set, overrides `products.base_price_amount` |
| `price_override_currency` | `varchar(3)` | YES | NULL | Required when `price_override_amount` is set |
| `stock_quantity` | `int` | NO | `0` | Units available; must be ≥ 0 |
| `is_active` | `boolean` | NO | `true` | Hides variant without deleting |
| `created_at` | `timestamptz` | NO | — | Row creation timestamp |
| `updated_at` | `timestamptz` | NO | — | Last update timestamp |

## Constraints

| Type | Columns | Notes |
|------|---------|-------|
| PRIMARY KEY | `id` | |
| UNIQUE | `sku` | Globally unique across all variants |
| UNIQUE | `(product_id, color_id, size_id)` | No duplicate variant per product |
| FOREIGN KEY | `product_id` → `products.id` | CASCADE DELETE |
| FOREIGN KEY | `color_id` → `colors.id` | RESTRICT |
| FOREIGN KEY | `size_id` → `sizes.id` | RESTRICT |
| CHECK | `stock_quantity >= 0` | |
| CHECK | `price_override_amount IS NULL OR price_override_amount >= 0` | |

## Indexes

| Name | Columns | Purpose |
|------|---------|---------|
| `idx_product_variants_product_id` | `product_id` | Load all variants of a product |
| `idx_product_variants_color_id` | `color_id` | Filter products by color |
| `idx_product_variants_size_id` | `size_id` | Filter products by size |
| `idx_product_variants_sku` | `sku` | Order / cart lookup by SKU |
| `idx_product_variants_stock` | `stock_quantity` | In-stock filter |

## Effective Price Logic

```
effective_price =
  COALESCE(price_override_amount, products.base_price_amount)
```

The currency is similarly resolved:
```
effective_currency =
  COALESCE(price_override_currency, products.base_price_currency)
```

## Stock Status Derivation

| stock_quantity | Status |
|---------------|--------|
| 0 | Out of stock |
| 1–5 | Low stock |
| > 5 | In stock |

(Thresholds are application-level, not enforced by the DB.)

## Relationships

| Relation | Direction | Notes |
|----------|-----------|-------|
| `products` | Many-to-one | Owner aggregate |
| `colors` | Many-to-one | Color lookup |
| `sizes` | Many-to-one | Size lookup |
| `product_images` | One-to-many | Variant-specific images |
