let flock;

let text;

function setup() {
  text = createP("Drag the mouse to generate new boids.");
  text.position(10, 365);

  createCanvas(640, 360);
  flock = new Flock();
  for (let i = 0; i < 60; i++) {
    let b = new Boid(width / 2, height / 2);
    flock.addBoid(b);
  }
}

function draw() {
  background(0);
  flock.run();
}

function mouseDragged() {
  flock.addBoid(new Boid(mouseX, mouseY));
}
