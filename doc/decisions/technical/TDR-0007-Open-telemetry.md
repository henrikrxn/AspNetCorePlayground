# Logging

## Context: System infrastructure / plumbing

Since I originally wrote the ADRs OpenTelemetry has moved from fringe to mainstream although not everything has been released as GA yet.

Traces and Metrics are GA, but logging is still not.

## Decisions

1. Start using OpenTelemetry traces and metrics.
2. Keep using Microsoft.Extensions.Logging wrapping Serilog as the way of doing logging for now.

### Rationale

OpenTelemetry seems like the future, so if I do not try it on a spare time project when should I do it.

## Status

I am the sole stakeholder so "Approved".

## Consequences

Positive: Seems like it is a good bet that OpenTelemetry is the future.

Negative: New technology is more likely to change.
