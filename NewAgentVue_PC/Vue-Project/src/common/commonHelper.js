
/**
 * [newGuid 生成唯一识别码]
 * @return {[string]} [返回生成的唯一识别码]
 */
export function newGuid() {
	var guid = "";
	for (var i = 0; i <= 32 ; i++) {
		var n = Math.floor(Math.random()*16.0).toString(16);
		guid += n;
	}			
	return guid;
}//newGuid The End	
/**
 * [isEmptyObject 判断一个对象是否是空对象]
 * @param  {[object]}  obj [需要判断的对象]
 * @return {Boolean}     [返回Boolean所判断的对象是否是空对象]
 */
export function isEmptyObject(obj) {
	var t;
	for (t in obj) {
		return !1;
	}
	return !0;
}//isEmptyObject The End
/**
 * [createTime 获取指定格式的当前时间]
 * @param  {[string]} fmt [指定格式]
 * @return {[type]}     [返回指定格式的时间]
 */
export function createTime(fmt) {
	Date.prototype.format = function(fmt) {
		var date = {
			"M+": this.getMonth() + 1,//月份
			"d+": this.getDate(),//日
			"h+": this.getHours(), //小时
            "m+": this.getMinutes(), //分
            "s+": this.getSeconds(), //秒
            "q+": Math.floor((this.getMonth() + 3) / 3), //季度
            "S": this.getMilliseconds() //毫秒
		}//date The End
		if (/(y+)/.test(fmt)) {
            fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        }
        for (var k in o) {
            if (new RegExp("(" + k + ")").test(fmt)) {
                fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
            }
        }
        return fmt;
	}
	return new Date().format(fmt);
}//createTime The End
/**
 * [getDateStr 获取当前时间前后几日的时间]
 * @param  {[int]} AddDayCount [前/后 天数]
 * @return {[type]}             [返回当前时间前后几天的时间]
 */
export function getDateStr(AddDayCount) {
    var dd = new Date();
    dd.setDate(dd.getDate() + AddDayCount); //获取AddDayCount天后的日期  
    var y = dd.getFullYear();
    var m = dd.getMonth() + 1; //获取当前月份的日期 
    var d = dd.getDate();
    m = (m < 10) ? ("0" + m) : m;
    d = (d < 10) ? ("0" + d) : d;
    var Data = y + "-" + m + "-" + d;
    return Data;
}//getDateStr The End
/**
 * [getSetting 读取指定目录下的文件内容]
 * @param  {Function} fn [处理读取到文件内容的函数]
 * @return {[type]}      [description]
 */
export function getSetting(fn) {　　
    var xmlHttp;
    if (window.XMLHttpRequest) {　　　　 // code for IE7+, Firefox, Chrome, Opera, Safari          　　　　
        xmlHttp = new XMLHttpRequest();　　
    } else { // code for IE6, IE5       　　　　
        xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");　　
    }　
    if (xmlHttp == null)　 return;　　
    var url = "static/setting.txt";//指定路径的文件　　
    xmlHttp.open("GET", url, true);　　
    xmlHttp.onreadystatechange = function() { //发送事件后，收到信息了调用函数
        if (xmlHttp.readyState == 4 && xmlHttp.status == 200) fn(xmlHttp.responseText);　
    }　
    xmlHttp.send();
}//getSetting The End
/**
 * [Export 将页面上table里面的内容导出到excel]
 * @param {[type]} tableid [table的ID]
 */
