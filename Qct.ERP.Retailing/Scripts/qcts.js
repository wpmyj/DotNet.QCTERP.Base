﻿var pharos = pharos || {};
pharos.dropdown = pharos.dropdown || {};
(function (para) {



    //#region 公共方法

    //正则方法
    para.regex = {
        //用户名应以字母开头，只能含有字母数字下划线，长度大于2位小于10位
        userName: function (userName) {
            return /^[a-zA-Z]{1}[0-9a-zA-Z_]{1,9}$/.test(userName);
        },
        number: function (source) {
            if (/^\d+(\.\d+)?$/.test(source))
                return source;
            return 0;
        }
    }
    //json方法
    para.json = {
        //编辑JSON对象 json：对象 property：属性 value：属性值 
        //如果没有value 则删除属性
        //如果有属性而又要加入 则加入失败 但不会返回错误
        edit: function (json, property, value, isjoin) {
            //如果value被忽略
            if (typeof value == 'undefined') {
                delete json[property];
            }
            //如果不含有属性
            if (!(property in json)) {
                json[property] = value;
                return;
            }
            else {
                if (typeof isjoin == 'undefined') {
                    delete json[property];
                    json[property] = value;
                    return;
                }
                else {
                    json[property] = (json[property] + ',' + value);
                    return;
                }
            }
        },
        //将json转化成string格式
        tostring: function (O) {
            var S = [];
            var J = "";
            if (Object.prototype.toString.apply(O) === '[object Array]') {
                for (var i = 0; i < O.length; i++)
                    S.push(pharos.json.tostring(O[i]));
                J = '[' + S.join(',') + ']';
            }
            else if (Object.prototype.toString.apply(O) === '[object Date]') {
                J = "new Date(" + O.getTime() + ")";
            }
            else if (Object.prototype.toString.apply(O) === '[object RegExp]' || Object.prototype.toString.apply(O) === '[object Function]') {
                J = O.toString();
            }
            else if (Object.prototype.toString.apply(O) === '[object Object]') {
                for (var i in O) {
                    O[i] = typeof (O[i]) == 'string' ? '"' + O[i] + '"' : (typeof (O[i]) === 'object' ? pharos.json.tostring(O[i]) : O[i]);
                    S.push('"' + i.toString() + '":' + (O[i] == null ? 'null' : O[i].toString()));
                }
                J = '{' + S.join(',') + '}';
            }
            return J;
        },
        //将json转化成表单数据传回后台
        //type : post get
        submit: function (json, type, url) {
            $form = $('<form></form>').attr('method', type).attr('action', url);
            for (var i in json) {
                $form.append($("<input name='" + i.toString() + "'/>").val((json[i].toString())));
            }
            $("body").append($form);
            $form.hide();
            $form.submit();
        },
        //如果数据里面为json,返回json.targetProperty==targetValue的json.returnProperty,否则返回nullReturnValue
        getArrayValue: function (array, targetProperty, targetValue, returnProperty, nullReturnValue) {
            for (var i = 0; i < array.length; i++) {
                if (targetProperty in array[i] && array[i][targetProperty] == targetValue) {
                    if (returnProperty in array[i])
                        return array[i][returnProperty];
                }
            }
            return nullReturnValue;
        },
        //将表单转换成json
        formtojson: function (e) {
            var array = e.serializeArray();
            var json = {};
            $.each(array, function (index, value) {
                pharos.json.edit(json, value.name, value.value, true);
            });
            return json;
        }
    }
    //window方法
    para.window = {
        //验证当前页面是否为最上层 如果不是则设置为最上层
        toTop: function () {
            if (window != window.top.window) {
                window.top.location = window.location;
            }
        },
        //取顶层window，如果没有则返回当前window
        topWindow: function () {
            return window.top.window;
        }
    },
        //easyui拓展 使用前必须引入 jquery.easyui.min.js
        para.easyui = {
            //窗口 参数与easyui一致 id,easyui参数
            window: {
                //顶层窗口
                topOpen: function (id, params) {
                    $top = window.top.$;
                    $win = $top("#" + id);
                    if ($win.size() > 0) {
                        $win.remove();
                    }
                    $win = $top("<div id='" + id + "'></div>");
                    $top("body").append($win);
                    $top("body").data(id + '_jquery', $);
                    $win.window(params);
                },
                topClose: function (id) {
                    $top = window.top.$;
                    $win = $top("#" + id);
                    if ($win.length > 0) {
                        $win.window("close");
                    }
                    else {
                        console.log("Cannot find the window.");//找不到窗口
                    }
                },
                curJquery: function (id) {
                    return window.top.$("body").data(id + '_jquery');
                }
            },
            //对话框窗口 参数与easyui一致 id,easyui参数
            dialog: {
                //顶层窗口
                topOpen: function (id, params) {
                    $top = window.top.$;
                    $win = $top("#" + id);
                    if ($win.length > 0) {
                        //$win.parent().remove();
                    }
                    $win = $top("<div id='" + id + "' class='" + id + "'></div>");
                    $top("body").append($win);
                    $top("body").data(id + '_jquery', $);
                    $win.dialog(params);
                },
                topClose: function (id) {
                    $top = window.top.$;
                    $win = $top("." + id).last();
                    if ($win.length > 0) {
                        $win.dialog("close");
                    }
                    else {
                        console.log("Cannot find the window.");
                    }
                },
                curJquery: function (id) {
                    return window.top.$("body").data(id + '_jquery');
                }
            },
            //提示
            alert: function (type, msg) {
                $.messager.alert(type, msg);
            },
            //确认 
            //type 类型
            //msg 确认信息
            //callback 确认后事件
            confirm: function (type, msg, callback) {
                $.messager.confirm(type, msg, function (r) { if (r) { callback(); } });
            }
        },
        //上传方法
        para.upload = {
            //ajaxfileupload拓展 使用前必须引入 ajaxfileupload.js
            ajaxfileupload: {
                //id input的ID (name与id同名)
                //dourl 处理地址 (处理完后只回传json(""))
                //msgurl 数据回传地址 (由于浏览器不同的兼容)
                //callback 从msgurl收到数据后的处理事件 参数为data
                //errorback 回传出错是处理事件
                upload: function (id, dourl, msgurl, callback, errorback) {
                    try {
                        $.ajaxFileUpload({
                            url: dourl,
                            type: 'post',
                            secureuri: false,
                            fileElementId: id,
                            success: function () {
                                $.ajax({
                                    url: msgurl,
                                    type: 'post',
                                    dataType: 'json',
                                    aysnc: false,
                                    success: function (data) {
                                        callback(data);
                                    }
                                });
                            },
                            error: function (e) {
                                if (typeof errorback == 'undefined') {
                                }
                                else {
                                    errorback(e);
                                }
                            }
                        });
                    }
                    catch (e) {
                        console.log("Please refer to the file:ajaxfileupload.js.");//请先引用ajaxfileupload.js
                    }
                }
            }
        },
        para.getQueryString = function (name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]); return null;
        },
        para.deepCopy = function (source) {
            var result = {};
            for (var key in source) {
                result[key] = typeof source[key] === 'object' ? this.deepCopy(source[key]) : source[key];
            }
            return result;
        },
        para.disabledMenu = function () {
            document.onmousedown = function (e) {
                if (window.Event) {
                    if (e.which == 2 || e.which == 3)
                        return false;
                } else if (event.button == 2 || event.button == 3) {
                    event.cancelBubble = true
                    event.returnValue = false;
                    return false;
                }
            }
            document.oncontextmenu = function () { event.returnValue = false }
        },
        //#endregion
        //预览图片
        para.previewFile = function (file, mult) {
            mult = mult || false;
            var url = para.imgUrl(file);
            var agent = navigator.userAgent;
            var item = file.files.item(0);
            if (item.size > 4194304) {
                $.messager.alert("提示", "目前只支持上传不能大于4M文件!"); return;
            }
            var html = "";
            if (item.name.indexOf("jpg") != -1 || item.name.indexOf("png") != -1 || item.name.indexOf("bmp") != -1 || item.name.indexOf("gif") != -1) {
                html += "<img style='height:100px;border:0px;margin-right:5px;' src='{0}' title='" + item.name + "' />";
            } else {
                html += "<label style='margin-right:5px;'>{0}</label>";
                url = item.name;
            }
            html += "<img src='{1}Content/image/close.png' width='12' height='12' class='remove-file' onclick='$(this).prev().remove(); $(this).next().remove(); $(this).remove()'/>";
            html = $.format(html, url, root);
            var objfile = $(file);
            var copyFile = objfile.clone(true);
            var id = objfile.attr("data-target");
            if (!id) {
                $.messager.alert("提示", "请配置data-target属性标记,以指定内容块ID!"); return;
            }
            var block = $("#" + id);
            if (!block[0]) {
                objfile.parent().append($("<div id='" + id + "' style='width:100%;height:100px;border:solid 0px;position:relative;overflow:auto;' />").append(html).append(objfile.hide())).append(copyFile);
            } else {
                if (mult == true || block.find("input").size() <= 0) {
                    block.append(html).append(objfile.hide()).parent().append(copyFile);
                } else {
                    block.empty().append(html).append(objfile.hide()).parent().append(copyFile);
                }
            }
        },
        para.imgUrl = function (file) {
            var url = "";
            var agent = navigator.userAgent;
            if (!!window.ActiveXObject || "ActiveXObject" in window) {
                //url = file.value;
                url = window.URL.createObjectURL(file.files.item(0));
            } else if (agent.indexOf("Firefox") > 0) {
                url = window.URL.createObjectURL(file.files.item(0));
            } else if (agent.indexOf("Chrome") > 0) {
                url = window.URL.createObjectURL(file.files.item(0));
            }
            return url;
        }
})(pharos);

