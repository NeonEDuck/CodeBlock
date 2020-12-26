var express = require('express');
var router = express.Router();

//增加引用函式
const class_member = require('./utility/class_member');

//接收POST請求
router.post('/', function(req, res, next) {
    var class_id = req.body.class_id;                  //取得課堂ID
    
    // 建立一個新資料物件
    var newData={
        class_id: class_id
    }
    
    class_member.search(newData).then(d => {
        if (d!=[]){
            res.send(d.member_data);  
        }else{
            res.render('addFail');     
        }  
    })
});

module.exports = router;