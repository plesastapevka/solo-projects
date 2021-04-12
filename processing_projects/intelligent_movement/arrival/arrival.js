let movers = [];
let v, count = 1, MODES = 2, mode = 0;

function setup() {
  createCanvas(1024,1024);
  for (var i = 0; i < count; i++) {
     movers[i] = new Element(width / 2, height / 2);
  }
}

function draw() {
  background(0);
  let mouse = createVector(mouseX, mouseY);

  fill(255);
  stroke(200);
  strokeWeight(2);
  ellipse(mouse.x, mouse.y, 24, 24);

  for (var i = 0; i < count; i++) {
    mode == 0 ? movers[i].arrive(mouse) : movers[i].seek(mouse);
    movers[i].update();
    movers[i].display();
  }
}

function mouseClicked() {
  mode = (mode + 1) % MODES;
}
