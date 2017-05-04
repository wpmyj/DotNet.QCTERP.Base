﻿$.extend($.fn.validatebox.defaults.rules, {   
    idcard: {// 验证身份证 
        validator: function (value) {
            return /^\d{15}(\d{2}[A-Za-z0-9])?$/i.test(value);
        },
        message: '身份证号码格式不正确!'
    },
    integer: {// 验证整数 
        validator: function (value) {
            return /^[+]?[0-9]+\d*$/i.test(value);
        },
        message: '请输入非负整数!'
    },
    intOrFloat: {// 验证整数或小数 
        validator: function (value) {
            return /^\d+(\.\d+)?$/i.test(value);
        },
        message: '请输入数字!'
    },
    lessThanMaxVal: {   //正整数，且小于maxVal
        validator: function (value, maxVal) {
            if (value > maxVal) {
                return false;
            }
            return /^[+]?[1-9]+\d*$/i.test(value);
        },
        message: "请输入小于5000的正整数"
    },
    lessThanMaxVal_Nonnegative: {   //非负整数，且小于maxVal
        validator: function (value, maxVal) {
            if (value > maxVal) {
                return false;
            }
            return /^[+]?[0-9]+\d*$/i.test(value);
        },
        message: "请输入小于5000的非负整数"
    },
    ip:{// 验证IP地址 
        validator: function (value) {
            return /\d+\.\d+\.\d+\.\d+/.test(value)
        },
        message: 'IP地址格式不正确!'
    },
    requiredForCombo: {
        validator: function (text, param) {
            if (param) {
                return $(param[0]).combobox("getValue") != "";
            }
            return !(text == "请选择" || text=="选择列");
        },
        message: '请选择一项!'
    },
    phone: {// 验证电话号码 
        validator: function (value) {
            var result = /^((\(\d{2,3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}(\-\d{1,4})?$/i.test(value);
            if (!result)
                result = /^(13|15|18)\d{9}$/i.test(value);
            return result;
        },
        message: '格式不正确,如:0592-88888888或手机号码!'
    },
    mobile: {// 验证手机号码 
        validator: function (value) {
            //return /^(13|15|18)\d{9}$/i.test(value);
            return value && !isNaN(value) && value.length == 11;
        },
        message: '请输入正确的手机号码!'
    },
	barcode: {// 条码
        validator: function (value) {
	        return /^[a-zA-Z0-9\-]{5,15}$/i.test(value);
        },
        message: '请输入5-15位的英文或数字!'
    },
    barcodejj: {// 计件
        validator: function (value) {
            return /^[0-9](\d{12}|\d{9})$/i.test(value);
        },
        message: '请输入10或13位的数字!'
    },
    barcodecz: {// 称重
        validator: function (value) {
            //return /^[0-9](\d{4}|\d{5}|\d{6}|\d{7}|\d{8}|\d{9}|)$/i.test(value);
            return /^[0-9]{5}$/i.test(value);
        },
        message: '请输入5位的数字!'
    },
    faxno: {// 传真
        validator: function (value) {
            return /^((\d2,3)|(\d{3}\-))?(0\d2,3|0\d{2,3}-)?[1-9]\d{6,7}(\-\d{1,4})?$/i.test(value);
        },
        message: '传真号码不正确'
    },
    zip: {// 邮编
        validator: function (value) {
            return /^[1-9]\d{5}$/i.test(value);
        },
        message: '邮政编码格式不正确'
    },
    greaterDate: { //结束日期>生效日期
        validator: function (value, param) { //参数value为当前文本框的值，也就是EndDate
            var sTime = null;
            try {
                sTime = $(param[0]).datetimebox('getValue');//获取生效日期的值
            } catch (e) {
                sTime = $(param[0]).val();
            }
            var s = $.fn.datebox.defaults.parser(sTime);
            var e = $.fn.datebox.defaults.parser(value);
            return (e > s);
            
        },
        message: '当前日期必须大于指定日期'
    },
    greaterThan: {
        validator: function (value, param) { 
            var sVal = null;
            try {
                sVal = $(param[0]).numberbox('getValue');
            } catch (e) {
                sVal = $(param[0]).val();
            }
            var s =sVal?parseFloat(sVal):0;
            var e =value?parseFloat(value):0;
            return e >=s;
            
        },
        message: '当前值小于指定数值'
    },
    greaterEqualDate: { 
        validator: function (value, param) { //参数value为当前文本框的值，也就是EndDate
            var sTime = null;  
            try {
                sTime = $(param[0]).datetimebox('getValue');//修改界面会异常
            } catch (e) {
                sTime = $(param[0]).val();
            }
            var s = $.fn.datebox.defaults.parser(sTime);
            var e = $.fn.datebox.defaults.parser(value);
            return (e >= s);
        },
        message: '当前日期必须大于或等于指定日期'
    },
    betweenDate: {  
        validator: function (value, param) { //参数value为当前文本框的值
            var sTime = null;
            var eTime = null;  
            try {
                sTime = $(param[0]).datetimebox('getValue'); 
                eTime = $(param[1]).datetimebox('getValue'); 
            } catch (e) {
                sTime = $(param[0]).val();
                eTime = $(param[1]).val();
            }
            var s = $.fn.datebox.defaults.parser(sTime);
            var v = $.fn.datebox.defaults.parser(value); 
            var e;//结束日期可能为空
            if (eTime == "" || eTime == undefined) { 
                return (v >= s);
            }
            else {
                e = $.fn.datebox.defaults.parser(eTime);
                return (v >= s && v <= e);
            }
        },
        message: '当前日期必须介于{0}和{1}之间'
    },
    range: {
        validator: function (value, param) {
            if (!value) return true;
            var val = parseFloat($.trim(value));
            var first = parseFloat($.trim(param[0]));
            var second = parseFloat($.trim(param[1]));
            return (second >= val && val >= first);
        },
        message: "输入值必须介于{0}和{1}之间!"
    },
    account: {  //账号
        validator: function (value,parms) {
            var rules = $.fn.validatebox.defaults.rules;
            rules.account.message = "只能输入邮箱或4-50位的字母、数字";
            if (/^(([A-Za-z0-9]{4,50})|(([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,})))$/i.test(value)) {
                if (!rules.remote.validator(value, parms)) {
                    rules.account.message = "该信息已存在";
                    return false;
                }
                return true;
            }
            return false;
        },
        message: '只能输入邮箱或4-50位的字母、数字'
    },
    loginno: { //登陆名验证
        validator: function (value) {
            return /^[a-zA-Z_\d]{4,20}$/i.test(value);
        },
        message: '登陆帐号只能是4-20位字母、数字、下划线组成'
    },
    equalTo: { //字段相等
        validator: function (value, param) {               
            return $(param[0]).val() == value; 
        },
        message: '字段不匹配'
    },
    noEqualTo: { //字段不相等
        validator: function (value, param) {
            return $(param[0]).val() != value;
        },
        message: '字段不能相等'
    },
    date: { //日期类别
        validator: function (value) {
            return /^\d{4}(\-|\/|\.)\d{1,2}\1\d{1,2}$/i.test(value);
        },
        message: '日期类型不匹配'
    }
});