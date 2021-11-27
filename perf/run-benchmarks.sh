#!/usr/bin/env bash

set -e

cd gfoidl.DataCompression.Benchmarks

# Running the benchmarks via --allCategories is much easier than via --filter and globbing.
dotnet run -c Release --no-build --allCategories Compression --join
dotnet run -c Release --no-build --allCategories Instantiation --join
