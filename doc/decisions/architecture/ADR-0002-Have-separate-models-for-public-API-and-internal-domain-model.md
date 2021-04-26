# Will use use Data Transfer Objects (DTOs) to decouple public API from internal model

## Context: Decouple internal domain model from public model

Using DTOs as input / output from the web API may seem like overkill, but experience has shown that not doing this will hamper development over time.

## Decisions

1. All public APIs must use DTO objects for input / output.

### Rationale

I want to decouple the internal model from the public model to

- enable the internal model to evolve freely because it is not exposed.
- avoid breaking consumers when the internal model changes.

## Status

I am the sole stakeholder so "Approved".

## Consequences

Positive: Can change internal model as necessary.

Negative: Introduces development overhead and performance overhead because of translation between internal model and DTO model.
