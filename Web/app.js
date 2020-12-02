
require('dotenv').config();
var createError = require('http-errors');
var express = require('express');
var path = require('path');
const fs = require('fs')
var cookieParser = require('cookie-parser');
var logger = require('morgan');
// var session = require('express-session');

var indexRouter = require('./routes/index');
var userRouter = require('./routes/user');
var classRouter = require('./routes/class');
var checkAuth = require('./routes/checkAuth');
var class_add = require('./routes/class_add');
var class_list = require('./routes/class_list');
var class_remove = require('./routes/class_remove');
var class_member_search = require('./routes/class_member_search');
var class_member_delete = require('./routes/class_member_delete');
var sqlRouter = require('./routes/sql');
var gameRouter = require('./routes/game');
var app = express();

//---------------------------------------------
// 使用passport-google-oauth2套件進行認證
//---------------------------------------------
var passport = require('passport');

app.use(require('express-session')({
    secret: 'keyboard cat',
    resave: true,
    saveUninitialized: true
}));

app.use(passport.initialize());
app.use(passport.session());

passport.serializeUser(function(user, done) {
    done(null, user);
});

passport.deserializeUser(function(user, done) {
    done(null, user);
});

//載入google oauth2
var GoogleStrategy = require('passport-google-oauth20').Strategy;

console.log(process.env.DB_CONN)
console.log(process.env.CLIENT_ID)
console.log(process.env.CLIENT_SECRET)
//填入自己在google cloud platform建立的憑證
passport.use(
    new GoogleStrategy({
        clientID: process.env.CLIENT_ID, 
        clientSecret: process.env.CLIENT_SECRET,
        callbackURL: "http://localhost:80/auth/google/callback"
    },
    function(accessToken, refreshToken, profile, done) {
        if (profile) {
            return done(null, profile);
        }else {
            return done(null, false);
        }
    }
));
//---------------------------------------------

// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'ejs');

app.use(logger('dev'));
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, 'public'), {
    setHeaders: function(res, path) {
        if(path.endsWith(".unityweb")){
            res.set("Content-Encoding", "gzip");
        }
    }
}));

app.use('/', indexRouter);
app.use('/index', indexRouter);
app.use('/user', checkAuth,userRouter);
app.use('/class', checkAuth,class_list);
app.use('/class/add',class_add);
app.use('/class/remove',class_remove);
app.use('/class_member_search',class_member_search);
app.use('/class_member_delete',class_member_delete);
app.use('/sql', sqlRouter);
app.use('/game',gameRouter);

//---------------------------------------------
// 設定登入及登出方法內容
//---------------------------------------------
app.get('/user/login',
    passport.authenticate('google', { scope: ['email', 'profile'] }));   //進行google第三方認證

app.get('/auth/google/callback', 
    passport.authenticate('google', { failureRedirect: '/login' }),   //導向登入失敗頁面	
    function(req, res) {
        // 如果登入成功, 使用者資料已存在session
        console.log(req.session.passport.user.id);
        console.log(req.session.passport.user.displayName);
        console.log(req.session.passport.user.emails[0].value);	   
        console.log(req.session.passport.user.photos[0].value);
        
        
        res.redirect('/user');   //導向登入成功頁面
    });

app.get('/user/logout', function(req, res){    
    req.logout();        //將使用者資料從session移除
    
    try{
        req.session.passport.user.id = null;       
    }catch(e){}
    
    res.redirect('/');   //導向登出頁面
});    
app.get('/teacher_guide_book', (req, res) => {
    const path = './public/pdf/teacher.pdf'
    if (fs.existsSync(path)) {
        res.contentType("application/pdf");
        fs.createReadStream(path).pipe(res)
    } else {
        res.status(500)
        console.log('File not found')
        res.send('File not found')
    }
})
app.get('/student_guide_book', (req, res) => {
    const path = './public/pdf/student.pdf'
    if (fs.existsSync(path)) {
        res.contentType("application/pdf");
        fs.createReadStream(path).pipe(res)
    } else {
        res.status(500)
        console.log('File not found')
        res.send('File not found')
    }
})
   
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