export function Export(tableid) {
    var idTmr;
    //获取当前浏览器类型  
    function getExplorer() {
        var explorer = window.navigator.userAgent;
        if (explorer.indexOf("MSIE") >= 0) return 'ie'; //ie                
        if (explorer.indexOf("Firefox") >= 0) return 'Firefox'; //firefox                             
        if (explorer.indexOf("Chrome") >= 0) return 'Chrome'; //Chrome                  
        if (explorer.indexOf("Opera") >= 0) return 'Opera'; //Opera                     
        if (explorer.indexOf("Safari") >= 0) return 'Safari'; //Safari     
    }
    //获取到类型需要判断当前浏览器需要调用的方法，目前项目中火狐，谷歌，360没有问题  
    //win10自带的IE无法导出  
    function exportExcel(tableid) {
        if (getExplorer() == 'ie') {
            var curTbl = document.getElementById(tableid);
            var oXL = new ActiveXObject("Excel.Application");
            var oWB = oXL.Workbooks.Add();
            var xlsheet = oWB.Worksheets(1);
            var sel = document.body.createTextRange();
            sel.moveToElementText(curTbl);
            sel.select();
            sel.execCommand("Copy");
            xlsheet.Paste();
            oXL.Visible = true;

            try {
                var fname = oXL.Application.GetSaveAsFilename("Excel.xls", "Excel Spreadsheets (*.xls), *.xls");
            } catch (e) {
                print("Nested catch caught " + e);
            } finally {
                oWB.SaveAs(fname);
                oWB.Close(savechanges = false);
                oXL.Quit();
                oXL = null;
                idTmr = window.setInterval("Cleanup();", 1);
            }
        } else {
            tableToExcel(tableid)
        }
    }

    function Cleanup() {
        window.clearInterval(idTmr);
        CollectGarbage();
    }
    //判断浏览器后调用的方法，把table的id传入即可  
    var tableToExcel = (function() {
        var uri = 'data:application/vnd.ms-excel;base64,',
            template = '<html><head><meta charset="UTF-8"></head><body><table>{table}</table></body></html>',
            base64 = function(s) {
                return window.btoa(unescape(encodeURIComponent(s)))
            },
            format = function(s, c) {
                return s.replace(/{(\w+)}/g,
                    function(m, p) {
                        return c[p];
                    })
            }
        return function(table, name) {
            if (!table.nodeType) table = document.getElementById(table)
            var ctx = {
                worksheet: name || 'Worksheet',
                table: table.innerHTML
            }
            window.location.href = uri + base64(format(template, ctx))
        }
    })();
    exportExcel(tableid);
}//Export The End
/**
 * [checkLetters 检查字符合法性，要求字符只能是大小写字母和数字]
 * @param  {[type]} letters [需要检测的字符串]
 * @return {[type]}         [description]
 */
export function checkLetters(letters) {
	var str = letters;
	if(str == null) return false;
	var reg = new RegExp(/^[a-zA-Z\d]+$/); //小写英文 大写英文 所有数字
	if (reg.test(str)) return true;
    return false;
}//checkLetters The End
 /** 
 * 获得相对当前周AddWeekCount个周的起止日期 
 * AddWeekCount为0代表当前周   为-1代表上一个周   为1代表下一个周以此类推
 * **/
export function getWeekStartAndEnd(AddWeekCount) {
    //起止日期数组   
    var startStop = new Array();
    //一天的毫秒数   
    var millisecond = 1000 * 60 * 60 * 24;
    //获取当前时间   
    var currentDate = new Date();
    //相对于当前日期AddWeekCount个周的日期
    currentDate = new Date(currentDate.getTime() + (millisecond * 7 * AddWeekCount));
    //返回date是一周中的某一天
    var week = currentDate.getDay();
    //返回date是一个月中的某一天   
    var month = currentDate.getDate();
    //减去的天数   
    var minusDay = week != 0 ? week - 1 : 6;
    //获得当前周的第一天   
    var currentWeekFirstDay = new Date(currentDate.getTime() - (millisecond * minusDay));
    //获得当前周的最后一天
    var currentWeekLastDay = new Date(currentWeekFirstDay.getTime() + (millisecond * 6));
    //添加至数组   
    startStop.push(this.getSystemTime(currentWeekFirstDay, true));
    startStop.push(this.getSystemTime(currentWeekLastDay, true));

    return startStop;
}
/**
 * [StrToPercent 将小数字符串转换成百分数]
 * @param {[type]} str [description]
 */
export function StrToPercent(str) {
    var num = Number(str);
    num = num > 0 ? num : 0;
    num = parseFloat(num) * 100;
    if (num == 0) return "0%"; // 0
    if ((num % 1) == 0) return num + "%"; //整数直接返回
    num = num.toFixed(2); //保留两位小数
    if (num.substr(num.length - 1) == 0) return num.substring(0, num.length - 1) + "%"; //小数末尾为0
    return num + "%";
}
/**
 * [toThousands 转化成大写金额]
 * @param  {[type]} num [description]
 * @return {[type]}     [description]
 */
