const uuid = require("uuid");
const UserModel = require("../models/userModel");
const GAME_TIME = 1800;
const MAX_POINTS = 300;

const words = [
  "shop",
  "dinosaurs",
  "mine",
  "taste",
  "robin",
  "van",
  "education",
  "person",
  "son",
  "division",
  "mark",
  "church",
  "caption",
  "bear",
  "button",
  "fairies",
  "history",
  "dime",
  "note",
  "cub",
  "end",
  "minute",
  "plate",
  "cent",
  "tub",
  "muscle",
  "wall",
  "drain",
  "sand",
  "summer",
];

let rooms = [];
let owners = [];
let clients = [];
// INTERVAL FUNCTION
function removeRoom(id) {
  let index = rooms.findIndex((r) => r.uuid === id);
  if (index !== -1) {
    return rooms.splice(index, 1)[0];
  }
}

function shuffle(array) {
  var currentIndex = array.length,
    temporaryValue,
    randomIndex;
  while (0 !== currentIndex) {
    randomIndex = Math.floor(Math.random() * currentIndex);
    currentIndex -= 1;
    temporaryValue = array[currentIndex];
    array[currentIndex] = array[randomIndex];
    array[randomIndex] = temporaryValue;
  }

  return array;
}

function randomWord() {
  return words[Math.floor(Math.random() * words.length)];
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
    let userIndex = r.players.findIndex(
      (player) => player.userId === data.userId
    );
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
      owner: data.owner,
      score: 0,
      globalWins: 0,
      turn: data.turn,
    });
    return true;
  }
  return false;
}

// CUSTOM CONDITIONAL INTERVAL
function setCustomConInterval(callback, room) {
  var intervalID = setInterval(function () {
    callback();
    if (room.state.turn === room.players.length || !room.state.alive) {
      clearInterval(intervalID);
    }
  }, 1000);
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
        io.to(r.ownerId).emit("owner", { owner: true });
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
      clients.push({ socketId: socket.id, username: username });
    });

    socket.on("mouse", (data) => {
      if (data.uuid !== "") {
        socket.to(data.uuid).emit("sync", data);
      }
    });

    socket.on("leaveRequest", (uuid) => {
      let index = clients.findIndex((c) => c.socketId === socket.id);
      let username = clients[index].username;
      userDisconnected(socket.id, socket, username);
      socket.leave(uuid);
      io.to(socket.id).emit("leaveApproved");
    });

    socket.on("createRoom", (data) => {
      let existIndex = rooms.findIndex((r) => r.name === data.name);
      // check the name
      if (existIndex !== -1) {
        io.to(socket.id).emit("status", {
          code: 1,
          msg: "Choose different room name",
        });
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
        userIndex = r.players.findIndex(
          (player) => player.userId === data.userId
        );
        if (userIndex !== -1) {
          exists = true;
        }
      });
      if (exists) {
        io.to(socket.id).emit("status", { code: 1, msg: "Already in a room" });
        return;
      }

      let guuid = uuid.v1();
      var room = {
        uuid: guuid,
        ownerId: socket.id,
        name: data.name,
        players: [],
        state: { alive: false, turn: 0, word: "", remaining: GAME_TIME, winner: "" },
      };
      rooms.push(room);
      var user = {
        uuid: guuid,
        userId: data.userId,
        username: data.username,
        socketId: socket.id,
        owner: true,
        turn: false,
      };
      joinRoom(user);
      owners.push(socket.id);
      socket.join(guuid);
      socket.broadcast.emit("roomUpdate", rooms);
      io.to(socket.id).emit("playerJoined", { username: data.username });
      socket.to(guuid).emit("playerJoined", { username: data.username });
      io.to(socket.id).emit("status", {
        msg: "Room created",
        code: 0,
        uuid: guuid,
      });
    });

    socket.on("joinRoom", (data) => {
      if (owners.includes(socket.id)) {
        io.to(socket.id).emit("status", { code: 1, msg: "Already an owner" });
        return;
      }
      data.socketId = socket.id;
      data.owner = false;
      data.turn = false;
      if (joinRoom(data)) {
        socket.join(data.uuid);
        io.to(socket.id).emit("status", {
          msg: "Room joined",
          code: 0,
          uuid: data.uuid,
        });
        socket.to(data.uuid).emit("playerJoined", { username: data.username });
        console.log(data.username + " joined " + data.uuid);
      } else {
        io.to(socket.id).emit("status", { msg: "Cannot join room", code: 1 });
      }
    });

    socket.on("newMsg", (data) => {
      socket.to(data.uuid).emit("newMsg", data);
      io.to(socket.id).emit("newMsg", data);
    });

    socket.on("startGame", (uuid) => {
      io.sockets.in(uuid).emit("clearCanvas");
      let index = rooms.findIndex((r) => r.uuid === uuid);
      let room = rooms[index];
      room.players.forEach((p) => {
        p.score = 0;
      });
      let word = randomWord();
      room.state.alive = true;
      room.state.word = word;
      room.state.turn = 0;
      room.players = shuffle(room.players);
      room.players[0].turn = true;
      room.state.winner = "";
      room.state.remaining = GAME_TIME;
      // INTERVAL TRACKING ROOM STATE
      setCustomConInterval(() => {
        room.state.remaining--;
        if (room.state.remaining <= 0) {
          nextPlayer(room);
          io.sockets.in(room.uuid).emit("clearCanvas");
        }
      }, room);
    });

    socket.on("guessWord", (data) => {
      let index = rooms.findIndex((r) => r.uuid === data.uuid);
      console.log;
      let room = rooms[index];
      if (data.word === room.state.word) {
        let index = room.players.findIndex((p) => data.userId === p.userId);
        let turnIndex = room.state.turn;
        let percentage = room.state.remaining / GAME_TIME;
        room.players[turnIndex].score += Math.round((MAX_POINTS * percentage) / 2);
        room.players[index].score += Math.round(MAX_POINTS * percentage);

        nextPlayer(room);
        io.sockets.in(room.uuid).emit("clearCanvas");
      }
    });

    socket.on("stopGame", (uuid) => {});

    socket.on("disconnect", () => {
      let index = clients.findIndex((c) => c.socketId === socket.id);
      let username = clients[index].username;
      clients.splice(index, 1);
      userDisconnected(socket.id, socket, username);
    });
  });
};

function nextPlayer(room) {
  let turnIndex = room.state.turn;
  room.players[turnIndex].turn = false;
  room.state.word = randomWord();
  room.state.turn++;
  if (room.state.turn === room.players.length) {
    // HANDLE GAME OVER
    let username = room.players[0].username;
    let hs = room.players[0].score;
    room.players.forEach((p) => {
      if (hs < p.score) {
        username = p.username;
      }
      UserModel.findOneAndUpdate({ username: p.username }, { $inc: { score: p.score }}, { upsert: true }, (err, doc) => {
        if (err) console.log(err);
      });
    });
    UserModel.findOneAndUpdate({ username: username }, { $inc: { globalWins: 1 }}, { upsert: true }, (err, doc) => {
      if (err) console.log(err);
    });
    room.state.winner = username;
    room.state.alive = false;
    return;
  }
  room.players[room.state.turn].turn = true;
  room.state.remaining = GAME_TIME;
}
