const mongoose = require("mongoose");
const Schema = mongoose.Schema;
const Joi = require("joi");
const jwt = require("jsonwebtoken");
require("dotenv").config()

const UserSchema = new Schema({
    name: { type: String, required: false },
    lastName: { type: String, required: false},
    username: { type: String, required: true },
    mail: { type: String, required: true, unique: true },
    password: { type: String, required: true, minlength: 8, maxlength: 256 }
}, {collection: "users"});


const UserModel = mongoose.model("UserModel", UserSchema);

module.exports = UserModel;