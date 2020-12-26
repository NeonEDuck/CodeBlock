var express = require('express');
const user = require('./utility/user');
var router = express.Router();

/* GET users listing. */
router.get('/', function(req, res, next) {
  
  var newData = {
    email: req.session.passport.user.emails[0].value,
    displayName: req.session.passport.user.displayName
  }

  user.login_add(newData)

  res.render('user', {
    session_user_id: req.session.passport.user.id ,
    session_user_displayName: req.session.passport.user.displayName,
    session_user_email: req.session.passport.user.emails[0].value,
    session_user_photo: req.session.passport.user.photos[0].value
  });
  
});

module.exports = router;
