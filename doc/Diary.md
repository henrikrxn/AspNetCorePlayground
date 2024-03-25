# Development diary

One thing is implementing / setting something up another is capturing the little things you learn / experience along the
way and the thoughts that this triggers.

This document is my attempt at capturing this information.

## 2024-03-25
Added auditing inspired by Mastodon [post}(https://hachyderm.io/@simoncropp/112153513559936669) by Simon Cropp.

## 2024-01-25
Started writing the diary on this date.
Everything before this date was written in retrospect, so might not be completely accurate.

## 2024-01-16
.editorconfig csharp_style_unused_value_expression_statement_preference for error when not using return value.

Unforeseen effect:
Fluent interfaces, e.g. builder / app usage in ASP.NET Core setup, are not very suited for this, because the "by
convention" usage pattern has been to ignore return values because it is the same, mutable object.

Will keep using this to see what "regular" application / business logic code feels like if I ever get to it.


## When did I look into CPM + Directory.Build.props ?

- Packages in `Directory.Build.props` does not work with tooling. VS, nuget.exe (and dotnet CLI?) adds updates to
  the `.csproj` file
- Central Package Management tag + Directory.Packages.props is not supported in VS, see issue <<??>>
