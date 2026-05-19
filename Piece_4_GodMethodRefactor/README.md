# Piece 4 — God Method Refactor

![CI](https://github.com/YOUR_USERNAME/YOUR_REPO_NAME/actions/workflows/ci.yml/badge.svg)

> Replace `YOUR_USERNAME` and `YOUR_REPO_NAME` in the badge URL above after pushing to GitHub.

## Task Summary

Refactored a deliberately bad `OrderController.cs` (AI-generated with 10+ code smells) into a clean, layered ASP.NET Core 9 API.

## Project Structure

```
Piece_4_GodMethodRefactor/
├── INITIAL_PROMPT.md              ← Prompt used to generate the bad code
├── REFACTOR_NOTES.md              ← 10+ smells identified with fixes
├── POST_REQUEST_EXAMPLES.md       ← How to test the API
├── OriginalBadCode/
│   └── OrderController.cs         ← AI-generated original (do not modify)
├── OrderRefactorApi/              ← Refactored clean project
│   ├── Controllers/               ← Thin HTTP layer only
│   ├── Services/                  ← Business logic
│   ├── Repositories/              ← Data access layer
│   ├── Models/                    ← Domain entities
│   ├── Dtos/                      ← Typed request/response shapes
│   ├── Data/                      ← EF Core DbContext
│   ├── Extensions/                ← Seed data
│   └── Constants/                 ← No more magic numbers
└── OrderRefactorApi.Tests/        ← 3 unit tests + 1 integration test
```

## How to Run

```bash
cd OrderRefactorApi
dotnet run
```

API starts at `http://localhost:5000`

## How to Test POST /api/orders

```bash
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": 1,
    "items": [{"productId": 1, "quantity": 5}],
    "shippingAddress": "123 Main Street, New York, NY 10001"
  }'
```

## How to Run Tests

```bash
cd OrderRefactorApi.Tests
dotnet test
```

Expected: **4 tests passed** (3 unit + 1 integration)

## What Was Fixed

| Smell | Fix |
|-------|-----|
| Empty catch blocks | Specific catches with logging |
| Sync EF calls in async method | Full async/await + CancellationToken |
| Returns `object` | Typed `OrderCreateResponse` DTO |
| All logic in controller | Controller / Service / Repository layers |
| Magic numbers | Constants files |
| No transaction | Single DB transaction wraps entire order |
| Off-by-one inventory bug | Fixed `>=` check |
| Null deref on customer.Discounts | Null-safe repository pattern |
| DateTime.Now | DateTime.UtcNow |
| Multiple SaveChanges | Single CommitAsync |
