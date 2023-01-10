// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license information.
// https://github.com/dotnet/docfx/issues/274
// https://gist.github.com/wcoder/1ffaae564978d048357c32652e3f84c7
/**
 * This method will be called at the start of exports.transform in toc.html.js
 */
exports.preTransform = function(model) {

  try {
    if (model.items[0].topicUid === undefined)
      return model;

    var levelItemsArr = LevelItems(model)

    var maxLvl = Math.max.apply(Math, levelItemsArr.map(function (item) { return item.level; }))
    var minLvl = Math.min.apply(Math, levelItemsArr.map(function (item) { return item.level; }))
    //console.log("maxLvl: " + minLvl)

    for (var level = maxLvl; level > 0; level--) {
      model = LevelParse(model, level)
      //console.log("newModel: " + JSON.stringify(newModel))
    }
    //console.log("newModel: " + JSON.stringify(newModel))

  } catch (exception) {
    console.error(exception);
  }
  return model;
}

function LevelParse(model, level) {
  levelItem =  model.items.filter(function(item) {
    return item.level === level
  });

  for (var i = 0; i < levelItem.length; i++) {
    var item = levelItem[i]
    var parentName = GetParentName(item.topicUid);
    item.name = GeNameThisLevel(item.topicUid);
    parent = model.items.filter(function (it) { return it.topicUid === parentName });

    if (parent[0] === undefined) {
      var newParent = { }
      newParent.topicUid = parentName
      newParent.name = parentName
      newParent.items = [];
      newParent.level = level-1;
      model.items.push(newParent);
      parent = model.items.filter(function (it) { return it.topicUid === parentName });
    }
    parent[0].items.push(item);

    model.items = model.items.filter(function (it) { return it.topicUid !== item.topicUid });
    console.log("parentName: " + parentName)

  }

  console.log("model: " + JSON.stringify(model))
  return model;

}
function GetParentName(topicUid) {

  var levels = topicUid.split(".");
  levels.pop();
  var parent = levels.join(".");
  return parent;
}

function GeNameThisLevel(topicUid) {

  var levels = topicUid.split(".");
  var name = levels.pop();
  return name;
}

function LevelItems(model) {

  var newItems = [];
  for (var i = 0; i < model.items.length; i++) {
    var levelItem = AddLevelItem(model.items[i])
    //console.log("levelItem: " + JSON.stringify(levelItem))
    newItems.push(levelItem);
  }
  //console.log("newItems: " + JSON.stringify(newItems))

  return newItems;
}

function AddLevelItem(item) {

  if (item.topicUid === undefined) {
    return item
  }

  var level = item.topicUid.split(".").length
  console.log(item.topicUid + " level : " + level)

  item.level = level

  return item;
}
/**
 * This method will be called at the end of exports.transform in toc.html.js
 */
exports.postTransform = function (model) {
  return model;
}