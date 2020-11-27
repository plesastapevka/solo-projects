var answerModel = require('../models/answerModel.js');

/**
 * answerController.js
 *
 * @description :: Server-side logic for managing answers.
 */
module.exports = {

    /**
     * answerController.list()
     */
    list: function (req, res) {
        answerModel.find(function (err, answers) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when getting answer.',
                    error: err
                });
            }
            return res.json(answers);
        });
    },

    /**
     * answerController.show()
     */
    show: function (req, res) {
        var id = req.params.id;
        answerModel.findOne({_id: id}, function (err, answer) {
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
            return res.json(answer);
        });
    },

    /**
     * answerController.create()
     */
    create: function (req, res) {
        var qID = req.body.questionID;
        var answer = new answerModel({
			content : req.body.answr,
			userID : res.locals.user._id,
            username: res.locals.user.username,
			questionID : req.body.questionID,
			likes : 0,
            accepted: 0

        });

        answer.save(function (err, answer) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when creating answer',
                    error: err
                });
            }
            return res.redirect("/questions/" + qID);
        });
    },

    /**
     * answerController.update()
     */
    update: function (req, res) {
        var id = req.params.id;
        answerModel.findOne({_id: id}, function (err, answer) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when getting answer',
                    error: err
                });
            }
            if (!answer) {
                return res.status(404).json({
                    message: 'No such answer'
                });
            }

            answer.accepted = true;
			
            answer.save(function (err, answer) {
                if (err) {
                    return res.status(500).json({
                        message: 'Error when updating answer.',
                        error: err
                    });
                }

                return res.redirect('/questions');
            });
        });
    },

    /**
     * answerController.remove()
     */
    remove: function (req, res) {
        var id = req.params.id;
        answerModel.findByIdAndRemove(id, function (err, answer) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when deleting the answer.',
                    error: err
                });
            }
            return res.status(204).json();
        });
    }
};
