#!/usr/bin/env bash

if [[ ! -d output ]]; then
    exit 1
fi

mkdir -p site
cp -r output/{api,assets,.github} site/
cp output/*.{atom,html,rss} site/

