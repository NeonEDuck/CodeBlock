var express = require('express');
var router = express.Router();

//增加引用函式
const class1 = require('./utility/class');

//接收POST請求
router.post('/', function(req, res, next) {
    var class_id = req.body.class_id;   //取得產品編號
   
    class1.remove(class_id).then(d => {
        if(d>=0){
            res.redirect('back');
        }else{
            res.render('removeFail');     //導向錯誤頁面
        }
    })    
});

module.exports = router;