var express = require('express');
var router = express.Router();

//增加引用函式
const user = require('./utility/user');

//接收POST請求
router.post('/', function(req, res, next) {
    var player_name = req.body.player_name;                 //取得帳號
    var password = req.body.password;     //取得密碼

    user.login(player_name, password).then(d => {
        if (d==null){
            req.session.user = null;
              
            res.render('login_fail');  //傳至登入失敗
        }else{
            req.session.user = d.player_name;
        
            res.render('user_show', {user:d.player_name});   //導向使用者
        }  
    })
});

module.exports = router;