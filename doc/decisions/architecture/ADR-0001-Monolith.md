# Building as modular monolith based on vertical slice concepts

## Context: High-level architecture and code organisation

When doing a sparetime project it could be fun to try a number of techniques, e.g. DDD, micro services, messaging, ...

But that would be building a very distributed system from the beginning, which would require an upfront use of time for something that will not be needed. I have no plans to provide this as a freemium service or make a business out of this.

## Decisions

1. Modular monolith
2. Vertical slice architecture

### Rationale

Even though I have been maintaining my own resume since 1998 in various formats (LaTeX, word, ...) and on various platforms (LinkedIn, recruiters propriatery system, ...) I do no know the domain and especially potential use cases well enough to go for something that splits this up.

See the [Modular Monolith blog series](http://www.kamilgrzybek.com/design/modular-monolith-primer/) for background information on Modular monolith.

See Jimmy Bogard's [blog post](https://jimmybogard.com/vertical-slice-architecture/) on vertical slice architecture.

## Status

I am the sole stakeholder so "Approved".

## Consequences

Positive: Modular and code base organised by features should be easier to browse and understand.

Negative: Need discipline going forward to ensure that I can change my mind going forward. Organising code in feature folders is new to me and will likely result in rework along the way.
