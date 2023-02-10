#!/bin/bash

source $(dirname "$0")/_common.sh
cd ${SITE_DIR}

TEMPLATE_DIR=${DOCFX_DIR}/templates

build_versions_json() {
    # Read versions
    readarray -d '' versions < <( \
        find . -maxdepth 1 -type d -regextype posix-extended -regex '\./[0-9]{4}\.[0-9]{1,}' -printf '%f\0' \
      | sort -z -r \
    )

    # Check if we have to remove old versions
    if [[ ${#versions[@]} -gt ${KEEP_TAG_VERSIONS} ]]; then
        echo "::group::Removing old versions"
        local version_count=${#versions[@]}
        for (( i=${KEEP_TAG_VERSIONS}; i<${version_count}; i++ )); do
            echo "${versions[$i]}"
            # rm -rf ${versions[$i]}
            unset "versions[$i]"
        done
        echo "::endgroup::"
    fi


    # Generate versions.json
    echo "[" > versions.json
    echo "  \"main\"" >> versions.json
    for (( i=0; i<${#versions[@]}; i++ )); do
        echo " ,\"${versions[$i]}\"" >> versions.json
    done
    echo "]" >> versions.json

    versions+=( "main" )

    echo "Generated versions.json"
}

copy_template() {
    local file=$1
    local src_file=${TEMPLATE_DIR}/${file}
    local dest_file=${SITE_DIR}/${file}

    mkdir -p $(dirname "${dest_file}")

    cat ${src_file} \
        | sed -E "s/%LATEST_VERSION%/${versions[0]}/" \
        > ${dest_file}

    echo " - ${file}"
}

build_versions_json

echo "::group::Copy templates"
find ${TEMPLATE_DIR} -type f -printf '%P\n' | while read f; do
    copy_template "$f"
done
echo "::endgroup::"