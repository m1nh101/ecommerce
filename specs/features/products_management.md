# Products Management Feature Specification

## Overview

The Products Management feature enables administrators to create, update, and manage fashion store products with support for variant-level stock and pricing (size × color combinations), gender targeting, and flat category taxonomy. This feature supports the core e-commerce workflow of product catalog management, ensuring products can be filtered by category, gender, brand, color, size, stock status, and price range.

The feature is built using Domain-Driven Design principles, with business logic encapsulated in the Domain layer, CQRS pattern in the Application layer, and RESTful API endpoints in the API layer.

## Entities and Relationships

Based on the database schema, the following entities form the product management domain:

### Core Entities

- **Product** (Aggregate Root): Represents a product master with base information (name, brand, gender, base price, category). Controls all child entities (variants, images).
- **ProductVariant** (Entity): Represents a unique size × color combination with SKU, optional price override, and stock quantity.
- **ProductImage** (Entity): Represents product or variant-level images with primary image scoping.
- **Category** (Aggregate): Global catalog of product categories (e.g., Shirts, Shoes, Pants).
- **Color** (Aggregate): Global catalog of colors with hex codes.
- **Size** (Aggregate): Global catalog of sizes grouped by type (clothing, shoe, accessory).

### Relationships

```
categories ──< products >── product_variants ──< product_images
                                 │
                    colors ──────┤
                    sizes  ──────┘

products ──< product_images  (product-level images, variant_id = NULL)
```

### Value Objects

- **Money**: Represents price with amount and currency (e.g., USD, EUR).
- **SKU**: Unique stock-keeping unit for variants.

## Business Rules

### Product Creation and Management
- Products must have a unique name within the system.
- Base price must be greater than $0.
- Gender must be one of: Men, Women, Unisex, Kids.
- Products can be activated/deactivated for availability.
- Products belong to exactly one category.

### Variant Management
- Variants are unique combinations of product + color + size.
- Each variant has a unique SKU.
- Variants can have price overrides from the base product price.
- Stock quantity cannot be negative.
- Variants can be activated/deactivated independently.

### Image Management
- Images can be attached to products (general) or specific variants.
- Only one primary image per scope (product or variant).
- Images are stored by URL (external CDN assumed).

### Stock Status Calculation
- **Out of Stock**: stock_quantity = 0
- **Low Stock**: stock_quantity = 1-5
- **In Stock**: stock_quantity > 5

### Filtering and Search
- Products can be filtered by: category, gender, brand, color, size, in-stock status, price range, active status.
- Effective price for variants: base_price + price_override (if set).

### Catalog Management
- Categories, colors, and sizes are global catalogs maintained by administrators.
- Colors include hex codes for display.
- Sizes are grouped by type and sorted for consistent display.

## User Stories

### As an Administrator, I want to:
1. Create a new product with base information (name, brand, gender, base price, category).
2. Update product details (name, brand, gender, base price, category, active status).
3. Add variants to a product (color, size, SKU, price override, initial stock).
4. Update variant details (price override, stock quantity, active status).
5. Remove variants from a product.
6. Add images to products or variants, marking primary images.
7. Remove images from products or variants.
8. View all products with pagination and filtering.
9. View detailed product information including variants and images.
10. Manage global catalogs (categories, colors, sizes).
11. Search products by name, brand, category, gender.
12. Filter products by color, size, stock status, price range.

### As a Customer (read-only access):
1. Browse products with filtering and search.
2. View product details with variants and images.

## API Endpoints

### Products

#### Create Product
- **POST** `/api/v1/products`
- **Request Body**:
  ```json
  {
    "name": "string",
    "brand": "string",
    "gender": "Men|Women|Unisex|Kids",
    "basePrice": {
      "amount": 0.0,
      "currency": "USD"
    },
    "categoryId": "guid",
    "isActive": true
  }
  ```
- **Response**: 201 Created with product ID

#### Update Product
- **PATCH** `/api/v1/products/{productId}`
- **Request Body**: Same as create, partial update allowed
- **Response**: 204 No Content

#### Get Products (List)
- **GET** `/api/v1/products?pageNumber=1&pageSize=20&name=...&brand=...&categoryId=...&gender=...&minPrice=...&maxPrice=...&isActive=...&colorId=...&sizeId=...&inStockOnly=...`
- **Response**: Paged list of products with basic info

#### Get Product Detail
- **GET** `/api/v1/products/{productId}`
- **Response**: Full product with variants and images

#### Delete Product
- **DELETE** `/api/v1/products/{productId}`
- **Response**: 204 No Content (soft delete via isActive=false)

### Product Variants

#### Add Variant
- **POST** `/api/v1/products/{productId}/variants`
- **Request Body**:
  ```json
  {
    "colorId": "guid",
    "sizeId": "guid",
    "sku": "string",
    "priceOverride": {
      "amount": 0.0,
      "currency": "USD"
    },
    "stockQuantity": 0,
    "isActive": true
  }
  ```
- **Response**: 201 Created with variant ID

