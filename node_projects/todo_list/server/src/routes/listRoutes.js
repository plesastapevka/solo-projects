const express = require("express");
const ObjectID = require("mongoose").Types.ObjectID;
const router = express.Router();
const ListModel = require("../models/listModel");
const TaskModel = require("../models/taskModel");

/// PUT: Create list
router.put("/add", async (req, res) => {
    let title = req.body.title;
    if (req.body.tags.length == 0) var tags = [];
    else var tags = req.body.tags;

    let newList = {
        title: title,
        tags: tags,
        tasks: []
    }

    ListModel.create(newList, (err, newList) => {
        if (err) {
            return res.status(500).json({
                status: 500,
                message: "Could not create list"
            })
        }

        return res.status(200).json({
            status: 200,
            message: "List created"
        })
    })
})

/// PUT: Update list
router.put("/update", async (req, res) => {
    let id = req.body.id;
    let title = req.body.title;
    var tags;
    if (req.body.tags.length === 0) tags = [];
    else tags = req.body.tags;

    ListModel.findByIdAndUpdate(id, {title: title, tags: tags}, { new: true, upsert: true, setDefaultsOnInsert: true }, (err, updatedList) => {
        if (err) return res.status(500).json({
            status: 500,
            message: "Cannot update list"
        });
        
        return res.status(200).json({
            status: 200,
            message: "List updated sucessfully!"
        });
    })
})

/// DELETE: Delete list
router.delete("/delete/:id", async (req, res) => {
    let id = req.params.id;

    let list = await ListModel.findOne({ _id: id });
    if(!list) return res.status(500).json({
        status: 500,
        message: "List not found."
    });

    if(list.tasks.length != 0) {
        list.tasks.forEach(e => {
            TaskModel.deleteOne({ _id: e }, (err) => {
                if (err) {
                    return res.status(500).json({
                        status: 500,
                        message: "Could not delete task"
                    })
                }
            });
        });
    }

    await list.deleteOne((err) => {
        if(err) return res.status(500).json({
            status: 500,
            message: "List could not be deleted."
        });
    })

    return res.status(200).json({
        status: 200,
        message: "List deleted."
    });
})

/// GET: Get all lists
router.get("/", async (req, res) => {
    await ListModel.find({}, (err, lists) => {
        if (err) return res.status(500).json({
            status: 500,
            message: "Error fetching database."
        });

        if(lists.length == 0) return res.status(200).json({
            status: 200,
            lists: []
        });

        return res.status(200).json({
            status: 200,
            lists: lists
        });
    });
})

/// GET: Get one list
router.get("/one/:id", async (req, res) => {
    let id = req.params.id;
    await ListModel.findOne({ _id: id }, (err, list) => {
        if (err) return res.status(500).json({
            status: 500,
            message: "Error fetching database."
        });

        return res.status(200).json({
            status: 200,
            list: list
        });
    });
})

/// PUT: Add tag
router.put("/tag", async (req, res) => {
    let id = req.body.id;
    let tags = req.body.tags;

    let list = await ListModel.findOne({ _id: id });
    let newTags = tags.concat(list.tags);

    ListModel.findByIdAndUpdate(id, { tags: newTags }, { new: true, upsert: true, setDefaultsOnInsert: true }, (err, updatedList) => {
        if (err) return res.status(500).json({
            status: 500,
            message: "Cannot update list"
        });
        
        return res.status(200).json({
            status: 200,
            message: "List updated sucessfully!"
        });
    })
})

/// PUT: Delete tag
router.put("/tag/remove", async (req, res) => {
    let id = req.body.id;
    let tag = req.body.tag;
    
    let list = await ListModel.findOne({ _id: id });
    let tags = list.tags;

    const index = tags.indexOf(tag);
    if (index > -1) {
        tags.splice(index, 1);
    }

    ListModel.findByIdAndUpdate(id, {tags: tags}, { new: true, upsert: true, setDefaultsOnInsert: true }, (err, updatedList) => {
        if (err) return res.status(500).json({
            status: 500,
            message: "Cannot update list"
        });
        
        return res.status(200).json({
            status: 200,
            message: "List updated sucessfully!"
        });
    })
});

module.exports = router;