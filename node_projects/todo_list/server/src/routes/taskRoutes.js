const express = require("express");
const ObjectID = require("mongoose").Types.ObjectID;
const router = express.Router();
const TaskModel = require("../models/taskModel");
const ListModel = require("../models/listModel");
const e = require("express");

/// PUT: Add task to list
router.put("/add", async (req, res) => {
    let title = req.body.title;
    let desc = req.body.description;
    let ownerId = req.body.ownerId;
    let date = req.body.date;
    let status = req.body.status;
    let reminder = req.body.reminder;
    if (req.body.tags.length == 0) var tags = [];
    else var tags = req.body.tags;

    let newTask = {
        ownerId: ownerId,
        title: title,
        description: desc,
        date: date,
        status: status,
        reminder: reminder,
        tags: tags
    }

    let list = await ListModel.findOne({ _id: ownerId });

    TaskModel.create(newTask, (err, newTask) => {
        if (err) {
            return res.status(500).json({
                status: 500,
                message: "Could not create task"
            })
        }
        // Add created task ID to owner list
        let tasks = []
        if (list.tasks.lenght == 0) {
            tasks = [newTask._id]
        } else {
            tasks = list.tasks;
            tasks.push(newTask._id);
        }

        ListModel.findByIdAndUpdate(ownerId, { tasks: tasks }, { new: true, upsert: true, setDefaultsOnInsert: true }, (err, updatedList) => {
            if (err) return res.status(500).json({
                status: 500,
                message: "Cannot update list"
            });
        })

        return res.status(200).json({
            status: 200,
            message: "Task created"
        })
    })
})

/// PUT: Edit task in list
router.put("/update", async (req, res) => {
    let id = req.body.id;
    let title = req.body.title;
    let desc = req.body.description;
    let ownerId = req.body.ownerId;
    let date = req.body.date;
    let status = req.body.status;
    let reminder = req.body.reminder;

    let newTask = {
        ownerId: ownerId,
        title: title,
        description: desc,
        date: date,
        status: status,
        reminder: reminder
    }

    TaskModel.findByIdAndUpdate(id, newTask, { new: true, upsert: true, setDefaultsOnInsert: true }, (err, updatedTask) => {
        if (err) return res.status(500).json({
            status: 500,
            message: "Cannot update task"
        });
        
        return res.status(200).json({
            status: 200,
            message: "Task updated sucessfully!"
        });
    })
})

/// DELETE: Delete task in list
router.delete("/:id", async (req, res) => {
    let id = req.params.id;
    let task = await TaskModel.findById(id);
    let list = await ListModel.findById(task.ownerId);
    let newTasks = list.tasks;
    const index = newTasks.indexOf(id);
    if (index > -1) {
        newTasks.splice(index, 1);
    }

    ListModel.findByIdAndUpdate(task.ownerId, { tasks: newTasks }, { new: true, upsert: true, setDefaultsOnInsert: true }, (err, updatedList) => {
        if (err) return res.status(500).json({
            status: 500,
            message: "Cannot update list"
        });
    })
    
    await task.deleteOne((err) => {
        if (err) return res.status(500).json({
            status: 500,
            message: "Could not delete task"
        })

        return res.status(200).json({
            status: 200,
            message: "Task deleted"
        })
    })
})

/// GET: Get tasks based on owner id
router.get("/:id", async (req, res) => {
    let ownerId = req.params.id;
    await TaskModel.find({ ownerId: ownerId }, (err, tasks) => {
        if (err) return res.status(500).json({
            status: 500,
            message: "Error fetching database."
        });

        if(tasks.length == 0) return res.status(200).json({
            status: 200,
            tasks: []
        });

        return res.status(200).json({
            status: 200,
            tasks: tasks
        });
    });
})

/// GET: Get a task based on id
router.get("/one/:id", async (req, res) => {
    let id = req.params.id;
    await TaskModel.findOne({ _id: id }, (err, task) => {
        if (err) return res.status(500).json({
            status: 500,
            message: "Error fetching database."
        });

        return res.status(200).json({
            status: 200,
            task: task
        });
    });
})

/// PUT: Add tag
router.put("/tag", async (req, res) => {
    let id = req.body.id;
    let tags = req.body.tags;

    let task = await TaskModel.findOne({ _id: id });
    let newTags = tags.concat(task.tags);

    TaskModel.findByIdAndUpdate(id, { tags: newTags }, { new: true, upsert: true, setDefaultsOnInsert: true }, (err, updatedTask) => {
        if (err) return res.status(500).json({
            status: 500,
            message: "Cannot update task"
        });
        
        return res.status(200).json({
            status: 200,
            message: "Task updated sucessfully!"
        });
    })
})

/// PUT: Delete tag
router.put("/tag/remove", async (req, res) => {
    let id = req.body.id;
    let tag = req.body.tag;
    
    let task = await TaskModel.findOne({ _id: id });
    let tags = task.tags;

    const index = tags.indexOf(tag);
    if (index > -1) {
        tags.splice(index, 1);
    }

    TaskModel.findByIdAndUpdate(id, { tags: tags }, { new: true, upsert: true, setDefaultsOnInsert: true }, (err, updatedTask) => {
        if (err) return res.status(500).json({
            status: 500,
            message: "Cannot update task"
        });
        
        return res.status(200).json({
            status: 200,
            message: "Task updated sucessfully!"
        });
    })
})

/// PUT: Set task status
router.put("/status", async (req, res) => {
    let id = req.body.id;
    let status = req.body.status;

    TaskModel.findByIdAndUpdate(id, { status: status }, { new: true, upsert: true, setDefaultsOnInsert: true }, (err, updatedTask) => {
        if (err) return res.status(500).json({
            status: 500,
            message: "Cannot update status"
        });
        
        return res.status(200).json({
            status: 200,
            message: "Status updated sucessfully!"
        });
    })
})

/// PUT: Set date
router.put("/date", async (req, res) => {
    let id = req.body.id;
    let date = req.body.date;

    // Date should always be in format YYYY-MM-DD
    TaskModel.findByIdAndUpdate(id, { date: new Date(date) }, { new: true, upsert: true, setDefaultsOnInsert: true }, (err, updatedTask) => {
        if (err) {
            return res.status(500).json({
                status: 500,
                message: "Cannot update date"
            }); 
        }
        
        return res.status(200).json({
            status: 200,
            message: "Date updated sucessfully!"
        });
    })
})

module.exports = router;