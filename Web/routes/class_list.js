var express = require('express');
var router = express.Router();

//增加引用函式
const class1 = require('./utility/class');

//接收GET請求
router.get('/', function(req, res, next) {
    console.log("EMAIL")
    console.log(req.session.passport.user.emails[0].value)
    class1.list(req.session.passport.user.emails[0].value).then(data => {
        console.log("OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO")
        console.log(data)
        console.log("OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO")
        if(data==null){
            res.render('error');  //導向錯誤頁面
        }else {
            res.render('class', {items:data});  //將資料傳給顯示頁面
        }
      })
});

module.exports = router;