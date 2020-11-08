var express = require('express');
var router = express.Router();

//增加引用函式
const class_member = require('./utility/class_member');

//接收POST請求
router.post('/', function(req, res, next) {
    console.log("HI")
    var member_id = req.body.member_id;
    var class_id = req.body.class_id;    
    console.log(member_id)
    console.log(class_id)
    
    // 建立一個新資料物件
    var newData={
        member_id:member_id,
        class_id:class_id
        
    } 
    console.log(newData)
    class_member.remove(newData).then(d => {
        if (d>0){
            console.log(d)
            res.send({message: d});
            
            //console.log(d);
        }else{
            res.render('addFail');     
        }  
    })
});

module.exports = router;