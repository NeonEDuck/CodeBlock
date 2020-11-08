'use strict';

//引用操作資料庫的物件
const sql = require('./asyncDB');

//------------------------------------------
//執行資料庫動作的函式-新增產品資料
//------------------------------------------
var login_add = async function(session_user_email,session_user_displayName){
    var result={};
    
    await sql('SELECT * FROM "user" WHERE email = $1', [session_user_email])
        .then((data) => {
            
            if (data.rows.length == 0){
                console.log("==========test=================")
                sql('INSERT INTO "user" (email, user_name) VALUES ($1, $2)', [session_user_email,session_user_displayName])
                .then((data) => {
                    result = 0;  
                }, (error) => {
                    result = -1;
                });
            }
            
            console.log(result)
            
        }, (error) => {
            console.log("===========================")
            result = null;
        })
    
		
    return result;
}

//匯出
module.exports = {login_add};