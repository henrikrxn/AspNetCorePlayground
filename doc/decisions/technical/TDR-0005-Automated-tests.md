# Automated tests

## Context: System infrastructure / plumbing

Need a test framework for unit and integration test, complete with test runner and assertions.

## Decisions

1. Using Xunit.net as unit test framework.
2. Using FluentAssertions.net as assertions framework.

### Rationale

These are my preferred tools for this area, which I know faily well. Have used / tried other test frameworks, i.e. NUnit, Fixie and prefer XUnit.net.

In particular I do not like that NUnit does (a) not follow semantic versioning and (b) only creates one instance of a test class which often leads to state leaking between test cases.

## Status

I am the sole stakeholder so "Approved".

## Consequences

Positive: Mainstream and lots of information available online + I know the tools.

Negative: None specific to the choices, but will not get to try anything new in this area.
