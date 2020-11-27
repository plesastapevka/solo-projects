var mongoose = require('mongoose');
var bcrypt = require('bcrypt');
var Schema   = mongoose.Schema;

var userSchema = new Schema({
	'username' : String,
	'email' : String,
	'password' : String
});

module.exports = mongoose.model('user', userSchema);

userSchema.statics.authenticate = function (username, password, callback) {
	User.findOne({ username: username })
		.exec(function (err, user) {
			if (err) {
				return callback(err)
			} else if (!user) {
				var err = new Error('User not found.');
				err.status = 401;
				return callback(err);
			}
			bcrypt.compare(password, user.password, function (err, result) {
				if (result === true) {
					return callback(null, user);
				} else {
					return callback();
				}
			})
		});
}

userSchema.pre('save', function(next) {
	var user = this;

	// only hash the password if it has been modified (or is new)
	if (!user.isModified('password')) return next();

	// generate a salt
	bcrypt.genSalt(10, function(err, salt) {
		if (err) return next(err);

		// hash the password using our new salt
		bcrypt.hash(user.password, salt, function(err, hash) {
			if (err) return next(err);

			// override the cleartext password with the hashed one
			user.password = hash;
			next();
		});
	});
});

var User = mongoose.model('User', userSchema);
module.exports = User;
