'use strict';

//引用操作資料庫的物件
const sql = require('./asyncDB');

//------------------------------------------
//執行資料庫動作的函式-新增產品資料
//------------------------------------------
var search = async function(newData){
    
    var member_data;
    await sql('SELECT member_name,pin  FROM "class_member" WHERE class_id = $1', [newData.class_id])
        .then((data) => {
            member_data = data.rows;  
        }, (error) => {
            result = [];
        });
    var result = {};
    result.member_data = member_data;
    return result;
}
var remove = async function(newData){
    var val;
    await sql('DELETE FROM "class_member" WHERE (class_id = $1) and (member_name = $2)', [newData.class_id,newData.member_id])
        .then((data) => {
            val = data.rowCount;
        }, (error) => {
            val = -1;
        });
		
    return val;
}
//匯出
module.exports = {search,remove};