| CI | Coverage | NuGet |
| -- | -- | -- |
| [![Build Status](https://dev.azure.com/gh-gfoidl/github-Projects/_apis/build/status/.NET/DataCompression?branchName=master)](https://dev.azure.com/gh-gfoidl/github-Projects/_build/latest?definitionId=36&branchName=master) | ![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/gh-gfoidl/github-Projects/36?style=flat-square) | [![NuGet](https://img.shields.io/nuget/v/gfoidl.DataCompression.svg?style=flat-square)](https://www.nuget.org/packages/gfoidl.DataCompression/) |

# gfoidl.DataCompression

## Algorithms

* [Dead band](./api-doc/articles/DeadBand.md)
* [Swinging Door](./api-doc/articles/SwingingDoor.md)

## Demos

See `./demos` for code.

_Note_: the graphs in the documentation are created by the code from `./demos`, so you can see which config-values got used.

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
