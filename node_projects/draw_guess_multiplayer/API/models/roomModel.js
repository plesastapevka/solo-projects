const mongoose = require("mongoose");
const Schema = mongoose.Schema;
const Joi = require("joi");
require("dotenv").config()

const RoomSchema = new Schema({
    uuid: { type: String, required: true },
    ownerId: { type: String, required: true },
    name: { type: String, required: true },
    players: [String]
}, {collection: "rooms"});


const RoomModel = mongoose.model("RoomModel", RoomSchema);

module.exports = RoomModel;