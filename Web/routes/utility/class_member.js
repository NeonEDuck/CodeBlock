'use strict';

//引用操作資料庫的物件
const sql = require('./asyncDB');

//------------------------------------------
//執行資料庫動作的函式-新增產品資料
//------------------------------------------
var search = async function(newData){
    
    var member_name;
    await sql('SELECT member_name  FROM "class_member" WHERE class_id = $1', [newData.class_id])
        .then((data) => {
            console.log(data.rows)  
            member_name = data.rows;  
        }, (error) => {
            result = [];
        });
    var result = {};
    result.member_name = member_name;
    return result;
}
var remove = async function(newData){
    var val;
    console.log("class_member.js")
    console.log(newData)
    await sql('DELETE FROM "class_member" WHERE (class_id = $1) and (member_name = $2)', [newData.class_id,newData.member_id])
        .then((data) => {
            val = data.rowCount;  
            console.log("sql")
            console.log(val)
        }, (error) => {
            val = -1;
        });
		
    return val;
}
//匯出
module.exports = {search,remove};