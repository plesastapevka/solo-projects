var questionModel = require('../models/questionModel.js');
var answerModel = require('../models/answerModel.js');
var userModel = require('../models/userModel.js');

/**
 * questionController.js
 *
 * @description :: Server-side logic for managing questions.
 */
module.exports = {

    /**
     * questionController.list()
     */
    list: function (req, res) {
        var mysort = {accepted: -1}
        questionModel.find(function (err, questions) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when getting question.',
                    error: err
                });
            }
            return res.render('question/list', {vpr: questions});
        }).sort(mysort);
    },

    showask: function (req, res) {
        res.render('question/ask');
    },

    /**
     * questionController.show()
     */
    show: function (req, res) {
        var id = req.params.id;
        var mysort = {
            accepted: -1
        }
        questionModel.findOne({_id: id}, function (err, question) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when getting question.',
                    error: err
                });
            }
            if (!question) {
                return res.status(404).json({
                    message: 'No such question'
                });
            }

            answerModel.find({ questionID: question._id}, function (err, answer) {
                if (err) {
                    return res.status(500).json({
                        message: 'Error when getting answer.',
                        error: err
                    });
                }
                if (!answer) {
                    return res.status(404).json({
                        message: 'No such answer'
                    });
                }

                if(res.locals.user != null && res.locals.user._id === question.userID) {
                    var owner = {isowner: 1}
                    return res.render('question/extended', {ans: answer, qst: question, lastnik: owner});
                } else {
                    return res.render('question/extended', {ans: answer, qst: question});
                }


            }).sort(mysort);
        });
            //return res.render('question/extended', {qst: question});

    },

    /**
     * questionController.create()
     */
    create: function (req, res) {
        var date = new Date();
        var question = new questionModel({
			title : req.body.title,
			content : req.body.quest,
			tags : req.body.tags,
			postdate : date,
            userID : res.locals.user
        });

        question.save(function (err, question) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when creating question',
                    error: err
                });
            }
            return res.redirect('../');
        });
    },

    /**
     * questionController.update()
     */
    update: function (req, res) {
        var id = req.params.id;
        questionModel.findOne({_id: id}, function (err, question) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when getting question',
                    error: err
                });
            }
            if (!question) {
                return res.status(404).json({
                    message: 'No such question'
                });
            }

            question.title = req.body.title ? req.body.title : question.title;
			question.content = req.body.content ? req.body.content : question.content;
			question.tags = req.body.tags ? req.body.tags : question.tags;
			question.postdate = req.body.postdate ? req.body.postdate : question.postdate;
			
            question.save(function (err, question) {
                if (err) {
                    return res.status(500).json({
                        message: 'Error when updating question.',
                        error: err
                    });
                }

                return res.json(question);
            });
        });
    },

    /**
     * questionController.remove()
     */
    remove: function (req, res) {
        var id = req.params.id;
        questionModel.findByIdAndRemove(id, function (err, question) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when deleting the question.',
                    error: err
                });
            }
            return res.status(204).json();
        });
    }
};
