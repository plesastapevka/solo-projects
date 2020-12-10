var canvas,
  ctx,
  globalRooms = [],
  brush = {
    x: 0,
    y: 0,
    color: "#000000",
    size: 5,
    down: false,
  },
  strokes = [],
  currentStroke = null;

rivets.configure({
  templateDelimiters: ["{{", "}}"]
});

rivets.bind($("#tableBody"),
  {
    rooms: globalRooms
  });

function paint() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  ctx.lineCap = "round";
  for (var i = 0; i < strokes.length; i++) {
    var s = strokes[i];
    ctx.strokeStyle = s.color;
    ctx.lineWidth = s.size;
    ctx.beginPath();
    ctx.moveTo(s.points[0].x, s.points[0].y);
    for (var j = 0; j < s.points.length; j++) {
      var p = s.points[j];
      ctx.lineTo(p.x, p.y);
    }
    ctx.stroke();
  }
}

function erase() {
  strokes = [];
  paint();
}

function joinRoom(obj) {
  var data = {
    uuid: obj,
    userId: document.getElementById("userId").value,
    username: document.getElementById("username").value
  }
  socket.emit("joinRoom", data);
}

function init() {
  canvas = document.getElementById("can");
  canvas.width = document.getElementById("mainContainer").clientWidth;
  (canvas.height = document.getElementById("mainContainer").clientHeight),
    (ctx = canvas.getContext("2d"));

  function mouseEvent(e) {
    currentStroke.points.push({
      x: brush.x,
      y: brush.y,
    });

    data = {
      x: brush.x,
      y: brush.y,
      color: currentStroke.color
    };

    socket.emit("mouse", data);
    paint();
  }

  // EVENT ON MOUSE MOVE
  canvas.addEventListener(
    "mousemove",
    (e) => {
      var rect = canvas.getBoundingClientRect();
      brush.x = e.clientX - rect.left;
      brush.y = e.clientY - rect.top;
      // console.log("X: " + brush.x);
      // console.log("Y: " + brush.y);

      if (brush.down) mouseEvent(e);

      paint();
    },
    false
  );

  // EVENT ON MOUSE DOWN
  canvas.addEventListener(
    "mousedown",
    (e) => {
      brush.down = true;
      currentStroke = {
        color: brush.color,
        size: brush.size,
        points: [],
      };

      strokes.push(currentStroke);

      mouseEvent(e);
    },
    false
  );

  // EVENT ON MOUSE UP
  canvas.addEventListener(
    "mouseup",
    (e) => {
      brush.down = false;

      mouseEvent(e);

      currentStroke = null;
    },
    false
  );

  $("#colorPicker").on("input", function () {
    brush.color = this.value;
  });
}

$(init);

let socket = io("http://localhost:3333");

socket.on("connect", () => {
  socket.on("mouse", (data) => {});
  socket.on("sync",
    (data) => {
      currentStroke = {
        color: data.color,
        size: brush.size,
        points: [],
      };

      strokes.push(currentStroke);

      currentStroke.points.push({
        x: data.x,
        y: data.y
      });

      paint();
    });
  socket.on("roomUpdate", (rooms) => {
    globalRooms = rooms;
    // bind.update(globalRooms);
  });

  socket.on("disconnect", () => {});
});

function syncEvent(e) {}
