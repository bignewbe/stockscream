if (!String.prototype.hashCode) {
    String.prototype.hashCode = function () {
        var hash = 0, i, chr, len;
        if (this.length == 0) return hash;
        for (i = 0, len = this.length; i < len; i++) {
            chr = this.charCodeAt(i);
            hash = ((hash << 5) - hash) + chr;
            hash |= 0; // Convert to 32bit integer
        }
        return hash;
    };
}

if (!String.prototype.format) {
    String.prototype.format = function () {
        var args = arguments;
        return this.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
              ? args[number]
              : match
            ;
        });
    };
}

(function (facility, $, undefined) {
    var tableOption = {
        "class": "table-condensed small table-hover table-striped",
        "data-toggle": "table",
        //"data-click-to-select": "true",
        "data-maintain-selected": "true",
        //"data-pagination": "true",
        "data-search": "true"
    };

    facility.MapAction = function (actionName) {
        if (actionName.toLowerCase() === "SearchW".toLowerCase())
            return "/TA/" + actionName;

        if (actionName.toLowerCase() === "StockDetails".toLowerCase())
            return "/TA/" + actionName;

        if (actionName.toLowerCase() === "RunCommand".toLowerCase())
            return "/search/" + actionName;

        if (actionName.toLowerCase() === "SaveFilter".toLowerCase())
            return "/data/" + actionName;

        if (actionName.toLowerCase() === "RemoveFilter".toLowerCase())
            return "/data/" + actionName;

        if (actionName.toLowerCase() === "Download".toLowerCase())
            return "/data/" + actionName;

        if (actionName.toLowerCase() === "GetTableItem".toLowerCase())
            return "/data/" + actionName;

        return null;
    };

    facility.CreateLocaoCache = function (timeout, limit) {
        var localCache = {
            timeout: timeout,   //expire time
            limit: limit,       //piece of content allowed in cache
            count: 0,
            data: {},
            empty: function () {
                for (var k in localCache.data)
                    delete localCache.data[k];
                localCache.count = 0;
            },
            remove: function (key) {
                if (localCache.data.hasOwnProperty(key)) {
                    delete localCache.data[key];
                    --localCache.count;
                }
            },
            exist: function (key) {
                return !!localCache.data[key] && (new Date().getTime() - localCache.data[key]._);
            },
            get: function (key) {
                return localCache.data[key].data;
            },
            set: function (key, cachedData, callback) {
                if (localCache.count > localCache.limit)
                    localCache.empty();
                else
                    localCache.remove(key);

                localCache.data[key] = { _: new Date().getTime(), data: cachedData };
                ++localCache.count;

                if ($.isFunction(callback)) callback(cachedData);
            }
        };
        return localCache;
    };
    
    facility.GetTableOption = function () {
        return tableOption;
    };

    facility.CreateTableDom = function (items, keys, options) {
        var table = document.createElement("table");
        var thead = document.createElement("thead");
        var tr = document.createElement("tr");
        var th = document.createElement("th");
        th.setAttribute("data-field", "state");
        th.setAttribute("data-checkbox", "true");
        tr.appendChild(th);

        for (var i = 0; i < keys.length; i++) {
            var th = document.createElement("th");
            var text = document.createTextNode(items[i]);
            th.setAttribute("data-field", keys[i]);
            th.setAttribute("data-sortable", "true");
            th.appendChild(text);
            tr.appendChild(th);
        }
        thead.appendChild(tr);
        table.appendChild(thead);

        //set attributes
        options = ($.isEmptyObject(options) || typeof options == "undefined") ? tableOption : options;
        for (var o in options)
            table.setAttribute(o, options[o]);

        //table.className += " table-striped table-hover table-condensed";
        return table;
    };

    facility.CreateTableJquery = function (items, keys, options) {
        options = ($.isEmptyObject(options) || typeof options == "undefined") ? tableOption : options;
        var table = $('<table></table>').attr(options);
        var thead = $("<thead></thead>");
        var tr = $('<tr></tr>');
        var th = $('<th></th>').attr({ "data-field": "state", "data-checkbox": "true" });
        tr.append(th);
        for (var i = 0; i < keys.length; i++) {
            var th = $('<th></th>').attr({ "data-field": keys[i], "data-sortable": "true" });
            th.text(items[i]);
            tr.append(th);
        }
        thead.append(tr);
        table.append(thead);
        return table;
    };

    facility.UpdateLocalData = function (data, key) {
        if (typeof Storage !== void (0)) {
            localStorage.setItem(key, JSON.stringify(data))
        }
    };

    facility.GetLocalData = function (key) {
        if (typeof Storage !== void (0)) {
            var userData = JSON.parse(localStorage.getItem(key));
            if (userData !== null) {
                return userData;
            }
        }
        return null;
    };

    //one problem is that it lose all space while we post data
    facility.RedirectToUrl = function(url, method, postData, isAntiforgery) {
        var form = document.createElement('form');
        form.method = method;
        form.id = "tempForm";
        form.action = url;
        form.style.visibility = 'hidden';

        //this will work even if postData is null, {} or undefined, i.e., void(0)
        for (var x in postData) {
            var el = document.createElement("input");
            el.type = "text";
            el.name = x;
            el.value = escape(postData[x]);
            form.appendChild(el);
        }

        if (isAntiforgery && (method == "post" || method == "POST")) {
            postData = facility.AddAntiForgeryToken(postData);
            var el = document.createElement("input");
            el.setAttribute("type", "hidden");
            el.setAttribute("name", "__RequestVerificationToken");
            el.setAttribute("value", postData["__RequestVerificationToken"]);
            form.appendChild(el);
        }

        document.getElementsByTagName('body')[0].appendChild(form);
        form.submit();
        ////remove form afterwards
        //var element = document.getElementById("tempForm");
        //element.parentNode.removeChild(element);
    }


    facility.GetAjaxRequest = function (url, postdata) {
        //if (!$.isEmptyObject(postdata) && postdata != null && postdata != void(0)){
        //    url += "?";
        //    for (var x in postdata) {
        //        url += x + "=" + postdata[x] + "&";
        //    }
        //    url = url.substr(0, url.length - 1);
        //}
        //url = encodeURIComponent(url);
        return $.ajax({
            method: "GET",
            url: url,
            data: postdata
        })
    };
    
    facility.GetAjaxRequestPost = function (url, postdata, enableCache) {
        return $.ajax({
            method: "POST",
            url: url,
            cache: enableCache,
            data: postdata
        });
    };

        //$.ajax({
        //    method: "GET",
        //    url: url,
        //}).done(function (data, textStatus, jqXHR) {
        //    //console.log("done");
        //    //console.log(JSON.stringify(data));
        //    window.AddTableToDiv("tbTest", data);
        //}).fail(function (jqXHR, textStatus, errorThrown) {
        //}).always(function () {
        //});
        ////we cannot use both then and done
        ////}).then(function (data) {
        ////    console.log("then");
        ////    console.log(JSON.stringify(data));
        ////followint three call back have the same functionalities however less flexibility
        ////}).complete(function () {
        ////    console.log("complete");
        ////}).success(function (data) {
        ////    console.log("success");
        ////}).error(function () {
        ////    console.log("error");
    
    facility.AddAntiForgeryToken = function (data) {
        data.__RequestVerificationToken = $("input[name=__RequestVerificationToken]").val();
        return data;
    };
    ////Private Property
    //var isHot = true;

    ////Public Property
    //skillet.ingredient = "Bacon Strips";

    ////Public Method
    //skillet.fry = function () {
    //    var oliveOil;
    //    addItem("\t\n Butter \n\t");
    //    addItem(oliveOil);
    //    console.log("Frying " + skillet.ingredient);
    //};

    ////Private Method
    //function addItem(item) {
    //    if (item !== undefined) {
    //        console.log("Adding " + $.trim(item));
    //    }
    //}
}(window.facility = window.facility || {}, jQuery));
