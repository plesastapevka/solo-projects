var userModel = require('../models/userModel.js');
var questionModel = require('../models/questionModel.js');
var mongoose = require('mongoose');
var app = require('../app.js')

/**
 * userController.js
 *
 * @description :: Server-side logic for managing users.
 */
module.exports = {

    /**
     * userController.list()
     */
    list: function (req, res) {
        userModel.find(function (err, users) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when getting user.',
                    error: err
                });
            }
            return res.render('user/list', users);
        });
    },

    /**
     * userController.show()
     */
    show: function (req, res) {
        var id = req.params.id;
        userModel.findOne({_id: id}, function (err, user) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when getting user.',
                    error: err
                });
            }
            if (!user) {
                return res.status(404).json({
                    message: 'No such user'
                });
            }
            return res.json(user);
        });
    },

    /**
     * userController.create()
     */
    create: function (req, res) {

        userModel.findOne({username: req.body.username}, function (err, user) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when getting user.',
                    error: err
                });
            }
            if (!user) {
                var user = new userModel({
                    email : req.body.email,
                    username : req.body.username,
                    password : req.body.password

                });
                user.save(function (err, user) {
                    if (err) {
                        return res.status(500).json({
                            message: 'Error when creating user',
                            error: err
                        });
                    }
                    return res.render('user/login');
                });
            }
            else {
                return res.render('user/register', {error: "User exists."});
            }
        });
    },

    /**
     * userController.login()
     */
    showLogin: function (req, res) {
        res.render('user/login');
    }
    ,
    showRegister: function (req, res) {
        res.render('user/register');
    }
    ,

    login: function (req, res,next) {
        userModel.authenticate(req.body.username, req.body.password, function (error, user) {
            if (error || !user) {
                var err = new Error('Wrong username or password.');
                err.status = 401;
                return next(err);
            } else {
                req.session.userId = user._id;
                req.session.user = user;
                return res.redirect('../');
            }
        })
    },

    logout: function (req, res,next) {
        if (req.session) {
            // delete session object
            req.session.destroy(function (err) {
                if (err) {
                    return next(err);
                } else {
                    return res.redirect('/');
                }
            });
        }
    },

    profile: function (req, res, next) {
        userModel.findById(req.session.userId)
            .exec(function (error, user) {
                if (error) {
                    return next(error);
                } else {
                    if (user === null) {
                        var err = new Error('Not authorized! Go back!');
                        err.status = 400;
                        return next(err);
                    } else {
                        questionModel.find({ userID: req.session.userId}, function (err, question) {
                            if (err) {
                                return res.status(500).json({
                                    message: 'Error when getting question',
                                    error: err
                                });
                            }
                            if (!user) {
                                return res.status(404).json({
                                    message: 'No such question'
                                });
                            }
                        res.render('user/profile', {usr: user, qst: question});
                        });
                    }
                }
            });
    }
    ,

    /**
     * userController.update()
     */
    update: function (req, res) {
        var id = req.params.id;
        userModel.findOne({_id: id}, function (err, user) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when getting user',
                    error: err
                });
            }
            if (!user) {
                return res.status(404).json({
                    message: 'No such user'
                });
            }

            user.username = req.body.username ? req.body.username : user.username;
			user.email = req.body.email ? req.body.email : user.email;
			user.password = req.body.password ? req.body.password : user.password;
			
            user.save(function (err, user) {
                if (err) {
                    return res.status(500).json({
                        message: 'Error when updating user.',
                        error: err
                    });
                }

                return res.json(user);
            });
        });
    },

    /**
     * userController.remove()
     */
    remove: function (req, res) {
        var id = req.params.id;
        userModel.findByIdAndRemove(id, function (err, user) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when deleting the user.',
                    error: err
                });
            }
            return res.status(204).json();
        });
    }
};