#### Update Variant
- **PATCH** `/api/v1/products/{productId}/variants/{variantId}`
- **Request Body**: Partial update of variant fields
- **Response**: 204 No Content

#### Remove Variant
- **DELETE** `/api/v1/products/{productId}/variants/{variantId}`
- **Response**: 204 No Content

### Product Images

#### Add Image
- **POST** `/api/v1/products/{productId}/images`
- **Request Body**:
  ```json
  {
    "url": "string",
    "altText": "string",
    "variantId": "guid?", // null for product-level
    "isPrimary": false
  }
  ```
- **Response**: 201 Created with image ID

#### Update Image
- **PATCH** `/api/v1/products/{productId}/images/{imageId}`
- **Request Body**: Partial update
- **Response**: 204 No Content

#### Remove Image
- **DELETE** `/api/v1/products/{productId}/images/{imageId}`
- **Response**: 204 No Content

### Catalogs

#### Categories
- **GET** `/api/v1/categories` — List all categories
- **POST** `/api/v1/categories` — Create category
- **PATCH** `/api/v1/categories/{id}` — Update category
- **DELETE** `/api/v1/categories/{id}` — Delete category

#### Colors
- **GET** `/api/v1/colors` — List all colors
- **POST** `/api/v1/colors` — Create color (with hex code)
- **PATCH** `/api/v1/colors/{id}` — Update color
- **DELETE** `/api/v1/colors/{id}` — Delete color

#### Sizes
- **GET** `/api/v1/sizes` — List all sizes
- **POST** `/api/v1/sizes` — Create size (with type)
- **PATCH** `/api/v1/sizes/{id}` — Update size
- **DELETE** `/api/v1/sizes/{id}` — Delete size

## Data Models

### Product Response
```json
{
  "id": "guid",
  "name": "string",
  "brand": "string",
  "gender": "string",
  "basePrice": {
    "amount": 99.99,
    "currency": "USD"
  },
  "category": {
    "id": "guid",
    "name": "string"
  },
  "isActive": true,
  "variants": [...],
  "images": [...]
}
```

### Variant Response
```json
{
  "id": "guid",
  "color": {
    "id": "guid",
    "name": "string",
    "hexCode": "#FFFFFF"
  },
  "size": {
    "id": "guid",
    "name": "string",
    "type": "Clothing"
  },
  "sku": "string",
  "priceOverride": {
    "amount": 0.0,
    "currency": "USD"
  },
  "effectivePrice": {
    "amount": 99.99,
    "currency": "USD"
  },
  "stockQuantity": 10,
  "stockStatus": "InStock",
  "isActive": true
}
```

### Image Response
```json
{
  "id": "guid",
  "url": "string",
  "altText": "string",
  "variantId": "guid?",
  "isPrimary": false
}
```

## Validation Rules

### Product Validation
- Name: Required, 1-200 characters
- Brand: Required, 1-100 characters
- Gender: Required, enum value
- Base Price: Required, amount > 0, currency 3 characters
- Category ID: Required, valid GUID, exists in database

### Variant Validation
- Color ID: Required, valid GUID, exists
- Size ID: Required, valid GUID, exists
- SKU: Required, unique across all variants
- Price Override: Optional, amount >= 0 if provided
- Stock Quantity: Required, >= 0
- Combination unique: Product + Color + Size must be unique

### Image Validation
- URL: Required, valid URL format
- Alt Text: Optional, 1-255 characters
- Variant ID: Optional, if provided must exist and belong to product

### Catalog Validation
- Category Name: Required, unique, 1-100 characters
- Color Name: Required, unique, 1-50 characters
- Color Hex Code: Required, valid hex format (#RRGGBB)
- Size Name: Required, 1-20 characters
- Size Type: Required, enum (Clothing, Shoe, Accessory)

## Error Handling

### Common Error Responses
- **400 Bad Request**: Validation errors, with detailed field errors
- **404 Not Found**: Resource not found
- **409 Conflict**: Unique constraint violations (SKU, variant combination)
- **500 Internal Server Error**: Unexpected errors

### Specific Business Errors
- ProductNotFound
- VariantNotFound
- ImageNotFound
- CategoryNotFound
- ColorNotFound
- SizeNotFound
- DuplicateSku
- DuplicateVariantCombination
- InvalidPrice
- NegativeStock

## Implementation Notes

### Existing Implementation
- Domain layer: Fully implemented with aggregates, entities, value objects, domain events
- Application layer: Partial (Add/Update Product, Get Products/Detail queries)
- API layer: Partial (corresponding endpoints)

### Missing Features (to be implemented)
- All variant CRUD operations
- All image CRUD operations
- Catalog management endpoints
- Advanced filtering (color, size, stock status)
- Stock status calculation in queries
- Delete operations (soft delete via isActive)

### Technical Considerations
- Use EF Core for data access with proper indexing
- Implement CQRS with Mediator for commands/queries
- Use FluentValidation for request validation
- Return Result<T> for error handling
- Raise domain events for audit trails
- Support pagination for list endpoints
- Use snake_case for database columns