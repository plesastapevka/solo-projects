var mongoose = require('mongoose');
var Schema   = mongoose.Schema;

var questionSchema = new Schema({
	'title' : String,
	'content' : String,
	'tags' : String,
	'postdate' : Date,
	'userID' : String
});

module.exports = mongoose.model('question', questionSchema);
