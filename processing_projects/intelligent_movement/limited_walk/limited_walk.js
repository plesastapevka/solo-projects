let v;

let debug = true;

let d = 25;

function setup() {
  createCanvas(640, 360);
  v = new Element(width / 2, height / 2);
}

function draw() {
  background(0);

  if (debug) {
    stroke(255);
    noFill();
    rectMode(CENTER);
    rect(width / 2, height / 2, width - d * 2, height - d * 2);
  }

  v.boundaries();

  v.update();
  v.display();

}

function mousePressed() {
  debug = !debug;
}
