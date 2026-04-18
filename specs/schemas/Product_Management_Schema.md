# Product Management — Database Schema

Fashion store product management schema supporting variant-level stock/pricing (size × color), gender targeting, and flat category taxonomy.

## Tables

| Table | Description |
|-------|-------------|
| [categories](categories.md) | Flat list of product categories (Shirts, Shoes, Pants, …) |
| [colors](colors.md) | Global color catalog with hex codes |
| [sizes](sizes.md) | Global size catalog grouped by type (clothing / shoe / accessory) |
| [products](products.md) | Product master — name, brand, gender, base price, category |
| [product_variants](product_variants.md) | Each unique size × color combination with SKU, optional price override, stock |
| [product_images](product_images.md) | Product-level or variant-level images |

## Entity Relationships

```
categories ──< products >── product_variants ──< product_images
                                 │
                    colors ──────┤
                    sizes  ──────┘

products ──< product_images  (product-level images, variant_id = NULL)
```

## Filtering Capabilities

| Filter | Table / Column |
|--------|---------------|
| Category | `products.category_id` |
| Gender | `products.gender` |
| Brand | `products.brand` |
| Color | `product_variants.color_id` |
| Size | `product_variants.size_id` |
| In stock | `product_variants.stock_quantity > 0` |
| Price range | `products.base_price_amount` or effective variant price |
| Active status | `products.is_active`, `product_variants.is_active` |
