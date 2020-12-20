#!/usr/bin/env bash

declare -r LOCAL_DIR=$(realpath $(dirname $0))
cd "${LOCAL_DIR}"

if [[ -d nuget ]]; then
    rm -rf nuget
fi

dotnet pack -c Release
pushd nuget
    for f in *.symbols.nupkg; do 
        dotnet nuget push --skip-duplicate -k "${NUGET_AUTH_KEY}" "${f}"
    done
popd

