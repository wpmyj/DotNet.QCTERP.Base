﻿var pharos = pharos || {};
(function (para) {
    para.managertree = {
        columns: [],
        geturl: "",
        editurl: "",
        delurl: "",
        $dg: $("#treegrid"),
        addText: "",
        editText: "",
        delText: "",
        Id: "",
        sortName: "Id",
        sortOrder: "desc",
        treeField:"",
        singleSelect: false,
        checkOnSelect: false,
        pagination:true,
        pageSize: 30,
        pageList: [10,20,30,40,50],
        editurlNocache: function () {
            var url = this.editurl.indexOf("?") == -1 ? this.editurl + "?" : this.editurl + "&";
            url += "t=" + Math.random();
            if (this.Id) url += "&Id=" + this.Id;
            return url;
        },
        loadGrid: function () {
            if(!this.$dg[0]) return;
            $("#frmsearch").keydown(function (e) {
                if (e.keyCode == 13)
                    para.managertree.gridReload();
            });
            this.$dg.treegrid({
                toolbar: '#toolbar',
                url: this.geturl,
                columns: this.columns,
                border: false,
                fit: true,
                rownumbers: true,
                singleSelect: this.singleSelect,
                fitColumns: true,
                striped: true,
                pagination: this.pagination,
                idField: "Id",
                checkOnSelect: false,
                treeField:this.treeField,
                sortName:this.sortName,
                sortOrder: this.sortOrder,
                pageSize: this.pageSize,
                pageList: this.pageList,
                onLoadSuccess: para.managertree.loadSuccess,
                onLoadError:loadError,
                onClickCell: function (title, rowData) {
                    if (editBefore(rowData)) {
                        para.managertree.editItem(rowData.Id);
                    }
                }
            });
        },
        loadSuccess:function(){},
        gridReload:function() {
            var url =this.geturl;
            if (url.indexOf("?") == -1) url += "?";
            this.$dg.treegrid('options').url = url + $('#frmsearch').serialize();
            this.$dg.datagrid("clearChecked").treegrid('reload');
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
            var rows = this.$dg.treegrid('getChecked');
            if (!rows || rows.length == 0) {
                $.messager.alert('提示', '请选择要处理的项');
                return null;
            }
            return rows;
        },
        removeItem:function (Id) {
            var ids = [];
            if (Id) ids.push(Id);
            else {
                var rows = this.$dg.treegrid('getChecked');
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
                    url: para.managertree.delurl,
                    data: { Ids: ids, t: Math.random() },
                    type: "POST",
                    traditional: true, //使用数组
                    dataType: "json",
                    success: function (d) {
                        if (d.successed) {
                            $.messager.alert("提示", "删除成功！", "info");
                            para.managertree.gridReload();
                        } else {
                            $.messager.alert("提示", "删除失败！" + d.message, "error");
                        }
                    },
                    error: function () {
                        $.messager.alert("错误", "删除失败！", "error");
                    }
                });
            });
        }
    };
    
})(pharos);
$(function () {
    pharos.managertree.loadGrid();
});

function removeBefore(row) {
    return true;
}
function editBefore(row) {
    return true;
}