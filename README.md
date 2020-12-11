| CircleCi | CodeCov | NuGet |
| -- | -- | -- |
| [![CircleCI](https://circleci.com/gh/gfoidl/DataCompression/tree/master.svg?style=svg)](https://circleci.com/gh/gfoidl/DataCompression/tree/master) | [![codecov](https://codecov.io/gh/gfoidl/DataCompression/branch/master/graph/badge.svg)](https://codecov.io/gh/gfoidl/DataCompression) | [![NuGet](https://img.shields.io/nuget/v/gfoidl.DataCompression.svg?style=flat-square)](https://www.nuget.org/packages/gfoidl.DataCompression/) |

# gfoidl.DataCompression

## Algorithms

* [Dead band](./api-doc/articles/DeadBand.md)
* [Swinging Door](./api-doc/articles/SwingingDoor.md)

## Demos

See `./demos` for code.

![](./api-doc/articles/images/demo_01.png)

![](./api-doc/articles/images/demo_02.png)

![](./api-doc/articles/images/demo_03.png)

## Development channel

To get packages from the development channel use a `nuget.config` similar to this one:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        <add key="gfoidl-public" value="https://pkgs.dev.azure.com/gh-gfoidl/github-Projects/_packaging/gfoidl-public/nuget/v3/index.json" />
    </packageSources>
</configuration>
```
