var mongoose = require('mongoose');
var Schema   = mongoose.Schema;

var answerSchema = new Schema({
	'content' : String,
	'userID' : String,
	'questionID' : String,
	'username' : String,
	'likes' : Number,
	'accepted': Boolean
});

module.exports = mongoose.model('answer', answerSchema);
