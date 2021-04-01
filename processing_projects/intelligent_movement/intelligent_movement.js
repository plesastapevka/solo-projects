let movers = [];

function setup() {
  createCanvas(640,360);
  for (var i = 0; i < 1; i++) {
     movers[i] = new Element();
  }
}

function draw() {
  background(0);
  for (let i = 0; i < movers.length; i++) {
    movers[i].update();
    movers[i].display();
  }
}
