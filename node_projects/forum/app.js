var createError = require('http-errors');
var express = require('express');
var path = require('path');
var cookieParser = require('cookie-parser');
var logger = require('morgan');
var Handlebars     = require('handlebars');
var HandlebarsIntl = require('handlebars-intl');
HandlebarsIntl.registerWith(Handlebars);

var mongoose = require('mongoose');
//Set up default mongoose connection
var mongoDB = 'mongodb://127.0.0.1/forum';
mongoose.connect(mongoDB, { useNewUrlParser: true });
// Get Mongoose to use the global promise library
mongoose.Promise = global.Promise;
//Get the default connection
var db = mongoose.connection;

//Bind connection to error event (to get notification of connection errors)
db.on('error', console.error.bind(console, 'MongoDB connection error:'));

var indexRouter = require('./routes/index');
//spremenimo route
var usersRouter = require('./routes/userRoutes');
var questionRouter = require('./routes/questionRoutes');
var answerRouter = require('./routes/answerRoutes');

var app = express();

// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'hbs');

app.use(logger('dev'));
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, 'public')));

var session = require('express-session');
app.use(session({
  secret: 'work hard',
  resave: true,
  saveUninitialized: false
}));

app.use(function(req, res, next){
  res.locals.user = req.session.user;
  next();
});

app.use('/', indexRouter);
app.use('/users', usersRouter);
app.use('/questions', questionRouter);
app.use('/answers', answerRouter);

/*Handlebars.registerHelper('formatDate', function(value){

  var dateString = value;
  var date = new Date(dateString);

  var returnformat = date.getDay() + "." + (date.getMonth()+1) + "." + date.getFullYear() + " " + date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds();


  return returnFormat;
});
app.engine('handlebars', engines.handlebars);*/


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
