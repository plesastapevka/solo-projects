var express = require('express');
var router = express.Router();
var questionModel = require('../models/questionModel.js');

/* GET home page. */
router.get('/', function(req, res, next) {
  res.redirect('../questions');
});

module.exports = router;
