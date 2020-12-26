'use strict';

//-----------------------
// 引用資料庫模組
//-----------------------
const {Client} = require('pg');

//-----------------------
// 自己的資料庫連結位址
//-----------------------
var pgConn = process.env.DB_CONN;
var ssl = true;

if ( process.env.USE_LOCAL !== undefined && process.env.USE_LOCAL === "TRUE"  ) {
	var pass = process.env.DB_CONN.split(':')[2].split("@")[0]
	
	pgConn = "postgres://postgres:" + pass + "@localhost:5432/postgres";
	ssl = false
}

process.env['NODE_TLS_REJECT_UNAUTHORIZED'] = 0;

//產生可同步執行sql的函式
function query(sql, value=null) {
    return new Promise((resolve, reject) => {
        //產生資料庫連線物件
        var client = new Client({
            connectionString: pgConn,
            ssl: ssl
        })     

        //連結資料庫
        client.connect();

        //執行並回覆結果  
        client.query(sql, value, (err, results) => {                   
            if (err){
                reject(err);
            }else{
                resolve(results);
            }

            //關閉連線
            client.end();
        });
    });
}

//匯出
module.exports = query;