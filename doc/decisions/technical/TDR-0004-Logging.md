# Logging

## Context: System infrastructure / plumbing

Need a logging library with multitude of sinks to enable experimentation going forward.

## Decisions

1. Using Microsoft.Extensions.Logging abstractions.
2. Using Serilog as the library underneath.

### Rationale

Microsoft.Extensions.Logging is an OK abstraction and Serilog is at the time of writing the obvious choice especially because of the multitude of available sinks.

## Status

I am the sole stakeholder so "Approved".

## Consequences

Positive: Mainstream and lot's of information available online + I know Serilog.

Negative: None specific to the choices.
