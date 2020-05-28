require('dotenv').config();

var createError = require('http-errors');
var express = require('express');
var path = require('path');
var cookieParser = require('cookie-parser');
var logger = require('morgan');
//------------------------------------------------------------
// 增加引用模組
//------------------------------------------------------------
var indexRouter = require('./routes/index');
var usersRouter = require('./routes/users');
var gameRouter = require('./routes/game');
var user_login_form = require('./routes/user_login_form');
var user_register_form = require('./routes/user_register_form');
var user_register = require('./routes/user_register');
var user_register_sucess = require('./routes/user_register_sucess');
var user_register_fail = require('./routes/user_register_fail');
var user_register_fail_error = require('./routes/user_register_fail_error');


var login_fail = require('./routes/login_fail');
var user_login = require('./routes/user_login');
var user_logout = require('./routes/user_logout');
var user_show = require('./routes/user_show');

var app = express();
//--------------------------------------------------------------------
// 增加引用express-session
//--------------------------------------------------------------------
var session = require('express-session');
app.use(session({secret: 'asd', cookie: { maxAge: 60000 }}));
//--------------------------------------------------------------------

// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'ejs');

app.use(logger('dev'));
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, 'public')));

//-----------------------------------------
// 設定模組使用方式
//-----------------------------------------
app.use('/', indexRouter);
app.use('/users', usersRouter);
app.use('/user_login_form', user_login_form);
app.use('/user_register_form', user_register_form);
app.use('/user_register', user_register);
app.use('/user_register_sucess', user_register_sucess);
app.use('/user_register_fail', user_register_fail);
app.use('/user_register_fail_error', user_register_fail_error);

app.use('/user/login_fail', login_fail);
app.use('/user/login', user_login);
app.use('/user/logout', user_logout);
app.use('/user/show', user_show);

// catch 404 and forward to error handler
app.use(function(req, res, next) {
  next(createError(404));
});

// error handler
app.use(function(err, req, res, next) {
  // set locals, only providing error in development
  res.locals.message = err.message;
  res.locals.error = req.app.get('env') === 'development' ? err : {};

  // render the error page
  res.status(err.status || 500);
  res.render('error');
});

module.exports = app;
