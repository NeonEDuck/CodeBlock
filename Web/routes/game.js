var express = require('express');
var router = express.Router();

/* GET users listing. */
router.get('/', function(req, res, next) {

    json = {};
    json['userId'] = 'U1000001';
    json['hostname'] = req.hostname;

    res.render('game', { title: 'Express', json: JSON.stringify(json) });
});

module.exports = router;