function openDialog420(title, url, showAdd) {
    openDialogNew(title, url, 420, 230, false, !showAdd);
}
function openDialog600(title, url, showAdd) {
    openDialogNew(title, url, 600, 370, false, !showAdd);
}
function openDialog800(title, url, showAdd) {
    openDialogNew(title, url, 800, 450, false, !showAdd);
}
function openDialog1000(title, url, showAdd) {
    openDialogNew(title, url, 1000, 560, false, !showAdd);
}
function openDialogMax(title, url, showAdd) {
    var w = $(window.parent).width() - 80;
    var h = $(window.parent).height() - 80;
    openDialogNew(title, url, w, h, false, !showAdd);
}
function openDialogNew(title, url, width, height, hideSave, hideAdd, buttons) {
    /// <summary>打开窗体</summary>
    ///<param name="title" type="String">标题</param>
    ///<param name="url" type="String">Url地址</param>
    ///<param name="width" type="Int">宽度</param>
    ///<param name="height" type="Int">高度</param>
    ///<param name="hideSave" type="Bool">是否隐藏保存按钮</param>
    ///<param name="hideAdd" type="Bool">是否隐藏保存并新增按钮</param>
    ///<param name="buttons" type="Object">附加按钮对象或数组</param>
    if (!width) width = 600;
    if (!height) height = 350;
    var btns = [];
    var btnadd = {
        text: '保存并新增',
        iconCls: 'icon-ok',
        id: "lbadd",
        width: 120,
        handler: function () {
            var form = window.top.$('.formDiv:last iframe')[0].contentWindow.$('.default-form form');
            if ($("#hidadd", form).size() <= 0)
                form.append("<input type='hidden' id='hidadd' name='hidadd' value='1'/>")
            form.submit();
        }
    };
    var btnsave = {
        text: '保存',
        iconCls: 'icon-ok',
        id: "lbsave",
        handler: function () {
            window.top.$('.formDiv:last iframe')[0].contentWindow.$('.messager-button .l-btn-small').click();
            window.top.$('.formDiv:last iframe')[0].contentWindow.$('.default-form form').submit();
        }
    };
    var btnclose = {
        text: '关闭',
        id: "lbclose",
        iconCls: 'icon-cancel',
        handler: function () { pharos.easyui.dialog.topClose("formDiv"); }
    };
    if (buttons) {
        if (isArray(buttons)) {
            for (var i = 0; i < buttons.length; i++) {
                btns.push(buttons[i]);
            }
        } else
            btns.push(buttons);
    }
    if (!hideAdd) btns.push(btnadd);
    if (!hideSave) btns.push(btnsave);
    btns.push(btnclose);
    url = url.indexOf("?") == -1 ? url + "?" : url + "&";
    url += "t=" + Math.random();
    var cont = "<iframe src='" + url + "' width='100%' height='99%' frameborder='0' />";
    $("body").openTopDialog({
        id: "formDiv",
        title: title,
        width: width,
        height: height,
        content: cont,
        //href: url,
        modal: true,
        cache: false,
        buttons: btns,
        tools: dialogTools(),
        onClose: function () {
            $(this).parent().nextAll(".window-shadow,.window-mask").remove().end().remove();
            closeDialog();
        },
        onBeforeOpen: function () {
            //$(this).parent().addClass("animated fadeIn");
        }
    });
}
function openDialog(title, url, width, height, hideSave, buttons) {
    /// <summary>打开窗体</summary>
    ///<param name="title" type="String">标题</param>
    ///<param name="url" type="String">Url地址</param>
    ///<param name="width" type="Int">宽度</param>
    ///<param name="height" type="Int">高度</param>
    ///<param name="hideSave" type="String">是否隐藏保存按钮</param>
    ///<param name="buttons" type="Object">附加按钮对象或数组</param>
    if (!width) width = 600;
    if (!height) height = 350;
    var btns = [];
    var btnsave = {
        text: '保存',
        iconCls: 'icon-ok',
        id: "lbsave",
        handler: function () {
            window.top.$('.formDiv:last iframe')[0].contentWindow.$('.messager-button .l-btn-small').click();
            window.top.$('.formDiv:last iframe')[0].contentWindow.$('.default-form form').submit();
        }
    };
    var btnclose = {
        text: '关闭',
        iconCls: 'icon-cancel',
        id: "lbclose",
        handler: function () { pharos.easyui.dialog.topClose("formDiv"); }
    };
    if (buttons) {
        if (isArray(buttons)) {
            for (var i = 0; i < buttons.length; i++) {
                btns.push(buttons[i]);
            }
        } else
            btns.push(buttons);
    }
    if (!hideSave) btns.push(btnsave);
    btns.push(btnclose);
    url = url.indexOf("?") == -1 ? url + "?" : url + "&";
    url += "t=" + Math.random();
    var cont = "<iframe src='" + url + "' width='100%' height='99%' frameborder='0' />";
    $("body").openTopDialog({
        id: "formDiv",
        title: title,
        width: width,
        height: height,
        content: cont,
        //href: url,
        modal: true,
        cache: false,
        buttons: btns,
        tools: dialogTools(),
        onClose: function () {
            $(this).parent().nextAll(".window-shadow,.window-mask").remove().end().remove();
            closeDialog();
        }
    });
}

