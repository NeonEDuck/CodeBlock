var express = require('express');
var router = express.Router();

/* GET users listing. */
router.get('/', function(req, res, next) {
    var json = req.query.json;
    console.log( json + 'PING!' );
    
    // res.render('blank', { message: json });
    json = (1000 - json).toString();
    res.send( json );
});

module.exports = router;
