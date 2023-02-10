#!/bin/bash

source $(dirname "$0")/_common.sh
cd ${SITE_DIR}

# Clean site
echo "::group::Clean site"
find . -mindepth 1 -maxdepth 1 -type f -exec rm -v {} +
find . -mindepth 1 -maxdepth 1 -type d -regextype posix-extended -not -regex '\./(\.git|main|[0-9]{4}\.[0-9]{1,})' -exec rm -rv {} \;
echo "::endgroup::"

# Delete current built branch/tag directory
rm -rf ${GITHUB_REF_NAME}
echo "Removed ${GITHUB_REF_NAME}"