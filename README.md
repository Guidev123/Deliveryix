# Deliveryix

A delivery platform built from scratch — think iFood, Keeta, 99Food. The goal is to build a production-grade system making real architectural decisions, not a tutorial project.

> **Status:** In active development. The Identity (IAM) module is nearly complete. Other modules are being defined.

---

## Architecture overview

Deliveryix is a **modular monolith in .NET**, with two modules running as independent APIs:

- **Identity API** — IAM, authentication, roles and permissions
- **Payments API** — payment processing (in planning)

**Request path:** `Frontend → WAF → APIM → AKS (Kubernetes)`


---

## Key architectural decisions

**Custom Authentication Extension (Entra ID)**
Before Entra confirms a sign up, it calls an Azure Function via `OnAttributeCollectionSubmit` hook. The function validates document and email uniqueness against the Identity database. No post-processing, no compensating transactions.

**Outbox Pattern**
Domain events are persisted to an Outbox table before any async processing. A dedicated `OutboxWorker` pod in K8s processes batches of 25 events every 5 seconds, decoupling the Entra flow from the domain.

**Hybrid RBAC**
Entra manages OAuth2 scopes in the JWT. Domain roles and permissions (`Customer`, `RestaurantOwner`, granular permissions like `orders:read:own`) live in the Identity module and are assigned via domain events after account creation.

**Token enrichment at the APIM layer**
Every request is enriched at the gateway: APIM validates the JWT, reads roles and permissions from Redis, and injects `X-Identity-Id`, `X-Roles` and `X-Permissions` headers before forwarding. On cache miss, the Identity API fetches from the database and writes to cache. Downstream workloads are fully decoupled from Entra.

---

## Diagrams

C4 Component diagrams for the Identity module are available in [`docs/diagrams/`](docs/diagrams/).

- Sign up flow
- Request enrichment flow

---

## Documentation

- [Entra ID manual setup](docs/infrastructure/entra-id-setup.md)
- [Local development token script](scripts/dev/README.md)

---

## Running locally

Infrastructure dependencies (SQL Server, Redis, Azure Service Bus emulator) are available via Docker Compose:

```bash
docker compose up -d
```

Refer to [`scripts/dev/README.md`](scripts/dev/README.md) to simulate APIM token enrichment locally.