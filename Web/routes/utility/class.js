'use strict';

//引用操作資料庫的物件
const sql = require('./asyncDB');

//------------------------------------------
//執行資料庫動作的函式-新增班級資訊
//------------------------------------------
var add = async function(newData){
    var result;
    var data =(newData.topics).join();
    // to = string.Join(',', newData.topics)

    console.log(data)

    await sql('INSERT INTO "class" (class_id, school, name, email, max_number, topics) VALUES ($1, $2, $3, $4, $5, $6)', [newData.class_id, newData.school, newData.name, newData.email, newData.max_number, data])
        .then((data) => {
            console.log("sucess")
            result = 0;  
        }, (error) => {
            result = -1;
        });
		
    return result;
}
//------------------------------------------
//執行資料庫動作的函式-傳回所有班集資料
//------------------------------------------
var list = async function(session_user_email){
    var result=[];
	
    await sql('SELECT * FROM "class" WHERE email = $1', [session_user_email])
        .then((data) => {            
            result = data.rows;  
        }, (error) => {
            result = null;
        });
		
    return result;
}
//----------------------------------
// 刪除
//----------------------------------
var remove = async function(class_id){
    var result;

    await sql('DELETE FROM "class" WHERE class_id = $1', [class_id])
        .then((data) => {
            result = data.rowCount;  
        }, (error) => {
            result = -1;
        });
		
    return result;
}

var type = async function(){
    var result=[];
	
    await sql('SELECT * FROM "topic"')
        .then((data) => {            
            result = data.rows;  
        }, (error) => {
            result = null;
        });
		
    return result;
}

//匯出
module.exports = {add,list,remove,type};