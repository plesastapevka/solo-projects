const mongoose = require("mongoose");
const Schema = mongoose.Schema;

const TaskSchema = new Schema({
    ownerId: { type: String, required: true },
    title: { type: String, required: true },
    description: { type: String, required: false },
    date: { type: Date, required: false },
    status: { type: String, required: false },
    reminder: { type: Date, required: false },
    tags: [String]
}, {collection: "tasks"});


const TaskModel = mongoose.model("TaskModel", TaskSchema);

module.exports = TaskModel;