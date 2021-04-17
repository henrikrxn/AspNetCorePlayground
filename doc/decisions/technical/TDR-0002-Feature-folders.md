# Feature folders

## Context: Code organisation

The usual Microsoft way with dividing the code into folders for specific types of thing, e.g. controllers, views, ... has annoyed me for a long time.

From the beginning it is hard to see what files are related to what features and as soon as the solution reaches a certain size there are too many files in each folder.

Jimmy Bogard has done an [example](https://github.com/jbogard/contosoUniversityDotNetCore-Pages) based on the Contoso University sample originally created by Microsoft.

## Decisions

1. Organising code according to Features.

### Rationale

Related code is kept close together so easier to get an overview of a feature and browse the code.

## Status

I am the sole stakeholder so "Approved".

## Consequences

Positive: Code base should be easier to understand and find your way around.

Negative: This way of structuring a code base is new for me so likely some overhead. Also likely to be some fighting with Microsoft tooling.
