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
var loginRouter = require('./routes/login');
var user_register_form = require('./routes/user_register_form');
var user_register = require('./routes/user_register');
var user_register_sucess = require('./routes/user_register_sucess');
var user_register_fail = require('./routes/user_register_fail');
var user_register_fail_error = require('./routes/user_register_fail_error');

var app = express();

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
app.use('/login', loginRouter);
app.use('/user_register_form', user_register_form);
app.use('/user_register', user_register);
app.use('/user_register_sucess', user_register_sucess);
app.use('/user_register_fail', user_register_fail);
app.use('/user_register_fail_error', user_register_fail_error);

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
