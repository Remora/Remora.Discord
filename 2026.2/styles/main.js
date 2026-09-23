// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license information.

function toggleMenu() {
               
    var x = document.getElementById("sidebar");
    var b = document.getElementById("blackout");

    if (x.style.left === "0px") 
    {
        x.style.left = "-350px";
        b.classList.remove("showThat");
        b.classList.add("hideThat");
    } 
    else 
    {
        x.style.left = "0px";
        b.classList.remove("hideThat");
        b.classList.add("showThat");
    }
}

(function () {
    anchors.options = {
      placement: 'right',
      visible: 'hover'
    };
    anchors.removeAll();
    anchors.add('article h2:not(.no-anchor), article h3:not(.no-anchor), article h4:not(.no-anchor)');

    function renderVersionBar() {
        let versionsPath = $("meta[property='docfx\\:versionsrel']").attr("content");
        if (!versionsPath) return;
        versionsPath = versionsPath.replace(/\\/g, '/');
        
        const versionBasePath = versionsPath.substring(0, versionsPath.lastIndexOf("/"));
        const currentVersion = $("meta[property='docfx\\:version']").attr("content");
        
        $.ajax({
            url: versionsPath,
            dataType: "json",
            cache: false,
            success: function (versions) {
                const versionBar = $("#versionbar");
                versionBar.empty();

                const versionBarElement = $('<ul class="nav level2"></ul>');
                versionBarElement.append(function() {
                    const list = $(`<ul class="nav level3" />`);
                    
                    for (var version of versions.filter(x => x !== currentVersion)) {
                        list.append(`<li><a class="sidebar-item" href="${versionBasePath}/${version}">${version}</a></li>`);
                    }

                    return $(`<li>
                                <span class="expand-stub"></span>
                                <a class="active sidebar-item">Version: <b>${currentVersion}</b></a>
                            </li>`)
                        .append(list);
                });

                versionBar.append(versionBarElement);

                $("#versionbar .nav > li > .expand-stub").click(function (e) {
                    $(e.target).parent().toggleClass("in");
                });
                $("#versionbar .nav > li > .expand-stub + a:not([href])").click(function (e) {
                    $(e.target).parent().toggleClass("in");
                });
            }
        });
    }

    renderVersionBar();
})();