export function toChinese(money) {  
    var cnNums = new Array('零', '壹', '贰', '叁', '肆', '伍', '陆', '柒', '捌', '玖'); //汉字的数字 
    var cnIntRadice = new Array('', '拾', '佰', '仟'); //基本单位
    var cnIntUnits = new Array('', '万', '亿', '兆'); //对应整数部分扩展单位
    var cnDecUnits = new Array('角', '分', '毫', '厘'); //对应小数部分单位
    var cnInteger = '整'; //整数金额时后面跟的字符 
    var cnIntLast = '元'; //整型完以后的单位
    var maxNum = 999999999999999.9999; //最大处理的数字        
    var integerNum; //金额整数部分        
    var decimalNum; //金额小数部分      
    var chineseStr = ''; //输出的中文金额字符串
    var parts; //分离金额后用的数组，预定义
    if (money == '') return '';
    money = parseFloat(money);
    if (money >= maxNum) return ''; //超出最大处理数字
    if (money == 0) {
        chineseStr = cnNums[0] + cnIntLast + cnInteger;
        return chineseStr;
    }
    //转换为字符串
    money = money.toString();
    if (money.indexOf('.') == -1) {
        integerNum = money;
        decimalNum = '';
    } else {
        parts = money.split('.');
        integerNum = parts[0];
        decimalNum = parts[1].substr(0, 4);
    }
    //获取整型部分转换
    if (parseInt(integerNum, 10) > 0) {
        var zeroCount = 0;
        var IntLen = integerNum.length;
        for (var i = 0; i < IntLen; i++) {
            var n = integerNum.substr(i, 1);
            var p = IntLen - i - 1;
            var q = p / 4;
            var m = p % 4;
            if (n == '0') {
                zeroCount++;
            } else {
                if (zeroCount > 0) {
                    chineseStr += cnNums[0];
                }
                //归零
                zeroCount = 0;
                chineseStr += cnNums[parseInt(n)] + cnIntRadice[m];
            }
            if (m == 0 && zeroCount < 4) {
                chineseStr += cnIntUnits[q];
            }
        }
        chineseStr += cnIntLast;
    }
    //小数部分
    if (decimalNum != '') {
        var decLen = decimalNum.length;
        for (var i = 0; i < decLen; i++) {
            var n = decimalNum.substr(i, 1);
            if (n != '0') {
                chineseStr += cnNums[Number(n)] + cnDecUnits[i];
            }
        }
    }
    if (chineseStr == '') {
        chineseStr += cnNums[0] + cnIntLast + cnInteger;
    } else if (decimalNum == '') {
        chineseStr += cnInteger;
    }
    return chineseStr;
}
/**
 * [toThousands 给超过3位的数字添加千分位]
 * @param  {[type]} num [description]
 * @return {[type]}     [description]
 */
export function toThousands(num) {
    var num = (num || 0).toString(),
        result = '';
    while (num.length > 3) {
        result = ',' + num.slice(-3) + result;
        num = num.slice(0, num.length - 3);
    }
    if (num) {
        result = num + result;
    }
    return result;
}
/**
 * [groupMsg 组装发送到服务端的报文]
 * @param  {[type]} head    [description]
 * @param  {[type]} request [description]
 * @return {[type]}         [description]
 */
export function groupMsg(head,request) {
    var retMsg ='', headMsg ='',requestMsg ='';
    if(!head){            
        headMsg = '{}';
    }else {
        headMsg = JSON.stringify(head);   
    }
    if(!request) {
        requestMsg = '{}';
    }else {
        requestMsg = JSON.stringify(request);
    }
    return '{"Head":'+headMsg+',"Request":'+requestMsg+"}'"; 
}
/**
 * Deep copy the given object considering circular structure.
 * This function caches all nested objects and its copies.
 * If it detects circular structure, use cached copy to avoid infinite loop.
 *
 * @param {*} obj
 * @param {Array<Object>} cache
 * @return {*}
 */
export function deepCopy (obj, cache = []) {
  // just return if obj is immutable value
  if (obj === null || typeof obj !== 'object') {
    return obj
  }

  // if obj is hit, it is in circular structure
  const hit = find(cache, c => c.original === obj)
  if (hit) {
    return hit.copy
  }

  const copy = Array.isArray(obj) ? [] : {}
  // put the copy into cache at first
  // because we want to refer it in recursive deepCopy
  cache.push({
    original: obj,
    copy
  })

  Object.keys(obj).forEach(key => {
    copy[key] = deepCopy(obj[key], cache)
  })

  return copy
}

