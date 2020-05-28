'use strict';

//引用操作資料庫的物件
const sql = require('./asyncDB');

//------------------------------------------
//執行資料庫動作的函式-新增產品資料
//------------------------------------------
var register = async function(newData){
    var result;

    console.log(newData);

    await sql('INSERT INTO player (player_name, email, password) VALUES ($1, $2, $3)', [newData.player_name, newData.email, newData.password])
        .then((data) => {
            result = 0;  
        }, (error) => {
            result = -1;
        });
		
    return result;
}

var id_check = async function(player_name){
    var result={};
    await sql('SELECT player_name FROM player WHERE player_name = $1', [player_name])
        .then((data) => {
            if(data.rows.length > 0){
                result = data.rows[0];   
                
            }else{
                result = -1;
                
            }    
        }, (error) => {
            result = null;
        });
		
    return result;
}

var login = async function(player_name, password){   
    var result;

    //取得員工資料
    await sql('SELECT * FROM player WHERE player_name=$1 and password=$2', [player_name, password])
        .then((data) => {
            if(data.rows.length > 0){
                result = data.rows[0];
            }else{
                result = null;
            } 
        }, (error) => {
            result = null;
        });
    
    //回傳物件
    return result;
}

//匯出
module.exports = {register,id_check,login};