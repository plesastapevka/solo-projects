class Element {
  constructor() {
    this.position = createVector(random(width),random(height));
    this.velocity = createVector();
    this.acceleration = createVector();
    this.topspeed = 5;
    this.offset = 16;
  }

  update() {
    // Compute a vector that points from position to mouse
    var mouse = createVector(mouseX,mouseY);
    this.acceleration = p5.Vector.sub(mouse,this.position);
    // Set magnitude of acceleration
    this.acceleration.setMag(0.2);

    this.velocity.add(this.acceleration);
    this.velocity.limit(this.topspeed);
    this.position.add(this.velocity);
  }

  display() {
    stroke(0);
    strokeWeight(2);
    fill(255);
    triangle(this.position.x - this.offset,     // x1
             this.position.y + this.offset,     // y1
             this.position.x + this.offset,     // x2
             this.position.y + this.offset,     // y2
             this.position.x,                   // x3
             this.position.y - this.offset);    // y3
  }
}
