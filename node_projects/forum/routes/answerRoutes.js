var express = require('express');
var router = express.Router();
var answerController = require('../controllers/answerController.js');

/*
 * GET
 */
router.get('/showall', answerController.list);

/*
 * GET
 */
router.get('/:id', answerController.show);
router.get('/accept/:id', answerController.update);

/*
 * POST
 */
router.post('/', answerController.create);

/*
 * PUT
 */
router.put('/:id', answerController.update);

/*
 * DELETE
 */
router.delete('/:id', answerController.remove);

module.exports = router;