function closeDialog() {
    //if (window.top.$('#formDiv iframe')[0].contentWindow.$('#hidadd').val() == "1") {
    //if (pharos.easyui.dialog.curJquery("formDiv")("#grid")[0]) {
    //    pharos.easyui.dialog.curJquery("formDiv")("#grid").datagrid("reload");
    //}
    //if (pharos.easyui.dialog.curJquery("formDiv")("#treegrid")[0]) {
    //    pharos.easyui.dialog.curJquery("formDiv")("#treegrid").treegrid("reload");
    //}
    //}
}
function dialogTools() {
    return [];
}
function isArray(object) {
    return object && typeof object === 'object' &&
        Array == object.constructor;
}
/*
作用：弹出子窗口
示例：
openWin({'title':'标题', 'width':'宽', 'height':'高', 'hideSave':true|false, 'url':'子页面URL',buttons:[]});
*/
function openWin(parms) {
    if (parms != null) {
        if (!parms.width) { parms.width = 600; }
        if (!parms.height) { parms.height = 350; }
        var btns = [{
            text: '保存',
            iconCls: 'icon-ok',
            id: "lbsave",
            handler: function () {
                window.top.$('.formDiv:last iframe')[0].contentWindow.$('.default-form form').submit();
            }
        }, {
            text: '关闭',
            iconCls: 'icon-cancel',
            handler: function () { pharos.easyui.dialog.topClose("formDiv"); }
        }];
        if (parms.hideSave == true) { btns.shift(); }
        if (parms.buttons) btns = parms.buttons;
        var cont = "<iframe src='" + parms.url + "' width='100%' height='99%' frameborder='0'/>";
        $("body").openTopDialog({
            id: parms.id || "formDiv",
            title: parms.title,
            width: parms.width,
            height: parms.height,
            content: cont,
            //href: url,
            modal: true,
            cache: false,
            buttons: btns,
            tools: dialogTools(),
            onClose: function () {
                $(this).parent().nextAll(".window-shadow,.window-mask").remove().end().remove();
                closeDialog();
            }
        });
    }
}
Array.prototype.add = function (item) {
    ///<summary>数组添加</summary>
    ///<param name="item" type="String">添加项</param>
    if (this.indexOf(item) != -1) return false;
    this.push(item);
    return true;
}
Array.prototype.addRange = function (items) {
    if (!items || items.length <= 0) return false;
    for (var i = 0; i < items.length; i++) {
        this.add(items[i]);
    }
    return true;
}
Array.prototype.remove = function (item) {
    ///<summary>数组删除</summary>
    ///<param name="item" type="String">删除项</param>
    if (this.length <= 0) return false;
    for (var i = 0; i < this.length; i++) {
        if (this[i] == item)
            this.splice(i, 1);
    }
    return true;
}
Array.prototype.removeRange = function (items) {
    if (!items || items.length <= 0) return false;
    for (var i = 0; i < items.length; i++) {
        this.remove(items[i]);
    }
    return true;
}
Array.prototype.sum = function () {
    var total = 0;
    if (this.length <= 0) return total;
    for (var i = 0; i < this.length; i++) {
        total += Number(this[i]);
    }
    return total;
}
// Extend the default Number object with a formatMoney() method:
// usage: someVar.formatMoney(symbol, precision, thousandsSeparator, decimalSeparator)
// defaults: ("$", 2, ",", ".")
Number.prototype.formatMoney = function (symbol, precision, thousandsSeparator, decimalSeparator) {
    precision = !isNaN(precision = Math.abs(precision)) ? precision : 2;
    symbol = symbol !== undefined ? symbol : "￥";
    thousandsSeparator = thousandsSeparator || ",";
    decimalSeparator = decimalSeparator || ".";
    var number = this,
        negative = number < 0 ? "-" : "",
        i = parseInt(number = Math.abs(+number || 0).toFixed(precision), 10) + "",
        j = (j = i.length) > 3 ? j % 3 : 0;
    return symbol + negative + (j ? i.substr(0, j) + thousandsSeparator : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousandsSeparator) + (precision ? decimalSeparator + Math.abs(number - i).toFixed(precision).slice(2) : "");
};

/*Date========================================*/
// 对Date的扩展，将 Date 转化为指定格式的String   
// 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符，   
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字)   
// 例子：   
// (new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423   
// (new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18   
Date.prototype.format = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1,                 //月份   
        "d+": this.getDate(),                    //日   
        "h+": this.getHours(),                   //小时   
        "m+": this.getMinutes(),                 //分   
        "s+": this.getSeconds(),                 //秒   
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度   
        "S": this.getMilliseconds()             //毫秒   
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
};
function numberToFixed(value, precision) {
    if (!isNaN(value)) {
        var val = value + "";
        precision = precision || 2;
        if (typeof (precision) == "object") precision = 2;
        return val.indexOf(".") != -1 ? Number(value).toFixed(precision) : value;
    }
    return 0;
}
function trimBlank() {
    $("form").find("input[name]").each(function (i, r) {
        if (r.type == "hidden" || r.type == "text")
            $(r).val($.trim($(r).val()));
    });
}