#!/bin/bash

set -e

log() {
    echo ""
    echo "$1"
    echo ""
}

core() {
    log "building $NAME"

    set -x
    
    dotnet restore
    dotnet build -c Release --no-restore
    find tests -name "*.csproj" | xargs -n1 dotnet test -c Release --no-build
    
    set +x
}

pack() {
    set -x

    dotnet pack -o "$(pwd)/NuGet-Packed" --no-build -c Release "source/$NAME"
    
    if [[ -z "$DEBUG" ]]; then
        dotnet nuget push --source "$1" --api-key "$2" -t 60 ./NuGet-Packed/*.nupkg
    else
        echo "push to $1"
    fi

    ls -l ./NuGet-Packed

    set +x
}

export BuildNumber=$(git log --oneline | wc -l)
echo "BuildNumber: $BuildNumber"

if [[ -n "$TAG_NAME" ]]; then
    # only build tags matching a version
    if [[ "$TAG_NAME" =~ ^v[0-9].[0-9].[0-9]$ ]]; then
        log "building tag $TAG_NAME"
        
        core
        pack "$NUGET_FEED" "$NUGET_KEY"
    fi
else
    if [[ -n "$CI_BUILD_NUMBER" ]]; then
        export VersionSuffix="preview-$CI_BUILD_NUMBER"
        echo "VersionSuffix: $VersionSuffix"
    fi

    log "building branch $BRANCH_NAME"
    core

    if [[ "$BRANCH_NAME" == "master" ]]; then
        pack "$MYGET_FEED" "$MYGET_KEY"
    fi
fi