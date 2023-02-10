// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license information.
// https://github.com/dotnet/docfx/issues/274
// https://gist.github.com/wcoder/1ffaae564978d048357c32652e3f84c7

/**
 * This method will be called at the start of exports.transform in toc.html.js
 */
exports.preTransform = function (model) {
    try {
        if (model.items[0].topicUid === undefined)
            return model;

        const levelItemsArr = LevelItems(model)

        const maxLvl = Math.max.apply(Math, levelItemsArr.map(function (item) {
            return item.level;
        }))

        for (let level = maxLvl; level > 0; level--) {
            model = LevelParse(model, level)
        }
    } catch (exception) {
        console.error(exception);
    }

    return model;
}

/**
 * This method will be called at the end of exports.transform in toc.html.js
 */
exports.postTransform = function (model) {
    return model;
}

function LevelParse(model, level) {
    const levelItem = model.items.filter(function (item) {
        return item.level === level
    });

    for (let i = 0; i < levelItem.length; i++) {
        const item = levelItem[i];
        const parentName = GetParentName(item.topicUid);
        item.name = GeNameThisLevel(item.topicUid);

        let parent = model.items.filter(function (it) {
            return it.topicUid === parentName
        });

        if (parent[0] === undefined) {
            const newParent = {};
            newParent.topicUid = parentName
            newParent.name = parentName
            newParent.items = [];
            newParent.level = level - 1;
            model.items.push(newParent);

            parent = model.items.filter(function (it) {
                return it.topicUid === parentName
            });
        }

        parent[0].items.push(item);

        model.items = model.items.filter(function (it) {
            return it.topicUid !== item.topicUid
        });
    }

    return model;

}

function GetParentName(topicUid) {
    const levels = topicUid.split(".");
    levels.pop();

    return levels.join(".");
}

function GeNameThisLevel(topicUid) {
    const levels = topicUid.split(".");

    return levels.pop();
}

function LevelItems(model) {
    const newItems = [];

    for (let i = 0; i < model.items.length; i++) {
        const levelItem = AddLevelItem(model.items[i]);
        newItems.push(levelItem);
    }

    return newItems;
}

function AddLevelItem(item) {
    if (item.topicUid === undefined) {
        return item
    }

    item.level = item.topicUid.split(".").length

    return item;
}
