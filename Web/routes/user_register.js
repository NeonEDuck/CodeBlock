var express = require('express');
var router = express.Router();

//增加引用函式
const user = require('./utility/user');

//接收POST請求
router.post('/', function(req, res, next) {
    var player_name = req.body.player_name;      //取得玩家ID
    var email = req.body.email;              //取得玩家Email
    var password = req.body.password;        //取得玩家密碼
    var check = 0;



    user.id_check(player_name).then(data => {
        if (data==-1){
            var newData={
                player_name:player_name,
                email:email,
                password:password,
            } 
            user.register(newData).then(d => {
                if (d==0){
                    res.render('user_register_sucess');  //傳至成功頁面
                }else{
                    res.render('user_register_fail_error');//資料庫寫入異常
                }
            })
        }else{
            res.render('user_register_fail'); //帳號已存在         
        }
    })
    
    // 建立一個新資料物件
    
    // if(check==1){
        
    // }else{
    //         //導向錯誤頁面
    // }
    // user.register(newData).then(d => {
        
    //     if (d==0 && check==0){
    //         res.render('user_register_sucess');  //傳至成功頁面
    //     }else{
    //         res.render('user_register_fail');     //導向錯誤頁面
    //     }  
    // })
});

module.exports = router;