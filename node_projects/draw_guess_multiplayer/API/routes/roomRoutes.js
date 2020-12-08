const express = require("express");
const ObjectID = require("mongoose").Types.ObjectID;
const router = express.Router();
const RoomModel = require("../models/roomModel");
const bcrypt = require("bcrypt");
const saltRounds = 10;
const jwt = require("jsonwebtoken");
require("dotenv").config();

// PUT Create room
router.put("/", async (req, res) => {
  let ownerId = req.body.id;
  let token = req.headers.bearer;
  let name = req.body.name;

  if (!token) return res.status(403).send({ message: "Unauthorized!" });

  await jwt.verify(token, process.env.PRIVATE_KEY, (err, decoded) => {
    if (err) return res.status(403).send({ message: "Unauthorized!" });

    if (err) {
      return res
        .status(500)
        .send({ message: "There was an error uploading room." });
    }

    // Set up new room model
    let newRoom = {
      ownerId: ownerId,
      name: name,
      players: [],
    };

    RoomModel.findOne({ name: name }, (err, room) => {
      if (room) {
        return res.status(500).send({ message: "Room already exists." });
      }
      RoomModel.create(newRoom, (err, updatedRoom) => {
        if (err) return res.status(500).send(err);

        return res
          .status(200)
          .send({ message: "Room created sucessfully!", room: updatedRoom });
      });
    });
  });
});

// DELETE room by ID
router.delete("/:id", async (req, res) => {
    let token = req.headers.bearer;
    let roomId = req.params.id;
    if (!token) return res.status(403).send({ message: "Unauthorized!" });

    await jwt.verify(token, process.env.PRIVATE_KEY, async (err, decode) => {
        if (err) return res.status(403).send({ message: "Unauthorized!" });
    });

    RoomModel.findById(roomId, (err, room) => {
        if (!room) return res.status(404).send({ message: "No room found." });

        room.deleteOne((err) => {
        if (err)
            return res.status(500).send({ message: "Room could not be deleted." });
        return res.status(200).send({ message: "Room deleted." });
        });
    });
    });

// GET all rooms
router.get("/", async (req, res) => {
    await RoomModel.find({}, (err, rooms) => {
        if (err) return res.status(500).send({ message: "Error fetching database." });
        if (rooms.length == 0) return res.status(404).send({ message: "No rooms found." });

        return res.status(200).send(rooms);
    });
});

// GET room by ID
router.get("/:id", async (req, res) => {
    let roomId = req.params.id;
    let room = await RoomModel.findById(roomId);
    if (!room) return res.status(404).send({ message: "No rooms found." });
    return res.status(200).send(room);
});

// POST join room
router.post("/", async (req, res) => {
    let token = req.headers.bearer;
    let userId = req.body.userId;
    let name = req.body.name;
    await jwt.verify(token, process.env.PRIVATE_KEY, async (err, decode) => {
        if (err) return res.status(403).send({ message: "Unauthorized!" });
    });

    RoomModel.findOne({ name: name }, (err, room) => {
        if (!room) return res.status(500).send({ message: "Room not found" });
        if (room.players.length >= 8) return res.status(200).send({ message: "Room full" });
        if (!room.players.includes(userId)) {
            let newPlayers = room.players;
            newPlayers.push(userId);
            RoomModel.update({ name: name }, { players: newPlayers }, (err, room) => {
                if (err) return res.status(500).send({ message: "Could not join the room" });
                return res.status(200).send({ message: "Room joined" });
            })
        } else {
            return res.status(200).send({ message: "Already joined" });
        }
    })
});

// POST leave room
router.post("/leave", async (req, res) => {
    let token = req.headers.bearer;
    let userId = req.body.userId;
    let name = req.body.name;
    await jwt.verify(token, process.env.PRIVATE_KEY, async (err, decode) => {
        if (err) return res.status(403).send({ message: "Unauthorized!" });
    });

    RoomModel.findOne({ name: name }, (err, room) => {
        if (!room) return res.status(500).send({ message: "Room not found" });
        if (room.players.includes(userId)) {
            let newPlayers = room.players;
            newPlayers.splice(newPlayers.indexOf(userId),1)
            RoomModel.update({ name: name }, { players: newPlayers }, (err, room) => {
                if (err) return res.status(500).send({ message: "Could not join the room" });
                return res.status(200).send({ message: "Left the room" });
            })
        } else {
            return res.status(200).send({ message: "Not in this room" });
        }
    })
})

module.exports = router;
