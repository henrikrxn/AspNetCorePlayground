# Sticking to existing scheme for dependency management of NuGet packages

## Context: System infrastructure / plumbing

Would very much want to try the WIP [feature](https://github.com/NuGet/Home/wiki/Centrally-managing-NuGet-package-versions) for centrally managing NuGet packages.

Making sure all projects in a repo use the same version of a NuGet package can be a pain, especially in MonoRepos with multiple solution files.

The overall experience for NuGet package management is a gigantix cluster fuck that needs redesign.

## Decisions

1. Keep having the PackageReferences in the individual .csproj files.

### Rationale

As can be seen in this long-lived GitHub [issue](https://github.com/NuGet/Home/issues/6764) there is still limited support for way of doing dependency managements and things break from time to time.

Even if this is a spare time project I do not have the time nor the patience for fighting with tooling and suffering under Microsoft changing it's mind mindstream. Been there, done that and got the ".NET Core Json project file debacle" T-shirt.

## Status

I am the sole stakeholder so "Approved".

Will revisting decision if there ever is significant movement on this.

## Consequences

Positive:

- I know the current tooling and how to use it properly.
- I am the only developer so versioning drift will be minimal.

Negative: None other than begin stuck with the current tooling.
