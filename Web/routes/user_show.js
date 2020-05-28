var express = require('express');
var router = express.Router();

//接收GET請求
router.get('/', function(req, res, next) {
    var user = req.session.user; 

    if(user==null || user==undefined){
        user = '尚未登入';
    }

    res.render('user_show', { user: user });
});

module.exports = router;