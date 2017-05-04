﻿var pharos = pharos || {};
(function (para) {
    para.manager = {
        columns: [],
        frozenColumns: [],
        detailurl: "",
        geturl: "",
        editurl: "",
        delurl: "",
        $dg: $("#grid"),
        detailText: "查看详情",
        addText: "",
        editText: "",
        delText: "",
        Id: "",
        sortName: "Id",
        sortOrder: "desc",
        singleSelect: false,
        pageSize: 20,
        pageList: [10, 20, 30, 40, 50],
        pagination: true,
        showFooter:false,
        data:[],
        editurlNocache: function () {
            var url = this.editurl.indexOf("?") == -1 ? this.editurl + "?" : this.editurl + "&";
            url += "t=" + Math.random();
            if (this.Id) url += "&Id=" + this.Id;
            return url;
        },
        getUrlAndParm:function(){
            var url = this.geturl;
            if (!url) return "";
            if (url.indexOf("?") == -1) url += "?";
            return url + $('#frmsearch').serialize();
        },
        loadGrid: function () {
            if (!this.$dg[0]) return;
            $("#frmsearch").keydown(function (e) {
                if (e.keyCode == 13)
                    para.manager.gridReload();
            });
            this.$dg.datagrid({
                toolbar: '#toolbar',
                url: this.getUrlAndParm(),
                data:this.data,
                columns: this.columns,
                frozenColumns: this.frozenColumns,
                border: false,
                fit: true,
                rownumbers: true,
                singleSelect: this.singleSelect,
                fitColumns: false,
                striped: true,
                nowrap:false,
                pagination: this.pagination,
                idField: "Id",
                checkOnSelect:false,
                sortName:this.sortName,
                sortOrder:this.sortOrder,
                onLoadSuccess: para.manager.loadSuccess,
                onLoadError: loadError,
                pageSize: this.pageSize,
                pageList: this.pageList,
                showFooter:this.showFooter,
                onClickCell: function (index, field, value) {
                    var rowDatas = para.manager.$dg.datagrid("getRows");
                    var rowData = rowDatas[index];
                    if (editBefore(rowData, field, index, value)) {
                        para.manager.editItem(rowData.Id, rowData,field, index, value);
                    }
                }
            });
        },
        loadSuccess: function () {},
        gridReload: function () {
            trimBlank();
            var url =this.geturl;
            if (url.indexOf("?") == -1) url += "?";
            this.$dg.datagrid('options').url = url + $('#frmsearch').serialize();
            this.$dg.datagrid("clearChecked").datagrid('reload');
            this.clearSearch();
        },
        clearSearch:function() {
            //$('#frmsearch').form("clear");
        },
        addItem: function () {
            this.Id = "";
            openDialog600(this.addText, this.editurlNocache());
        },
        editItem: function (Id) {
            this.Id = Id;
            openDialog600(this.editText,this.editurlNocache());
        },
        selectItems:function(){
            var rows = this.$dg.datagrid('getChecked');
            if (!rows || rows.length == 0) {
                $.messager.alert('提示', '请选择要处理的项');
                return null;
            }
            if (!selectBefore(rows)) return null;
            return rows;
        },
        removeItem:function (Id) {
            var ids = [];
            if (Id) ids.push(Id);
            else {
                var rows = this.$dg.datagrid('getChecked');
                if (!rows || rows.length == 0) {
                    $.messager.alert('提示', '请选择要删除的项');
                    return;
                }
                var result = true;
                $.each(rows, function (i, r) {
                    if (!removeBefore(r)) {
                        result = false; return false;
                    }
                    ids.push(r.Id);
                });
                if (!result) return;
            }
            $.messager.confirm('提示', "是否确定删除该项信息?", function (r) {
                if (!r) {
                    return r;
                }
                $.ajax({
                    url: para.manager.delurl,
                    data: { Ids: ids, t: Math.random() },
                    type: "POST",
                    traditional: true, //使用数组
                    dataType: "json",
                    success: function (d) {
                        if (d.successed) {
                            $.messager.alert("提示", "删除成功！", "info");
                            para.manager.gridReload();
                        } else {
                            $.messager.alert("提示", "删除失败！" + d.message, "error");
                        }
                    },
                    error: function () {
                        $.messager.alert("错误", "删除失败！", "error");
                    }
                });
            });
        },
        hideToolbar: function () {
            this.$dg.datagrid({ toolbar: '' });
            $("#toolbar").hide();
        }
    };
    
})(pharos);
$(function () {
    pharos.manager.loadGrid(); 
    if (pharos.manager.columns.length > 0) {
        var width = 0;
        var cols = [];
        $.each(pharos.manager.columns,function (i, r) {
            $.each(r,function (j, o) {
                cols.add(o);
            });
        });
        $.each(pharos.manager.frozenColumns, function (i, r) {
            $.each(r, function (j, o) {
                cols.add(o);
            });
        });
        $.each(cols, function (i, item) {
            if (item.width) width += item.width;
        });
        if (width < $(window).width()) {//如果总宽度比屏幕小，则自动拉长列
            pharos.manager.$dg.datagrid("options").fitColumns = true;
        }
    }
});

function removeBefore(row) {
    return true;
}
function editBefore(row) {
    return true;
}
function selectBefore(rows) {
    return true;
}