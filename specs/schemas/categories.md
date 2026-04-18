# Table: `categories`

Flat list of product categories for a fashion store (e.g., T-Shirts, Sneakers, Pants).

## Columns

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| `id` | `uuid` | NO | — | Primary key |
| `name` | `varchar(100)` | NO | — | Display name (e.g., "T-Shirts") |
| `slug` | `varchar(100)` | NO | — | URL-safe identifier (e.g., "t-shirts") |
| `description` | `text` | YES | NULL | Optional marketing description |
| `display_order` | `int` | NO | `0` | Controls sort order in listings |
| `is_active` | `boolean` | NO | `true` | Soft-delete / hide from storefront |
| `created_at` | `timestamptz` | NO | — | Row creation timestamp |

## Constraints

| Type | Columns | Notes |
|------|---------|-------|
| PRIMARY KEY | `id` | |
| UNIQUE | `slug` | Enforces URL uniqueness |

## Indexes

| Name | Columns | Purpose |
|------|---------|---------|
| `idx_categories_slug` | `slug` | Lookup by URL slug |
| `idx_categories_is_active` | `is_active` | Filter active categories |

## Sample Data

| name | slug | display_order |
|------|------|---------------|
| T-Shirts | t-shirts | 1 |
| Shirts | shirts | 2 |
| Pants | pants | 3 |
| Shorts | shorts | 4 |
| Sneakers | sneakers | 5 |
| Boots | boots | 6 |
| Sandals | sandals | 7 |
| Jackets | jackets | 8 |
| Accessories | accessories | 9 |
