#!/usr/bin/env bash

for demoProject in ./**/*.csproj; do
    echo ""
    echo "-------------------------------------------------"
    echo "Running demo '$(basename $demoProject .csproj)'"

    cd $(dirname $demoProject)
    dotnet run --no-build
    cd - >/dev/null
done
