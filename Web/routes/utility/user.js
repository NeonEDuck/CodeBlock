'use strict';

//引用操作資料庫的物件
const sql = require('./asyncDB');

//------------------------------------------
//執行資料庫動作的函式-新增產品資料
//------------------------------------------
var login_add = async function(newData){
    var result={};
    
    await sql('SELECT * FROM "user" WHERE email = $1', [newData.email])
        .then((data) => {
            
            if (data.rows.length == 0){
                sql('INSERT INTO "user" (email, user_name) VALUES ($1, $2)', [newData.email,newData.displayName])
                .then((data) => {
                    result = 0;  
                }, (error) => {
                    result = -1;
                });
            }
            
        }, (error) => {
            result = null;
        })
    
    return result;
}

//匯出
module.exports = {login_add};