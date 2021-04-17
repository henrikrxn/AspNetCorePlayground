# Decisions Record

See the README.md in the root folder for background information on this project.

This folder records the decisions for my project.

But then I reliased that there are a number of decision made in any project that are not architectural, but still have a major effect on the project, e.g. for C# project versioning the package used in a file in the root instead of each project.

Thus far I have in my order of importance / impact:

1. **Business decisions**. TODO, e.g. this is not for making money, but for creating awareness, e.g. freemium tiers on various services.
2. **Architectural decisions**. Based 100% on Michael Nygard's seminal article [Michael Nygard](https://cognitect.com/blog/2011/11/15/documenting-architecture-decisions). TODO: Why is this second to business? Should be obvious, but...
3. **Technical decisions**, e.g. (i) each developer must be able to build from scratch on her/his own computer or (ii) using Cake for all things related to building the project. To me this has got nothing to do with ADRs.
  (i) is a fundamental decision that is technological agnostic while (ii) is a deliberate choice and a decision record would be a good way of capturing WHY the project made this decision at this point in time.

I am sure there are more, which I will added once I have reallized that I have made them. Some decisions are simple, e.g. why xunit.net instead of nunit, but the why is virtually never recorded.
This probably does not scale / would have negative effect in teams as it would make it hard to introduce new things, e.g. FluentAssertions if every decision had to be recorded (and thus scrutinized)-

## Business decisions

There will likyly be no business decisions as this is purely for my own sake. But s I work as a consultant it could be seen as a showcase of my work.

## Architectural decisions

These are, not surprisingly in the folder architecure.

I have wanted to try ADRs for some time and a sparetime project, which hopefully will be experimental, is a good place to document why I choose one approacg over the other.
I hope that the ADRs will shed some light upon which decisions are purely for the sake of experimenting and which I would be likely to make in any project that I get paid to do.

So ADRs are included because:

* It is a sparetime project so I'll be making a number of decisions that are not driven by need or sense, but by a desire to try new "things" in a setting I control 100%.
* Given my history when it comes to coding in my sparetime this project will be dormant for extended periods of time and ADRs will be a good reminder of why I chose a particular direction.

## Technical decisions

As this will be a one man show I plan to record these for my own sake so I can remember where I am at especially in experimental branches.
