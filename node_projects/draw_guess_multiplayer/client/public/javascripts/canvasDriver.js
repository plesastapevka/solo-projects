let socket = io("http://localhost:3333");
var currentRoom;
var canvas,
  ctx,
  flag = false,
  prevX = 0,
  currX = 0,
  prevY = 0,
  currY = 0,
  dot_flag = false;

function erase() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  var data = { erase: true };
  socket.emit("mouse", data);
}

var x = "black",
  y = 5;

function init() {
  canvas = document.getElementById("can");
  canvas.width = document.getElementById("mainContainer").clientWidth;
  canvas.height = document.getElementById("mainContainer").clientHeight;
  ctx = canvas.getContext("2d");
  ctx.lineCap = "round";
  w = canvas.width;
  h = canvas.height;
  rect = canvas.getBoundingClientRect();

  canvas.addEventListener(
    "mousemove",
    function (e) {
      findxy("move", e);
      color();
      data = {
        currX: currX,
        currY: currY,
        prevX: prevX,
        prevY: prevY,
        color: x,
        size: y,
        e: { clientX: e.clientX, clientY: e.clientY },
        res: "move",
        dot_flag: dot_flag,
        flag: flag,
      };
      socket.emit("mouse", data);
    },
    false
  );
  canvas.addEventListener(
    "mousedown",
    function (e) {
      findxy("down", e);
      data = {
        currX: currX,
        currY: currY,
        prevX: prevX,
        prevY: prevY,
        color: x,
        size: y,
        e: { clientX: e.clientX, clientY: e.clientY },
        res: "down",
        dot_flag: dot_flag,
        flag: flag,
      };

      socket.emit("mouse", data);
    },
    false
  );
  canvas.addEventListener(
    "mouseup",
    function (e) {
      findxy("up", e);
      data = {
        currX: currX,
        currY: currY,
        prevX: prevX,
        prevY: prevY,
        color: x,
        size: y,
        e: { clientX: e.clientX, clientY: e.clientY },
        res: "up",
        dot_flag: dot_flag,
        flag: flag,
      };
      socket.emit("mouse", data);
    },
    false
  );
  canvas.addEventListener(
    "mouseout",
    function (e) {
      findxy("out", e);
      data = {
        currX: currX,
        currY: currY,
        prevX: prevX,
        prevY: prevY,
        color: x,
        size: y,
        e: { clientX: e.clientX, clientY: e.clientY },
        res: "out",
        dot_flag: dot_flag,
        flag: flag,
      };
      socket.emit("mouse", data);
    },
    false
  );
}

function color(obj) {
  x = document.getElementById("colorPicker").value;
  if (x == "white") y = 14;
  else y = 5;
}

function draw() {
  ctx.beginPath();
  ctx.moveTo(prevX, prevY);
  ctx.lineTo(currX, currY);
  ctx.strokeStyle = x;
  ctx.lineWidth = y;
  ctx.stroke();
  ctx.closePath();
}

function save() {
  document.getElementById("can").style.border = "2px solid";
  var dataURL = canvas.toDataURL();
  document.getElementById("can").src = dataURL;
  document.getElementById("can").style.display = "inline";
}

function findxy(res, e) {
  if (res == "down") {
    prevX = currX;
    prevY = currY;
    currX = e.clientX - rect.left;
    currY = e.clientY - rect.top;

    flag = true;
    dot_flag = true;
    if (dot_flag) {
      ctx.beginPath();
      ctx.fillStyle = x;
      ctx.fillRect(currX, currY, 2, 2);
      ctx.closePath();
      dot_flag = false;
    }
  }
  if (res == "up" || res == "out") {
    flag = false;
  }
  if (res == "move") {
    prevX = currX;
    prevY = currY;
    currX = e.clientX - rect.left;
    currY = e.clientY - rect.top;
    if (flag) {
      draw();
    }
  }
}

$(init);

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

  socket.on("disconnect", () => {});
});

function leaveRoom() {
  socket.emit("leaveRequest", currentRoom.uuid);
  currentRoom = null;
  document.getElementById("playersTableBody").innerHTML = "<tr><td>Not in room</td><td></tr>";
  document.getElementById(
    "roomStatus"
  ).innerHTML = `<p style='text-align:center;'>In lobby ...</p>`;
  document.getElementById("playerCount").innerHTML = "0/8";
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
              <td>${r.players.length}</td>
              <td><input type="button" class="btn btn-link" value="${r.uuid}" onclick="joinRoom(this.value)"></td>
            </tr>`;
  });
  document.getElementById("roomTableBody").innerHTML = rows;
}

function updateRoomStatus(room) {
  console.log(room);
  let rows = "";
  document.getElementById("playerCount").innerHTML = `${room.players.length}/8`;
  room.players.forEach((p) => {
    rows += `
            <tr>
              <td>${p.username}</td>`;
    console.log(p.owner);
    if (p.owner) {
      rows += "<td style='color: grey;'><i>Owner</i></td>";
    } else {
      rows += "<td></td>";
    }
    rows += "</tr>";
  });
  document.getElementById("playersTableBody").innerHTML = rows;
  document.getElementById(
    "roomStatus"
  ).innerHTML = `<p style='text-align:center;'>In <b>${room.name}</b></p>`;
}

function statusMsg(status) {
  if (status.code === 0) {
    document.getElementById(
      "status"
    ).innerHTML = `<p style='color: green;'>${status.msg}</p>`;
  } else {
    document.getElementById(
      "status"
    ).innerHTML = `<p style='color: red;'>${status.msg}</p>`;
  }
}
