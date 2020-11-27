var express = require('express');
var axios = require('axios');
var router = express.Router();

/* GET home page. */
router.get('/', function(req, res, next) {
  axios.get('http://localhost:5700/lists')
  .then((response) => {
    let lists = response.data.lists;

    res.render('index', {
      lists: lists
    });    
  })
  .catch(error => {
    res.status(err.status || 500);
    res.render('error');
  });
});

/* GET show all the tasks in corresponding list */
router.get('/list/:id', function(req, res, next) {
  axios.get(`http://localhost:5700/tasks/${req.params.id}`)
  .then((response) => {
    let tasks = response.data.tasks;

    res.render('list', {
      tasks: tasks,
      ownerId: req.params.id
    });    
  })
  .catch((error) => {
    res.status(error.status || 500);
    res.render('error');
  });
});

/* POST add a task in corresponding list */
router.post('/tasks/add/:id', function(req, res, next) {
  var tags = req.body.tags.split(" ");
  var body = {
    ownerId: req.params.id,
    title: req.body.title,
    description: req.body.description,
    date: req.body.date,
    status: req.body.status,
    tags: tags,
    reminder: req.body.reminder
  }
  axios.put('http://localhost:5700/tasks/add', body)
  .then((response) => {
    if (response.status == 200) {
      res.redirect('/list/' + req.params.id);
    } else {
      res.status(500);
      res.render('error');  
    }
  })
  .catch((error) => {
    res.status(error.status || 500);
    res.render('error');
  });
});

/* GET delete a task in corresponding list */
router.get('/tasks/delete/:id', function(req, res, next) {
  axios.delete('http://localhost:5700/tasks/' + req.query.id)
  .then((response) => {
    if (response.status == 200) {
      res.redirect('/list/' + req.params.id);
    } else {
      res.status(500);
      res.render('error');  
    }
  })
  .catch((error) => {
    res.status(error.status || 500);
    res.render('error');
  });
});

/* GET update status */
router.get('/tasks/status/:id', function(req, res, next) {
  var body = {
    id: req.query.id,
    status: req.query.status
  }

  axios.put('http://localhost:5700/tasks/status', body)
  .then((response) => {
    if (response.status == 200) {
      res.redirect('/list/' + req.params.id);
    } else {
      res.status(500);
      res.render('error');  
    }
  })
  .catch((error) => {
    res.status(error.status || 500);
    res.render('error');
  });
});

/* POST add a list */
router.post('/list/add', function(req, res, next) {
  var tags = req.body.tags.split(" ");
  var body = {
    title: req.body.title,
    tags: tags
  }
  axios.put('http://localhost:5700/lists/add', body)
  .then((response) => {
    if (response.status == 200) {
      res.redirect('/');
    } else {
      res.status(500);
      res.render('error');  
    }
  })
  .catch((error) => {
    res.status(error.status || 500);
    res.render('error');
  });
});

/* GET delete a list */
router.get('/list/delete/:id', function(req, res, next) {
  axios.delete('http://localhost:5700/lists/delete/' + req.params.id)
  .then((response) => {
    if (response.status == 200) {
      res.redirect('/');
    } else {
      res.status(500);
      res.render('error');  
    }
  })
  .catch((error) => {
    res.status(error.status || 500);
    res.render('error');
  });
});

/* GET navigate to edit a task */
router.get('/tasks/edit/:id', function(req, res, next) {
  let ownerId = req.params.id;
  let taskId = req.query.id;

  axios.get('http://localhost:5700/tasks/one/' + taskId)
  .then((response) => {
    if (response.status == 200) {
      let dateT = response.data.task.date;
      response.data.task.date = new Date(dateT).toISOString();
      let newDate = response.data.task.date.slice(0, 16);
      console.log(newDate);
      // let ye = new Intl.DateTimeFormat('en', { year: 'numeric' }).format(response.data.task.date);
      // let mo = new Intl.DateTimeFormat('en', { month: '2-digit' }).format(response.data.task.date);
      // let da = new Intl.DateTimeFormat('en', { day: '2-digit' }).format(response.data.task.date);
      // response.data.task.date = `${da}-${mo}-${ye}`;
      res.render('editTask', {
        task: response.data.task,
        ownerId: ownerId
      });    
    } else {
      res.status(500);
      res.render('error');  
    }
  })
  .catch((error) => {
    res.status(error.status || 500);
    res.render('error');
  });
});

/* POST update a task */
router.post('/tasks/update/:id', function(req, res, next) {
  let ownerId = req.params.id;
  var tags = req.body.tags.split(" ");
  var body = {
    id: req.query.id,
    ownerId: req.params.id,
    title: req.body.title,
    description: req.body.description,
    date: req.body.date,
    status: req.body.status,
    tags: tags,
    reminder: req.body.reminder
  }

  axios.put('http://localhost:5700/tasks/update', body)
  .then((response) => {
    if (response.status == 200) {
      res.redirect('/list/' + ownerId);
    } else {
      res.status(500);
      res.render('error');  
    }
  })
  .catch((error) => {
    res.status(error.status || 500);
    res.render('error');
  });
});

/* GET navigate to edit a list */
router.get('/list/edit/:id', function(req, res, next) {
  let id = req.params.id;

  axios.get('http://localhost:5700/lists/one/' + id)
  .then((response) => {
    if (response.status == 200) {
      res.render('editList', {
        list: response.data.list
      });    
    } else {
      res.status(500);
      res.render('error');  
    }
  })
  .catch((error) => {
    res.status(error.status || 500);
    res.render('error');
  });
});

/* POST update a list */
router.post('/list/update/:id', function(req, res, next) {
  let id = req.params.id;
  var tags = req.body.tags.split(" ");
  console.log (id);
  var body = {
    id: id,
    title: req.body.title,
    tags: tags
  }

  axios.put('http://localhost:5700/lists/update', body)
  .then((response) => {
    if (response.status == 200) {
      res.redirect('/');
    } else {
      res.status(500);
      res.render('error');  
    }
  })
  .catch((error) => {
    res.status(error.status || 500);
    res.render('error');
  });
});

module.exports = router;