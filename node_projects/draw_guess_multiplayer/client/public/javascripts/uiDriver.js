socket.on("connect", () => {
  socket.on("mouse", (data) => {});
  socket.on("status", (status) => {
    statusMsg(status);
  });

  socket.on("requestUsername", () => {
    socket.emit("newUser", document.getElementById("username").value);
  });

  socket.on("sync", (data) => {
    if (data.erase) {
      ctx.clearRect(0, 0, canvas.width, canvas.height);
    } else {
      document.getElementById("can").disabled = true;
      currX = data.currX;
      currY = data.currY;
      prevX = data.prevX;
      prevY = data.prevY;
      x = data.color;
      y = data.size;
      e = data.e;
      flag = data.flag;
      dot_flag = data.dot_flag;
      res = data.res;
      if (flag) {
        draw();
      }
    }
  });
  socket.on("roomUpdate", (rooms) => {
    updateRooms(rooms);
  });
  socket.on("roomStatusUpdate", (room) => {
    currentRoom = room;
    updateRoomStatus(room);
  });

  socket.on("playerJoined", (data) => {
    document.getElementById("messages").innerHTML += `
        <div id="playerJoinInfo">
        <p><i>${data.username} joined</i></p>
        </div>
        `;
    scrollChat();
  });

  socket.on("userDisconnected", (data) => {
    document.getElementById("messages").innerHTML += `
        <div id="playerJoinInfo">
        <p><i>${data} left</i></p>
        </div>
        `;
    scrollChat();
  });

  socket.on("newMsg", (data) => {
    if (data.userId === document.getElementById("userId").value) {
      document.getElementById("messages").innerHTML += `
        <div id="message">
        <div id="yourUsername">
        <span>${data.username}</span>
        </div>
        <div id="rawMessage">
        <p id="userMessage">${data.message}</p>
        </div>
        </div>
        `;
    } else {
      document.getElementById("messages").innerHTML += `
        <div id="message">
        <div id="opponentUsername">
        <span>${data.username}<span>
        </div>
        <div id="rawMessage">
        <p id="oponnentMessage">${data.message}</p>
        </div>
        </div>
        `;
    }
    scrollChat();
  });

  socket.on("clearCanvas", () => {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
  });

  socket.on("disconnect", () => {});
});

function startGame() {
  socket.emit("startGame", currentRoom.uuid);
  document.getElementById("startBtn").disabled = true;
}

function leaveRoom() {
  socket.emit("leaveRequest", currentRoom.uuid);
  currentRoom = null;
  document.getElementById("playersTableBody").innerHTML =
    "<tr><td>Not in room</td><td></tr>";
  document.getElementById("seconds").innerHTML = "0 seconds";
  document.getElementById(
    "roomStatus"
  ).innerHTML = `<p style='text-align:center;'>In lobby ...</p>`;
  document.getElementById("playerCount").innerHTML = "0/8";
  document.getElementById("currRoomId").value = "";
  document.getElementById("leaveBtn").disabled = true;
  document.getElementById("startBtn").disabled = true;
}

function createRoom() {
  let name = document.getElementById("createRoomName").value;
  if (!name) {
    return;
  }
  let data = {
    userId: document.getElementById("userId").value,
    username: document.getElementById("username").value,
    name: name,
  };
  socket.emit("createRoom", data);
}

function joinRoom(uuid) {
  var data = {
    uuid: uuid,
    userId: document.getElementById("userId").value,
    username: document.getElementById("username").value,
  };
  socket.emit("joinRoom", data);
}

function updateRooms(rooms) {
  let rows = "";
  rooms.forEach((r) => {
    rows += `
              <tr id="${r.uuid}" class="clickable-row">
                <td id="roomName" value="${r.name}" scope="row">${r.name}</td>
                <td>${r.players.length}</td>`;
    if (!r.state.alive) {
      rows += `<td><input id="${r.uuid}" type="button" class="btn btn-link" value="Join" onclick="joinRoom(this.id)"></td>`;
    } else {
      rows += "<td></td>";
    }
    rows += `</tr>`;
  });
  document.getElementById("roomTableBody").innerHTML = rows;
}

