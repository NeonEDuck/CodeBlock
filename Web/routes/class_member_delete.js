var express = require('express');
var router = express.Router();

//增加引用函式
const class_member = require('./utility/class_member');

//接收POST請求
router.post('/', function(req, res, next) {
    var member_id = req.body.member_id;
    var class_id = req.body.class_id;
    
    // 建立一個新資料物件
    var newData={
        member_id:member_id,
        class_id:class_id
        
    }

    class_member.remove(newData).then(d => {
        if (d>0){
            res.send({message: d});
        }else{
            res.render('addFail');     
        }  
    })
});

module.exports = router;