const mongoose = require("mongoose");
const Schema = mongoose.Schema;
const Joi = require("joi");
require("dotenv").config()

const ListSchema = new Schema({
    title: { type: String, required: true },
    tasks: [String],
    tags: [String]
}, {collection: "lists"});


const ListModel = mongoose.model("ListModel", ListSchema);

module.exports = ListModel;