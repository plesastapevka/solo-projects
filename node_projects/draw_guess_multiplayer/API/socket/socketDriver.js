const uuid = require("uuid");
const { remove } = require("../models/roomModel");
const RoomModel = require("../models/roomModel");

let rooms = [{ uuid: uuid.v1(), ownerId: null, name: "Public", players: [] }];
let owners = [];
let clients = [];
// INTERVAL FUNCTION
function removeRoom(id) {
  let index = rooms.findIndex((r) => r.uuid === id);
  if (index !== -1) {
    return rooms.splice(index, 1)[0];
  }
}

function userDisconnected(sid, socket, username) {
  let index = owners.findIndex((o) => o === sid);
  var room;
  if (index !== -1) {
    // Is an owner
    let ownerId = owners[index];
    owners.splice(index, 1)[0];
    let roomIndex = rooms.findIndex((r) => r.ownerId === ownerId);
    room = rooms[roomIndex];
    if (!room) return;
    if (room.players.length == 1) {
      removeRoom(room.uuid);
    } else {
      let nestedIndex = room.players.findIndex((p) => p.socketId === sid);
      // username = room.players[nestedIndex].username;
      room.players.splice(nestedIndex, 1)[0];
      room.ownerId = room.players[0].socketId;
      room.players[0].owner = true;
      owners.push(room.players[0].socketId);
    }
    socket.to(room.uuid).emit("userDisconnected", username);
  } else {
    // Not an owner
    rooms.forEach((r) => {
      let userIndex = r.players.findIndex((player) => player.socketId === sid);
      if (userIndex !== -1) {
        // username = r.players[userIndex].username;
        // console.log(username);
        r.players.splice(index, 1)[0];
        room = r;
        socket.to(room.uuid).emit("userDisconnected", username);
      }
    });
  }
}

function joinRoom(data) {
  let index = rooms.findIndex((r) => r.uuid === data.uuid);
  if (rooms[index].players.length == 8) {
    return -1;
  }
  
  rooms.forEach((r) => {
    let userIndex = r.players.findIndex((player) => player.userId === data.userId);
    if (userIndex !== -1) {
      return false;
    }
  });

  let userIndex = rooms[index].players.findIndex(
    (p) => p.userId === data.userId
  );
  if (userIndex === -1) {
    rooms[index].players.push({
      userId: data.userId,
      username: data.username,
      socketId: data.socketId,
      owner: data.owner
    });
    return true;
  }
  return false;
}

module.exports = (io) => {
  io.on("connection", (socket) => {
    socket.broadcast.to(socket.id).emit("roomUpdate", rooms);
    console.log("Client connected: " + socket.id);
    io.to(socket.id).emit("requestUsername");

    // INTERVAL FUNCTION TO KEEP UPDATING ROOM LIST
    setInterval(function () {
      socket.broadcast.emit("roomUpdate", rooms);
    }, 1000);

    // INTERVAL FUNCTION TO KEEP UPDATING ROOM INFO
    setInterval(function () {
      rooms.forEach((r) => {
        socket.to(r.uuid).emit("roomStatusUpdate", r);
        io.to(r.ownerId).emit("owner", { owner: true })
      });
    }, 1000);

    // INTERVAL FUNCTION TO DELETE EMPTY ROOMS
    setInterval(function () {
      rooms.forEach((r) => {
        if (r.players.length == 0) {
          removeRoom(r.uuid);
        }
      });
    }, 5000);

    socket.on("newUser", (username) => {
      clients.push({ socketId: socket.id, username: username })
    });

    socket.on("mouse", (data) => {
      if (data.uuid !== "") {
        socket.to(data.uuid).emit("sync", data);
      }
    });

    socket.on("leaveRequest", (uuid) => {
      console.log(uuid);
      let index = clients.findIndex((c) => c.socketId === socket.id);
      let username = clients[index].username;
      userDisconnected(socket.id, socket, username);
      socket.leave(uuid);
      io.to(socket.id).emit("leaveApproved");
    })

    socket.on("createRoom", (data) => {
      let existIndex = rooms.findIndex((r) => r.name === data.name);
      // check the name
      if (existIndex !== -1) {
        io.to(socket.id).emit("status", { code: 1, msg: "Choose different room name" });
        return;
      }
      // check if owner
      if (owners.includes(socket.id)) {
        io.to(socket.id).emit("status", { code: 1, msg: "Already an owner" });
        return;
      }
      var userIndex;
      var exists;
      rooms.forEach((r) => {
        userIndex = r.players.findIndex((player) => player.userId === data.userId);
        if (userIndex !== -1) {
          exists = true;
        }
      });
      if (exists) {
        io.to(socket.id).emit("status", { code: 1, msg: "Already in a room" });
        return;
      }
      console.log("continuing")

      let guuid = uuid.v1();
      var room = {
        uuid: guuid,
        ownerId: socket.id,
        name: data.name,
        players: [],
      };
      rooms.push(room);
      var user = {
        uuid: guuid,
        userId: data.userId,
        username: data.username,
        socketId: socket.id,
        owner: true
      };
      joinRoom(user);
      owners.push(socket.id);
      socket.join(guuid);
      socket.broadcast.emit("roomUpdate", rooms);
      io.to(socket.id).emit("playerJoined", { username: data.username });
      socket.to(guuid).emit("playerJoined", { username: data.username });
      io.to(socket.id).emit("status", { msg: "Room created", code: 0, uuid: guuid });
    });

    socket.on("joinRoom", (data) => {
      if (owners.includes(socket.id)) {
        io.to(socket.id).emit("status", { code: 1, msg: "Already an owner" })
        return;
      }
      data.socketId = socket.id;
      data.owner = false;
      if (joinRoom(data)) {
        socket.join(data.uuid);
        io.to(socket.id).emit("status", { msg: "Room joined", code: 0, uuid: data.uuid });
        socket.to(data.uuid).emit("playerJoined", { username: data.username });
        console.log(data.username + " joined " + data.uuid);
      } else {
        io.to(socket.id).emit("status", { msg: "Cannot join room", code: 1 });
      }
    });

    socket.on("newMsg", (data) => {
      socket.to(data.uuid).emit("newMsg", data);
      io.to(socket.id).emit("newMsg", data);
    })

    socket.on("disconnect", () => {
      let index = clients.findIndex((c) => c.socketId === socket.id);
      let username = clients[index].username;
      console.log("Username is: " + username);
      clients.splice(index, 1);
      userDisconnected(socket.id, socket, username);
    });
  });
};
