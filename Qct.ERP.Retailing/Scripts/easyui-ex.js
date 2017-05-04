﻿$.extend($.fn.datagrid.methods, {
    editCell: function (jq, param) {
        return jq.each(function () {
            var opts = $(this).datagrid('options');
            var fields = $(this).datagrid('getColumnFields', true).concat($(this).datagrid('getColumnFields'));
            for (var i = 0; i < fields.length; i++) {
                var col = $(this).datagrid('getColumnOption', fields[i]);
                col.editor1 = col.editor;
                if (fields[i] != param.field) {
                    col.editor = null;
                }
            }
            $(this).datagrid('beginEdit', param.index);
            var ed = $(this).datagrid('getEditor', param);
            if (ed) {
                if ($(ed.target).hasClass('textbox-f')) {
                    $(ed.target).textbox('textbox').select().focus();
                } else {
                    $(ed.target).select().focus();
                }
            }
            for (var i = 0; i < fields.length; i++) {
                var col = $(this).datagrid('getColumnOption', fields[i]);
                col.editor = col.editor1;
            }
        });
    },
    enableCellEditing: function (jq) {
        return jq.each(function () {
            var grid = $(this);
            var opts = grid.datagrid('options');
            opts.oldOnClickCell = opts.onClickCell;
            opts.onClickCell = function (index, field) {
                if (opts.editIndex != undefined) {
                    if (grid.datagrid('validateRow', opts.editIndex)) {
                        grid.datagrid('endEdit', opts.editIndex);
                        opts.editIndex = undefined;
                    } else {
                        return;
                    }
                }
                grid.datagrid('selectRow', index).datagrid('editCell', {
                    index: index,
                    field: field
                });
                opts.editIndex = index;
                opts.oldOnClickCell.call(this, index, field);
            }
        });
    },
    autoMergeCells: function (jq, fields) {
        return jq.each(function () {
            var target = $(this);
            if (!fields) {
                fields = target.datagrid("getColumnFields");
            }
            var rows = target.datagrid("getRows");
            var i = 0,
			j = 0,
			temp = {};
            for (i; i < rows.length; i++) {
                var row = rows[i];
                j = 0;
                for (j; j < fields.length; j++) {
                    var field = fields[j];
                    var tf = temp[field];
                    if (!tf) {
                        tf = temp[field] = {};
                        tf[row[field]] = [i];
                    } else {
                        var tfv = tf[row[field]];
                        if (tfv) {
                            tfv.push(i);
                        } else {
                            tfv = tf[row[field]] = [i];
                        }
                    }
                }
            }
            $.each(temp, function (field, colunm) {
                $.each(colunm, function () {
                    var group = this;

                    if (group.length > 1) {
                        var before,
						after,
						megerIndex = group[0];
                        for (var i = 0; i < group.length; i++) {
                            before = group[i];
                            after = group[i + 1];
                            if (after && (after - before) == 1) {
                                continue;
                            }
                            var rowspan = before - megerIndex + 1;
                            if (rowspan > 1) {
                                target.datagrid('mergeCells', {
                                    index: megerIndex,
                                    field: field,
                                    rowspan: rowspan
                                });
                            }
                            if (after && (after - before) != 1) {
                                megerIndex = after;
                            }
                        }
                    }
                });
            });
        });
    },
    autoMergeCellsGroupby: function (jq, obj) {//指定合并组
        return jq.each(function () {
            //debugger;
            var groupby = obj.groupby, fields = obj.columns;
            if (!groupby) return;
            var target = $(this);
            if (!fields) {
                fields = target.datagrid("getColumnFields");
            }
            var rows = target.datagrid("getRows");
            if (rows.length <= 0) return;

            var curTxt = "";
            var tempIndex = 1;
            var temp = {};
            for (var i = 0; i < rows.length; i++) {
                var cur = rows[i][groupby];
                var row = temp[cur]||{index:0};
                if (curTxt == "")  
                    curTxt = cur;
                else if (cur == curTxt) {
                    tempIndex++;
                    row.span = tempIndex;
                    temp[cur] = row;
                }
                else {
                    curTxt = cur;
                    tempIndex = 1;
                    row.span = tempIndex;
                    row.index = i;
                    temp[cur] = row;
                }
            }
            $.each(temp, function (k,v) {
                for (var j = 0; j < fields.length; j++) {
                    target.datagrid("mergeCells", {
                        index: v.index,
                        field: fields[j],　　//合并字段
                        rowspan: v.span,
                        colspan: null
                    });
                }
            });
        });
    },
    autoSubTotalGroupby: function (jq, obj) {//分组小计
        return jq.each(function () {
            //debugger;
            var get = function (list, field) {
                for (var i = 0; i < list.length; i++) {
                    if (list[i].field == field)
                        return list[i];
                }
                return null;
            };
            var groupby = obj.groupby, fields = obj.columns;
            if (!groupby || !fields) return;
            var target = $(this);
            var rows = target.datagrid("getRows");
            if (rows.length <= 0) return;
            var curTxt = "";
            var tempIndex = 1;
            var temp = {};
            for (var i = 0; i < rows.length; i++) {
                var cur = rows[i][groupby];
                var row = temp[cur] || { title:cur,index: i, fields: [] };
                row.index = i;
                for (var j = 0; j < fields.length; j++) {
                    var field = { field: fields[j], value: Number(rows[i][fields[j]]) };
                    if (field.value > 0) {//不累加负数
                        var fd = get(row.fields, field.field);
                        if (fd != null) {
                            fd.value += field.value;
                        } else {
                            row.fields.add(field);
                        }
                    }
                }
                temp[cur] = row;
            }
            var n = 1;
            $.each(temp, function (i, r) {
                var row = {};
                row[groupby] ="<b>"+ r.title+"</b>";
                for (var j = 0; j < fields.length; j++) {
                    if (!isNaN(r.fields[j].value))
                        row[fields[j]] = "<b>" +numberToFixed(r.fields[j].value) + "</b>";
                }
                target.datagrid("insertRow", {
                    index: r.index + n,
                    row: row
                }).datagrid("highlightRow", r.index + n).datagrid("getPanel").find(".datagrid-cell-rownumber:eq(" + (r.index + n) + ")").hide();
                n++;
            });
            target.datagrid("getPanel").find(".datagrid-cell-rownumber:visible").each(function (i) { $(this).html(i + 1); });
            var hidsize= target.datagrid("getPanel").find(".datagrid-cell-rownumber:hidden").size();
            var pager = target.datagrid("getPager");
            var tot = pager.pagination("options").total;
            pager.pagination('refresh', { total: tot - hidsize });//重置总条数
        });
    },
    setColumnTitle: function (jq, option) {//动态设置列标题
        if (option.field) {
            return jq.each(function () {
                var $panel = $(this).datagrid("getPanel");
                var $field = $('td[field=' + option.field + ']', $panel);
                if ($field.length) {
                    var $span = $("span", $field).eq(0);
                    $span.html(option.text);
                }
            });
        }
        return jq;
    }

});
var errNum = 0;
function loadError() {
    if (errNum == 3) {
        alert("加载失败,请关闭页面重试!");
        errNum = 0;
    } else {
        $(this).datagrid("reload");
        errNum++;
    }
}
//修改easyui-combobox默认下拉面板属性
$.extend($.fn.combobox.defaults, {
    panelHeight: 'auto',
    panelMaxHeight: 200,
})