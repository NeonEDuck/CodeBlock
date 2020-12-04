var express = require('express');
var router = express.Router();

//增加引用函式
const sql = require('./utility/sql');

router.post('/', function(req, res, next) {
    var stmt = req.body.stmt;
    // var json = req.query.json;
    // console.log( json + 'PING!' );

    
    sql.request( stmt ).then(d => {
        res.send( d );
    });
    
    // res.render('blank', { message: json });
    // json = (1000 - json).toString();
});

module.exports = router;
