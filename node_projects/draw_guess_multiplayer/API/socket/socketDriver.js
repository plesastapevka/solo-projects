const uuid = require("uuid");
const RoomModel = require("../models/roomModel");

module.exports = (io) => {
    var rooms = [];

    io.on("connection", (socket) => {
        console.log("Client connected: " + socket.id);

        setInterval(function(){
            socket.broadcast.emit('roomUpdate', rooms);
        }, 1000);

        socket.on("mouse", (data) => {
            socket.broadcast.emit("sync", data);
        });
        socket.on("createRoom", (data) => {
            var room = {
                uuid: uuid.v1(),
                ownerId: data.userId,
                name: data.name,
                players: [data.username]
            };
            rooms.push(room);
            RoomModel.create(room, (err, newRoom) => {});
        })
        socket.on("joinRoom", (data) => {
            console.log(data);
        })
        socket.on("disconnect", () => console.log ("Client disconnected"));
    })
}