function updateRoomStatus(room) {
  let rows = "";
  turn = false;
  let ownerId;
  document.getElementById("playerCount").innerHTML = `${room.players.length}/8`;
  document.getElementById(
    "seconds"
  ).innerHTML = `${room.state.remaining} seconds`;
  room.players.forEach((p) => {
    rows += `<tr>`;
    if (p.turn) {
      rows += `<td style="color:green;">${p.username}</td>`;
    } else if (p.username === room.state.winner) {
      rows += `<td style="color:blue;"><b>${p.username}</b></td>`;
    } else {
      rows += `<td>${p.username}</td>`;
    }
    rows += `<td>${p.score}</td>`;
    if (p.owner) {
      rows += "<td style='color: grey;'><i>Owner</i></td>";
      ownerId = p.userId;
    } else {
      rows += "<td></td>";
    }
    rows += "</tr>";
    if (p.turn && p.userId === document.getElementById("userId").value) {
      turn = true;
    }
  });
  if (turn) {
    document.getElementById("msgGuessBtn").disabled = true;
    document.getElementById("clearBtn").disabled = false;
    document.getElementById("colorPicker").disabled = false;
    document.getElementById("wordToDraw").innerHTML =
      "<b>" + room.state.word + "</b>";
  } else if (!room.state.alive && !turn) {
    document.getElementById("msgGuessBtn").disabled = true;
    document.getElementById("clearBtn").disabled = true;
    document.getElementById("colorPicker").disabled = true;
    document.getElementById("wordToDraw").innerHTML = "";
  } else if (room.state.alive && !turn) {
    document.getElementById("msgGuessBtn").disabled = false;
    document.getElementById("clearBtn").disabled = true;
    document.getElementById("colorPicker").disabled = true;
    document.getElementById("wordToDraw").innerHTML = "";
  }
  if (
    ownerId == document.getElementById("userId").value
    // room.players.length >= 3 MARK
  ) {
    document.getElementById("startBtn").disabled = false;
  }
  document.getElementById("playersTableBody").innerHTML = rows;
  document.getElementById(
    "roomStatus"
  ).innerHTML = `<p style='text-align:center;'>In <b>${room.name}</b></p>`;
}

function statusMsg(status) {
  if (status.code === 0) {
    if (status.msg === "Room joined" || status.msg === "Room created") {
      document.getElementById("currRoomId").value = status.uuid;
      currentRoom = status.uuid;
      document.getElementById("leaveBtn").disabled = false;
    }
    document.getElementById(
      "status"
    ).innerHTML = `<p style='color: green;'>${status.msg}</p>`;
  } else {
    document.getElementById(
      "status"
    ).innerHTML = `<p style='color: red;'>${status.msg}</p>`;
  }
}

function scrollChat() {
  document.getElementById("chat").scrollTop = document.getElementById(
    "chat"
  ).scrollHeight;
}

function sendMsg() {
  let msg = document.getElementById("messageBox").value;
  console.log(msg);
  if (!msg) {
    return;
  }
  if (msg === "" || msg === " ") {
    return;
  }
  const uuid = document.getElementById("currRoomId").value;
  console.log(uuid);
  if (uuid === "" || !uuid) {
    return;
  }
  const username = document.getElementById("username").value;
  const userId = document.getElementById("userId").value;
  socket.emit("newMsg", {
    message: msg,
    username: username,
    userId: userId,
    uuid: uuid,
  });
  document.getElementById("messageBox").value = null;
}

function guess() {
  let word = document.getElementById("messageBox").value;
  let data = {
    userId: document.getElementById("userId").value,
    uuid: currentRoom.uuid,
    word: word,
  };
  socket.emit("guessWord", data);
}
