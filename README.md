[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
![Nuget Package](https://badgen.net/nuget/v/EdiLibrary.Core)

# EDILibrary
EDI Library for German energy market

This library won't provide much value without the corresponding template files.
These are distributed separately by Hochfrequenz. Please contact info (at) hochfrequenz.de for more information.

# Development

## I want to release a new version of this library
Click on [Draft new Release](https://github.com/Hochfrequenz/EDILibrary/releases/new) to create a new release.
When you create a new release, you may also create a new tag.
Tags and releases should have a `v` prefix. So use `v1.2.3` and **not** `1.2.3`.

Pushes of built packages to nuget [are only triggered on release](.github/workflows/release_nuget.yml), not on tag alone.
Having releases instead of only tags is preferable, because of the release notes which are included in e.g. dependabot PRs.

## I want to debug this library from a project that uses it
EDILibrary comes with SourceLink enabled, so you can load its debugging symbols from nuget.
In Visual Studio, under Settings➡Debugging **un**check "Enable Just My Code" (or follow the instructions [here](https://stackoverflow.com/a/654711/10009545))
