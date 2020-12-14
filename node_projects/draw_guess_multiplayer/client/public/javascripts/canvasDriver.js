var socket = io("http://localhost:3333");
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
        uuid: document.getElementById("currRoomId").value ? document.getElementById("currRoomId").value : ""
      };
      socket.emit("mouse", data);
    },
    false
  );
  canvas.addEventListener(
    "mousedown",
    function (e) {
      findxy("down", e);
      console.log(document.getElementById("currRoomId").value);
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
        uuid: document.getElementById("currRoomId").value ? document.getElementById("currRoomId").value : ""
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
        uuid: document.getElementById("currRoomId").value ? "" : document.getElementById("currRoomId").value
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
        uuid: document.getElementById("currRoomId").value ? document.getElementById("currRoomId").value : ""
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