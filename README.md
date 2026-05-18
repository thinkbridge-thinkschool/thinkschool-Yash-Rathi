# ThinkSchool Day 1 — Yash Rathi

This repository contains the completed Day 1 exercises for ThinkSchool.

---

# Piece 1 — Tools Check

Verified installation and setup of:

- .NET SDK 10
- Node.js 24
- Git
- Angular CLI
- VS Code
- GitHub Copilot
- Claude Code

Included:
- version command outputs

---

# Piece 2 — Hello in Two Languages

Created simple programs in:

## C#
- Console application using .NET

## TypeScript
- Node 24 native TypeScript execution

Concepts learned:
- runtime differences
- project structure
- TypeScript vs C# setup

---

# Piece 3 — ASP.NET Core 10 Minimal API

Built a Quotes API using ASP.NET Core Minimal API and EF Core SQLite.

Implemented endpoints:

- GET /api/quotes?page=N&size=N
- POST /api/quotes
- GET /api/quotes/{id}
- DELETE /api/quotes/{id}

Features:
- EF Core SQLite
- Repository Pattern
- Dependency Injection
- ValidationProblemDetails
- Exception Middleware
- Structured Logging
- Cancellation Tokens
- Auto-applied migrations

---

# Piece 4 — Node 24 + TypeScript Strict API

Built the same Quotes API using:

- Native Node.js HTTP server
- TypeScript strict mode
- better-sqlite3
- pino logging

Features:
- strict TypeScript configuration
- request validation
- graceful shutdown
- structured logging
- SQLite persistence
- no build step execution

---

# Repository Structure

```text
thinkschool_day_1
│
├── Piece_1_Versions
├── Piece_2_Hello in two languages
├── Piece_3_QuotesApi
└── Piece_4_QuotesApiNode
