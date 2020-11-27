const mongoose = require("mongoose");
const Schema = mongoose.Schema;
const Joi = require("joi");
require("dotenv").config()

const ContactSchema = new Schema({
    userId: { type: String, required: true },
    name: { type: String, required: true },
    lastName: { type: String, required: true},
    phoneNumber: { type: String, required: false },
    email: { type: String, required: false }
}, {collection: "contacts"});


const ContactModel = mongoose.model("ContactModel", ContactSchema);

module.exports = ContactModel;